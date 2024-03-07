using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutoChessWPF
{
    [DataContract]
    public class GameGridPresenter
    {
        [DataMember]
        protected Canvas _renderCanvas;

        [DataMember]
        protected GameGrid _model;

        public const int CellWidth = 60;
        public const int CellHeight = 60;

        [DataMember]
        public bool Enable { get; set; }

        public GameGridPresenter(Canvas canvas, GameGrid model)
        {
            _renderCanvas = canvas;

            _model = model;
            Enable = true;

            GameEvents.OnLastUpdate += DrawMap;
        }

        public virtual void DrawMap()
        {
            _renderCanvas.Children.Clear();
            DrawLines();
            DrawUnits();
            ChoiseCellDraw();
        }

        #region DRAW

        private void DrawLines()
        {
            for (int i = 0; i <= _model.GridHeight; i++)
            {
                Line map_line = new Line();
                map_line.Stroke = System.Windows.Media.Brushes.Black;
                map_line.StrokeThickness = 2;

                map_line.X1 = i * CellWidth;
                map_line.Y1 = 0;

                map_line.X2 = i * CellWidth;
                map_line.Y2 = _model.GridWidth * CellHeight;

                map_line.HorizontalAlignment = HorizontalAlignment.Left;
                map_line.VerticalAlignment = VerticalAlignment.Center;

                _renderCanvas.Children.Add(map_line);
            }

            for (int i = 0; i <= _model.GridWidth; i++)
            {
                Line map_line = new Line();
                map_line.Stroke = System.Windows.Media.Brushes.Black;
                map_line.StrokeThickness = 2;

                map_line.X1 = 0;
                map_line.Y1 = i * CellHeight;

                map_line.X2 = _model.GridHeight * CellWidth;
                map_line.Y2 = i * CellHeight;

                map_line.HorizontalAlignment = HorizontalAlignment.Left;
                map_line.VerticalAlignment = VerticalAlignment.Center;

                _renderCanvas.Children.Add(map_line);
            }
        }

        private void DrawUnit(GameUnit unit)
        {
            int x = unit.Position.X;
            int y = unit.Position.Y;

            Image sector = new Image
            {
                Width = CellWidth,
                Height = CellHeight,
                Source = new BitmapImage(new Uri(unit.GetRenderImage())),
                Name = $"Sector{x}_{y}",
            };

            sector.FlowDirection = (unit.GameSide == GameSide.PLAYER1) ? (FlowDirection.LeftToRight) : (FlowDirection.RightToLeft);

            Canvas.SetTop(sector, y * CellHeight);
            Canvas.SetLeft(sector, x * CellWidth);

            _renderCanvas.Children.Add(sector);
        }

        private void DrawUnitHealth(float x, float y, string health)
        {
            TextBlock healthText = new TextBlock();
            healthText.Text = health;

            healthText.Width = CellWidth;
            healthText.Height = CellHeight;

            healthText.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));

            float top = y * CellHeight;
            float left = x * CellWidth;

            Canvas.SetLeft(healthText, left);
            Canvas.SetTop(healthText, top);

            _renderCanvas.Children.Add(healthText);
        }

        private void DrawUnits()
        {
            foreach (GameUnit unit in _model.Units)
            {
                DrawUnit(unit);
                string unitHealth = $"{unit.CurrentHealth}/{unit.Info.MaxHealth}";
                DrawUnitHealth(unit.Position.X, unit.Position.Y, unitHealth);
            }
        }

        private void ChoiseCellDraw()
        {
            if (GameManager.SelectedUnit == null)
                return;

            if (GameManager.SelectedUnit.CurrentGameGrid != _model)
                return;

            int x = GameManager.SelectedUnit.Position.X;
            int y = GameManager.SelectedUnit.Position.Y;

            Image sector = new Image
            {
                Width = CellWidth,
                Height = CellHeight,
                Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Resources\\NullSector.png")),
                Name = $"Sector",
            };

            Canvas.SetTop(sector, y * CellHeight);
            Canvas.SetLeft(sector, x * CellWidth);

            _renderCanvas.Children.Add(sector);
        }

        #endregion

        public void CellChoise()
        {
            if (!Enable)
                return;

            Point position = Mouse.GetPosition(_renderCanvas);;

            int x = (int)(Math.Floor(position.X) / CellWidth);
            int y = (int)(Math.Floor(position.Y) / CellHeight);

            if (x < 0 || x >= _model.GridHeight)
            {
                _model.ChoiseCellNull();
                return;
            }

            if (y < 0 || y >= _model.GridWidth)
            {
                _model.ChoiseCellNull();
                return;
            }

            _model.ChoiseCell(x, y);
            GameEvents.SendOnLastUpdate();
        }
    }
}
