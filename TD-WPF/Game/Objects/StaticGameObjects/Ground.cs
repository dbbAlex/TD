using System;
using TD_WPF.Properties;

namespace TD_WPF.Game.Objects.StaticGameObjects
{
    public class Ground : Path
    {
        public const string Name = "Ground";
        public const int Money = 5;

        public Ground(double x, double y, double width, double height, int index) : base(x, y, width, height, index)
        {
            Image = Resource.ground;
        }

        public Tower Tower { get; set; } = null;
        public int UpdateSellMoney => Convert.ToInt32(Math.Ceiling(Money / 2d));

        public void Update(GameControl gameControl, long currentInterval)
        {
            if (!Active) return;
            base.Update(gameControl);
            Tower?.Update(gameControl, currentInterval);
        }

        public override void Render(GameControl gameControl)
        {
            base.Render(gameControl);
            Tower?.Render(gameControl);
        }

        public override void Deaktivate()
        {
            base.Deaktivate();
            Tower?.Deaktivate();
        }

        public override void Destroy(GameControl gameControl)
        {
            base.Destroy(gameControl);
            gameControl.GameCreator.Ground.Remove(this);
        }
    }
}