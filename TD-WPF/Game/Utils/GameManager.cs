using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TD_WPF.Game.Utils
{
    public class GameManager
    {
        private const float Fps = 60f;
        private const float MaxLoopTime = 1000 / Fps;

        private bool Running { get; } = true;

        public async void Run(GameControl gameControl)
        {
            var timer = new Stopwatch();
            timer.Start();

            Start(gameControl, timer.ElapsedMilliseconds);
            while (Running)
            {
                // get start time
                float lastTime = timer.ElapsedMilliseconds;
                // update all instances
                Update(gameControl, lastTime);
                // set time after update
                float time = timer.ElapsedMilliseconds;
                // check if we are in time
                if (time - lastTime > MaxLoopTime) continue;
                // we are in time so we can render this frame
                Render(gameControl);
                // get timer after rendering
                time = timer.ElapsedMilliseconds;
                // check if we are to fast
                if (time - lastTime < MaxLoopTime) await Task.Delay(Convert.ToInt32(MaxLoopTime - (time - lastTime)));
            }
        }

        private void Start(GameControl gameControl, float currentInterval)
        {
            foreach (var item in gameControl.GameCreator.Paths) item.Start(gameControl);
            foreach (var item in gameControl.GameCreator.Ground) item.Start(gameControl);
            gameControl.GameCreator.Waves?.Start(gameControl, currentInterval);
        }

        private void Update(GameControl gameControl, float currentinterval)
        {
            foreach (var item in gameControl.GameCreator.Paths) item.Update(gameControl);
            foreach (var item in gameControl.GameCreator.Ground) item.Update(gameControl, currentinterval);
            foreach (var item in gameControl.Shots) item.Update(gameControl);
            gameControl.GameCreator.Waves?.Update(gameControl, currentinterval);
            foreach (var item in gameControl.Marks) item.Update(gameControl);
        }

        private void Render(GameControl gameControl)
        {
            foreach (var item in gameControl.GameCreator.Paths) item.Render(gameControl);
            foreach (var item in gameControl.GameCreator.Ground) item.Render(gameControl);
            foreach (var item in gameControl.Shots) item.Render(gameControl);
            gameControl.GameCreator.Waves?.Render(gameControl);
            foreach (var item in gameControl.Marks) item.Render(gameControl);
        }

        public void EndGame(GameControl gameControl)
        {
            foreach (var item in gameControl.GameCreator.Paths) item.Deaktivate();
            foreach (var item in gameControl.GameCreator.Ground) item.Deaktivate();
            foreach (var item in gameControl.Shots) item.Deaktivate();
            gameControl.GameCreator.Waves?.Deaktivate();
        }
    }
}