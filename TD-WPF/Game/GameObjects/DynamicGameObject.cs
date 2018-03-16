namespace TD_WPF.Game.GameObjects
{
    public abstract class DynamicGameObject : GameObject
    {
        public DynamicGameObject(float x, float y, float width, float height, float speed) : base(x, y, width, height)
        {
            this.speed = speed;
        }

        public float speed { get; set; }
    }
}