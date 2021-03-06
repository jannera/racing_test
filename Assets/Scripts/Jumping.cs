﻿using UnityEngine;
using System.Collections;

public class Jumping : MonoBehaviour {

    private WheelCollider[] wheels;

    private float chargingTimer = 0f;
    private float atMaxTimer = 0f;
    public float maxChargingTimer = 1.5f;
    public float timeToHoldAtMaxCharge = 0.2f;

    public float fullJumpHeight = 3f;

    public float minSecsToBeConsideredOffGround = 0.5f; // after at least one wheel has been off the ground for at least this long, charging of jump is stopped and jumping is not possible

    private float g;

    private float wheelsOffGroundTimer = 0;
    private float[] normalSuspensionDistance;

    private enum GolfCharger { INCREASING, DECREASING, MAX };

    private GolfCharger risingStatus = GolfCharger.INCREASING;

	void Start () {
        wheels = GetComponentsInChildren<WheelCollider>();
        g = Physics.gravity.magnitude;
        normalSuspensionDistance = new float[wheels.Length];
        for (int i = 0; i < wheels.Length; i++)
        {
            normalSuspensionDistance[i] = wheels[i].suspensionDistance;
        }
	}
	
	void Update () {
        if (WheelsOffGroundLongEnough() && GetChargeStatus() > 0)
        {
            ResetCharging();
            // todo: maybe earlier charging could remain the same, instead of resetting?
            return;
        }
        
        if (Input.GetButtonDown("Jump")) 
        {
            StartChargingForJump();
        }
        else if (Input.GetButtonUp("Jump"))
        {
            ReleaseJump();
        }
        else if (Input.GetButton("Jump"))
        {
            ContinueCharging();
        }
	}

    void FixedUpdate()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            if (!wheels[i].isGrounded)
            {
                wheelsOffGroundTimer += Time.deltaTime;
                return;
            }
        }

        wheelsOffGroundTimer = 0;
    }

    bool WheelsOffGroundLongEnough()
    {
        return wheelsOffGroundTimer > minSecsToBeConsideredOffGround;
    }

    void StartChargingForJump()
    {
        chargingTimer = 0;
        risingStatus = GolfCharger.INCREASING;
    }

    void ContinueCharging()
    {
        if (risingStatus == GolfCharger.INCREASING)
        {
            chargingTimer += Time.deltaTime;

            if (chargingTimer > maxChargingTimer)
            {
                chargingTimer = maxChargingTimer;
                atMaxTimer = 0;
                risingStatus = GolfCharger.MAX;
            }
        }
        else if (risingStatus == GolfCharger.DECREASING)
        {
            chargingTimer -= Time.deltaTime;

            if (chargingTimer < 0)
            {
                chargingTimer = 0;
                risingStatus = GolfCharger.INCREASING;
            }
        }
        else if (risingStatus == GolfCharger.MAX)
        {
            atMaxTimer += Time.deltaTime;

            if (atMaxTimer > timeToHoldAtMaxCharge)
            {
                risingStatus = GolfCharger.DECREASING;
            }
        }
        
        UpdateSuspensionDistances();
    }

    void ReleaseJump()
    {
        float relativeCharge = GetChargeStatus();
        float fullStartVelocity = GetFullJumpStartingVelocity();
        rigidbody.AddForce(transform.up * relativeCharge * fullStartVelocity, ForceMode.VelocityChange);
        ResetCharging();
    }

    public float GetChargeStatus()
    {
        return chargingTimer/maxChargingTimer;
    }

    private float GetFullJumpStartingVelocity()
    {
        return Mathf.Sqrt(2f * fullJumpHeight * g);
    }

    void ResetCharging()
    {
        chargingTimer = 0;
        UpdateSuspensionDistances();
    }

    void UpdateSuspensionDistances()
    {
        float status = GetChargeStatus();
        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].suspensionDistance = normalSuspensionDistance[i] - Mathf.Lerp(0, normalSuspensionDistance[i], status);
        }
    }
}
