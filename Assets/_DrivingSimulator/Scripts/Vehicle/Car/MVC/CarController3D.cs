using UnityEngine;

namespace DrivingSimulator.Services.Vehicles.Car
{
    public class CarController3D
    {
        private Rigidbody _rb;

        public CarModel CarModel { get; set; }
        public CarView CarView { get; set; }

        public CarController3D(CarModel carModel, CarView carView)
        {
            CarModel = carModel;
            CarView = carView;
        }

        public void ResetCar()
        {
            CarView.transform.rotation = Quaternion.identity;
            CarView.transform.position += Vector3.up * 2f;
            CarView.Rb.linearVelocity = Vector3.zero;
            CarView.Rb.angularVelocity = Vector3.zero;
        }


        public float GetCurrentSpeed()
        {
            return CarView.Rb.linearVelocity.magnitude * 3.6f; // m/s -> km/h
        }
    }
}
