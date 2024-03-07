using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AutoChessWPF
{
    [DataContract]
    public class GamePresenter : INotifyPropertyChanged
    {
        #region FIELDS

        [DataMember]
        private GameManager _model;

        [DataMember]
        private Canvas _selectUnitImage;
        [DataMember]
        private Grid _selectedUnitInfoPanel;

        #endregion

        #region INPropChanged

        // Событие интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void RaisePropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region PROPERTIES

        [DataMember]
        private string _selectedUnitAttack;
        [DataMember]
        private string _selectedUnitHealth;

        [DataMember]
        private string _selectedUnitRange;
        [DataMember]
        private string _selectedUnitPrice;
        [DataMember]
        private string _selectedUnitName;

        [DataMember]
        private string _player1Health;
        [DataMember]
        private string _player2Health;

        [DataMember]
        private string _player1Mana;
        [DataMember]
        private string _player2Mana;

        public string SelectedUnitAttack
        {
            get { return _selectedUnitAttack; }
            set { 
                _selectedUnitAttack = $"Атака: {value}";
                RaisePropertyChanged("SelectedUnitAttack");
            }
        }
        public string SelectedUnitHealth
        {
            get { return _selectedUnitHealth; }
            set {
                _selectedUnitHealth = $"Здоровье: {value}";
                RaisePropertyChanged("SelectedUnitHealth");
            }
        }
        public string SelectedUnitRange
        {
            get { return _selectedUnitRange; }
            set {
                _selectedUnitRange = value;
                RaisePropertyChanged("SelectedUnitRange");
            }
        }
        public string SelectedUnitPrice
        {
            get { return _selectedUnitPrice; }
            set
            {
                _selectedUnitPrice = value;
                RaisePropertyChanged("SelectedUnitPrice");
            }
        }
        public string SelectedUnitName
        {
            get { return _selectedUnitName; }
            set
            { 
                _selectedUnitName = value;
                RaisePropertyChanged("SelectedUnitName");
            }
        }

        public string Player1Health
        {
            get { return _player1Health; }
            set { 
                _player1Health = value;
                RaisePropertyChanged("Player1Health");
            }
        }
        public string Player2Health
        {
            get { return _player2Health; }
            set
            {
                _player2Health = value;
                RaisePropertyChanged("Player2Health");
            }
        }

        public string Player1Mana
        {
            get { return _player1Mana; }
            set
            {
                _player1Mana = value;
                RaisePropertyChanged("Player1Mana");
            }
        }
        public string Player2Mana
        {
            get { return _player2Mana; }
            set
            {
                _player2Mana = value;
                RaisePropertyChanged("Player2Mana");
            }
        }

        #endregion

        public GamePresenter(GameManager model, Canvas _selectedUnitDraw, Grid _selectedUnitInformation)
        {
            _model = model;

            _selectUnitImage = _selectedUnitDraw;
            _selectedUnitInfoPanel = _selectedUnitInformation;

            GameEvents.OnLastUpdate += PlayerViewUpdate;
            GameEvents.OnFightEnd += PlayerViewUpdate;

            GameManager.OnSelectedUnitChange += SelectedUnitDraw;

            PlayerViewUpdate();
            SelectedUnitInfoActive(false);
        }

        private void SelectedUnitInfoActive(bool active)
        {
            Visibility status = active ? Visibility.Visible : Visibility.Hidden;
            _selectedUnitInfoPanel.Visibility = status;
        }

        private void SelectedUnitDraw()
        {
            if (GameManager.SelectedUnit == null)
            {
                SelectedUnitInfoActive(false);
                return;
            }

            GameUnit gameUnit = GameManager.SelectedUnit;

            Image UnitImage = new Image
            {
                Width = _selectUnitImage.Width,
                Height = _selectUnitImage.Height,
                Source = new BitmapImage(new Uri(gameUnit.GetRenderImage(UnitState.STAY))),
                Name = $"SelectedUnit",
            };

            Canvas.SetTop(UnitImage, 0);
            Canvas.SetLeft(UnitImage, 0);

            SelectedUnitInfoActive(true);

            _selectUnitImage.Children.Clear();
            _selectUnitImage.Children.Add(UnitImage);

            SelectedUnitAttack = gameUnit.Info.Attack.ToString();
            SelectedUnitHealth = $"{gameUnit.CurrentHealth}/{gameUnit.Info.MaxHealth}";
            SelectedUnitRange = $"Дальность атаки {gameUnit.Info.Range}";
            SelectedUnitPrice = gameUnit.Info.Price == 0 ? "" : $"Цена: {gameUnit.Info.Price}";
            SelectedUnitName = gameUnit.Info.Name;
        }

        public void Update()
        {
            _model.Update();
            PlayerViewUpdate();
        }

        public void PlayerViewUpdate()
        {
            Player1Health = $"Здоровье {_model.Player1.CurrentHealth}";
            Player2Health = $"Здоровье {_model.Player2.CurrentHealth}";

            Player1Mana = $"Мана {_model.Player1.PlayerMana}";
            Player2Mana = $"Мана {_model.Player2.PlayerMana}";
        }
    }
}
