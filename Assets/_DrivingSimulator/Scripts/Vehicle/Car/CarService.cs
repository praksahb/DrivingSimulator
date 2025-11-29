using System;
using UnityEngine;

namespace DrivingSimulator.Services.Vehicles.Car
{
    public class CarService : GenericSingleton<CarService>
    {
        [SerializeField] private CarView carPrefab;
        [SerializeField] private Transform spawnPoint;

        private CarController3D _currentCarController;  
        
        protected override void Awake()
        {
            base.Awake();

        }

        private void Start()
        {
            SpawnCar();
        }




        public void SpawnCar()
        {
            CarView carGO = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
            CarModel carModel = new CarModel();

            _currentCarController = new CarController3D(carModel, carGO);

            OnVehicleSpawned?.Invoke();
        }


        public event Action OnVehicleSpawned;

        public CarController3D GetCarController()
        {
            return _currentCarController;
        }
    }
}

