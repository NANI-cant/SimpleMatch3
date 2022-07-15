using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace TableLogic {
    public class TableView : MonoBehaviour {
        [SerializeField] private Vector2Int _size;
        [SerializeField] private FigureView _figureTemplate;

        private Table _table;
        private Vector2 _drawOffset;
        private Dictionary<Figure, FigureView> _figuresDictionary = new Dictionary<Figure, FigureView>();

        private List<Task> _runningTasks = new List<Task>();

        [Inject]
        public void Construct(Table table) {
            _table = table;
            _table.Generate(_size);
        }

        private void OnEnable() {
            _table.FiguresDestroyed += OnFiguresDestroyed;
            _table.FiguresReplaced += OnFiguresReplaced;
        }

        private void OnDisable() {
            _table.FiguresDestroyed -= OnFiguresDestroyed;
            _table.FiguresReplaced -= OnFiguresReplaced;
        }

        public Vector2 ToWorldPosition(Vector2Int position) {
            return transform.TransformPoint(_drawOffset) + new Vector3(position.x, position.y, 0);
        }

        private void DisableTableInput() {
            foreach (var pair in _figuresDictionary) {
                pair.Value.Disable();
            }
        }

        private void EnableTableInput() {
            foreach (var pair in _figuresDictionary) {
                pair.Value.Enable();
            }
        }

        private async void OnFiguresArrived(List<Figure> figures) {
            await Task.WhenAll(_runningTasks);
            DisableTableInput();

            foreach (var figure in figures) {
                Vector2Int position = figure.Position;
                position.y = _size.y;

                FigureView figureView = Instantiate(_figureTemplate, ToWorldPosition(position), Quaternion.identity, transform);
                figureView.Construct(figure, this);

                _runningTasks.Add(figureView.MoveToPosition());
            }

            await Task.WhenAll(_runningTasks);
            EnableTableInput();
        }

        private async void OnFiguresReplaced(List<Figure> figures) {
            await Task.WhenAll(_runningTasks);
            DisableTableInput();

            foreach (var figure in figures) {
                _runningTasks.Add(_figuresDictionary[figure].MoveToPosition());
            }

            await Task.WhenAll(_runningTasks);
            EnableTableInput();
        }

        private async void OnFiguresDestroyed(List<Figure> figures) {
            await Task.WhenAll(_runningTasks);
            DisableTableInput();

            foreach (var figure in figures) {
                _runningTasks.Add(_figuresDictionary[figure].Pop());
                _figuresDictionary.Remove(figure);
            }

            await Task.WhenAll(_runningTasks);
            EnableTableInput();
        }

        private void Start() {
            _drawOffset = _table.Size / -2;
            DrawTable(_table.Size);
        }

        private void DrawTable(Vector2Int size) {
            for (int y = 0; y < size.y; y++) {
                for (int x = 0; x < size.x; x++) {
                    Figure figure = _table.GetFigure(new Vector2Int(x, y));

                    var figureView = Instantiate(_figureTemplate, ToWorldPosition(new Vector2Int(x, y)), Quaternion.identity, transform);
                    figureView.Construct(figure, this);

                    _figuresDictionary.Add(figure, figureView);
                }
            }
        }

        [ContextMenu("Re Generate")]
        public void ReGenerate() {
            foreach (var pair in _figuresDictionary) {
                Destroy(pair.Value.gameObject);
            }
            _figuresDictionary.Clear();

            _table.Generate(_size);
            _drawOffset = _table.Size / -2;
            DrawTable(_table.Size);
        }
    }
}
