using UnityEngine;
using System.Collections;

public class AntiRollBar : MonoBehaviour {
    public WheelCollider a, b;
    public float antiRollMultiplier = 1f;
    WheelHit wheelHit = new WheelHit();
    float travelA, travelB;
    bool groundedA, groundedB;
	
	// Update is called once per frame
	void FixedUpdate () {
        travelA = travelB = 1.0f;

        groundedA = a.GetGroundHit(out wheelHit);
        if (groundedA) {
            travelA = GetTravel(a);
        }
        groundedB = b.GetGroundHit(out wheelHit);
        if (groundedB)
        {
            travelB = GetTravel(b);
        }

        var antiRollForce = (travelA - travelB) * (antiRollMultiplier * a.suspensionSpring.spring);

        if (groundedA)
        {
            ApplyForce(a, antiRollForce);
        }
        if (groundedB)
        {
            ApplyForce(b, antiRollForce);
        }    
	}

    private float GetTravel(WheelCollider c)
    {
        return (-c.transform.InverseTransformPoint(wheelHit.point).y - c.radius) / c.suspensionDistance;
    }

    private void ApplyForce(WheelCollider c, float antiRollForce)
    {
        rigidbody.AddForceAtPosition(c.transform.up * -antiRollForce, c.transform.position);
    }
}
