using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunContainer : MonoBehaviour
{
    [Header("References")]
    public Transform followObject;

    private void Update()
    {
        transform.position = followObject.position + (followObject.right * 0.4420013f) + (followObject.up * (1f - 0.9427595f)) + (followObject.forward * 0.4109993f);
        transform.forward = followObject.right;

    }
}
