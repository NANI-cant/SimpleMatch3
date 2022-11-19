using Architecture;
using UnityEngine;

namespace UI {
    [RequireComponent(typeof(TMPro.TMP_Dropdown))]
    public class LanguageDropDown : MonoBehaviour {
        private TMPro.TMP_Dropdown _dropdownList;
        private Localization _localization;

        private void Awake() {
            Bootstrapper.TryGetInstance<Localization>(out _localization);
            _dropdownList = GetComponent<TMPro.TMP_Dropdown>();
            _dropdownList.value = _localization.SelectedLanguage;
        }

        private void OnEnable() => _dropdownList.onValueChanged.AddListener(OnDropdownValueChanged);
        private void OnDisable() => _dropdownList.onValueChanged.RemoveListener(OnDropdownValueChanged);
        private void OnDropdownValueChanged(int value) => _localization.ChangeLanguage(value);
    }
}
