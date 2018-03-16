using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TD_WPF.Game.GameUtils
{
    public class GameManager
    {
        public const float FPS = 60F;
        public const float MAX_LOOP_TIME = 1000 / FPS;

        public bool running { get; set; } = true;

        public async void run(GameControl gameControl)
        {
            var timer = new Stopwatch();
            timer.Start();
            float time = timer.ElapsedMilliseconds;
            float oldTime = timer.ElapsedMilliseconds;

            start(gameControl);
            while (running)
            {
                // get start time
                oldTime = timer.ElapsedMilliseconds;
                // update all instances
                update(gameControl, oldTime - time);
                // set time after update
                time = timer.ElapsedMilliseconds;
                // check if we are in time
                if (time - oldTime > MAX_LOOP_TIME) continue;
                // we are in time so we can render this frame
                render(gameControl);
                // get timer after rendering
                time = timer.ElapsedMilliseconds;
                // check if we are to fast
                if (time - oldTime < MAX_LOOP_TIME) await Task.Delay(Convert.ToInt32(MAX_LOOP_TIME - (time - oldTime)));
            }
        }

        public void start(GameControl gameControl)
        {
            foreach (var item in gameControl.gameCreator.paths) item.start(gameControl);
            foreach (var item in gameControl.gameCreator.ground) item.start(gameControl);
            gameControl.gameCreator.waves?.start(gameControl);
        }

        public void update(GameControl gameControl, float deltaTime)
        {
            foreach (var item in gameControl.gameCreator.paths) item.update(gameControl, deltaTime);
            foreach (var item in gameControl.gameCreator.ground) item.update(gameControl, deltaTime);

            gameControl.gameCreator.waves?.update(gameControl, deltaTime);
            foreach (var item in gameControl.marks) item.update(gameControl, deltaTime);
        }

        public void render(GameControl gameControl)
        {
            foreach (var item in gameControl.gameCreator.paths) item.render(gameControl);
            foreach (var item in gameControl.gameCreator.ground) item.render(gameControl);
            gameControl.gameCreator.waves?.render(gameControl);
            foreach (var item in gameControl.marks) item.render(gameControl);
        }
    }
}