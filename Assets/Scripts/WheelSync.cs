using UnityEngine;
using System.Collections;

public class WheelSync : MonoBehaviour {
    public WheelCollider collider;

    private Vector3 tmp, basePos;

    void Start() {
        basePos = transform.localPosition;
    }

	void Update () {
        transform.localPosition = basePos + collider.center;
        transform.Rotate(-collider.rpm * 6 * Time.deltaTime, 0, 0);
	}
}
