using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TD_WPF.Game.Utils
{
    public class GameManager
    {
        private const int Fps = 60;
        private const double MaxLoopTime = 1000d / Fps;

        private bool Running { get; set; } = true;
        public bool Pause { get; set; }
        public bool End { get; private set; }
        public Stopwatch Timer { get; }= new Stopwatch();

        public async void Run(GameControl gameControl)
        {
            Timer.Start();

            Start(gameControl, Timer.ElapsedMilliseconds);
            while (Running)
            {
                // get start time
                var lastTime = Timer.ElapsedMilliseconds;
                // update all instances
                Update(gameControl, lastTime);
                // set time after update
                var time = Timer.ElapsedMilliseconds;
                // check if we are in time
                if (time - lastTime > MaxLoopTime) continue;
                // we are in time so we can render this frame
                Render(gameControl);
                // get timer after rendering
                time = Timer.ElapsedMilliseconds;
                // check if we are to fast
                if (time - lastTime < MaxLoopTime) await Task.Delay(Convert.ToInt32(MaxLoopTime - (time - lastTime)));
            }
        }

        private static void Start(GameControl gameControl, long currentInterval)
        {
            foreach (var item in gameControl.GameCreator.Paths) item.Start(gameControl);
            foreach (var item in gameControl.GameCreator.Ground) item.Start(gameControl);
            gameControl.GameCreator.Waves?.Start(gameControl, currentInterval);
        }

        private static void Update(GameControl gameControl, long currentinterval)
        {
            foreach (var item in gameControl.GameCreator.Paths) item.Update(gameControl);
            foreach (var item in gameControl.GameCreator.Ground) item.Update(gameControl, currentinterval);
            foreach (var item in gameControl.Shots) item.Update(gameControl);
            gameControl.GameCreator.Waves?.Update(gameControl, currentinterval);
            foreach (var item in gameControl.Marks) item.Update(gameControl);
        }

        private static void Render(GameControl gameControl)
        {
            foreach (var item in gameControl.GameCreator.Paths) item.Render(gameControl);
            foreach (var item in gameControl.GameCreator.Ground) item.Render(gameControl);
            foreach (var item in gameControl.Shots) item.Render(gameControl);
            gameControl.GameCreator.Waves?.Render(gameControl);
            foreach (var item in gameControl.Marks) item.Render(gameControl);
        }

        public void EndGame()
        {
            End = true;
            Pause = true;
        }

        public void EndLoop()
        {
            EndGame();
            Running = false;
        }
    }
}