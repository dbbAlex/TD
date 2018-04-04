namespace TD_WPF.Game.Objects
{
    public abstract class DynamicGameObject : GameObject
    {
        protected DynamicGameObject(float x, float y, float width, float height, float speed) : base(x, y, width,
            height)
        {
            Speed = speed;
        }

        protected float Speed { get; }
    }
}