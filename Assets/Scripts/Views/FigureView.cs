using UnityEngine;

namespace TableLogic {
    public class FigureView : MonoBehaviour {
        [SerializeField] private SpriteRenderer _figureSpriteRenderer;

        private Figure _figure;

        public void SetFigure(Figure figure) {
            _figure = figure;
            _figureSpriteRenderer.color = _figure.Color;
        }

        public void SetAlpha(float alpha) {
            alpha = Mathf.Clamp(alpha, 0, 1);
            Color currentColor = _figure.Color;
            currentColor.a = alpha;
            _figureSpriteRenderer.color = currentColor;
        }
    }
}
