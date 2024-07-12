using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private float _speed;
    [SerializeField] private float _scrollMultiplier;

    private Vector3 _dir;
    private Camera _camera;
    private float _scrollAmount;

    private void Awake() {
        _camera = GetComponent<Camera>();
    }

    private void Update() {
        PlayerInput();
        transform.position += _speed * Time.deltaTime * _dir;
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
