using System.Threading.Tasks;
using UnityEngine;

namespace TableLogic {
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class CellView : MonoBehaviour {
        [SerializeField] private float _pulllingSpeed = 6f;
        [SerializeField] private float _popSpeed = 0.5f;

        [HideInInspector] public FigureView FigureView;

        private const double PULLINGACCURACITY = 0.1;

        private SpriteRenderer _borderSpriteRenderer;
        private Collider2D _collider;
        private Cell _cell;

        public Vector2Int Position => _cell.Position;

        public void Construct(Cell cell, FigureView figureView) {
            _cell = cell;
            FigureView = figureView;

            _cell.Choosen += OnChoosen;
            _cell.UnChoosen += OnUnChoosen;

            ReDraw();
        }

        private void Awake() {
            _borderSpriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
        }

        private void OnMouseDown() {
            _cell.HandleClick();
        }

        public void ReDraw() {
            FigureView.SetFigure(_cell.Figure);
        }

        public void Mark() {
            _borderSpriteRenderer.color = Color.black;
        }

        public void UnMark() {
            _borderSpriteRenderer.color = Color.white;
        }

        private void OnUnChoosen(Cell cell) => UnMark();
        private void OnChoosen(Cell cell) => Mark();

        public async Task PullFigure() {
            float distance = Vector2.Distance(transform.position, FigureView.transform.position);
            while (distance > PULLINGACCURACITY) {
                Vector2 direction = (transform.position - FigureView.transform.position).normalized;
                FigureView.transform.Translate(direction * _pulllingSpeed * Time.deltaTime);
                await Task.Yield();
                distance = Vector2.Distance(transform.position, FigureView.transform.position);
            }
            FigureView.transform.position = transform.position;
        }

        public void Enable() => _collider.enabled = true;
        public void Disable() => _collider.enabled = false;

        public async Task PopFigure() {
            float currentAlpha = 1f;
            while (currentAlpha > 0) {
                currentAlpha -= _popSpeed * Time.deltaTime;
                FigureView.SetAlpha(currentAlpha);
                await Task.Yield();
            }
        }
    }
}
