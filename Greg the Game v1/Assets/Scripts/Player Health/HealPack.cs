using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPack : MonoBehaviour
{
    [Header("References")]
    private HealPad healPad;

    [Header("Heal")]
    public int healAmount;
    
    private void Start()
    {
        healPad = gameObject.GetComponentInParent<HealPad>();
        if (healAmount == 0) healAmount = 50;    
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Touched HealthPack");
        //Heals Player if Collision is with player
        if (other.CompareTag("Player"))
        {
            //Does nothing if Player is at max HP
            if (other.GetComponent<HealthHandler>().currentHealth == other.GetComponent<HealthHandler>().maxHealth) return;
            other.GetComponent<HealthHandler>().HealPlayer(healAmount);
            Debug.Log("Health Pack Heals");

            //Prevents Overheals, set hp to max hp if it is > than max
            if (other.GetComponent<HealthHandler>().currentHealth > other.GetComponent<HealthHandler>().maxHealth)
                other.GetComponent<HealthHandler>().SetMaxHP();

            HaveBeenUsed();
        }
    }
    private void HaveBeenUsed()
    {
        //Calls Heal Pad to start a respawn timer and Set self to deactive
        healPad.StartTimer();
        gameObject.SetActive(false);
    }
}
