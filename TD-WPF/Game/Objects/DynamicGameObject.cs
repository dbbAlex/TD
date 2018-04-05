namespace TD_WPF.Game.Objects
{
    public abstract class DynamicGameObject : GameObject
    {
        protected DynamicGameObject(double x, double y, double width, double height, double speed) : base(x, y, width,
            height)
        {
            Speed = speed;
        }

        public double Speed { get; set; }
    }
}