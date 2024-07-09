using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SavePanel : MonoBehaviour {
    public Button deleteButton;
    public Button loadButton;
    public TextMeshProUGUI saveName;

    public void Setup(string name) {
        saveName.text = name;
        deleteButton.onClick.AddListener(() => {
            SaveSystem.DeleteSave("NoiseSettings", name);
            UI_SaveSystem._instance.gameObject.SetActive(false);
        });
        loadButton.onClick.AddListener(() => {
            UI_Handler._instance.LoadSetting(name);
            GetComponentInParent<UI_SaveSystem>().Refresh();
        });
    }
}
