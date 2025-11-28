using UnityEngine;

namespace DrivingSimulator.Services
{
    public class DayNightCycleService : MonoBehaviour
    {
        [SerializeField] private Light sunLight;
        [SerializeField] private float dayDurationInSeconds = 120f; // full cycle = 2 mins

        [SerializeField] private float minSunIntensity = 0f;
        [SerializeField] private float maxSunIntensity = 1f;

        private float _timeOfDay;

        private void Update()
        {
            // Time progression
            _timeOfDay += Time.deltaTime / dayDurationInSeconds;
            if (_timeOfDay > 1f) _timeOfDay = 0f;

            // Sun rotation (360 degrees cycle)
            float sunAngle = _timeOfDay * 360f;
            sunLight.transform.rotation = Quaternion.Euler(sunAngle - 90f, 170f, 0f);

            // Adjust light intensity according to angle
            float intensity = Mathf.Clamp01(Vector3.Dot(sunLight.transform.forward, Vector3.down));
            sunLight.intensity = Mathf.Lerp(minSunIntensity, maxSunIntensity, intensity);

            // Optional skybox exposure
            RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(0.3f, 1.2f, intensity));
        }
    }
}
