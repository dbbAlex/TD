using System.Diagnostics;

namespace TD_WPF.Game.GameUtils
{
    public class GameManager
    {
        public const float FPS = 60F;
        public const float MAX_LOOP_TIME = 1000 / FPS;
        public bool running { get; set; } = true;

        public GameManager() { }

        public async void run(GameControl gameControl)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            float previousTime = timer.ElapsedMilliseconds;
            float lag = 0F;
            start(gameControl);
            while (running)
            {
                float currentTime = timer.ElapsedMilliseconds;
                float ellapsed = currentTime - previousTime;
                previousTime = currentTime;
                lag += ellapsed;

                while (lag >= MAX_LOOP_TIME)
                {
                    update(gameControl, 0F);
                    lag -= MAX_LOOP_TIME;
                }
                
                render(gameControl);
                
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
            gameControl.gameCreator.waves?.start(gameControl);
        }

        public void update(GameControl gameControl, float deltaTime)
        {
            foreach (var item in gameControl.gameCreator.paths)
            {
                item.update(gameControl, deltaTime);
            }
            foreach (var item in gameControl.gameCreator.ground)
            {
                item.update(gameControl, deltaTime);
            }

            gameControl.gameCreator.waves?.update(gameControl, deltaTime);
            foreach (var item in gameControl.marks)
            {
                item.update(gameControl, deltaTime);
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
            gameControl.gameCreator.waves?.render(gameControl);
            foreach (var item in gameControl.marks)
            {
                item.render(gameControl);
            }
        }

    }
}
