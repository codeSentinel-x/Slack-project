using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

    public static MouseController _instance;
    public static Action<Vector2> _OnMouseClickLeft;
    public static Action<Vector2> _OnMouseClickRight;
    public static Action<Vector2> _OnMouseClickScroll;

    private void Awake() {
        _instance = this;
    }
    private void Update() {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetKeyDown(KeyCode.Mouse0)) _OnMouseClickLeft?.Invoke(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (Input.GetKeyDown(KeyCode.Mouse1)) _OnMouseClickRight?.Invoke(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (Input.GetKeyDown(KeyCode.Mouse2)) _OnMouseClickScroll?.Invoke(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}
