using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AutoChessWPF
{
    public class BattleGridPresenter : GameGridPresenter
    {

        public BattleGridPresenter(Canvas canvas, BattleGrid model):base(canvas, model)
        {
            _renderCanvas = canvas;

            _model = model;
            Enable = true;

            GameEvents.OnLastUpdate += DrawMap;
        }

        private void DrawPlayerCell(GridVector pos)
        {
            Image sector = new Image
            {
                Width = CellWidth,
                Height = CellHeight,
                Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Resources\\PlayerSector.png")),
                Name = $"Sector{pos.X}{pos.Y}",
            };

            Canvas.SetTop(sector, pos.Y * CellHeight);
            Canvas.SetLeft(sector, pos.X * CellWidth);

            _renderCanvas.Children.Add(sector);
        }

        public void DrawPlayerCells()
        {
            if (GameManager.SelectedUnit == null)
                return;

            if (GameManager.SelectedUnit.CurrentGameGrid == _model)
                return;

            GameSide playerSide = GameManager.SelectedUnit.GameSide;
            List<GridVector> cells = ((BattleGrid)_model).PlayerCells(playerSide);

            foreach(GridVector cellPosition in cells)
            {
                DrawPlayerCell(cellPosition);
            }

        }

        public override void DrawMap()
        {
            base.DrawMap();
            DrawPlayerCells();
        }
    }
}
