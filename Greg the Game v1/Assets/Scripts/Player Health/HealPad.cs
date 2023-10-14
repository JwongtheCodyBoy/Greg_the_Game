using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPad : MonoBehaviour
{
    [Header("References")]
    public Transform healthPack;

    [Header("Stats")]
    public float respawnTime;
    public float rotationSpeed;

    private void Update()
    {
        //Rotating Health pack
        healthPack.Rotate(new Vector3(0f, 0f, rotationSpeed) * Time.deltaTime);
    }

    public void StartTimer()
    {
        Invoke(nameof(RespawnPack), respawnTime);
    }

    private void RespawnPack()
    {
        healthPack.gameObject.SetActive(true);
    }
}
