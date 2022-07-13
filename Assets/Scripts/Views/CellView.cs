using UnityEngine;

namespace TableLogic {
    public class CellView : MonoBehaviour {
        [SerializeField] private SpriteRenderer _fillSpriteRenderer;
        [SerializeField] private SpriteRenderer _borderSpriteRenderer;

        private Cell _cell;

        public Vector2Int Position => _cell.Position;

        public void Construct(Cell cell) {
            _cell = cell;
            ReDraw();
        }

        public void ReDraw() {
            _fillSpriteRenderer.color = _cell.Figure.Color;
        }

        public void Mark() {
            Color color = _cell.Figure.Color;
            color.a = 0.5f;
            _fillSpriteRenderer.color = color;
        }

        public void UnMark() {
            _fillSpriteRenderer.color = _cell.Figure.Color;
        }
    }
}
