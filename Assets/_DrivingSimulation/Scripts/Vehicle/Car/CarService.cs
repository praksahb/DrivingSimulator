using UnityEngine;
using DrivingSimulator.Controllers;
using DrivingSimulator.Models;

namespace DrivingSimulator.Services
{
    public class CarService : MonoBehaviour
    {
        public static CarService Instance;

        [SerializeField] private GameObject carPrefab;
        [SerializeField] private Transform spawnPoint;

        private CarController3D _currentCarController;
        private CarModel _carModel = new CarModel();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            SpawnCar();
        }

        public void SpawnCar()
        {
            GameObject carGO = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
            _currentCarController = carGO.GetComponent<CarController3D>();
            _currentCarController.Initialize(_carModel);
        }

        public CarController3D GetCarController()
        {
            return _currentCarController;
        }
    }
}

