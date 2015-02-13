using UnityEngine;
using System.Collections;

public class WheelSync : MonoBehaviour {
    public WheelCollider collider;

    private Vector3 tmp, basePos;
    private WheelHit wheelHit = new WheelHit();

    void Start() {
        basePos = transform.localPosition;
    }

	void Update () {
        // spin the wheel to create illusion of moving
        transform.Rotate(collider.rpm * 6 * Time.deltaTime, 0, 0);

        // turn the wheel to match steering
        tmp = transform.localEulerAngles;
        tmp.y = collider.steerAngle - transform.localEulerAngles.z;
        transform.localEulerAngles = tmp;
        

        // move the wheel to match the suspension
        if (collider.GetGroundHit(out wheelHit))
        {
            transform.position = wheelHit.point + collider.transform.up * collider.radius;
        }
        else
        {
            // the wheel is in air
            transform.position = collider.transform.position - collider.transform.up * collider.suspensionDistance;
        }
	}
}
