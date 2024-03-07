using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace AutoChessWPF
{
    /// <summary> Игровое поле на котором находятся персонажи </summary>
    [DataContract]
    public class GameGrid
    {
        [DataMember]
        public List<GameUnit> Units { get; protected set; }

        [DataMember]
        public int GridHeight { get; private set; }
        [DataMember]
        public int GridWidth { get; private set; }

        [DataMember]
        public bool Enable { get; set; }

        public GameGrid(int width, int height)
        {
            GridWidth = height;
            GridHeight = width;

            Units = new List<GameUnit>(0);
            Enable = true;

            GameEvents.OnFightStart += () => Enable = false;
            GameEvents.OnFightEnd += () => Enable = true;
        }

        /// <summary> Размещение персонажа по поле </summary>
        public virtual void PutUnit(GameUnit unit, int x , int y)
        {
            if(unit.CurrentGameGrid != null)
                unit.CurrentGameGrid.TakeUnitFromGrid(unit);

            unit.Position = new GridVector(x, y);
            Units.Add(unit);

            unit.CurrentGameGrid = this;
        }
        /// <summary> Удаление персонажа с поля </summary>
        public void TakeUnitFromGrid(GameUnit unit)
        {
            Units.Remove(unit);
        }
        /// <summary> Выбор персонажа с поля </summary>
        public virtual void ChoiseCell(int x, int y)
        {
            GridVector position = new GridVector(x, y);

            if (!Units.Any(u => u.Position == position))
                return;

            GameManager.SelectedUnit = Units.Find(u => u.Position == position);
        }
        /// <summary> Обнуление выбора персонажа с поля </summary>
        public virtual void ChoiseCellNull()
        {
            GameManager.SelectedUnit = null;
        }
        /// <summary> Загрузка персонажей на поле </summary>
        public virtual void LoadUnits(List<GameUnit> list)
        {
            foreach (GameUnit unit in list)
            {
                PutUnit(unit, unit.Position.X, unit.Position.Y);
            }
        }

        /// <summary> Очистить поле </summary>
        public void ClearGrid() => Units.Clear();

        /// <summary> Инициализация </summary>
        public void UnitsInit()
        {
            foreach (GameUnit unit in Units)
                unit.CurrentGameGrid = this;
        }
    }
}
