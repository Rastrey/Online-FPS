using Player;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public bool IsShootButtonPressed { get; private set; }
    public bool IsZoomButtonPressed { get; private set; }
    public bool IsJumpButtonPressed { get; private set; }
    public bool IsReloadButtonPressed { get; private set; }
    public Vector2 MoveDirection { get; private set; }
    public Vector2 Rotation { get { var a = _rotation; _rotation = Vector2.zero; return a; } private set { _rotation = value; } }
    private Vector2 _rotation;

    public static InputManager Singleton { get { return _singleton; } }
    private static InputManager _singleton;


    private void Awake()
    {
        _singleton = this;
    }

    private void FixedUpdate()
    {
        ComputerInput();
    }

    private void ComputerInput()
    {
        if (Cursor.lockState == CursorLockMode.Locked) return;

        MoveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        IsJumpButtonPressed = Input.GetKey(KeyCode.Space);
        IsShootButtonPressed = Input.GetKey(KeyCode.Mouse0);
        IsReloadButtonPressed = Input.GetKey(KeyCode.R);
        Rotation = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
    }
}
