using Architecture;
using Infrastructure;
using UnityEngine;

namespace Ads {
    public class AdOnStart : MonoBehaviour {
        private AdService _adService;

        private void Awake() {
            Bootstrapper.TryGetInstance<AdService>(out _adService);
        }

        private void Start() {
            _adService.ShowAd();
        }
    }
}
