using UnityEngine;

namespace CameraLogic {
    [RequireComponent(typeof(Camera))]
    public class AutoFocus : MonoBehaviour {
        [SerializeField][Min(0)] private float _buffer;

        private Camera _camera;

        private void Awake() {
            _camera = GetComponent<Camera>();
        }

        private void Start() {
            var (center, size) = CalculateOrthographicSize();
            _camera.transform.position = center;
            _camera.orthographicSize = size;
        }

        private (Vector3 center, float size) CalculateOrthographicSize() {
            Bounds bounds = new Bounds();

            foreach (var collider in FindObjectsOfType<Collider2D>()) {
                bounds.Encapsulate(collider.bounds);
            }

            bounds.Expand(_buffer);

            float vertical = bounds.size.y;
            float horizontal = bounds.size.x * _camera.pixelHeight / _camera.pixelWidth;

            float size = Mathf.Max(horizontal, vertical) / 2;
            Vector3 center = bounds.center + new Vector3(0, 0, -10);

            return (center, size);
        }
    }
}
