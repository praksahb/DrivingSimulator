using UnityEngine;

namespace DrivingSimulator.Models
{
    [System.Serializable]
    public class CarModel
    {
        public float maxSpeed = 120f;
        public float acceleration = 3000f;
        public float brakeForce = 4000f;
        public float turnSpeed = 60f;
        public float handbrakeStrength = 8000f;
    }
}
