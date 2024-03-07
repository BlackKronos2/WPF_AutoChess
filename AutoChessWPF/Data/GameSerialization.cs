
namespace AutoChessWPF
{
    /// <summary> Сохранение и загрузка игры </summary>
    public class GameSerialization
    {
        #region FILES_NAMES

        private const string PlaySavesFile = "PlayersSave";

        private const string BattleGridSave = "BattleGridSave";
        private const string Player1HandSave = "Player1GridSave";
        private const string Player2HandSave = "Player2GridSave";

        private const string Player1Save = "Player1Save";
        private const string Player2Save = "Player2Save";

        #endregion

        private GameManager _game;

        public GameSerialization(GameManager game)
        {
            _game = game;
        }

        public void SaveGame(GameManager game)
        {
            SerializationDataBehavior.Save(game.BattleGrid, BattleGridSave);

            SerializationDataBehavior.Save(game.Player1, Player1Save);
            SerializationDataBehavior.Save(game.Player2, Player2Save);

            SerializationDataBehavior.Save(game.Player1Grid, Player1HandSave);
            SerializationDataBehavior.Save(game.Player2Grid, Player2HandSave);

            GameSettings.Default.SavedGame = true;
            GameSettings.Default.Save();
        }
        public static GameManager LoadGame()
        {
            BattleGrid battleGrid = SerializationDataBehavior.Load<BattleGrid>(BattleGridSave);

            Player player1 = SerializationDataBehavior.Load<Player>(Player1Save);
            Player player2 = SerializationDataBehavior.Load<Player>(Player2Save);

            PlayerGrid playerGrid1 = SerializationDataBehavior.Load<PlayerGrid>(Player1HandSave);
            PlayerGrid playerGrid2 = SerializationDataBehavior.Load<PlayerGrid>(Player2HandSave);

            GameManager loadedGame = new GameManager(battleGrid, playerGrid1, playerGrid2, player1, player2);

            loadedGame.BattleGrid.UnitsInit();
            loadedGame.Player1Grid.UnitsInit();
            loadedGame.Player2Grid.UnitsInit();

            return loadedGame;
        }
        public static void DeleteSave()
        {
            GameSettings.Default.SavedGame = false;
            GameSettings.Default.Save();
        }


        public void SaveBattle()
        {
            SerializationDataBehavior.Save(_game.BattleGrid, BattleGridSave);
        }
        public static BattleGrid LoadBattle()
        {
            BattleGrid battleGrid = SerializationDataBehavior.Load<BattleGrid>(BattleGridSave);
            return battleGrid;
        }
    }
}
