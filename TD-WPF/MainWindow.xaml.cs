using System.Windows;
using TD_WPF.DataBase;

namespace TD_WPF
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += Initialize;
        }

        private void Initialize(object sender, RoutedEventArgs e)
        {
            DbManager.CreateDataBase();
        }
    }
}