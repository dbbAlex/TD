using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD_WPF.Game.GameObjects;
using TD_WPF.Game.GameObjects.StaticGameObjects;

namespace TD_WPF.Game.GameUtils
{
    public class GameUtils
    {
        public static LinkedList<Path> GenerateRandomPath(float fieldWidth, float fieldHeight, int x , int y)
        {
            Random random = new Random();
            LinkedList<Path> paths = new LinkedList<Path>(); // store the path
            List<Path> space = new List<Path>(); // store spaces between path elements -> just for nice path

            // determine path length
            int min = Convert.ToInt32(x * y * 0.1);
            int max = Convert.ToInt32(x * y * 0.2);
            int maxPathObjects = random.Next(min, max);

            // calculate width and hight
            float width = fieldWidth / x;
            float height = fieldHeight / y;

            // get a random spawn 
            paths.AddLast(new Spawn(random.Next(x), random.Next(y), width, height));

            // store current path object for further use
            Path current = paths.First.Value;
            do
            {
                // get next possible fileds
                List<Path> list = PossiblePaths(NextPaths(x, y, width, height, current.x, current.y), space, paths, null);

                
                if (list.Count == 0) // if list is empty then generation failed
                {
                    // restart generation
                    paths.Clear();
                    space.Clear();
                    paths.AddLast(new Spawn(random.Next(x), random.Next(y), width, height));
                    current = paths.First.Value;                    
                    continue;
                }

                // get next possible path randomly
                int index = random.Next(list.Count);
                Path next = paths.Count + 1 == maxPathObjects ?
                    new Base(list[index].x, list[index].y, width, height) : list[index];
                paths.AddLast(next);

                // add other path to space
                list.Remove(next);
                space.AddRange(list);

                // increase
                current = next;
            } while (paths.Count < maxPathObjects);

            return paths;
        }

        public static LinkedList<Ground> GenerateRandomGround(LinkedList<Path> paths, float fieldWidth, float fieldHeight, int x, int y)
        {
            Random random = new Random();
            LinkedList<Ground> ground = new LinkedList<Ground>();
            int groundCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(paths.Count / x)));

            // calculate width and hight
            float width = fieldWidth / x;
            float height = fieldHeight / y;
            do
            {
                // get random path
                int randomIndex = random.Next(paths.Count);
                LinkedListNode<Path> node = paths.First;
                while (randomIndex > 0)
                {
                    node = node.Next;
                    randomIndex--;
                }


                List<Path> list = PossiblePaths(NextPaths(x, y , width, height, node.Value.x, node.Value.y), null, paths, new LinkedList<Path>(ground.Cast<Path>()));
                if (list.Count == 0)
                    continue;

                randomIndex = random.Next(list.Count);
                ground.AddLast(new Ground(list[randomIndex].x, list[randomIndex].y, width, height));

                groundCount--;
            } while (groundCount != 0);

            return ground;
        }

        public static List<Path> NextPaths(int x, int y, float width, float height, float _x, float _y)
        {
            List<Path> list = new List<Path>();
            //unten
            if (_y + 1 < y)
                list.Add(new Path(_x, _y+1, width, height));

            //oben
            if (_y - 1 >= 0)
                list.Add(new Path(_x, _y-1, width, height));

            //links
            if (_x - 1 >= 0)
                list.Add(new Path(_x-1, _y, width, height));

            //rechts
            if (_x + 1 < x)
                list.Add(new Path(_x+1, _y, width, height));

            return list;
        }

        public static List<Path> PossiblePaths(List<Path> next, List<Path> space, LinkedList<Path> paths, LinkedList<Path> ground)
        {
            List<Path> removable = new List<Path>();
            List<LinkedList<Path>> lists = new List<LinkedList<Path>> { paths != null ? paths : new LinkedList<Path>(), 
                ground != null ? ground : new LinkedList<Path>() , space != null ? new LinkedList<Path> (space) : new LinkedList<Path>() };
            foreach (var list in lists)
            {
                foreach (var item in list)
                {
                    foreach (var element in next)
                    {
                        if (item.x == element.x && item.y == element.y)
                            removable.Add(element);
                    }
                }
            }

            foreach (var item in removable)
            {
                next.Remove(item);
            }

            return next;
        }

        public static Path GetPathPosition(GameObject gameObject, GameControl gameControl)
        {
            foreach (var item in gameControl.gameCreator.paths)
            {
                if (gameObject.x >= item.x && gameObject.x < item.x + item.width
                    && gameObject.y >= item.y && gameObject.y < item.y + item.height)
                {
                    return item;
                }
            } 

            return null;
        }

        public static Path GetNextPath(Path current, GameControl gameControl)
        {
            LinkedListNode<Path> cur = gameControl.gameCreator.paths.First;
            while(cur.Value != current)
            {
                cur = cur.Next;
                if (cur == null)
                    return null;
            }

            return cur.Next != null ? cur.Next.Value : null;
        }
    }    
}
