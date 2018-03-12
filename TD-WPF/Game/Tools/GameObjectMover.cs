using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TD_WPF.Game.Spielobjekte;
using TD_WPF.Game.Spielobjekte.Items;

namespace TD_WPF.Game.Tools
{
    class GameObjectMover
    {
        public static void moveGameObject(GameFrame gameFrame, LinkedList<Spielobjekt> weg, MoveableObject gameObject)
        {
            // get current node
            LinkedListNode<Spielobjekt> current = weg.First;

            // get next node
            LinkedListNode<Spielobjekt> next = current.Next;

            // get next Direction
            gameObject.nextDirection = getNextDirection(current.Value, next.Value);

            // create rectangle for image to move in canvas
            Rectangle rec = new Rectangle()
            {
                Name = "gegner",
                Width = (gameFrame.Map.ActualWidth / gameFrame.x) - 1,
                Height = (gameFrame.Map.ActualHeight / gameFrame.y) - 1,
                Fill = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(gameObject.image.GetHbitmap(),
                                   IntPtr.Zero,
                                   Int32Rect.Empty,
                                   BitmapSizeOptions.FromEmptyOptions()))
            };

            // rotate rectangle to displayed it right on the canvas 
            //RotateTransform rt = new RotateTransform(getAngle(gameObject), gameObject.width / 2, gameObject.height / 2);
            //rec.RenderTransform = rt;

            // calculate position in canvas and add rectangle
            Point position = new Point(current.Value.x * (gameFrame.Map.ActualWidth / gameFrame.x), current.Value.y * (gameFrame.Map.ActualHeight / gameFrame.y));
            Canvas.SetLeft(rec, position.X + 1);
            Canvas.SetTop(rec, position.Y + 1);
            gameFrame.Map.Children.Add(rec);

            // calculate 1/3 width/height of distance to add
            //double widthPart = (gameFrame.Map.ActualWidth / gameFrame.x) / 3;
            //double heightPart = (gameFrame.Map.ActualHeight / gameFrame.y) / 3;

            // move/rotate rectangle until it reaches end or get destroyed
            do
            {
                gameObject.currentDirection = gameObject.nextDirection;
                gameObject.nextDirection = getNextDirection(current.Value, next.Value);
                // end loop to destroy if health <= 0
                if (gameObject.GetType() != typeof(Gegner))
                    break;
                // set new position
                if (0 == (int)gameObject.nextDirection)
                {
                    position.Y -= (gameFrame.Map.ActualHeight / gameFrame.y);
                    var moveAnimY = new DoubleAnimation(Canvas.GetTop(rec), position.Y, new Duration(TimeSpan.FromMilliseconds(1000)));
                    rec.BeginAnimation(Canvas.TopProperty, moveAnimY);
                }
                else if (1 == (int)gameObject.nextDirection)
                {
                    position.X += (gameFrame.Map.ActualWidth / gameFrame.x);
                    var moveAnimX = new DoubleAnimation(Canvas.GetLeft(rec), position.X, new Duration(TimeSpan.FromMilliseconds(1000)));
                    rec.BeginAnimation(Canvas.LeftProperty, moveAnimX);
                }
                else if (2 == (int)gameObject.nextDirection)
                {
                    position.Y += (gameFrame.Map.ActualHeight / gameFrame.y);
                    var moveAnimY = new DoubleAnimation(Canvas.GetTop(rec), position.Y, new Duration(TimeSpan.FromMilliseconds(1000)));
                    rec.BeginAnimation(Canvas.TopProperty, moveAnimY);
                }
                else
                {
                    position.X -= (gameFrame.Map.ActualWidth / gameFrame.x);
                    var moveAnimX = new DoubleAnimation(Canvas.GetLeft(rec), position.X, new Duration(TimeSpan.FromMilliseconds(1000)));
                    rec.BeginAnimation(Canvas.LeftProperty, moveAnimX);
                }
                #region comment
                //for (double j = 0, i = (0 == (int)gameObject.nextDirection || 2 == (int)gameObject.nextDirection) ? (gameFrame.Map.ActualHeight / gameFrame.y) : (gameFrame.Map.ActualWidth / gameFrame.x); 
                //        j < i; j += (0 == (int)gameObject.nextDirection || 2 == (int)gameObject.nextDirection) ? heightPart : widthPart)
                //    {
                //        // set new position                        
                //        if (0 == (int)gameObject.nextDirection)
                //            position.Y +=  heightPart + 1;
                //        else if (1 == (int)gameObject.nextDirection)
                //            position.X += widthPart * 1 + 1;
                //        else if (2 == (int)gameObject.nextDirection)
                //            position.Y += heightPart * 1 + 1;
                //        else
                //            position.X += widthPart  + 1;

                //        var moveAnimX = new DoubleAnimation(Canvas.GetLeft(rec), position.X, new Duration(TimeSpan.FromMilliseconds(333)));
                //        var moveAnimY = new DoubleAnimation(Canvas.GetTop(rec),  position.Y, new Duration(TimeSpan.FromMilliseconds(333)));

                //        rec.BeginAnimation(Canvas.LeftProperty, moveAnimX);
                //        rec.BeginAnimation(Canvas.TopProperty, moveAnimY);

                //        // set rotation
                //        //double angle = getAngle(gameObject);
                //        //if(angle != 0)
                //        //{
                //        //    RotateTransform trans = new RotateTransform(getAngle(gameObject)/3, gameObject.width / 2, gameObject.height / 2);
                //        //    rec.RenderTransform = trans;
                //        //}
                //        await Task.Delay(333);
                //    }
                #endregion
                current = next;
                next = current.Next;
            } while (next != null);
            //gameFrame.Map.Children.Remove(rec);
        }

