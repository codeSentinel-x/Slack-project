using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SavePanel : MonoBehaviour {
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Button _loadButton;
    [SerializeField] private TextMeshProUGUI _saveName;

    public void Setup(string name) {
        _saveName.text = name;
        _deleteButton.onClick.AddListener(() => {
            SaveSystem.DeleteSave(SaveSystem.NOISE_SETTING_DEFAULT_SAVE_PATH, name);
            UI_SaveSystem._instance.Refresh();
        });
        _loadButton.onClick.AddListener(() => {
            UI_Handler._instance.LoadSetting(name);
            GetComponentInParent<UI_SaveSystem>().Refresh();
        });
    }
}
