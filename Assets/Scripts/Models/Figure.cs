using System;
using UnityEngine;

namespace TableLogic {
    public class Figure {
        private readonly Color[] _availableColors = new Color[] {
            Color.red,
            Color.blue,
            Color.yellow,
            Color.green,
            Color.magenta,
        };

        public event Action<Figure> Choosed;
        public event Action<Figure> UnChoosed;

        private Table _table;
        private Vector2Int _position;
        private bool _isChoosen;
        private Color _color;
        private int _id;

        public Color Color => _color;
        public int Id => _id;
        public Vector2Int Position => _position;

        public Figure(Table table, Vector2Int position) {
            _id = UnityEngine.Random.Range(0, _availableColors.Length);
            _color = _availableColors[_id];

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
            Choosed?.Invoke(this);
        }

        public void UnChoose() {
            _table.UnChooseFigure(this);
            _isChoosen = false;
            UnChoosed?.Invoke(this);
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