        public static Enumerations.Navigation getNextDirection(Spielobjekt current, Spielobjekt next)
        {
            if (current.x == next.x && current.y > next.y)
                return Enumerations.Navigation.North;
            if (current.x == next.x && current.y < next.y)
                return Enumerations.Navigation.South;
            if (current.y == next.y && current.x < next.x)
                return Enumerations.Navigation.East;
            return Enumerations.Navigation.West;
        }

        public static double getAngle(MoveableObject obj)
        {
            int fist = (int)obj.currentDirection;
            int second = (int)obj.nextDirection;

            if (fist + 1 == second || fist == (int)Enumerations.Navigation.West && second == (int)Enumerations.Navigation.North)
                return -90;
            else if (fist - 1 == second || fist == (int)Enumerations.Navigation.North && second == (int)Enumerations.Navigation.West)
                return 90;
            return 0;
        }

        public static void startWaves(GameFrame gameFrame)
        {
            LinkedListNode<Wave> current = gameFrame.feld.waves.waves.First;
            if (current == null)
                return;
            LinkedListNode<Wave> next = current.Next;
            double prevWaveDelay = 1;
            do
            {
                Wave wave = current.Value;
                System.Timers.Timer timer = new System.Timers.Timer(prevWaveDelay);
                timer.Elapsed += async (sender, e) =>
                {                    
                    timer.Stop();
                    timer.Close();
                    timer.Dispose();
                    try
                    {
                        await gameFrame.Dispatcher.InvokeAsync((Action)(() => startWave(ref gameFrame, ref wave)));
                    }
                    catch (Exception)
                    {
                        //Application closed
                    }
                    
                };
                timer.Start();

                prevWaveDelay += gameFrame.feld.waves.intervalInMilli + ((wave.enemy.Count - 1) * wave.intervalInMilli) + (500 * gameFrame.feld.strecke.Count - 1);
                current = next;
                if (current == null)
                    break;
                next = current.Next;
            } while (true);
        }

        public static void startWave(ref GameFrame gameFrame, ref Wave wave)
        {
            GameFrame g = gameFrame;
            Wave w = wave;
            LinkedListNode<Gegner> current = w.enemy.First;
            if (current == null)
                return;
            LinkedListNode<Gegner> next = current.Next;
            double delay = 1;
            do
            {
                MoveableObject m = current.Value;
                System.Timers.Timer timer = new System.Timers.Timer(delay);
                timer.Elapsed += async (sender, e) => 
                {
                    timer.Stop();
                    timer.Close();
                    timer.Dispose();
                    try
                    {
                        await g.Dispatcher.InvokeAsync((Action)(() => startTimerForMovableObject(ref g, ref m)));
                    }
                    catch (Exception)
                    {
                        //Application closed
                    }
                    
                };
                timer.Start();

                delay += w.intervalInMilli;

                current = next;
                if (current == null)
                    break;
                next = current.Next;
            } while (true);
        }

