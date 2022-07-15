using System;
using Abstraction;
using UnityEngine;

namespace TableLogic {
    public class Figure {
        public event Action Choosed;
        public event Action UnChoosed;

        private Table _table;
        private Vector2Int _position;
        private bool _isChoosen;
        private IFigureData _data;

        public string Id => _data.Id;
        public IFigureData Data => _data;
        public Vector2Int Position => _position;

        public Figure(Table table, Vector2Int position, IFigureData figureData) {
            _data = figureData;
            _position = position;
            _table = table;
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
            if (!_table.TryChooseFigure(this)) return;

            _isChoosen = true;
            Choosed?.Invoke();
        }

        public void UnChoose() {
            _table.UnChooseFigure(this);
            _isChoosen = false;
            UnChoosed?.Invoke();
        }

        public void SetPosition(Vector2Int position) {
            _position = position;
        }

        public override string ToString() {
            return "Figure\n" +
                    "\tPosition: " + Position + "\n" +
                    "\tId: " + Id + "\n";
        }
    }
}
