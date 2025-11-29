using UnityEngine;

namespace DrivingSimulator.Services
{
    public class GameManager : GenericSingleton<GameManager>
    {
        private bool _isPaused;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Update()
        {
            // Pause / Unpause Game
            //if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }

            // Restart simulation
            //if (Input.GetKeyDown(KeyCode.T))
            {
                RestartSimulation();
            }
        }

        public void TogglePause()
        {
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0 : 1;
        }

        public void RestartSimulation()
        {
            // simple restart logic
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
    }
}
