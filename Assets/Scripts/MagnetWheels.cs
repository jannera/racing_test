using UnityEngine;
using System.Collections;

public class MagnetWheels : MonoBehaviour {
    public WheelCollider[] colliders;
    public float magnetAcceleration = 10f;
    public float maxMagnetDst = 2f;

    WheelHit wheelHit = new WheelHit();
    RaycastHit raycastHit = new RaycastHit();
    Vector3 force, tmp, tmp2;
    float[] normalSpringValues;

    void Start()
    {
        forces = new Vector3[colliders.Length];
        normalSpringValues = new float[colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            normalSpringValues[i] = colliders[i].suspensionSpring.spring;
        }
    }

    private Vector3[] forces;

    private bool magnetOn = false;

    // apply magnet only if every wheel is touching or close enough
    void FixedUpdate()
    {
        magnetOn = true;
        for (int i = 0; i < colliders.Length; i++)
        {
            WheelCollider c = colliders[i];
            if (c.GetGroundHit(out wheelHit))
            {
                forces[i] = GetForceFromColliderToPoint(c, wheelHit.point);
            }
            else if (Physics.Raycast(c.transform.position, -c.transform.up, out raycastHit, maxMagnetDst))
            {
                // Debug.Log("hit " + raycastHit.collider);
                forces[i] = GetForceFromColliderToPoint(c, raycastHit.point);
                // todo: should we reduce the force based on the distance?   
            }
            else
            {
                magnetOn = false;
                break;
            }
        }

        if (magnetOn)
        {
            for (int i = 0; i < forces.Length; i++)
            {
                WheelCollider c = colliders[i];
                rigidbody.AddForceAtPosition(forces[i], c.transform.position, ForceMode.Acceleration);
                float force = forces[i].magnitude * rigidbody.mass;
                spring = c.suspensionSpring;
                spring.spring = normalSpringValues[i] + force;
                c.suspensionSpring = spring;
            }
        }
    }

    void Update()
    {
        if (magnetOn)
        {
            for (int i = 0; i < forces.Length; i++)
            {
                Debug.DrawLine(colliders[i].transform.position, colliders[i].transform.position + forces[i].normalized, Color.blue);
            }
        }
    }

    JointSpring spring;

    Vector3 GetForceFromColliderToPoint(WheelCollider c, Vector3 point)
    {
        return (point - c.transform.position).normalized * magnetAcceleration * Time.deltaTime;
    }
}
