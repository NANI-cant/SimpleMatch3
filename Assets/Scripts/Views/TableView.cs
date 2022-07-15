using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace TableLogic {
    public class TableView : MonoBehaviour, ITableView {
        [SerializeField] private Vector2Int _size;
        [SerializeField] private FigureView _figureTemplate;

        private Table _table;
        private Vector2 _drawOffset;
        private Dictionary<Figure, FigureView> _figuresDictionary = new Dictionary<Figure, FigureView>();

        private void Awake() {
            _table = new Table(this);
            _table.Generate(_size);
            _drawOffset = _table.Size / -2;
            DrawStartTable();
        }

        public Vector2 ToWorldPosition(Vector2Int position)
            => transform.TransformPoint(_drawOffset) + new Vector3(position.x, position.y, 0);

        public async Task OnFiguresArrivedAsync(List<Figure> figures) {
            DisableTableInput();

            Dictionary<int, int> xMinYRelation = new Dictionary<int, int>();
            foreach (var figure in figures) {
                if (xMinYRelation.ContainsKey(figure.Position.x)) {
                    xMinYRelation[figure.Position.x] = Mathf.Min(xMinYRelation[figure.Position.x], figure.Position.y);
                }
                else {
                    xMinYRelation[figure.Position.x] = figure.Position.y;
                }
            }

            List<Task> movings = new List<Task>();
            foreach (var figure in figures) {
                Vector2Int position = figure.Position;
                position.y = _size.y + position.y - xMinYRelation[figure.Position.x];

                FigureView figureView = Instantiate(_figureTemplate, ToWorldPosition(position), Quaternion.identity, transform);
                figureView.Construct(figure, this);
                _figuresDictionary.Add(figure, figureView);

                movings.Add(figureView.MoveToPosition());
            }

            await Task.WhenAll(movings);
            EnableTableInput();
        }

        public async Task OnFiguresReplacedAsync(List<Figure> figures) {
            DisableTableInput();

            List<Task> movings = new List<Task>();
            foreach (var figure in figures) {
                movings.Add(_figuresDictionary[figure].MoveToPosition());
            }

            await Task.WhenAll(movings);
            EnableTableInput();
        }

        public async Task OnFiguresDestroyedAsync(List<Figure> figures) {
            DisableTableInput();

            List<Task> popings = new List<Task>();
            foreach (var figure in figures) {
                popings.Add(_figuresDictionary[figure].Pop());
                _figuresDictionary.Remove(figure);
            }

            await Task.WhenAll(popings);
            EnableTableInput();
        }

        private void EnableTableInput() {
            foreach (var pair in _figuresDictionary) {
                pair.Value.Enable();
            }
        }

        private void DisableTableInput() {
            foreach (var pair in _figuresDictionary) {
                pair.Value.Disable();
            }
        }

        private void DrawStartTable() {
            for (int y = 0; y < _table.Size.y; y++) {
                for (int x = 0; x < _table.Size.x; x++) {
                    Figure figure = _table.GetFigure(new Vector2Int(x, y));

                    var figureView = Instantiate(_figureTemplate, ToWorldPosition(new Vector2Int(x, y)), Quaternion.identity, transform);
                    figureView.Construct(figure, this);

                    _figuresDictionary.Add(figure, figureView);
                }
            }
        }
    }
}
