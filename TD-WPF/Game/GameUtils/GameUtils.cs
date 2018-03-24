using System;
using System.Collections.Generic;
using TD_WPF.Game.GameObjects;
using TD_WPF.Game.GameObjects.DynamicGameObjects;
using TD_WPF.Game.GameObjects.StaticGameObjects;
using TD_WPF.Game.RoundObjects;

namespace TD_WPF.Game.GameUtils
{
    public static class GameUtils
    {
        #region generate methods

        public static List<Path> GenerateRandomPath(float fieldWidth, float fieldHeight, int x, int y)
        {
            var random = new Random();
            var paths = new List<Path>(); // store the path
            var space = new List<Path>(); // store spaces between path elements -> just for nice path

            // determine path length
            var min = Convert.ToInt32(x * y * 0.1);
            var max = Convert.ToInt32(x * y * 0.2);
            var maxPathObjects = random.Next(min, max);

            // calculate width and hight
            var width = fieldWidth / x;
            var height = fieldHeight / y;

            // get a random spawn 
            paths.Add(new Spawn(random.Next(x), random.Next(y), width, height, 0));

            // store current path object for further use
            GameObject current = paths[0];
            do
            {
                // get next possible fileds
                var list = PossiblePaths(NextPaths(x, y, width, height, current.X, current.Y, paths.Count), space, paths, null);


                if (list.Count == 0) // if list is empty then generation failed
                {
                    // restart generation
                    paths.Clear();
                    space.Clear();
                    paths.Add(new Spawn(random.Next(x), random.Next(y), width, height, 0));
                    current = paths[0];
                    continue;
                }

                // get next possible path randomly
                var index = random.Next(list.Count);
                var next = paths.Count == maxPathObjects
                    ? new Base(list[index].X, list[index].Y, width, height, list[index].Index)
                    : list[index];
                paths.Add(next);

                // add other path to space
                list.Remove(next);
                space.AddRange(list);

                // increase
                current = next;
            } while (paths.Count <= maxPathObjects);

            return paths;
        }

        public static List<Ground> GenerateRandomGround(List<Path> paths, float fieldWidth,
            float fieldHeight, int x, int y)
        {
            var random = new Random();
            var ground = new List<Ground>();
            var groundCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(paths.Count / x)));

            // calculate width and hight
            var width = fieldWidth / x;
            var height = fieldHeight / y;
            do
            {
                // get random path
                var randomIndex = random.Next(paths.Count);
                GameObject obj = paths.Find(p => p.Index == randomIndex);
                var list = PossiblePaths(NextPaths(x, y, width, height, obj.X, obj.Y, ground.Count+1), null, paths,
                    new List<Path>(ground));
                if (list.Count == 0)
                    continue;

                randomIndex = random.Next(list.Count);
                ground.Add(new Ground(list[randomIndex].X, list[randomIndex].Y, width, height, -1));

                groundCount--;
            } while (groundCount != 0);

            return ground;
        }

        public static Waves GenerateRandomWaves(float intervalBetweenWaves, float intervalBetweenEnemies, Spawn spawn)
        {
            var random = new Random();
            var waves = new Waves(intervalBetweenWaves);

            for (var i = random.Next(10)+1; i > 0; i--)
                waves.WaveList.Add(GenerateRandomWave(intervalBetweenEnemies, spawn));

            return waves;
        }

        private static Wave GenerateRandomWave(float intervalBetweenEnemies, Spawn spawn)
        {
            var random = new Random();
            var wave = new Wave(intervalBetweenEnemies);

            for (var i = random.Next(10)+1; i > 0; i--)
                wave.Enemies.Add(new Enemy(spawn.X, spawn.Y, spawn.Width, spawn.Height, 0.09f, random.Next(10, 51),
                    random.Next(10, 21), wave, 0));

            return wave;
        }

        #endregion

        #region other methods

        public static List<Path> NextPaths(int x, int y, float width, float height, float _x, float _y, int index)
        {
            var list = new List<Path>();
            //unten
            if (_y + 1 < y)
                list.Add(new Path(_x, _y + 1, width, height, index));

            //oben
            if (_y - 1 >= 0)
                list.Add(new Path(_x, _y - 1, width, height, index));

            //links
            if (_x - 1 >= 0)
                list.Add(new Path(_x - 1, _y, width, height, index));

            //rechts
            if (_x + 1 < x)
                list.Add(new Path(_x + 1, _y, width, height, index));

            return list;
        }

        public static List<Path> PossiblePaths(List<Path> next, List<Path> space, List<Path> paths,
            List<Path> ground)
        {
            var removable = new List<Path>();
            var lists = new List<List<Path>>
            {
                paths ?? new List<Path>(),
                ground ?? new List<Path>(),
                space ?? new List<Path>()
            };
            foreach (var list in lists)
            foreach (var item in list)
            foreach (var element in next)
                if (item.X == element.X && item.Y == element.Y)
                    removable.Add(element);

            foreach (var item in removable) next.Remove(item);

            return next;
        }

        #endregion
    }
}