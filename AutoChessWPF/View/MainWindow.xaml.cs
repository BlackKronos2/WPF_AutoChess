using System;
using System.Windows;

namespace AutoChessWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadGame.IsEnabled = (GameSettings.Default.SavedGame);
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            GameScence gameScence = new GameScence();
            gameScence.Show();
            this.Hide();
        }

        private void LoadGame_Click(object sender, RoutedEventArgs e)
        {
            GameManager game = GameSerialization.LoadGame();
            GameScence gameScence = new GameScence(game);
            gameScence.Show();
            this.Hide();
        }

        private void Window_Closed(object sender, System.EventArgs e) => Environment.Exit(0);

    }
}
