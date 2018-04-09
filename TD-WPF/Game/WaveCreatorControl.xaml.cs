using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TD_WPF.DataBase;
using TD_WPF.Game.Enumerations;
using TD_WPF.Game.Objects.DynamicGameObjects;
using TD_WPF.Game.Objects.RoundObjects;
using TD_WPF.Game.Save;
using TD_WPF.Menu;
using TD_WPF.Menu.Dialog;

namespace TD_WPF.Game
{
    public partial class WaveCreatorControl : UserControl
    {
        private const long IntervalWaves = 5000;
        private const long IntervalEnemies = 2000;
        private const int Health = 10;
        private const int Damage = 20;
        private const int Money = 20;
        private const double Speed = 0.09;

        public WaveCreatorControl(DbObject dbObject, GameControlMode gameControlMode)
        {
            InitializeComponent();
            Loaded += Initialize;
            DbObject = dbObject;
            GameControlMode = gameControlMode;
        }

        private DbObject DbObject { get; }
        private int SelectedWaveIndex { get; set; }
        private int SelectedEnemyIndex { get; set; }
        private GameControlMode GameControlMode { get; }

        private void Initialize(object sender, RoutedEventArgs e)
        {
            LoadFromSaveObject();
        }

        private void HighlightSelection()
        {
            foreach (var item in WavePanel.Children)
            {
                if (!(item is Label label)) continue;
                var i = int.Parse(label.Name.Substring("Wave".Length, label.Name.Length - "Wave".Length));
                label.Background = i == SelectedWaveIndex
                    ? new SolidColorBrush(Color.FromArgb(150, 124, 252, 0))
                    : Brushes.Transparent;
            }

            foreach (var item in EnemyPanel.Children)
            {
                if (!(item is Label label)) continue;
                var i = int.Parse(label.Name.Substring("Enemy".Length, label.Name.Length - "Enemy".Length));
                label.Background = i == SelectedEnemyIndex
                    ? new SolidColorBrush(Color.FromArgb(150, 124, 252, 0))
                    : Brushes.Transparent;
            }
        }

        private void SaveAndReturnToMenu()
        {
            if (GameControlMode == GameControlMode.CreateMap)
            {
                DbObject.MetaData.CreationDate = DateTime.Now;
                DbObject.MetaData.Guid = Guid.NewGuid();
            }

            DbObject.MetaData.ModifiedDate = DateTime.Now;


            if (GameControlMode == GameControlMode.EditMap)
            {
                DbManager.SaveModifiedMapToDataBase(DbObject);
                ((ContentControl) Parent).Content = new MapMenu(GameControlMode);
            }
            else
            {
                DbManager.SaveMapToDataBase(DbObject);
                ((ContentControl) Parent).Content = new EditorMenu();
            }
        }

        #region selection event

