using System.Collections.Generic;
using System.Threading.Tasks;
using Abstraction;
using Infrastructure;
using TableLogic;
using UnityEngine;

namespace TableView {
    public class TableView : MonoBehaviour, ITableView {
        [SerializeField] private FigureView _figureTemplate;
        [SerializeField][Min(3)] private float _helpDelay;
        [SerializeField] private string _mapPath;

        private Table _table;
        private Dictionary<Figure, FigureView> _figuresDictionary = new Dictionary<Figure, FigureView>();
        private Vector2 _drawOffset;

        Figure[] _helpFigures;
        private float _savedTableChangedTime = 0;
        private bool _isHelpingBlocking = false;
        private bool CanHelp => (Time.time - _savedTableChangedTime) >= _helpDelay && !_isHelpingBlocking;

        private void Awake() {
            TableScheme scheme;
            try {
                scheme = new FileParcer(Application.dataPath + _mapPath).GenerateScheme();
            }
            catch (System.Exception ex) {
                Debug.LogException(ex);
                return;
            }

            _table = new Table(this, new FigureFabric(), scheme);
            _table.Generate();
            _drawOffset = _table.Size / -2;
            DrawStartTable();
        }

        private void Update() {
            if (_table != null) HandleHelp();
        }

        public Vector2 ToWorldPosition(Vector2Int position)
            => transform.TransformPoint(_drawOffset) + new Vector3(position.x, position.y, 0);

        public async Task OnFiguresArrivedAsync(List<Figure> figures) {
            _isHelpingBlocking = true;
            HideHelp();
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
                position.y = _table.Size.y + position.y - xMinYRelation[figure.Position.x];

                FigureView figureView = Instantiate(_figureTemplate, ToWorldPosition(position), Quaternion.identity, transform);
                figureView.Construct(figure, this);
                _figuresDictionary.Add(figure, figureView);

                movings.Add(figureView.MoveToPosition());
            }

            await Task.WhenAll(movings);
            EnableTableInput();
            _isHelpingBlocking = false;
            _savedTableChangedTime = Time.time;
            _helpFigures = _table.Helper.GetHelp();
        }

        public async Task OnFiguresReplacedAsync(List<Figure> figures) {
            _isHelpingBlocking = true;
            HideHelp();
            DisableTableInput();

            List<Task> movings = new List<Task>();
            foreach (var figure in figures) {
                movings.Add(_figuresDictionary[figure].MoveToPosition());
            }

            await Task.WhenAll(movings);
            EnableTableInput();
            _isHelpingBlocking = false;
        }

        public async Task OnFiguresDestroyedAsync(List<Figure> figures) {
            _isHelpingBlocking = true;
            HideHelp();
            DisableTableInput();

            List<Task> popings = new List<Task>();
            foreach (var figure in figures) {
                popings.Add(_figuresDictionary[figure].Pop());
                _figuresDictionary.Remove(figure);
            }

            await Task.WhenAll(popings);
            EnableTableInput();
            _isHelpingBlocking = false;
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

        private void HandleHelp() {
            if (CanHelp) {
                foreach (var figure in _helpFigures) {
                    _figuresDictionary[figure].ShowHelp();
                }
            }
        }

        private void HideHelp() {
            foreach (var figure in _helpFigures) {
                if (_figuresDictionary.ContainsKey(figure)) {
                    _figuresDictionary[figure].StopHelp();
                }
            }
        }

        private void DrawStartTable() {
            for (int y = 0; y < _table.Size.y; y++) {
                for (int x = 0; x < _table.Size.x; x++) {
                    Figure figure = _table.GetFigure(new Vector2Int(x, y));
                    if (figure == null) continue;

                    var figureView = Instantiate(_figureTemplate, ToWorldPosition(new Vector2Int(x, y)), Quaternion.identity, transform);
                    figureView.Construct(figure, this);

                    _figuresDictionary.Add(figure, figureView);
                }
            }
            _helpFigures = _table.Helper.GetHelp();
        }
    }
}
