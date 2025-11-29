using UnityEngine;

namespace DrivingSimulator.Services.Vehicles.Car
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarView : MonoBehaviour
    {
        public CarController3D CarController { get; private set; }
        public Rigidbody Rb { get { return _rb; } }
        public Transform CarFollowPoint { get { return _carFollowPoint; } }


        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Transform _carFollowPoint;

        private void Awake()
        {
            _rb.centerOfMass = new Vector3(0, -0.5f, 0); // more stable handling
        }

    }
}
