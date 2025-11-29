using UnityEngine;
using DrivingSimulator.Services.Vehicles.Car; // adjust if namespace different

namespace DrivingSimulator.Controllers
{
    public class CameraFollowController : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = new Vector3(0, 3, -8);
        [SerializeField] private float followSpeed = 5f;
        [SerializeField] private float rotationSpeed = 5f;

        private Transform _target;

        private void OnEnable()
        {
            CarService.Instance.OnVehicleSpawned += LinkCar;
        }

        private void OnDisable()
        {
            CarService.Instance.OnVehicleSpawned -= LinkCar;
        }

        private void LinkCar()
        {
            // Automatically get car instance when spawned
            var carController = CarService.Instance.GetCarController();
            if (carController != null)
            {
                _target = carController.CarView.CarFollowPoint;
            }
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            // Follow movement
            Vector3 targetPosition = _target.TransformPoint(offset);
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

            // Look towards car
            Vector3 flatForward = Vector3.ProjectOnPlane(_target.forward, Vector3.up);
            Quaternion targetRot = Quaternion.LookRotation(flatForward, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

        }
    }
}
