using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingRope : MonoBehaviour
{
    public LineRenderer lr;
    public GrappleSwining gs;

    public int quality;
    public float damper, strength, velocity, waveCount, waveHeight;
    public AnimationCurve affectCurve;


    private Vector3 currentGrapplePosition;
    private Spring spring;

    private void Awake()
    {
        spring = new Spring();
        spring.SetTarget(0);
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void DrawRope()
    {
        //if not grappling, dont draw rope
        if (!gs.IsGrappling())
        {
            currentGrapplePosition = gs.gunTip.position;
            spring.Reset();
            if (lr.positionCount > 0)
                lr.positionCount = 0;
            return;
        }

        if (lr.positionCount == 0)
        {
            spring.SetVelocity(velocity);
            lr.positionCount = quality + 1;
        }

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        Vector3 grapplePoint = gs.GetSwingPoint();
        Vector3 gunTipPos = gs.gunTip.position;
        Vector3 up = Quaternion.LookRotation((grapplePoint - gunTipPos).normalized) * Vector3.up;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        for(var i = 0; i < quality + 1; i++)
        {
            float delta = i / (float)quality;
            Vector3 offset =  up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);

            lr.SetPosition(i, Vector3.Lerp(gunTipPos, currentGrapplePosition, delta) + offset);
        }
    }
}
