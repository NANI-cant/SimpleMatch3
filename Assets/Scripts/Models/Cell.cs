using System;
using UnityEngine;

namespace TableLogic {
    public class Cell {
        public event Action<Cell> Choosen;
        public event Action<Cell> UnChoosen;

        private Table _table;
        private Figure _figure;
        private Vector2Int _position;
        private bool _isChoosen;

        public Figure Figure => _figure;
        public Vector2Int Position => _position;

        public Cell(Table table, Vector2Int position) {
            _position = position;
            _table = table;
            _figure = new Figure();
            _isChoosen = false;
        }

        public void HandleClick() {
            if (_isChoosen) {
                UnChoose();
            }
            else {
                Choose();
            }
        }

        public void Choose() {
            if (!_table.TryChooseCell(this)) return;

            _isChoosen = true;
            Choosen?.Invoke(this);
        }

        public void UnChoose() {
            _table.UnChooseCell(this);
            _isChoosen = false;
            UnChoosen?.Invoke(this);
        }

        public void SetFigure(Figure figure) {
            _figure = figure;
        }
    }
}
