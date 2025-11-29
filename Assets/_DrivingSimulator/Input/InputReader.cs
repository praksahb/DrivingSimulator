using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, CarInputActions.IDrivingActions
{
    private CarInputActions _input;

    public static Vector2 Move;
    public static bool ResetPressed;

    private void Awake()
    {
        _input = new CarInputActions();
        _input.Driving.SetCallbacks(this);
    }

    private void OnEnable() => _input.Enable();
    private void OnDisable() => _input.Disable();

    public void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        if (context.performed) ResetPressed = true;
    }

    public void OnCameraSwitch(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnHandbrake(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
}
