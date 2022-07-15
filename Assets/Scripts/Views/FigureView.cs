using System.Threading.Tasks;
using UnityEngine;

namespace TableLogic {
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class FigureView : MonoBehaviour {
        [SerializeField] private float _pulllingSpeed = 6f;
        [SerializeField] private float _popSpeed = 0.5f;

        private const double PULLINGACCURACITY = 0.1;

        private SpriteRenderer _spriteRenderer;
        private Collider2D _collider;
        private Figure _figure;
        private TableView _table;

        public Vector2Int Position => _figure.Position;

        private void Awake() {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
        }

        public void Construct(Figure figure, TableView table) {
            _figure = figure;
            _table = table;

            _figure.Choosed += Mark;
            _figure.UnChoosed += UnMark;

            _spriteRenderer.color = _figure.Color;
        }

        private void OnMouseDown() {
            _figure.HandleClick();
        }

        public void Enable() => _collider.enabled = true;
        public void Disable() => _collider.enabled = false;

        public async Task MoveToPosition() {
            Vector3 targetPosition = _table.ToWorldPosition(_figure.Position);
            float distance = Vector2.Distance(transform.position, targetPosition);

            while (distance > PULLINGACCURACITY) {
                Vector2 direction = (targetPosition - transform.position).normalized;
                transform.Translate(direction * _pulllingSpeed * Time.deltaTime);
                await Task.Yield();
                distance = Vector2.Distance(transform.position, targetPosition);
            }
            transform.position = targetPosition;
        }

        public async Task Pop() {
            float currentAlpha = 1f;
            while (currentAlpha > 0) {
                currentAlpha -= _popSpeed * Time.deltaTime;

                Color currentColor = _figure.Color;
                currentColor.a = currentAlpha;
                _spriteRenderer.color = currentColor;

                await Task.Yield();
            }
            Destroy(gameObject);
        }

        private void Mark() {
            Color transparent = _figure.Color;
            transparent.a = 0.5f;
            _spriteRenderer.color = transparent;
        }

        private void UnMark() {
            _spriteRenderer.color = _figure.Color;
        }
    }
}
