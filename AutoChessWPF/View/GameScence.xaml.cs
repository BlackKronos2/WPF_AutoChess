using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace AutoChessWPF
{
    /// <summary>
    /// Логика взаимодействия для GameScence.xaml
    /// </summary>
    public partial class GameScence : Window
    {
        #region FIEDLS

        private GameManager _gameManager;

        #region PRESENT

        private GamePresenter _gamePresenter;

        private BattleGridPresenter _battleGridPresenter;

        private GameGridPresenter _player1GridPresenter;
        private GameGridPresenter _player2GridPresenter;

        #endregion

        private DispatcherTimer timer = new DispatcherTimer();

        #endregion

        public GameScence()
        {
            _gameManager = new GameManager();

            InitializeComponent();
            Init();
        }

        public GameScence(GameManager game)
        {
            _gameManager = game;

            InitializeComponent();
            Init();

            GameManager.SelectedUnit = null;
            _gameManager.BattleGrid.Enable = true;
        }

        private void Init()
        {
            timer.Interval = TimeSpan.FromSeconds(0.5f);
            timer.Tick += new EventHandler(Update);

            GameEvents.OnFightStart += () => timer.Start();
            GameEvents.OnFightEnd += () => timer.Stop();

            GameEvents.OnFightStart += () => StartButtonEnable(false);
            GameEvents.OnFightEnd += () => StartButtonEnable(true);

            GameEvents.OnGameEnd += OnGameEnd;

            _player1GridPresenter = new GameGridPresenter(Player1Hand, _gameManager.Player1Grid);
            _player2GridPresenter = new GameGridPresenter(Player2Hand, _gameManager.Player2Grid);
            _battleGridPresenter = new BattleGridPresenter(FightGrid, _gameManager.BattleGrid);
            _gamePresenter = new GamePresenter(_gameManager, SelectedUnitImage, SelectedUnitInfoPanel);

            DataContext = _gamePresenter;

            ViewUpdate();
            GameEvents.OnLastUpdate += ViewUpdate;
        }

        #region INPUT

        private void FightStartButton_Click(object sender, RoutedEventArgs e)
        {
            _gameManager.SaveBattle();
            GameEvents.SendFightStart();
        }
        private void Player1Hand_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _player1GridPresenter.CellChoise();
        }
        private void Player2Hand_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _player2GridPresenter.CellChoise();
        }
        private void FightGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //_battleGridPresenter.CellChoise();
        }

        #endregion

        #region EVENTS

        private void Update(object sender, EventArgs e)
        {
            _gamePresenter.Update();
            ViewUpdate();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _battleGridPresenter.CellChoise();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            GameSerialization saveGame = new GameSerialization(_gameManager);
            saveGame.SaveGame(_gameManager);

            Environment.Exit(0);
        }

        #endregion 

        private void StartButtonEnable(bool value) => FightStartButton.IsEnabled = value;

        private void ViewUpdate()
        {
            _gamePresenter.PlayerViewUpdate();

            _battleGridPresenter.DrawMap();
            _player1GridPresenter.DrawMap();
            _player2GridPresenter.DrawMap();
        }

        private void OnGameEnd(GameSide win)
        {
            MainWindow main = new MainWindow();

            MessageBox.Show(win == GameSide.PLAYER2 ? "Первый игрок победил!" : "Второй игрок победил!", "Игра окончена");

            main.Show();
            this.Hide();
        }
    }
}
