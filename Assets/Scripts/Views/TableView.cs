using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace TableLogic {
    public class TableView : MonoBehaviour {
        [SerializeField] private Vector2Int _size;
        [SerializeField] private CellView _cellTemplate;

        private Table _table;
        private Vector2 _drawOffset;
        private List<CellView> _cells = new List<CellView>();

        [Inject]
        public void Construct(Table table) {
            _table = table;
            _table.Generate(_size);
        }

        private void Start() {
            _drawOffset = _table.Size / -2;

            DrawTable(_table.Size);
        }

        private void DrawTable(Vector2Int size) {
            for (int y = 0; y < size.y; y++) {
                for (int x = 0; x < size.x; x++) {
                    Vector3 position = transform.TransformPoint(_drawOffset) + new Vector3(x, y, 0);
                    var cell = Instantiate(_cellTemplate, position, Quaternion.identity, transform);
                    cell.Construct(_table.GetCell(new Vector2Int(x, y)));
                    _cells.Add(cell);
                }
            }
        }

        [ContextMenu("Re Generate")]
        public void ReGenerate() {
            foreach (var cell in _cells) {
                Destroy(cell.gameObject);
            }
            _cells.Clear();

            _table.Generate(_size);
            _drawOffset = _table.Size / -2;
            DrawTable(_table.Size);
        }

        [ContextMenu("Find Matches")]
        public void FindMatches() {
            List<Match> matches = _table.FindMatches();
            List<Vector2Int> allPositions = new List<Vector2Int>();
            foreach (var match in matches) {
                allPositions.AddRange(match.Positions);
            }

            foreach (var cell in _cells) {
                if (allPositions.Contains(cell.Position)) {
                    cell.Mark();
                }
                else {
                    cell.UnMark();
                }
            }
        }
    }
}
