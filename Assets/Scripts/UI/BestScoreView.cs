using Architecture;
using PersistentProgress;
using TMPro;
using UnityEngine;

namespace UI{
    [RequireComponent(typeof(TMP_Text))]
    public class BestScoreView : MonoBehaviour {
        private TMP_Text _text;
        private PersistentProgressService _persistentProgressService;

        private void Awake() {
            if (!Bootstrapper.TryGetInstance<PersistentProgressService>(out _persistentProgressService)) {
                Debug.LogException(new System.Exception("PersistentProgressService is null"));
                return;
            }
            
            _text = GetComponent<TMP_Text>();
        }

        private void OnEnable() => _persistentProgressService.Changed += UpdateUI;
        private void OnDisable() => _persistentProgressService.Changed -= UpdateUI;
        private void Start() => UpdateUI();
        
        private void UpdateUI() => _text.text = _persistentProgressService.PersistentProgress.BestScore.ToString();
    }
}
