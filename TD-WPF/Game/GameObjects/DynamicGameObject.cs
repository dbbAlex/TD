namespace TD_WPF.Game.GameObjects
{
    public abstract class DynamicGameObject : GameObject
    {
        protected DynamicGameObject(float x, float y, float width, float height, float speed) : base(x, y, width, height)
        {
            Speed = 1 * speed;
        }

        protected float Speed { get; private set; }
    }
}