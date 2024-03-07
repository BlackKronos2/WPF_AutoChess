using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace AutoChessWPF
{
    /// <summary> Этап сражения </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GameStage
    { 
        NONE,
        FIGHT,
        FINAL_WAIT,
        FINAL
    }

    /// <summary> Игровое поле на котором сражаются персонажи </summary>
    [DataContract]
    public class BattleGrid : GameGrid
    {
        [DataMember]
        private int _fightStage;
        [DataMember]
        private int _delay;

        [DataMember]
        private GameStage _gameStage;

        [DataMember]
        public Player Player1 { get; set; }
        [DataMember]
        public Player Player2 { get; set; }

        private const int Delay = 5;
        private const int UnitsUpdateLevelCount = 3;

        public BattleGrid(int width, int height) : base(width, height)
        {
            _fightStage = -1;

            GameEvents.OnFightStart += StartFight;
            _gameStage = GameStage.NONE;
        }

        public override void ChoiseCell(int x, int y)
        {
            bool putUnit = GameManager.SelectedUnit != null
                && GameManager.SelectedUnit.CurrentGameGrid != this
                && Enable;

            if (!putUnit)
            {
                base.ChoiseCell(x, y);
                return;
            }

            GameUnit unit = GameManager.SelectedUnit;

            if (!CellLimitCheck(unit.GameSide, x, y))
                return;

            if (Units.Any(u => u.Position.X == x && u.Position.Y == y))
                return;

            PutUnit(unit, x , y);
        }

        #region FIGHT

        /// <summary> Начало боя </summary>
        private void StartFight()
        {
            _fightStage = 0;
            _gameStage = GameStage.FIGHT;

            ListDistribution();
        }

        /// <summary> Распределение приоритетов хода </summary>
        private void ListDistribution()
        {
            var player1Units = Units.Where(u => u.GameSide == GameSide.PLAYER1).ToList();
            var player2Units = Units.Where(u => u.GameSide == GameSide.PLAYER2).ToList();

            var maxList = (player1Units.Count >= player2Units.Count) ? player1Units : player2Units;
            var minList = (player1Units.Count < player2Units.Count) ? player1Units : player2Units;

            Units.Clear();

            int index = 0;

            while(index < minList.Count)
            {
                Units.Add(minList[index]);
                Units.Add(maxList[index]);

                index++;
            }

            for (int i = index; i < maxList.Count; i++)
                Units.Add(maxList[i]);
        }

        /// <summary> Стадия активного боя </summary>
        private void BattlePhase()
        {
            foreach (GameUnit unit in Units)
                unit.State = UnitState.STAY;

            Units[_fightStage % Units.Count].UnitTurn();
            _fightStage++;

            int player1UnitsCount = Units.Count(u => u.GameSide == GameSide.PLAYER1);
            int player2UnitsCount = Units.Count(u => u.GameSide == GameSide.PLAYER2);

            if (player1UnitsCount == 0 || player2UnitsCount == 0)
            {
                _gameStage = GameStage.FINAL_WAIT;
                _delay = Delay;
            }
        }

        /// <summary> Удаление мертвых персонажей с поля </summary>
        private void UnitsListVerif()
        {
            Units = Units.Where(u => u.IsAlive).ToList();


        }

        #endregion

        #region FIGHT_FINAL

        /// <summary> Ожидание конца боя </summary>
        private void FinalWait()
        {
            foreach (GameUnit unit in Units)
                unit.State = UnitState.STAY;

            if (_delay-- > 0)
                return;

            _gameStage = GameStage.FINAL;
            _fightStage = 0;
        }

        /// <summary> Последняя стадия сражения - атака игрока </summary>
        private void FinalPhase()
        {
            if (_fightStage < Units.Count)
            { 
                GameUnit unit = Units[_fightStage];
                unit.Attack((unit.GameSide == GameSide.PLAYER1) ? Player2 : Player1);
            }

            _fightStage++;

            if (_fightStage < Units.Count + Delay)
                return;

            EndFight();
            _gameStage = GameStage.NONE;
        }

        /// <summary> Конец боя </summary>
        private void EndFight()
        {
            _fightStage = -1;

            GameEvents.SendOnFightEnd();
            GameEvents.SendOnLastUpdate();
        }

        #endregion

        /// <summary> Обновление состояния </summary>
        public void Update()
        {
            switch (_gameStage)
            {
                case GameStage.NONE: return;
                case GameStage.FIGHT: UnitsListVerif(); BattlePhase(); break;
                case GameStage.FINAL_WAIT: UnitsListVerif(); FinalWait();  break;
                case GameStage.FINAL: FinalPhase(); break;
            }
        }

        #region PLAYER_CELL_LIMIT

        /// <summary> Ограничения для перемещения персонажа на поле </summary>
        private bool CellLimitCheck(GameSide playerSide, int x, int y)
        {
            if (playerSide == GameSide.PLAYER1)
                return x <= GameManager.PlayerCellLimit - 1;
            else
                return x >= GridHeight - GameManager.PlayerCellLimit;
        }

        /// <summary> Поля доступные для новых персонажей </summary>
        public List<GridVector> PlayerCells(GameSide playerSide)
        {
            List<GridVector> list = new List<GridVector>();

            int xBorder1 = 0;
            int xBorder2 = 0;

            if (playerSide == GameSide.PLAYER1)
            {
                xBorder1 = 0;
                xBorder2 = GameManager.PlayerCellLimit;
            }
            else
            {
                xBorder1 = GridHeight - GameManager.PlayerCellLimit;
                xBorder2 = GridHeight;
            }

            for (int x = xBorder1; x < xBorder2; x++)
                for (int y = 0; y < GridWidth; y++)
                {
                    GridVector pos = new GridVector(x, y);
                    if (!Units.Any(u => u.Position == pos))
                        list.Add(pos);
                }

            return list;
        }



        #endregion

        /// <summary> Улучшение персонажей </summary>
        private void UnitUpdateLevel(List<GameUnit> units, GameSide side) 
        {
            List<GameUnit> list = new List<GameUnit>(0);

            foreach (GameUnit gameUnit in units)
            {
                if (!list.Any(u => u.Info.Name == gameUnit.Info.Name))
                    list.Add(gameUnit);
            }

            foreach (GameUnit gameUnit in list)
            {
                string name = gameUnit.Info.Name;

                if (units.Count(u => u.Info.Name == name) == UnitsUpdateLevelCount
                    && UnitsDatabase.GetInstance().SuperUnitCheck(name))
                {
                    GridVector pos = units.Find(u => u.Info.Name == name).Position;

                    var deleteUnits = units.Where(u => u.Info.Name == name).ToList();

                    for (int i = 0; i < UnitsUpdateLevelCount; i++)
                        Units.Remove(deleteUnits[i]);

                    UnitInfo info = UnitsDatabase.GetInstance().GetSuperUnit(name);
                    GameUnit newUnit = new GameUnit(info, side);

                    PutUnit(newUnit, pos.X, pos.Y);
                    GameManager.SelectedUnit = null;
                }
            }
        }

        /// <summary> Перемещение персонажа на поле сражения </summary>
        public override void PutUnit(GameUnit unit, int x, int y)
        {
            Player player = unit.GameSide == GameSide.PLAYER1 ? Player1 : Player2;

            if (player.PlayerMana < unit.Info.Price)
                throw new Exception("Недостаточно маны");

            base.PutUnit(unit, x, y);
            player.SpendMana(unit.Info.Price);

            var player1Units = Units.Where(u => u.GameSide == GameSide.PLAYER1).ToList();
            var player2Units = Units.Where(u => u.GameSide == GameSide.PLAYER2).ToList();

            UnitUpdateLevel(player1Units, GameSide.PLAYER1);
            UnitUpdateLevel(player2Units, GameSide.PLAYER2);
        }

        /// <summary> Перемещение персонажа на поле сражения </summary>
        private void PutUnitFree(GameUnit unit, int x, int y)
        {
            base.PutUnit(unit, x, y);
        }

        /// <summary> Перемещение персонажей на поле сражения </summary>
        public override void LoadUnits(List<GameUnit> list)
        {
            foreach (GameUnit unit in list)
            {
                PutUnitFree(unit, unit.Position.X, unit.Position.Y);
            }
        }
    }
}
