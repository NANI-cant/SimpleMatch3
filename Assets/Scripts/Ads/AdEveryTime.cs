using Architecture;
using Infrastructure;
using UnityEngine;

namespace Ads {
    public class AdEveryTime : MonoBehaviour {
        [SerializeField][Min(10f)] private float _showDelay = 360;

        private AdService _adService;
        private float _accumulator = 0f;

        private void Awake() {
            Bootstrapper.TryGetInstance<AdService>(out _adService);
        }

        private void Update() {
            _accumulator += Time.deltaTime;
            if (_accumulator >= _showDelay) {
                _adService.ShowAd();
                _accumulator = 0f;
            }
        }
    }
}
