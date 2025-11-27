using UnityEngine;
using DrivingSimulator.Models;

namespace DrivingSimulator.Controllers
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarController3D : MonoBehaviour
    {
        [SerializeField] private CarModel model;
        private Rigidbody _rb;

        private float _steerInput;
        private float _throttleInput;

        public void Initialize(CarModel carModel)
        {
            model = carModel;
        }

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
                ResetCar();
            }
        }

        private void FixedUpdate()
        {
            // forward/backward force
            Vector3 force = transform.forward * _throttleInput * model.acceleration * Time.fixedDeltaTime;
            _rb.AddForce(force, ForceMode.Acceleration);

            // steering
            float turn = _steerInput * model.turnSpeed * Time.fixedDeltaTime * (_rb.linearVelocity.magnitude / 10f);
            transform.Rotate(0f, turn, 0f);
        }

        private void ResetCar()
        {
            transform.rotation = Quaternion.identity;
            transform.position += Vector3.up * 2f;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }
}
