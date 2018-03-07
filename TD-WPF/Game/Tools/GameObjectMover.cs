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

        public static void startTimerForMovableObject(GameFrame gameFrame, LinkedList<Spielobjekt> weg, MoveableObject gameObject)
        {
            int fps = 1000/60;
            gameObject.x = weg.First.Value.x;
            gameObject.y = weg.First.Value.y;
            gameObject.currentDirection = getNextDirection(weg.First.Value, weg.First.Next.Value);
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
            Point position = new Point(gameObject.x * (gameFrame.Map.ActualWidth / gameFrame.x), gameObject.y * (gameFrame.Map.ActualHeight / gameFrame.y));
            Canvas.SetLeft(rec, position.X + 1);
            Canvas.SetTop(rec, position.Y + 1);
            gameFrame.Map.Children.Add(rec);

            System.Timers.Timer timer = new System.Timers.Timer(fps); // 60 fps
            timer.Elapsed += async (sender, e) => await gameFrame.Dispatcher.InvokeAsync((Action)(
                () => {
                    if(!(gameObject.x == weg.Last.Value.x && gameObject.y == weg.Last.Value.y))
                        handleEnemyMove(gameFrame, weg, gameObject, fps, rec);
                    else
                    {
                        gameFrame.Map.Children.Remove(rec);
                        timer.Stop();
                        timer.Close();
                    }
                }));
            timer.Start();
        }

        public static void handleEnemyMove(GameFrame gameFrame, LinkedList<Spielobjekt> weg, MoveableObject gameObject, int fps, Rectangle rec)
        {
            LinkedListNode<Spielobjekt> current = getCurrentPosition(weg, gameObject);
            LinkedListNode<Spielobjekt> next = current.Next;

            if (next != null)
                gameObject.nextDirection = getNextDirection(current.Value, next.Value);
            else
                gameObject.nextDirection = gameObject.currentDirection;
            double distanceWidth = (gameFrame.Map.ActualWidth / gameFrame.x) / fps;
            double distanceHeight = (gameFrame.Map.ActualHeight / gameFrame.y) / fps;
            Point position = new Point(Canvas.GetLeft(rec), Canvas.GetTop(rec));

            gameObject.currentDirection = gameObject.nextDirection;
            gameObject.nextDirection = getNextDirection(current.Value, next.Value);

            if (0 == (int)gameObject.nextDirection)
            {
                position.Y -= distanceHeight;
                if (((gameFrame.Map.ActualHeight / gameFrame.y) * next.Value.y) - 1 >= position.Y)
                    gameObject.y = next.Value.y;
            }
            else if (1 == (int)gameObject.nextDirection)
            {
                position.X += distanceWidth;
                if (((gameFrame.Map.ActualWidth / gameFrame.x) * next.Value.x) - 1 <= position.X)
                    gameObject.x = next.Value.x;
            }
            else if (2 == (int)gameObject.nextDirection)
            {
                position.Y += distanceHeight;
                if (((gameFrame.Map.ActualHeight / gameFrame.y) * next.Value.y) - 1 <= position.Y)
                    gameObject.y = next.Value.y;
            }
            else
            {
                position.X -= distanceWidth;
                if (((gameFrame.Map.ActualWidth / gameFrame.x) * next.Value.x) - 1 >= position.X)
                    gameObject.x = next.Value.x;
            }
            rec.SetValue(Canvas.TopProperty, position.Y);
            rec.SetValue(Canvas.LeftProperty, position.X);

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
