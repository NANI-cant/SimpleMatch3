using Architecture;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour {
    [SerializeField] private string _localizationKey;

    private Localization _localization;
    private TextMeshProUGUI _uGUI;

    private void Awake() {
        Bootstrapper.TryGetInstance<Localization>(out _localization);
        _uGUI = GetComponent<TextMeshProUGUI>();
    }
    private void Start() => ChangeLanguage();
    private void OnEnable() => _localization.LanguageChanged += ChangeLanguage;
    private void OnDestroy() => _localization.LanguageChanged -= ChangeLanguage;

    private void ChangeLanguage() {
        if (_localization == null) return;
        GetComponent<TextMeshProUGUI>().text = _localization.GetLocalizedText(_localizationKey);
    }
}
