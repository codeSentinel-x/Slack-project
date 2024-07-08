using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
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
        _camera.orthographicSize += _scrollAmount * _scrollMultiplier * Time.deltaTime;
    }

    private void PlayerInput() {
        _dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _scrollAmount = Input.mouseScrollDelta.y;
    }
    public void ChangeSpeed(float v) {
        _speed = v;
    }
}
