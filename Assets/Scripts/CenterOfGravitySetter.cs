using UnityEngine;
using System.Collections;

public class CenterOfGravitySetter : MonoBehaviour {
    public GameObject centerOfGravity;

	// Use this for initialization
	void Start () {
        rigidbody.centerOfMass = centerOfGravity.transform.localPosition;
	}
}
