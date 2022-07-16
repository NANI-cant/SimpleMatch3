using UnityEngine;

namespace TableLogic {
    public class SelectingHandler {
        private Figure _selectedFigure;
        private Table _table;

        public SelectingHandler(Table table) {
            _table = table;
        }

        public bool TryChooseFigure(Figure figure) {
            if (_selectedFigure == null) {
                _selectedFigure = figure;
                return true;
            }

            float distance = Vector2Int.Distance(figure.Position, _selectedFigure.Position);
            if (distance > 1) {
                _selectedFigure.UnChoose();
                _selectedFigure = figure;
                return true;
            }
            else {
                _table.Change(_selectedFigure, figure);
                return false;
            }
        }

        public void UnChooseFigure(Figure figure) {
            if (_selectedFigure == figure) {
                _selectedFigure = null;
            }
        }
    }
}
