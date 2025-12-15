using UnityEngine;
using UnityEngine.InputSystem;

namespace DrivingSimulator.Input
{
    public class InputController : MonoBehaviour
    {
        private CarInputActions _input;

        public Vector2 MoveInput { get; private set; }
        public bool ResetPressed { get; private set; }
        public bool HandbrakeHeld { get; private set; }
        public bool CameraSwitchPressed { get; private set; }

        private void Awake()
        {
            _input = new CarInputActions();
        }

        private void OnEnable()
        {
            _input.Enable();

            _input.Driving.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
            _input.Driving.Move.canceled += _ => MoveInput = Vector2.zero;

            _input.Driving.Reset.performed += _ => ResetPressed = true;

            _input.Driving.Handbrake.performed += _ => HandbrakeHeld = true;
            _input.Driving.Handbrake.canceled += _ => HandbrakeHeld = false;

            _input.Driving.CameraSwitch.performed += _ => CameraSwitchPressed = true;
        }

        private void LateUpdate()
        {
            // Consume single-frame events
            ResetPressed = false;
            CameraSwitchPressed = false;
        }

        private void OnDisable()
        {
            _input.Disable();
        }
    }
}
