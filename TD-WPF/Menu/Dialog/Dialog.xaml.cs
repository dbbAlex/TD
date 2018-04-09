using System;
using System.Windows;
using System.Windows.Controls;
using TD_WPF.Game.Enumerations;

namespace TD_WPF.Menu.Dialog
{
    public partial class Dialog : Window
    {
        private const string GameOver = "Game Over";
        private const string Victory = "Victory";
        private const string GameOverMessage = "You did not survive all the waves";
        private const string VictoryMessage = "You have survived all waves";
        private const string Attention = "Attention";
        private const string WaveException = "There should be at least on wave, we have added a wave for you";
        private const string EnemyException =
            "There should be at least one enemy in this wave, we have added an enemy for you";
        
        public Dialog(Window owner, DialogType dialogType)
        {
            InitializeComponent();
            Owner = owner;
            DialogType = dialogType;
            Loaded += Initialize;
        }

        private void Initialize(object sender, RoutedEventArgs e)
        {
            switch (DialogType)
            {
                case DialogType.GameOver:
                    Headline.Content = GameOver;
                    Message.Content = GameOverMessage;
                    break;
                case DialogType.Victory:
                    Headline.Content = Victory;
                    Message.Content = VictoryMessage;
                    break;
                case DialogType.WaveException:
                    Headline.Content = Attention;
                    Message.Content = WaveException;
                    break;
                case DialogType.EnemyException:
                    Headline.Content = Attention;
                    Message.Content = EnemyException;
                    break;
            }
        }

        private DialogType DialogType { get; }

        private void ControlClickHandler(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}