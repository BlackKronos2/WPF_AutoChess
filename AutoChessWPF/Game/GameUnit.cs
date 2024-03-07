using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace AutoChessWPF
{
    #region UNIT_INFO

    [JsonConverter(typeof(StringEnumConverter))]
    /// <summary> Одна из игровых сторон </summary>
    public enum GameSide
    {
        NONE,
        PLAYER1,
        PLAYER2,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    /// <summary> Состояние персонажа </summary>
    public enum UnitState
    {
        STAY,
        ATTACK,
        DAMAGE,
        DEATH,
    }

    [DataContract]
    /// <summary> Информация о персонаже </summary>
    public struct UnitInfo
    {
        [DataMember]
        public string Name;

        [DataMember]
        public int Attack;
        [DataMember]
        public int MaxHealth;
        [DataMember]
        public int Range;
        [DataMember]
        public int Price;

        [DataMember]
        public string[] Sprites;

        public UnitInfo(string name, int attack, int health, int range, int price, string[] spritesPath)
        {
            Name = name;

            Attack = attack;
            MaxHealth = health;
            Range = range;

            Price = price;

            Sprites = new string[4];

            for (int i = 0; i < 4; i++)
                Sprites[i] = Environment.CurrentDirectory + spritesPath[i];
        }

        public string GetSprite(UnitState state) => Sprites[(int)state];
    }

    [DataContract]
    /// <summary> Хранение позиции на игровом поле </summary>
    public struct GridVector
    {
        [DataMember]
        public int X { get; set; }
        [DataMember]
        public int Y { get; set; }

        public static GridVector NullVector => new GridVector(-1, -1);
        public static GridVector ZeroVector => new GridVector(0, 0);

        public static GridVector VectorUp => new GridVector(0, 1);
        public static GridVector VectorDown => new GridVector(0, -1);
        public static GridVector VectorLeft => new GridVector(-1, 0);
        public static GridVector VectorRight => new GridVector(1, 0);

        public GridVector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static int Distance(GridVector vector1, GridVector vector2)
        {
            int distanceX = Math.Abs(vector1.X - vector2.X);
            int distanceY = Math.Abs(vector1.Y - vector1.Y);

            return Math.Max(distanceX, distanceY);
        }

        public static GridVector operator +(GridVector vector1, GridVector vector2) =>
            new GridVector(vector1.X + vector2.X, vector1.Y + vector2.Y);

        public static GridVector operator -(GridVector vector1, GridVector vector2) =>
            new GridVector(vector1.X - vector2.X, vector1.Y - vector2.Y);

        public static GridVector operator *(GridVector vector, int value)
            => new GridVector(vector.X * value, vector.Y * value);

        public static bool operator !=(GridVector vector1, GridVector vector2)
            => vector1.X != vector2.X || vector1.Y != vector2.Y;

        public static bool operator ==(GridVector vector1, GridVector vector2)
            => vector1.X == vector2.X && vector1.Y == vector2.Y;
    }

    #endregion

    /// <summary> Класс персонажа </summary>
    [DataContract]
    public class GameUnit : GameEntity
    {
        #region FIELDS

        [DataMember]
        public UnitInfo Info;
        [DataMember]
        public GridVector Position;
        [DataMember]
        public UnitState State;

        /// <summary> Поле на котором находится персонаж </summary>
        public GameGrid CurrentGameGrid;

        #endregion

        public GameUnit(UnitInfo unitInfo, GameSide gameSide)
        {
            Info = unitInfo;
            Position = GridVector.NullVector;

            State = UnitState.STAY;
            _gameSide = gameSide;

            CurrentHealth = unitInfo.MaxHealth;

            _isAlive = true;
        }

        #region UNTIS_INTERPLAY
        public void Attack(GameEntity entity)
        {
            State = UnitState.ATTACK;
            entity.Damage(Info.Attack);
        }

        public override void Damage(int damage)
        {
            State = UnitState.DAMAGE;
            base.Damage(damage);
        }

        public override void Death()
        {
            base.Death();
            State = UnitState.DEATH;

            if (GameManager.SelectedUnit == this)
                GameManager.SelectedUnit = null;

            //CurrentGameGrid.Units.Remove(this);
        }

        #endregion

        #region UNIT_TURNS

        /// <summary> Проверка есть ли персонаж на клетке </summary>
        private bool UnitOnCellCheck(GridVector position)
        {
            var aliveUnits = CurrentGameGrid.Units.Where(u => u.IsAlive).ToList();

            if (aliveUnits.Count == 0)
                return false;

            if (position.X < 0 || position.Y < 0)
                return true;

            return CurrentGameGrid.Units.Any(u => u.Position == position);
        }

        /// <summary> Двидение по оси высоты (Поумолчанию) </summary>
        private void StandrartMoveY(GridVector target)
        {
            GridVector Yoffset = GridVector.ZeroVector;

            if (target.Y != Position.Y)
            {
                Yoffset = (target.Y > Position.Y) ? (GridVector.VectorUp) : (GridVector.VectorDown);
                if (!UnitOnCellCheck(Position))
                    Position += Yoffset;
            }
        }

        /// <summary> Двидение по оси высоты (При вызове) </summary>
        private void MoveY()
        {
            if (!UnitOnCellCheck(Position + GridVector.VectorUp))
            {
                Position += GridVector.VectorUp;
                return;
            }

            if (!UnitOnCellCheck(Position + GridVector.VectorDown))
            {
                Position += GridVector.VectorDown;
                return;
            }
        }

        /// <summary> Движение к цели </summary>
        private void UnitMoveToTarget(GridVector target)
        {
            GridVector Xoffset = (target.X > Position.X) ? GridVector.VectorRight : GridVector.VectorLeft;

            if (!UnitOnCellCheck(Position + Xoffset))
            {
                Position += Xoffset;
                StandrartMoveY(target);
            }
            else
            {
                MoveY();
            }
        }

        /// <summary> Ход персонажа </summary>
        public void UnitTurn()
        {
            var enemyList = CurrentGameGrid.Units.Where(u => u.GameSide != _gameSide).ToList();

            if (enemyList.Count == 0)
                return;

            int closestDistance = enemyList.Min(u => GridVector.Distance(u.Position, Position));
            var closestEnemy = enemyList.First(u => GridVector.Distance(u.Position, Position) == closestDistance);

            if (closestDistance <= Info.Range)
            {
                Attack(closestEnemy);
            }
            else
            {
                UnitMoveToTarget(closestEnemy.Position);
            }
        }

        #endregion

        /// <summary> Изображение персонажа</summary>
        public string GetRenderImage() => Info.GetSprite(State);
        /// <summary> Изображение персонажа</summary>
        public string GetRenderImage(UnitState state) => Info.GetSprite(state);
    }
}
