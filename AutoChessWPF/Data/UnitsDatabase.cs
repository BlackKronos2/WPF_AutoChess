using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoChessWPF
{
    /// <summary> Данные о персонажах игры </summary>
    public class UnitsDatabase
    {
        #region INSTANE

        private static UnitsDatabase _instance;

        public static UnitsDatabase GetInstance()
        {
            if (_instance == null)
                _instance = new UnitsDatabase();

            return _instance;
        }

        #endregion

        private List<UnitInfo> _startUnits;
        private List<UnitInfo> _units;

        private UnitsDatabase()
        {
            _units = new List<UnitInfo>(0);
            _startUnits = new List<UnitInfo>(0);

            _startUnits.Add(CreateUnit("Warrior", 1, 8, 1, 5));
            _startUnits.Add(CreateUnit("Sceleton", 1, 6, 1, 3));
            _startUnits.Add(CreateUnit("SkeletonArcher", 1, 5, 4, 7));

            _units.Add(CreateUnit("SuperWarrior", 1, 15, 1, 0));
            _units.Add(CreateUnit("SuperSceleton", 3, 10, 1, 0));
        }

        #region DATA_INIT

        /// <summary> Создание персонажа в базу </summary>
        private UnitInfo CreateUnit(string name, int attack, int heath, int range, int price)
        {
            UnitInfo info = new UnitInfo(
                name, attack, heath, range, price,
                UnitViewLoad(name)
            );

            return info;
        }

        /// <summary> Загрузка изображений персонажа в базу </summary>
        private string[] UnitViewLoad(string path)
        {
            string[] data = new string[4]
            {
                    $"\\Resources\\{path}\\Stay.png",
                    $"\\Resources\\{path}\\Attack.png",
                    $"\\Resources\\{path}\\Damage.png",
                    $"\\Resources\\{path}\\Death.png"
            };

            return data;
        }

        #endregion

        #region DATA_LOAD


        /// <summary> Загрузка персонажа из базы </summary>
        public UnitInfo GetRandomStartUnit()
        {
            Random random = new Random();
            int index = random.Next(0, _startUnits.Count);

            return _startUnits[index];
        }


        /// <summary> Проверка налиция удучшения для персонажа </summary>
        public bool SuperUnitCheck(string name) => _units.Any(u => u.Name == ("Super" + name));

        /// <summary> Загрузка персонажа из базы </summary>
        public UnitInfo GetFromName(string name) => _units.Find(u => u.Name == name);

        /// <summary> Загрузка персонажа из базы </summary>
        public UnitInfo GetSuperUnit(string name) => GetFromName("Super" + name);

        #endregion

    }
}
