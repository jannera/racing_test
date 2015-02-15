using UnityEngine;
using System.Collections;

public class MagnetWheels : MonoBehaviour {
    public WheelCollider[] colliders;
    public float magnetAcceleration = 10f;
    public float maxMagnetDst = 2f;

    WheelHit wheelHit = new WheelHit();
    RaycastHit raycastHit = new RaycastHit();
    Vector3 force, tmp, tmp2;

    void Start()
    {
        forces = new Vector3[colliders.Length];
    }

    private Vector3[] forces;

    // apply magnet only if every wheel is touching or close enough
    void FixedUpdate()
    {
        bool allAreTouching = true;
        for (int i = 0; i < colliders.Length; i++)
        {
            WheelCollider c = colliders[i];
            if (c.GetGroundHit(out wheelHit))
            {
                forces[i] = GetForceFromColliderToPoint(c, wheelHit.point);
            }
            else if (Physics.Raycast(c.transform.position, -c.transform.up, out raycastHit, maxMagnetDst))
            {
                Debug.Log("hit " + raycastHit.collider);
                forces[i] = GetForceFromColliderToPoint(c, raycastHit.point);
                // todo: should we reduce the force based on the distance?   
            }
            else
            {
                allAreTouching = false;
                break;
            }
        }

        if (allAreTouching)
        {
            for (int i = 0; i < forces.Length; i++)
            {
                rigidbody.AddForceAtPosition(forces[i], colliders[i].transform.position, ForceMode.Acceleration);
                // todo: we should probably do this only with torque, not force..
                /*
                float f = -1;
                if (i == 0 || i == 2)
                {
                    f = 1;
                }
                
                 rigidbody.AddTorque(GetAxisForTorque(rigidbody.worldCenterOfMass, colliders[i].transform.position) 
                    * -f * magnetAcceleration / Time.deltaTime, ForceMode.Acceleration);
                 */
                
                /*
                float horizontal = 1, vertical = 1;
                if (i == 1)
                {
                    horizontal = -1;
                }
                else if (i == 2)
                {
                    vertical = -1;
                }
                else if (i == 3)
                {
                    horizontal = -1;
                    vertical = -1;
                }
                force.Set(vertical, 0, horizontal);
                force *= magnetAcceleration * Time.deltaTime;
                Debug.Log(force);
                rigidbody.AddRelativeTorque(force, ForceMode.Acceleration);
                 * */
            }
        }
    }

    void Update()
    {
        return;
        foreach (WheelCollider c in colliders)
        {
            Debug.DrawLine(c.transform.position, c.transform.position + GetAxisForTorque(rigidbody.worldCenterOfMass, c.transform.position), Color.blue);
        }
    }

    Plane p = new Plane();

    Vector3 GetAxisForTorque(Vector3 cog, Vector3 pos)
    {
        Vector3 fromCoGtoCollider = pos - cog;
        Vector3 reflected = Vector3.Reflect(fromCoGtoCollider, transform.up);

        p.Set3Points(Vector3.zero, fromCoGtoCollider, reflected);
        return p.normal;
    }

    Vector3 GetForceFromColliderToPoint(WheelCollider c, Vector3 point)
    {
        return (point - c.transform.position).normalized * magnetAcceleration / Time.deltaTime;
    }
}
