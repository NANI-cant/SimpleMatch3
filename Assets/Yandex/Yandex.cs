using Architecture;
using PersistentProgress;
using UnityEngine;

namespace Yandex{
    public class Yandex : MonoBehaviour{
        private PersistentProgressService _persistentProgressService;

        private void Awake() {
            if (!Bootstrapper.TryGetInstance(out _persistentProgressService)) {
                Debug.LogException(new System.Exception("PersistentProgressService is null"));
                return;
            }
            
#if UNITY_EDITOR
#else
        _persistentProgressService.Load();
#endif
        }

        public void SetProgress(string progress) {
            _persistentProgressService.SetProgress(progress);
        }
    }
}
