using System.Collections.Generic;
using System.Threading.Tasks;
using Abstraction;
using Infrastructure;
using TableLogic;
using UnityEngine;

namespace TableView {
    [RequireComponent(typeof(TableAudio))]
    public class TableView : MonoBehaviour, ITableView {
        [SerializeField] private FigureView _figureTemplate;
        [SerializeField][Min(3)] private float _helpDelay;
        [SerializeField] private string _mapPath;

        private Table _table;
        private Dictionary<Figure, FigureView> _figuresDictionary = new Dictionary<Figure, FigureView>();
        private Vector2 _drawOffset;
        private TableAudio _audio;

        private Figure[] _helpFigures;
        private float _savedTableChangedTime = 0;
        private bool _isHelpingBlocked = false;
        private bool CanHelp => (Time.time - _savedTableChangedTime) >= _helpDelay && !_isHelpingBlocked;

        private void Awake() {
            TableScheme scheme;
            try {
                scheme = new FileParcer(Application.dataPath + _mapPath).GenerateScheme();
            }
            catch (System.Exception ex) {
                Debug.LogException(ex);
                return;
            }

            _audio = GetComponent<TableAudio>();

            _table = new Table(this, new FigureFabric(), scheme);
            _table.Generate();
            _drawOffset = _table.Size / -2;

            DrawStartTable();
        }

        private async Task Update() {
            if (_table != null) await HandleHelp();
        }

        public Vector2 ToWorldPosition(Vector2Int position)
            => transform.TransformPoint(_drawOffset) + new Vector3(position.x, position.y, 0);

        public async Task OnFiguresArrivedAsync(List<Figure> figures) {
            _isHelpingBlocked = true;
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
            _isHelpingBlocked = false;
            _savedTableChangedTime = Time.time;
            _helpFigures = _table.Helper.GetHelp();
        }

        public async Task OnFiguresReplacedAsync(List<Figure> figures) {
            _isHelpingBlocked = true;
            HideHelp();
            DisableTableInput();

            List<Task> movings = new List<Task>();
            foreach (var figure in figures) {
                movings.Add(_figuresDictionary[figure].MoveToPosition());
            }
            await Task.WhenAll(movings);
            EnableTableInput();
            _isHelpingBlocked = false;
        }

        public async Task OnFiguresDestroyedAsync(List<Figure> figures) {
            _isHelpingBlocked = true;
            HideHelp();
            DisableTableInput();

            List<Task> popings = new List<Task>();
            foreach (var figure in figures) {
                popings.Add(_figuresDictionary[figure].Pop());
                _figuresDictionary.Remove(figure);
            }

            await Task.WhenAll(popings);
            _audio.PlayPop();
            EnableTableInput();
            _isHelpingBlocked = false;
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

        private async Task HandleHelp() {
            if (CanHelp) {
                List<Task> tweenings = new List<Task>();
                for (int i = 0; i < _helpFigures.Length; i++) {
                    Figure thisFigure = _helpFigures[i];
                    Figure nextFigure = _helpFigures[(i + 1) % _helpFigures.Length];
                    tweenings.Add(_figuresDictionary[thisFigure].TweenHelp(ToWorldPosition(nextFigure.Position)));
                }
                await Task.WhenAll(tweenings);
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
