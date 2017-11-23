using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TD_WPF
{
    class Menu
    {
        public static StackPanel getMenuContainer()
        {
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Vertical;
            //panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            //Hintergrund
            ImageBrush imgBrush = new ImageBrush();
            var uri = new Uri("pack://application:,,,/Grafik/grass.jpg");
            BitmapImage image = new BitmapImage(uri);
            imgBrush.ImageSource = image;
            panel.Background = imgBrush;


            //Label
            Label lbl = new Label();
            lbl.Content = "Tower Defense";
            lbl.FontFamily = new FontFamily("Bauhaus 93");
            lbl.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            lbl.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            lbl.FontSize = 100;
            var lblMargin = lbl.Margin;
            lblMargin.Bottom = 60;
            lblMargin.Top = 100;
            lbl.Margin = lblMargin;
            panel.Children.Add(lbl);


            //Buttons
            int btnBottomMargin = 35;
            CornerButton btnGame = new CornerButton();
            btnGame.Content = "Spiel starten";
            btnGame.FontSize = 50;
            btnGame.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            btnGame.Background = Brushes.Transparent;
            btnGame.BorderBrush = Brushes.OrangeRed;
            var gameMargin = btnGame.Margin;
            gameMargin.Bottom = btnBottomMargin;
            btnGame.Margin = gameMargin;
            btnGame.FontFamily = new FontFamily("Bauhaus 93");
            btnGame.CornerRadius = new CornerRadius(10);
            btnGame.Style = (Style)Application.Current.FindResource("RoundButton");
            panel.Children.Add(btnGame);

            CornerButton btnMap = new CornerButton();
            btnMap.Content = "Map Editor";
            btnMap.FontSize = 50;
            btnMap.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            btnMap.Background = Brushes.Transparent;
            btnMap.BorderBrush = Brushes.OrangeRed;
            var mapMargin = btnMap.Margin;
            mapMargin.Bottom = btnBottomMargin;
            btnMap.Margin = mapMargin;
            btnMap.FontFamily = new FontFamily("Bauhaus 93");
            btnMap.CornerRadius = new CornerRadius(100);
            btnMap.Style = (Style)Application.Current.FindResource("RoundButton");
            panel.Children.Add(btnMap);

            CornerButton btnScore = new CornerButton();
            btnScore.Content = "Highscores";
            btnScore.FontSize = 50;
            btnScore.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            btnScore.Background = Brushes.Transparent;
            btnScore.BorderBrush = Brushes.OrangeRed;
            var scoreMargin = btnScore.Margin;
            scoreMargin.Bottom = btnBottomMargin;
            btnScore.Margin = scoreMargin;
            btnScore.FontFamily = new FontFamily("Bauhaus 93");
            btnScore.CornerRadius = new CornerRadius(100);
            btnScore.Style = (Style)Application.Current.FindResource("RoundButton");
            panel.Children.Add(btnScore);

            CornerButton btnSettings = new CornerButton();
            btnSettings.Content = "Einstellungen";
            btnSettings.FontSize = 50;
            btnSettings.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            btnSettings.Background = Brushes.Transparent;
            btnSettings.BorderBrush = Brushes.OrangeRed;
            var settingMargin = btnSettings.Margin;
            settingMargin.Bottom = btnBottomMargin;
            btnSettings.Margin = settingMargin;
            btnSettings.FontFamily = new FontFamily("Bauhaus 93");
            btnSettings.CornerRadius = new CornerRadius(100);
            btnSettings.Style = (Style)Application.Current.FindResource("RoundButton");
            panel.Children.Add(btnSettings);





            return panel;
        }
    }
}
