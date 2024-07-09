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
    void Start() {
        CreateMenu();
    }
    public void Refresh() {
        foreach (var t in savesHolder.GetComponentsInChildren<Transform>()) {
            if (t.gameObject == null) continue;
            if (t == savesHolder) continue;
            Destroy(t.gameObject);
        }
        CreateMenu();
    }
    public void CreateMenu() {
        foreach (var s in Directory.GetFiles("NoiseSettings", "*", SearchOption.TopDirectoryOnly)) {
            if (s.EndsWith(".meta")) continue;
            string name = s.TrimStart("NoiseSettings/".ToArray<Char>());
            name = name.TrimEnd(new char[4] { '.', 'b', 'a', 'k' });
            Instantiate(LoadMenuPrefab, savesHolder).GetComponent<UI_SavePanel>().Setup(name);
        }
    }
    public void SaveSetting() {
        UI_Handler._instance.SaveCurrentSetting(saveNameInput.text);
    }

}

