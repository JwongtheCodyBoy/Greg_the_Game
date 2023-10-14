using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleSwining : MonoBehaviour
{
    [Header("Refrences")]
    public Transform gunTip, cam, player;
    public LayerMask whatIsGrappleable;
    public PlayerMovement pm;
    public GameObject hook;

    [Header("Swinging")]
    private float maxSwingDistance = 25f;
    private Vector3 swingPoint;
    private SpringJoint joint;

    [Header("Inputs")]
    public KeyCode swingKey = KeyCode.Mouse0;

    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode shortenCableKey = KeyCode.Space;
    public KeyCode extendCableKey = KeyCode.S;

    [Header("Odm Gear Movement")]
    public Transform orientation;
    public Rigidbody rb;
    public float horizontalThrustForce;
    public float forwardThrustForce;
    public float extendeCableSpeed;

    [Header("Prediction")]
    public RaycastHit predictionHit;
    public float predictionSphereCastRadius;
    public Transform predictionPoint;

    private void Update()
    {
        //if Player Movement is Restricted cannot grapple
        if (pm.isDead)
        {
            StopSwing();
            return;
        }

        if (Input.GetKeyDown(swingKey)) StartSwing();
        if (Input.GetKeyUp(swingKey)) StopSwing();

        CheckForSwingPoints();

        if (joint != null) OdmGearMovement();
    }

    private void StartSwing()
    {
        //return if predictionHit not found
        if (predictionHit.point == Vector3.zero) return;

        pm.swining = true;
        pm.restricted = true;

        hook.SetActive(false);

        swingPoint = predictionHit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

        //the distance grapple will try to keep from grapple point
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;
    }

    private void StopSwing()
    {
        pm.swining = false;
        pm.restricted = false;
        Destroy(joint);

        hook.SetActive(true);

        pm.desiredMoveSpeed = 9.5f;
    }

    private void OdmGearMovement()
    {
        if (pm.isDead) return;
        //right
        if (Input.GetKey(rightKey)) rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
        //left
        if (Input.GetKey(leftKey)) rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);

        //shorten cable
        if (Input.GetKey(shortenCableKey))
        {
            Vector3 directionToPoint = swingPoint - transform.position;
            rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

            float distanceFormPoint = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = distanceFormPoint * 0.8f;
            joint.minDistance = distanceFormPoint * 0.25f;
        }

        //extend cable
        if (Input.GetKey(KeyCode.S))
        {
            float extendDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendeCableSpeed;

            joint.maxDistance = extendDistanceFromPoint * 0.8f;
            joint.minDistance = extendDistanceFromPoint * 0.25f;
        }
    }

    private void CheckForSwingPoints()
    {
        //if already swining no need to find new points
        if (joint != null) return;

        RaycastHit raycastHit;
        Physics.Raycast(cam.position, cam.forward, out raycastHit, maxSwingDistance, whatIsGrappleable);

        RaycastHit spherecastHit;
        Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, out spherecastHit, maxSwingDistance, whatIsGrappleable);


        Vector3 realHitPoint;

        //Option 1 - Direct Hit
        if (raycastHit.point != Vector3.zero)
            realHitPoint = raycastHit.point;

        //Option 2 - Indirect Hit (SphereCast hit but not RayCast)
        else if (spherecastHit.point != Vector3.zero)
            realHitPoint = spherecastHit.point;

        //Option 3 - Nothing was hit
        else
            realHitPoint = Vector3.zero;

        
        //realHitPoint found
        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }
        //realHitPoint not found
        else
            predictionPoint.gameObject.SetActive(false);

                        //if raycast (direct) is zero, then spherecastHit will be used for prediction point, else raycastHit will be used
        predictionHit = raycastHit.point == Vector3.zero ? spherecastHit : raycastHit;

    }

    //for grapple rope animation
    public Vector3 GetSwingPoint()
    {
        return swingPoint;
    }

    public bool IsGrappling()
    {
        return joint != null;
    }
}
