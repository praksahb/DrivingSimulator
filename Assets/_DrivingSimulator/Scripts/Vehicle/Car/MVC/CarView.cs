using UnityEngine;

namespace DrivingSimulator.Services.Vehicles.Car
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarView : MonoBehaviour
    {
        public CarController3D CarController {  get; private set; }
        public Rigidbody Rb { get { return _rb; } }



        [SerializeField] private Rigidbody _rb;

        private float _steerInput;
        private float _throttleInput;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.centerOfMass = new Vector3(0, -0.5f, 0); // more stable handling
        }

        private void Update()
        {
            _steerInput = Input.GetAxis("Horizontal");
            _throttleInput = Input.GetAxis("Vertical");

            if (Input.GetKeyDown(KeyCode.R))
            {
                CarController.ResetCar();
            }
        }

        private void FixedUpdate()
        {
            // forward/backward force
            Vector3 force = transform.forward * _throttleInput * CarController.CarModel.acceleration * Time.fixedDeltaTime;
            _rb.AddForce(force, ForceMode.Acceleration);

            // steering
            float turn = _steerInput * CarController.CarModel.turnSpeed * Time.fixedDeltaTime * (_rb.linearVelocity.magnitude / 10f);
            transform.Rotate(0f, turn, 0f);
        }
    }
}
