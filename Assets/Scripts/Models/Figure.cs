using UnityEngine;

namespace TableLogic {
    public class Figure {
        private readonly Color[] _availableColors = new Color[] {
            Color.red,
            Color.blue,
            Color.yellow,
            Color.green
        };

        private Color _color;
        private int _id;

        public Color Color => _color;
        public int Id => _id;

        public Figure() {
            _id = Random.Range(0, _availableColors.Length);
            _color = _availableColors[_id];
        }
    }
}
