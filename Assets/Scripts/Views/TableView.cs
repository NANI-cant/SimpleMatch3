using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace TableLogic {
    public class TableView : MonoBehaviour {
        [SerializeField] private Vector2Int _size;
        [SerializeField] private CellView _cellTemplate;
        [SerializeField] private FigureView _figureTemplate;

        private Table _table;
        private Vector2 _drawOffset;
        private Dictionary<Cell, CellView> _cellsDictionary = new Dictionary<Cell, CellView>();

        private List<Task> _runningTasks = new List<Task>();

        [Inject]
        public void Construct(Table table) {
            _table = table;
            _table.Generate(_size);
        }

        private void OnEnable() {
            _table.MatchFound += OnMatchFound;
            _table.CellsChanged += OnCellsChanged;
        }
        private void OnDisable() {
            _table.MatchFound -= OnMatchFound;
            _table.CellsChanged -= OnCellsChanged;
        }

        private async void OnCellsChanged(Cell first, Cell second) {
            await Task.WhenAll(_runningTasks);

            CellView firstView = _cellsDictionary[first];
            CellView secondView = _cellsDictionary[second];

            FigureView bufFigure = firstView.FigureView;
            firstView.FigureView = secondView.FigureView;
            secondView.FigureView = bufFigure;

            DisableTableInput();
            Task firstPulling = firstView.PullFigure();
            Task secondPulling = secondView.PullFigure();
            _runningTasks.AddRange(new Task[] { firstPulling, secondPulling });

            await Task.WhenAll(new Task[] { firstPulling, secondPulling });
            EnableTableInput();
        }

        private void DisableTableInput() {
            foreach (var pair in _cellsDictionary) {
                pair.Value.Disable();
            }
        }

        private void EnableTableInput() {
            foreach (var pair in _cellsDictionary) {
                pair.Value.Enable();
            }
        }

        private async void OnMatchFound(Match match) {
            await Task.WhenAll(_runningTasks);
            List<Vector2Int> matchedPositions = new List<Vector2Int>(match.Positions);

            List<Task> pops = new List<Task>();
            foreach (var cell in match.Cells) {
                pops.Add(_cellsDictionary[cell].PopFigure());
            }
            _runningTasks.AddRange(pops);
            await Task.WhenAll(pops);
        }

        private void Start() {
            _drawOffset = _table.Size / -2;
            DrawTable(_table.Size);
        }

        private void DrawTable(Vector2Int size) {
            for (int y = 0; y < size.y; y++) {
                for (int x = 0; x < size.x; x++) {
                    Vector3 worldPosition = transform.TransformPoint(_drawOffset) + new Vector3(x, y, 0);
                    var cell = Instantiate(_cellTemplate, worldPosition, Quaternion.identity, transform);
                    var figure = Instantiate(_figureTemplate, worldPosition, Quaternion.identity, transform);

                    cell.Construct(_table.GetCell(new Vector2Int(x, y)), figure);
                    _cellsDictionary.Add(_table.GetCell(new Vector2Int(x, y)), cell);
                }
            }
        }

        [ContextMenu("Re Generate")]
        public void ReGenerate() {
            foreach (var pair in _cellsDictionary) {
                Destroy(pair.Value.gameObject);
            }
            _cellsDictionary.Clear();

            _table.Generate(_size);
            _drawOffset = _table.Size / -2;
            DrawTable(_table.Size);
        }
    }
}