        public static void startTimerForMovableObject(ref GameFrame gameFrame, ref MoveableObject gameObject)
        {
            GameFrame g = gameFrame;
            MoveableObject m = gameObject;
            double fps = 1000 / 60;
            gameObject.x = gameFrame.feld.strecke.First.Value.x;
            gameObject.y = gameFrame.feld.strecke.First.Value.y;
            gameObject.currentDirection = getNextDirection(gameFrame.feld.strecke.First.Value, gameFrame.feld.strecke.First.Next.Value);
            Ellipse ell = new Ellipse()
            {
                Name = "gegner",
                Width = (gameFrame.Map.ActualWidth / gameFrame.x) / 2,
                Height = (gameFrame.Map.ActualHeight / gameFrame.y) / 2,
                Fill = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(gameObject.image.GetHbitmap(),
                                   IntPtr.Zero,
                                   Int32Rect.Empty,
                                   BitmapSizeOptions.FromEmptyOptions()))
            };
            Point position = new Point(gameObject.x * (gameFrame.Map.ActualWidth / gameFrame.x), gameObject.y * (gameFrame.Map.ActualHeight / gameFrame.y));
            Canvas.SetLeft(ell, position.X + ((gameFrame.Map.ActualWidth / gameFrame.x) - ell.Width) /2);
            Canvas.SetTop(ell, position.Y + ((gameFrame.Map.ActualHeight / gameFrame.y) - ell.Height) / 2);
            gameFrame.Map.Children.Add(ell);

            System.Timers.Timer timer = new System.Timers.Timer(fps);
            timer.Elapsed += async (sender, e) =>
            {
                if (!(m.x == g.feld.strecke.Last.Value.x && m.y == g.feld.strecke.Last.Value.y))
                    try
                    {
                        await g.Dispatcher.InvokeAsync((Action)(() => handleEnemyMove(ref g, ref m, ref fps, ref ell)));
                    }
                    catch (Exception)
                    {
                        // Application was closed                       
                    }
                    
                else
                {
                    timer.Stop();
                    timer.Close();
                    timer.Dispose();
                }
            };
            timer.Start();
        }

        public static void handleEnemyMove(ref GameFrame gameFrame, ref MoveableObject gameObject, ref double fps, ref Ellipse ell)
        {
            // get current and next field 
            LinkedListNode<Spielobjekt> current = getCurrentPosition(gameFrame.feld.strecke, gameObject);
            LinkedListNode<Spielobjekt> next = current.Next;

            // current/next direction
            if(current.Previous == null)
            {
                gameObject.nextDirection = getNextDirection(current.Value, next.Value);
                gameObject.currentDirection = gameObject.nextDirection;
            }
            else
            {
                gameObject.currentDirection = getNextDirection(current.Previous.Value, current.Value);
                if(next != null)
                    gameObject.nextDirection = getNextDirection(current.Value, next.Value);
                else
                    gameObject.nextDirection = gameObject.currentDirection;
            }                

            // calculate distance to add to current position
            double distanceWidth = (gameFrame.Map.ActualWidth / gameFrame.x) / fps;
            double distanceHeight = (gameFrame.Map.ActualHeight / gameFrame.y) / fps;

            // get current point
            Point position = new Point(Canvas.GetLeft(ell), Canvas.GetTop(ell));
            
            if (0 == (int)gameObject.nextDirection)//up
            {
                position.Y -= distanceHeight;
                if (((gameFrame.Map.ActualHeight / gameFrame.y) * (current.Value.y - 1) + ((gameFrame.Map.ActualHeight / gameFrame.y) - ell.Height) / 2) >= position.Y)
                {
                    gameObject.y--;
                }                                   
            }
            else if (1 == (int)gameObject.nextDirection)//right
            {
                position.X += distanceWidth;
                if (((gameFrame.Map.ActualWidth / gameFrame.x) * (current.Value.x + 1) + ((gameFrame.Map.ActualWidth / gameFrame.x) - ell.Width) / 2) <= position.X)
                {
                    gameObject.x++;
                }
            }
            else if (2 == (int)gameObject.nextDirection)//down
            {
                position.Y += distanceHeight;
                if (((gameFrame.Map.ActualHeight / gameFrame.y) * (current.Value.y + 1) + ((gameFrame.Map.ActualHeight / gameFrame.y) - ell.Height) / 2) <= position.Y)
                {
                    gameObject.y++;
                }
            }
            else//left
            {
                position.X -= distanceWidth;
                if (((gameFrame.Map.ActualWidth / gameFrame.x) * (current.Value.x - 1) + ((gameFrame.Map.ActualWidth / gameFrame.x) - ell.Width) / 2)  >= position.X)
                {
                    gameObject.x--;
                }
            }
            ell.SetValue(Canvas.TopProperty, position.Y);
            ell.SetValue(Canvas.LeftProperty, position.X);

            gameObject.currentDirection = gameObject.nextDirection;
        }        

        public static LinkedListNode<Spielobjekt> getCurrentPosition(LinkedList<Spielobjekt> weg, MoveableObject gameObject)
        {
            LinkedListNode<Spielobjekt> current = weg.First;
            do
            {
                if (current.Value.x == gameObject.x && current.Value.y == gameObject.y)
                    return current;
                current = current.Next;
            } while (current != null);

            return null;
        }
    }
}
