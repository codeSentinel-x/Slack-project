using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

    public static MouseController _instance;
    public static Action<Vector2> OnMouseClickLeft;
    public static Action<Vector2> OnMouseClickRight;
    public static Action<Vector2> OnMouseClickScroll;

    private void Awake() {
        _instance = this;
    }
    private void Update() {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetKeyDown(KeyCode.Mouse0)) OnMouseClickLeft?.Invoke(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (Input.GetKeyDown(KeyCode.Mouse1)) OnMouseClickRight?.Invoke(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (Input.GetKeyDown(KeyCode.Mouse2)) OnMouseClickScroll?.Invoke(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}
