using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarMovementController : MonoBehaviour
{
    private Rigidbody _rb;

    [Header("Car Settings")]
    public float acceleration = 3000f;
    public float turnSpeed = 60f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    private void FixedUpdate()
    {
        _rb.AddForce(Vector3.down * 2000f);


        Vector2 move = InputReader.Move;

        float throttle = move.y;
        float steer = move.x;

        if (throttle != 0f)
        {
            _rb.AddForce(transform.forward * throttle * acceleration * Time.fixedDeltaTime, ForceMode.Acceleration);
        }

        if (Mathf.Abs(steer) > 0.1f)
        {
            float turn = steer * turnSpeed * Time.fixedDeltaTime * (_rb.linearVelocity.magnitude / 10f);
            transform.Rotate(0f, turn, 0f);
        }

        if (InputReader.ResetPressed)
        {
            ResetCar();
            InputReader.ResetPressed = false;
        }


        StabilizeCar();

    }

    private void StabilizeCar()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(_rb.linearVelocity);

        // Only stabilize when moving forward or airborne
        if (_rb.linearVelocity.magnitude > 1f)
        {
            Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        }
    }


    private void ResetCar()
    {
        transform.rotation = Quaternion.identity;
        transform.position += Vector3.up * 2f;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }
}
