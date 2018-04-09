using System;
using System.Drawing;
using System.Web.Script.Serialization;
using TD_WPF.Game.Enumerations;
using TD_WPF.Properties;

namespace TD_WPF.Game.Objects.StaticGameObjects.TowerExtensions
{
    public class Sniper : Tower
    {
        public new const string Name = "Sniper";
        public new const int Money = 35;
        public new const int Damage = 15;
        public new const double Range = 7;
        private const double Interval = 900;
        private const double Speed = 1.3;
        
        public Sniper(double x, double y, double width, double height,
            TargetCondition targetCondition = TargetCondition.Closest)
            : base(x, y, width, height, targetCondition)
        {
        }
        
        [ScriptIgnore] public override Bitmap Image { get; } = Resource.sniper;
        public override int FireDamage { get; set; } = Damage;
        public override double FireRange { get; set; } = Range;
        public override double FireInterval { get; set; } = Interval;
        public override double FireSpeed { get; set; } = Speed;
        public override int UpdateSellMoney => Convert.ToInt32(Math.Ceiling(Money / 2d));
        
    }
}