 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickupItem : MonoBehaviour
{
    [Header("References")]
    public Gun gunScript;
    public Rigidbody rb;
    public BoxCollider coll;
    public Transform player;
    public Transform gunContainer;
    public Transform fpsCam;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI reloadText;

    [Header("Pickup Stat")]
    public float pickUpRange;
    public float dropForceForward;
    public float dropForceUp;

    public bool equipped;
    public static bool slotFull;

    [Header("Keybind")]
    public KeyCode dropKey = KeyCode.Q;
    public KeyCode pickUpKey = KeyCode.F;

    private void Start()
    {
        //Setup
        if (!equipped)
        {
            gunScript.enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
        if (equipped)
        {
            gunScript.enabled = true;
            rb.isKinematic = true;
            coll.isTrigger = true;
            slotFull = true;
        }
    }

    private void Update()
    {
        //Drop Gun if Player is pressing drop gun button and a gun is inrage
        Vector3 distanceToPlayer = player.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(pickUpKey) && !slotFull) Pickup();

        //Pick up Gun if dropKey is pressed and if gun is equpied by player
        if (equipped && Input.GetKeyDown(dropKey)) Drop();
    }

    private void Pickup()
    {
        equipped = true;
        slotFull = true;

        //Make weapon a child of Gun Container and move it to default position
        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        //Make RigidBody Kinematic and box Colider a trigger
        rb.isKinematic = true;
        coll.isTrigger = true;

        //Enable Gun Script
        gunScript.enabled = true;
    }

    public void Drop()
    {
        equipped = false;
        slotFull = false;

        //Set parent to none
        transform.SetParent(null);

        //Make RigidBody not Kinematic and box Colider not a trigger
        rb.isKinematic = false;
        coll.isTrigger = false;

        //Gun Carries momentum of player
        rb.velocity = player.GetComponent<Rigidbody>().velocity;

        //AddForce
        rb.AddForce(fpsCam.forward * dropForceForward, ForceMode.Impulse);
        rb.AddForce(fpsCam.up * dropForceUp, ForceMode.Impulse);
        //Add randomRotation
        float random = Random.Range(-1, 1);
        rb.AddTorque(new Vector3(random, random, random));

        //Set Ammo Text to nothing
        ammoText.SetText("");
        reloadText.SetText("");

       //Disable Gun Script
       gunScript.enabled = false;
    }

    public void setFull()
    {
        slotFull = true;
    }
}
