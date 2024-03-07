using System.Runtime.Serialization;

namespace AutoChessWPF
{
    /// <summary> Поле персонажей игрока </summary>
    [DataContract]
    public class PlayerGrid : GameGrid
    {
        private const int UnitCount = 5;

        [DataMember]
        private GameSide _gameSide;

        public PlayerGrid(GameSide gameSide, int width, int height) : base(width, height)
        {
            _gameSide = gameSide;
            Reload();

            GameEvents.OnFightEnd += Reload;
        }

        /// <summary> Добавить случайного персонажа на поле </summary>
        private void AddUnit(int y)
        {
            if (GridWidth == Units.Count)
                return;

            UnitInfo info = UnitsDatabase.GetInstance().GetRandomStartUnit();
            int x = 0;

            GameUnit newUnit = new GameUnit(info, _gameSide);
            PutUnit(newUnit, x, y);
        }

        /// <summary> Пересоздать персонажей на поле </summary>
        private void Reload()
        {
            ClearGrid();

            for(int i = 0; i < UnitCount; i++)
                AddUnit(i);
        }
    }
}
