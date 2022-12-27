using Architecture;
using PersistentProgress;
using TableLogic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    [RequireComponent(typeof(CanvasGroup))]
    public class GameOverPanel : MonoBehaviour {
        [SerializeField] private Button _restartButton;

        private CanvasGroup _canvasGroup;
        private Table _table;

        private void Awake() {
            if (!Bootstrapper.TryGetInstance(out _table)) {
                Debug.LogException(new System.Exception("Table is null"));
                return;
            }

            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable() {
            _table.NoMoreMatches += OnNoMoreMatches;
            _restartButton.onClick.AddListener(OnRestart);
        }

        private void OnDisable() {
            _table.NoMoreMatches -= OnNoMoreMatches;
            _restartButton.onClick.RemoveListener(OnRestart);
        }

        private void Start() {
            Hide();
        }

        private void OnRestart() {
            _table.Generate();
            Hide();
        }

        private void Show() {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        private void Hide() {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        private void OnNoMoreMatches() => Show();
    }
}
