using UnityEngine;
using System.Collections;

public class LandingAssistant : MonoBehaviour {
    public float minFlyingSecs = 2f; // after you've been this time away from ground, you're considered "flying"
    public float maxLandingTime = 0.5f;
    public float maxGroundDistance = 1f;
    public float landingAcceleration = 1f;

    enum FlyingState { Landed, TakingOff, Flying, Landing };

    FlyingState flyingState;

    WheelCollider[] wheels;

    private RaycastHit hitInfo;

    private float takingOffTimer; // once this exceeds minFlyingSecs, we're officially flying
    private float flyingTimer;
    private float landingTimer;

    void Start()
    {
        wheels = GetComponentsInChildren<WheelCollider>();
        flyingState = FlyingState.Landed;
    }
	
	void FixedUpdate () {
        switch (flyingState)
        {
            case FlyingState.Landed:
                if (AllWheelsOffGround())
                {
                    takingOffTimer = Time.deltaTime;
                    flyingState = FlyingState.TakingOff;
                    Debug.Log("Wheels off the ground");
                }
                break;
            case FlyingState.TakingOff:
                if (AllWheelsOffGround())
                {
                    takingOffTimer += Time.deltaTime;
                    if (takingOffTimer > minFlyingSecs)
                    {
                        flyingState = FlyingState.Flying;
                        Debug.Log("Started flying");
                        flyingTimer = takingOffTimer;
                    }
                }
                else
                {
                    flyingState = FlyingState.Landed;
                    Debug.Log("Wheels were off the ground for " + takingOffTimer + " secs");
                }
                break;
            case FlyingState.Landing:
                landingTimer += Time.deltaTime;
                if (landingTimer > maxLandingTime) {
                    Debug.Log("Landed");
                    flyingState = FlyingState.Landed;
                    // todo: what if the car hasn't actually landed, regardless of trying?
                }
                else {
                    if (GroundBelow()) {
                        PushTowardsGround();
                    }
                }
                break;
            case FlyingState.Flying:
                if (GroundBelow())
                {
                    // ground sighted!
                    flyingState = FlyingState.Landing;
                    Debug.Log("Started landing");
                    landingTimer = 0f;
                    PushTowardsGround();
                }

                break;
        }
	}

    private bool GroundBelow() {
        return Physics.Raycast(transform.position, -transform.up, out hitInfo, maxGroundDistance);
    }

    private void PushTowardsGround() {
        Vector3 force = (hitInfo.point - transform.position).normalized * landingAcceleration * Time.deltaTime;
        rigidbody.AddForce(force, ForceMode.Acceleration);
    }

    private bool AllWheelsOffGround()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            if (wheels[i].isGrounded)
            {
                return false;
            }
        }
        return true;
    }
}
