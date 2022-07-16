using System;
using Abstraction;
using UnityEngine;

namespace TableLogic {
    public class Figure : TableMember {
        public event Action Choosed;
        public event Action UnChoosed;

        private Table _table;
        private Vector2Int _position;
        private bool _isChoosen;
        private IFigureData _data;

        public string Id => _data.Id;
        public IFigureData Data => _data;
        public Vector2Int Position => _position;

        public override bool isMovable => true;

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
            if (!_table.Selector.TryChooseFigure(this)) return;

            _isChoosen = true;
            Choosed?.Invoke();
        }

        public void UnChoose() {
            _table.Selector.UnChooseFigure(this);
            _isChoosen = false;
            UnChoosed?.Invoke();
        }

        public void SetPosition(Vector2Int position) {
            _position = position;
        }

        public override Figure FindAroundById(Vector2Int position, string id, Vector2Int dontLookInDirection) {
            for (int i = 0; i < 4; i++) {
                Vector3 lookDirection = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90 * i)).MultiplyPoint3x4(Vector3.up);
                Vector2Int formalizedDirection = new Vector2Int(Mathf.RoundToInt(lookDirection.x), Mathf.RoundToInt(lookDirection.y));

                if (formalizedDirection == dontLookInDirection) continue;

                Figure figure = _table.GetFigure(position + formalizedDirection);
                if (figure == null) continue;

                if (figure.Id == id) {
                    return figure;
                }
            }

            return null;
        }

        public override bool TryFallInPosition(Vector2Int position) {
            if (position.x != _position.x) return false;

            int offset = 0;
            while (_table.GetMember(position + Vector2Int.up * offset) != null) {
                offset++;
            }

            if (position.y + offset > _position.y) return false;

            _table.SetFigure(_position, null);
            _table.SetFigure(position + Vector2Int.up * offset, this);
            return true;
        }

        public override string ToString() {
            return "Figure\n" +
                    "\tPosition: " + Position + "\n" +
                    "\tId: " + Id + "\n";
        }
    }
}
