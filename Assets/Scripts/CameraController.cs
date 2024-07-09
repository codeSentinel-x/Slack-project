using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public bool is2D;
    public float _speed;
    public float _scrollMultiplier;
    private Vector3 _dir;
    private float _scrollAmount;
    private Camera _camera;
    void Awake() {
        _camera = GetComponent<Camera>();
    }
    void Update() {
        PlayerInput();
        transform.position += _dir * _speed * Time.deltaTime;
        if (is2D) _camera.orthographicSize += _scrollAmount * _scrollMultiplier * Time.deltaTime;
        else transform.position += new Vector3(0, _scrollAmount * _scrollMultiplier * Time.deltaTime, 0);
    }

    private void PlayerInput() {
        if (is2D) _dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        else _dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        _scrollAmount = Input.mouseScrollDelta.y;
    }
    public void ChangeSpeed(float v) {
        _speed = v;
    }
}
