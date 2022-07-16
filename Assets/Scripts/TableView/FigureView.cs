using System.Threading.Tasks;
using PersistentData;
using TableLogic;
using UnityEngine;

namespace TableView {
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class FigureView : MonoBehaviour {
        [SerializeField] private float _movingSpeed = 6f;
        [SerializeField] private float _popSpeed = 0.5f;

        [SerializeField] private Sprite _markedSprite;
        [SerializeField] private Sprite _unMarkedSprite;
        [SerializeField] private SpriteRenderer _cellRenderer;

        private const double PULLINGACCURACITY = 0.1;

        private SpriteRenderer _figureRenderer;
        private Collider2D _collider;
        private Figure _figure;
        private TableView _table;

        public Vector2Int Position => _figure.Position;

        private void Awake() {
            _figureRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
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

            while (distance > PULLINGACCURACITY) {
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

        public void ShowHelp() => Mark();
        public void StopHelp() => UnMark();

        private void Mark() => _cellRenderer.sprite = _markedSprite;
        private void UnMark() => _cellRenderer.sprite = _unMarkedSprite;
    }
}
