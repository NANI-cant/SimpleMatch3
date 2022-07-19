using UnityEngine;

namespace CameraLogic {
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class ProceduralBackground : MonoBehaviour {
        [SerializeField] private Gradient _gradient;
        [SerializeField] private float timeForCycle = 60f;

        private float _progress = 0f;
        private Camera _camera;

        private void Awake() {
            _camera = GetComponent<Camera>();
            _progress = Random.Range(0f, 1f);
        }

        private void Update() {
            if (Application.isPlaying) _progress += Time.deltaTime * 1 / timeForCycle;
            if (_progress > 1) _progress = 0;

            _camera.backgroundColor = _gradient.Evaluate(_progress);
        }
    }
}
