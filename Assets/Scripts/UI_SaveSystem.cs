using TMPro;
using UnityEngine;

public class UI_SaveSystem : MonoBehaviour {
    public static UI_SaveSystem _instance;
    [SerializeField] private RectTransform _savesHolder;
    [SerializeField] private GameObject _loadMenuPrefab;
    [SerializeField] private TMP_InputField _saveNameInput;


    private void Awake() {
        _instance = this;
    }

    public void Refresh() {
        foreach (var t in _savesHolder.GetComponentsInChildren<Transform>()) {
            if (t.gameObject == null) continue;
            if (t == _savesHolder) continue;
            Destroy(t.gameObject);
        }
        CreateMenu();
    }

    public void CreateMenu() {
        foreach (string name in SaveSystem.GetAllFileNameFromDirectory(SaveSystem.NOISE_SETTING_DEFAULT_SAVE_PATH)) {
            Instantiate(_loadMenuPrefab, _savesHolder).GetComponent<UI_SavePanel>().Setup(name);
        }
    }

    public void SaveSetting() {
        UI_Handler._instance.SaveCurrentSetting(_saveNameInput.text);
    }

}

