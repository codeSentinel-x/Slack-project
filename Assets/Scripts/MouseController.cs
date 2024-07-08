using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

    public static MouseController _instance;
    public Action<Vector2> OnMouseClick;
    void Awake() {
        _instance = this;
    }
    public void Update() {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetKeyDown(KeyCode.Mouse0)) OnMouseClick?.Invoke(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}