        private void MouseWaveEnemySelection(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Label label)) return;
            label.Background = new SolidColorBrush(Color.FromArgb(150, 124, 252, 0));
            if (label.Name.Contains("Wave"))
            {
                SelectedWaveIndex = int.Parse(label.Name.Substring("Wave".Length, label.Name.Length - "Wave".Length));
                LoadWaveContent();
            }
            else
            {
                SelectedEnemyIndex =
                    int.Parse(label.Name.Substring("Enemy".Length, label.Name.Length - "Enemy".Length));
                LoadEnemyContent();
            }
        }

        #endregion

        #region update methods

        private void UpdateEnemyPosition()
        {
            var spawn = DbObject.GameData.Paths[0];
            foreach (var wave in DbObject.GameData.Waves.WaveList)
            {
                var enemies = wave.Enemies.FindAll(enemy => enemy.X != spawn.X && enemy.Y != spawn.Y);
                foreach (var enemy in enemies)
                {
                    enemy.X = spawn.X;
                    enemy.Y = spawn.Y;
                }
            }
        }

        #endregion

        #region create methods

        private void CreateNewWave()
        {
            DbObject.GameData.Waves.WaveList.Add(new Wave(IntervalEnemies));
            SelectedWaveIndex = DbObject.GameData.Waves.WaveList.Count - 1;
            Createlabel(SelectedWaveIndex, WavePanel, true);
            UnregisterPanelLabel(EnemyPanel);
            EnemyPanel.Children.Clear();
            CreateNewEnemy();
            LoadWaveContent();
            HighlightSelection();
        }

        private void CreateNewEnemy()
        {
            var wave = DbObject.GameData.Waves.WaveList[SelectedWaveIndex];
            var spawn = DbObject.GameData.Paths[0];
            wave.Enemies.Add(new Enemy(spawn.X, spawn.Y, spawn.Width, spawn.Height, Speed, Health, Damage, Money));
            SelectedEnemyIndex = DbObject.GameData.Waves.WaveList[SelectedWaveIndex].Enemies.Count - 1;
            Createlabel(SelectedEnemyIndex, EnemyPanel, true);
            LoadEnemyContent();
            HighlightSelection();
        }

        private void Createlabel(int i, StackPanel panel, bool isSelected)
        {
            var label = new Label
            {
                Name = (panel.Name.Equals("WavePanel") ? "Wave" : "Enemy") + i,
                Content = i + 1 + ". " + (panel.Name.Equals("WavePanel") ? "Wave" : "Enemy"),
                FontFamily = new FontFamily("Bauhaus 93"),
                FontSize = 20,
                Background =
                    !isSelected ? Brushes.Transparent : new SolidColorBrush(Color.FromArgb(150, 124, 252, 0))
            };
            label.MouseLeftButtonUp += MouseWaveEnemySelection;
            panel.RegisterName(label.Name, label);
            panel.Children.Add(label);
        }

        #endregion

        #region delete methods

        private void DeleteWave()
        {
            var label = (Label) WavePanel.FindName("Wave" + (DbObject.GameData.Waves.WaveList.Count - 1));
            if (label != null)
            {
                WavePanel.UnregisterName(label.Name);
                WavePanel.Children.Remove(label);
            }

            DbObject.GameData.Waves.WaveList.RemoveAt(SelectedWaveIndex);
            if (SelectedWaveIndex == DbObject.GameData.Waves.WaveList.Count) SelectedWaveIndex--;
            if (DbObject.GameData.Waves.WaveList.Count <= 0)
            {
                CreateNewWave();
                SelectedWaveIndex = 0;
                var dialog = new Dialog(Window.GetWindow(this), DialogType.WaveException);
                dialog.ShowDialog();
            }

            LoadWaveContent();
            HighlightSelection();
        }

        private void DeleteEnemy()
        {
            var label = (Label) EnemyPanel.FindName(
                "Enemy" + (DbObject.GameData.Waves.WaveList[SelectedWaveIndex].Enemies.Count - 1));
            if (label != null)
            {
                EnemyPanel.UnregisterName(label.Name);
                EnemyPanel.Children.Remove(label);
            }

            DbObject.GameData.Waves.WaveList[SelectedWaveIndex].Enemies.RemoveAt(SelectedEnemyIndex);
            if (SelectedEnemyIndex == DbObject.GameData.Waves.WaveList[SelectedWaveIndex].Enemies.Count)
                SelectedEnemyIndex--;
            if (DbObject.GameData.Waves.WaveList[SelectedWaveIndex].Enemies.Count <= 0)
            {
                CreateNewEnemy();
                SelectedEnemyIndex = 0;
                var dialog = new Dialog(Window.GetWindow(this), DialogType.EnemyException);
                dialog.ShowDialog();
            }

            LoadEnemyContent();
            HighlightSelection();
        }

        #endregion

        #region load content

        private void LoadFromSaveObject()
        {
            if (DbObject.GameData.Waves != null)
            {
                WavesInterval.Text = DbObject.GameData.Waves.Interval.ToString(CultureInfo.InvariantCulture);
                for (var i = 0; i < DbObject.GameData.Waves.WaveList.Count; i++) Createlabel(i, WavePanel, i == 0);
                LoadWaveContent();
                UpdateEnemyPosition();
            }
            else
            {
                DbObject.GameData.Waves = new Waves(IntervalWaves);
                CreateNewWave();
            }
        }

        private void LoadWaveContent()
        {
            var wave = DbObject.GameData.Waves.WaveList[SelectedWaveIndex];
            EnemyInterval.Text = wave.Interval.ToString(CultureInfo.InvariantCulture);
            UnregisterPanelLabel(EnemyPanel);
            EnemyPanel.Children.Clear();
            for (var i = 0; i < wave.Enemies.Count; i++) Createlabel(i, EnemyPanel, i == 0);

            SelectedEnemyIndex = 0;
            LoadEnemyContent();
        }

        private void UnregisterPanelLabel(StackPanel panel)
        {
            foreach (var item in panel.Children)
                if (item is Label label)
                    panel.UnregisterName(label.Name);
        }

        private void LoadEnemyContent()
        {
            var enemy = DbObject.GameData.Waves.WaveList[SelectedWaveIndex].Enemies[SelectedEnemyIndex];
            EnemyHealth.Text = enemy.Health.ToString();
            EnemyDamage.Text = enemy.Damage.ToString();
            EnemyMoney.Text = enemy.Money.ToString();
            EnemySpeed.Value = enemy.Speed;
            HighlightSelection();
        }

        #endregion

        #region EventHandler

        private void ControlPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void ControlLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.Source is Slider slider)
                DbObject.GameData.Waves.WaveList[SelectedWaveIndex].Enemies[SelectedEnemyIndex].Speed = slider.Value;

            if (!(e.Source is TextBox textBox)) return;
            switch (textBox.Name)
            {
                case "WavesInterval":
                    if (string.IsNullOrWhiteSpace(textBox.Text) || long.Parse(textBox.Text) <= 0)
                    {
                        DbObject.GameData.Waves.Interval = IntervalWaves;
                        textBox.Text = IntervalWaves.ToString();
                    }
                    else
                    {
                        DbObject.GameData.Waves.Interval = long.Parse(textBox.Text);
                    }

                    break;
                case "EnemyInterval":
                    if (string.IsNullOrWhiteSpace(textBox.Text) || long.Parse(textBox.Text) <= 0)
                    {
                        DbObject.GameData.Waves.WaveList[SelectedWaveIndex].Interval = IntervalEnemies;
                        textBox.Text = IntervalEnemies.ToString();
                    }
                    else
                    {
                        DbObject.GameData.Waves.WaveList[SelectedWaveIndex].Interval = long.Parse(textBox.Text);
                    }

                    break;
                case "EnemyHealth":
                    if (string.IsNullOrWhiteSpace(textBox.Text) || int.Parse(textBox.Text) <= 0)
                    {
                        DbObject.GameData.Waves.WaveList[SelectedWaveIndex].Enemies[SelectedEnemyIndex].Health = Health;
                        textBox.Text = Health.ToString();
                    }
                    else
                    {
                        DbObject.GameData.Waves.WaveList[SelectedWaveIndex].Enemies[SelectedEnemyIndex].Health =
                            int.Parse(textBox.Text);
                    }

                    break;
                case "EnemyDamage":
                    if (string.IsNullOrWhiteSpace(textBox.Text) || int.Parse(textBox.Text) <= 0)
                    {
                        DbObject.GameData.Waves.WaveList[SelectedWaveIndex].Enemies[SelectedEnemyIndex].Damage = Damage;
                        textBox.Text = Damage.ToString();
                    }
                    else
                    {
                        DbObject.GameData.Waves.WaveList[SelectedWaveIndex].Enemies[SelectedEnemyIndex].Damage =
                            int.Parse(textBox.Text);
                    }

                    break;
                case "EnemyMoney":
                    if (string.IsNullOrWhiteSpace(textBox.Text) || int.Parse(textBox.Text) <= 0)
                    {
                        DbObject.GameData.Waves.WaveList[SelectedWaveIndex].Enemies[SelectedEnemyIndex].Money = Money;
                        textBox.Text = Damage.ToString();
                    }
                    else
                    {
                        DbObject.GameData.Waves.WaveList[SelectedWaveIndex].Enemies[SelectedEnemyIndex].Money =
                            int.Parse(textBox.Text);
                    }

                    break;
            }
        }

        private void ControlClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button)) return;
            switch (button.Name)
            {
                case "AddWave":
                    CreateNewWave();
                    break;
                case "RemoveWave":
                    DeleteWave();
                    break;
                case "RemoveEnemy":
                    DeleteEnemy();
                    break;
                case "AddEnemy":
                    CreateNewEnemy();
                    break;
                case "Back":
                    ((ContentControl) Parent).Content = new GameControl(DbObject, GameControlMode);
                    break;
                case "Done":
                    SaveAndReturnToMenu();
                    break;
            }
        }

        #endregion
    }
}