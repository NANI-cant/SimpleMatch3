using UnityEngine;

namespace TableLogic {
    [System.Serializable]
    public class Cell {
        private Figure _figure;
        private Table _table;
        private Vector2Int _position;

        public Figure Figure => _figure;
        public Vector2Int Position => _position;

        public Cell(Table table, Vector2Int position) {
            _table = table;
            _position = position;
            _figure = new Figure();
        }
    }
}
