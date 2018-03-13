using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TD_WPF.Game.GameManagerTask
{
    public class GameManager
    {
        public const float FPS = 60F;
        public const float MAX_LOOP_TIME = 1000 / FPS;
        public bool running { get; set; } = true;

        public GameManager() { }

        public async void run(GameControl gameControl)
        {
            float time;
            float oldTime;
            Stopwatch timer = new Stopwatch();
            timer.Start();

            start(gameControl);

            while (running)
            {
                // get start time
                oldTime = timer.ElapsedMilliseconds;
                // update all instances
                update(gameControl);
                // set time after update
                time = timer.ElapsedMilliseconds;
                // check if we are in time
                if (time - oldTime > MAX_LOOP_TIME)
                {
                    // update took to long so we skip this frame
                    continue;
                }
                // we are in time so we can render this frame
                render(gameControl);
                // get timer after rendering
                time = timer.ElapsedMilliseconds;
                // check if we are to fast
                if (time - oldTime < MAX_LOOP_TIME)
                {
                    // wait until the looptime is reached to keep the speed constant
                    await Task.Delay(Convert.ToInt32(MAX_LOOP_TIME - (time - oldTime)));
                }
            }
        }

        public void start(GameControl gameControl)
        {
            foreach (var item in gameControl.gameCreator.paths)
            {
                item.start(gameControl);
            }
            foreach (var item in gameControl.gameCreator.ground)
            {
                item.start(gameControl);
            }
        }

        public void update(GameControl gameControl)
        {
            foreach (var item in gameControl.gameCreator.paths)
            {
                item.update(gameControl);
            }
            foreach (var item in gameControl.gameCreator.ground)
            {
                item.update(gameControl);
            }
            foreach (var item in gameControl.gameCreator.shots)
            {
                item.update(gameControl);
            }
            foreach (var item in gameControl.gameCreator.enemies)
            {
                item.update(gameControl);
            }
        }

        public void render(GameControl gameControl)
        {
            foreach (var item in gameControl.gameCreator.paths)
            {
                item.render(gameControl);
            }
            foreach (var item in gameControl.gameCreator.ground)
            {
                item.render(gameControl);
            }
            foreach (var item in gameControl.gameCreator.shots)
            {
                item.render(gameControl);
            }
            foreach (var item in gameControl.gameCreator.enemies)
            {
                item.render(gameControl);
            }
        }

    }
}
