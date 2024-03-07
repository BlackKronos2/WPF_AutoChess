using System;
using System.Runtime.Serialization;

namespace AutoChessWPF
{
    /// <summary> Основа логики игры </summary>
    public class GameManager
    {
        #region SETTINGS

        private const int PlayerHealth = 50;
        public const int PlayerMana = 10;

        private static GridVector BattleGridSize = new GridVector(9, 6);
        private static GridVector PlayerGridSize = new GridVector(1, 6);

        public const int PlayerCellLimit = 2;

        #endregion

        #region FIELDS

        private static GameUnit? s_selectedUnit;

        [DataMember]
        private Player _player1;
        [DataMember]
        private Player _player2;

        [DataMember]
        private GameSerialization _gameSave;

        #endregion

        #region PROPERTIES

        public Player Player1 { set { _player1 = value; } get { return _player1; } }
        public Player Player2 { set { _player2 = value; } get { return _player2; } }


        [DataMember]
        public BattleGrid BattleGrid { get; private set; }

        [DataMember]
        public PlayerGrid Player1Grid { get; private set; }
        [DataMember]
        public PlayerGrid Player2Grid { get; private set; }

        #endregion

        public static Action? OnSelectedUnitChange { get; set; }

        public static GameUnit? SelectedUnit { 
            get { return s_selectedUnit; }
            set { 
                s_selectedUnit = value;
                OnSelectedUnitChange?.Invoke();
            } 
        }

        public GameManager()
        {
            _player1 = new Player(PlayerHealth, PlayerMana);
            _player2 = new Player(PlayerHealth, PlayerMana);

            BattleGrid = new BattleGrid(BattleGridSize.X, BattleGridSize.Y);

            Player1Grid = new PlayerGrid(GameSide.PLAYER1, PlayerGridSize.X, PlayerGridSize.Y);
            Player2Grid = new PlayerGrid(GameSide.PLAYER2, PlayerGridSize.X, PlayerGridSize.Y);

            BattleGrid.Player1 = _player1;
            BattleGrid.Player2 = _player2;

            Init();
        }

        public GameManager(BattleGrid battleGrid, PlayerGrid player1Grid, PlayerGrid player2Grid, Player player1, Player player2)
        {
            _player1 = player1;
            _player2 = player2;

            BattleGrid = battleGrid;

            Player1Grid = player1Grid;
            Player2Grid = player2Grid;

            BattleGrid.Player1 = _player1;
            BattleGrid.Player2 = _player2;

            Init();
        }

        /// <summary> Инициализация </summary>
        private void Init()
        {
            _gameSave = new GameSerialization(this);

            GameEvents.OnFightStart += SaveBattle;
            GameEvents.OnFightEnd += LoadBattle;

            GameEvents.OnFightStart += () => SelectedUnit = null;
            GameEvents.OnFightEnd += () => SelectedUnit = null;
            GameEvents.OnGameEnd += (GameSide win) => GameSerialization.DeleteSave();
        }

        public void Update()
        {
            BattleGrid.Update();
        }

        public void SaveBattle() => _gameSave.SaveBattle();

        public void LoadBattle()
        {
            BattleGrid.ClearGrid();

            BattleGrid load = GameSerialization.LoadBattle();
            BattleGrid.LoadUnits(load.Units);

            BattleGrid.Enable = true;

            GameEvents.SendOnLastUpdate();
            OnSelectedUnitChange?.Invoke();
        }

    }
}
