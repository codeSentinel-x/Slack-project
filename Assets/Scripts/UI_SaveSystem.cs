using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class UI_SaveSystem : MonoBehaviour {
    public static UI_SaveSystem _instance;
    public RectTransform savesHolder;
    public GameObject LoadMenuPrefab;
    public TMP_InputField saveNameInput;
    // public TextMeshProUGUI 

    void Awake() {
        _instance = this;
    }
    // void Start() {
    //     CreateMenu();
    // }
    public void Refresh() {
        foreach (var t in savesHolder.GetComponentsInChildren<Transform>()) {
            if (t.gameObject == null) continue;
            if (t == savesHolder) continue;
            Destroy(t.gameObject);
        }
        CreateMenu();
    }
    public void CreateMenu() {
        foreach (string name in SaveSystem.GetAllFileNameFromDirectory(SaveSystem.NOISE_SETTING_DEFAULT_SAVE_PATH)) {
            Debug.Log('s');
            Instantiate(LoadMenuPrefab, savesHolder).GetComponent<UI_SavePanel>().Setup(name);
        }
    }
    public void SaveSetting() {
        UI_Handler._instance.SaveCurrentSetting(saveNameInput.text);
        Debug.Log(SaveSystem.PERSISTANCE_DATA_PATH + " + " + SaveSystem.NOISE_SETTING_DEFAULT_SAVE_PATH);
    }

}

