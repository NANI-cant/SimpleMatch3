using Architecture;
using TableLogic;
using TMPro;
using UnityEngine;

namespace UI {
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ScoreView : MonoBehaviour {
        private Score _score;
        private TextMeshProUGUI _gui;

        private void Awake() {
            if (!Bootstrapper.TryGetInstance<Score>(out _score)) {
                Debug.LogException(new System.Exception("Score is null"));
                return;
            }

            _gui = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable() => _score.Changed += OnChanged;
        private void OnDisable() => _score.Changed -= OnChanged;

        private void Start() => OnChanged(_score.CurrentScore);

        private void OnChanged(int newScore) => _gui.text = newScore.ToString();
    }
}

