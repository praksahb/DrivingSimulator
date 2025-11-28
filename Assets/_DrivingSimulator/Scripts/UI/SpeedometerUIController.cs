using UnityEngine;
using TMPro;
using DrivingSimulator.Services.Vehicles.Car;

namespace DrivingSimulator.Controller.UI
{
    public class SpeedometerUIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private string speedFormat = "{0} km/h";

        private CarController3D _carController;

        private void Start()
        {
            // get Car rigidbody via CarService
            _carController = CarService.Instance.GetCarController();
            if (_carController == null)
            {
                Debug.LogWarning("SpeedometerUIController: No car found. UI will not update.");
            }
        }

        private void Update()
        {
            if (_carController == null) return;

            float speed = _carController.GetCurrentSpeed();
            speedText.SetText(string.Format(speedFormat, Mathf.RoundToInt(speed)));
        }
    }
}
