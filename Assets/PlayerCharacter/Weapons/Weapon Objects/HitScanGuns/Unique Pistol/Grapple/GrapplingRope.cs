using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingRope : MonoBehaviour
{
    private Spring spring;
    private LineRenderer lr;
    private Vector3 currentGrapplePosition;
    public GrapplingHookSecondaryFire grapplingGun;
    public int quality;
    public float damper;
    public float strength;
    public float velocity;
    public float waveCount;
    public float waveHeight;
    public float ropeSpeed;
    public AnimationCurve affectCurve;

    private bool ropeInitialized = false;
    private bool hasSetDirection = true;
    private Vector3 grappleDirection;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        spring = new Spring();
        spring.SetTarget(0);
    }

    //Called after Update
    void LateUpdate()
    {
        DrawRope();
    }

    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!grapplingGun.IsGrappling() && !grapplingGun.IsReturning())
        {
            currentGrapplePosition = grapplingGun.gunTip.position;
            spring.Reset();
            if (lr.positionCount > 0)
                lr.positionCount = 0;
            return;
        }

        if (lr.positionCount != quality + 1)
        {
            lr.positionCount = quality + 1;
        }

        var grapplePoint = grapplingGun.GetGrapplePoint();
        var gunTipPosition = grapplingGun.gunTip.position;
        var up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.FakeUpdate(Time.deltaTime);

        if (!ropeInitialized)
        {
            spring.SetVelocity(velocity);
            currentGrapplePosition = gunTipPosition;
            ropeInitialized = true;
        }

        //pulling
        if (!grapplingGun.IsReturning())
        {
            if (hasSetDirection)
            {
                grappleDirection = (grapplePoint - gunTipPosition).normalized;
                hasSetDirection = false;
            }

            currentGrapplePosition += grappleDirection * ropeSpeed * Time.deltaTime;

            if (Vector3.Dot(grappleDirection, (grapplePoint - currentGrapplePosition).normalized) < 0)
            {
                currentGrapplePosition = grapplePoint;

                if (!grapplingGun.IsPulling())
                    StartCoroutine(grapplingGun.ExecuteGrapple());
            }


            for (var i = 0; i < quality + 1; i++)
            {
                var delta = i / (float)quality;
                var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value *
                             affectCurve.Evaluate(delta);

                lr.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
            }
        }
        //Returning
        else
        {
            Vector3 returnDirection = (gunTipPosition - currentGrapplePosition).normalized;

            currentGrapplePosition += returnDirection * (ropeSpeed/1.5f) * Time.deltaTime;

            if (Vector3.Dot(returnDirection, (gunTipPosition - currentGrapplePosition).normalized) < 0)
            {
                currentGrapplePosition = gunTipPosition;
                grapplingGun.Reset();
                hasSetDirection = true;
                ropeInitialized = false;
                spring.Reset();
            }

            for (var i = 0; i < quality + 1; i++)
            {
                var delta = i / (float)quality;
                var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value *
                             affectCurve.Evaluate(delta);

                lr.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
            }
        }
    }
}
