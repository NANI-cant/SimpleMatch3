using System.Threading.Tasks;
using PersistentData;
using TableLogic;
using UnityEngine;

namespace TableView {
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class FigureView : MonoBehaviour {
        [SerializeField][Min(0)] private float _movingSpeed = 6f;
        [SerializeField][Min(0)] private float _popSpeed = 0.5f;
        [SerializeField][Min(0)] private float _helpTweeningReloadTime = 2f;
        [SerializeField][Min(0)] private float _timeForShowing = 2f;

        [SerializeField] private Sprite _markedSprite;
        [SerializeField] private Sprite _unMarkedSprite;
        [SerializeField] private SpriteRenderer _cellRenderer;

        private const double ACCURACITY = 0.1;

        private SpriteRenderer _figureRenderer;
        private Collider2D _collider;
        private Figure _figure;
        private TableView _table;
        private float _savedHelpTime;
        private bool _isInterrupted;

        private bool CanTweenHelp => (Time.time - _savedHelpTime) >= _helpTweeningReloadTime;

        public Vector2Int Position => _figure.Position;

        private void Awake() {
            _figureRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
        }

        private async void Start() {
            await Show();
        }

        private async Task Show() {
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);

            float alpha = 0;
            while (alpha < 1) {
                foreach (var renderer in renderers) {
                    renderer.color = new Color(1, 1, 1, alpha);
                }
                await Task.Yield();
                alpha += 1 / _timeForShowing * Time.deltaTime;
            }
        }

        public void Construct(Figure figure, TableView table) {
            _figure = figure;
            _table = table;

            _figure.Choosed += Mark;
            _figure.UnChoosed += UnMark;

            _figureRenderer.sprite = (_figure.Data as FigureData).Sprite;
            UnMark();
        }

        private void OnMouseDown() {
            _figure.HandleClick();
        }

        public void Enable() => _collider.enabled = true;
        public void Disable() => _collider.enabled = false;

        private void OnDestroy() {
            StopHelp();
        }

        public async Task MoveToPosition() {
            Vector3 targetPosition = _table.ToWorldPosition(_figure.Position);
            float distance = Vector2.Distance(transform.position, targetPosition);

            while (distance > ACCURACITY) {
                Vector2 direction = (targetPosition - transform.position).normalized;
                transform.Translate(direction * _movingSpeed * Time.deltaTime);
                await Task.Yield();
                distance = Vector2.Distance(transform.position, targetPosition);
            }
            transform.position = targetPosition;
        }

        public async Task Pop() {
            float currentAlpha = 1f;
            while (currentAlpha > 0) {
                currentAlpha -= _popSpeed * Time.deltaTime;

                Color currentColor = _figureRenderer.color;
                currentColor.a = currentAlpha;
                _figureRenderer.color = currentColor;

                await Task.Yield();
            }
            Destroy(gameObject);
        }

        public async Task TweenHelp(Vector2 toPosition) {
            if (!CanTweenHelp) return;

            _savedHelpTime = Time.time;
            _isInterrupted = false;
            Vector2 startPosition = transform.position;

            float distance = Vector2.Distance(transform.position, toPosition);
            while (distance > ACCURACITY && !_isInterrupted) {
                Vector2 direction = ((Vector3)toPosition - transform.position).normalized;
                transform.Translate(direction * _movingSpeed * Time.deltaTime);
                await Task.Yield();
                distance = Vector2.Distance(transform.position, toPosition);
            }
            transform.position = toPosition;

            distance = Vector2.Distance(transform.position, startPosition);
            while (distance > ACCURACITY && !_isInterrupted) {
                Vector2 direction = ((Vector3)startPosition - transform.position).normalized;
                transform.Translate(direction * _movingSpeed * Time.deltaTime);
                await Task.Yield();
                distance = Vector2.Distance(transform.position, startPosition);
            }
            transform.position = startPosition;
        }

        public void StopHelp() => _isInterrupted = true;

        private void Mark() => _cellRenderer.sprite = _markedSprite;
        private void UnMark() => _cellRenderer.sprite = _unMarkedSprite;
    }
}
