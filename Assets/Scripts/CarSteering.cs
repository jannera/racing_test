using UnityEngine;
using System.Collections;

public class CarSteering : MonoBehaviour {
    public WheelCollider[] steeringWheels;
    public WheelCollider[] torqueWheels;
    public WheelCollider[] brakingWheels;

    public float steerMax = 20;
    public float motorMax = 10;
    public float brakeMax = 100;
	
	void Start () {

	}
	
	
	void FixedUpdate () {
        float steer = Mathf.Clamp(Input.GetAxis("Horizontal"), -1, 1);
        float motor = Mathf.Clamp(Input.GetAxis("Vertical"), 0, 1);
        float brake = -1 * Mathf.Clamp(Input.GetAxis("Vertical"), -1, 0);

        for (int i = 0; i < steeringWheels.Length; i++)
        {
            steeringWheels[i].steerAngle = steer * steerMax;
        }

        for (int i = 0; i < torqueWheels.Length; i++)
        {
            torqueWheels[i].motorTorque = motor * motorMax * rigidbody.mass;
        }

        for (int i = 0; i < brakingWheels.Length; i++)
        {
            brakingWheels[i].brakeTorque = brake * brakeMax * rigidbody.mass;
        }
	}
}
