using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using TD_WPF.Game.Enumerations;
using TD_WPF.Menu;

namespace TD_WPF.Game.Objects.RoundObjects
{
    public class Waves
    {
        public Waves(long interval)
        {
            Interval = interval;
        }

        public Waves()
        {
        }

        public long Interval { get; set; }
        public List<Wave> WaveList { get; set; } = new List<Wave>();

        [ScriptIgnore] private int WaveIndex { get; set; }

        [ScriptIgnore] private bool Active { get; set; }

        public void Start(GameControl gameControl, long currentInterval)
        {
            if (Active) return;
            WaveList[WaveIndex].Start(gameControl, currentInterval);
            WaveIndex++;
            Active = true;
        }

        public void Update(GameControl gameControl, long currentInterval)
        {
            if (!Active)
            {
                MessageBox.Show("You have survived all waves");
                switch (gameControl.GameControlMode)
                {
                    case GameControlMode.CreateMap:
                        ((ContentControl) gameControl.Parent).Content = new EditorMenu();
                        break;
                    case GameControlMode.PlayRandom:
                        ((ContentControl) gameControl.Parent).Content = new GameMenu();
                        break;
                    case GameControlMode.EditMap:
                    case GameControlMode.PlayMap:
                        ((ContentControl) gameControl.Parent).Content = new MapMenu(gameControl.GameControlMode);
                        break;
                }

                gameControl.GameManager.EndLoop();
                return;
            }

            var wave = WaveList[WaveIndex - 1];
            if (wave.Active)
            {
                wave.Update(gameControl, currentInterval);
            }
            else if (WaveIndex < WaveList.Count && currentInterval - wave.LastInterval >= Interval)
            {
                WaveList[WaveIndex].Start(gameControl, currentInterval);
                WaveIndex++;
            }
            else if (WaveIndex == WaveList.Count && !wave.Active)
            {
                Active = false;
            }
        }

        public void Render(GameControl gameControl)
        {
            foreach (var item in WaveList.FindAll(wave => wave.Active)) item.Render(gameControl);
        }

        public void Deaktivate()
        {
            Active = false;
        }
    }
}