using UnityEngine;

namespace DrivingSimulator.Services.Vehicles.Car
{
    public class CarService : GenericSingleton<CarService>
    {
        [SerializeField] private CarView carPrefab;
        [SerializeField] private Transform spawnPoint;

        private CarController3D _currentCarController;
        private CarModel _carModel = new CarModel();

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
        }

        public CarController3D GetCarController()
        {
            return _currentCarController;
        }
    }
}

