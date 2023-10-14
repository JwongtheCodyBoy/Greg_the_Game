using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;
    public HealthBar healthBar;
    public GameManagerScript gameManagerScript;

    [Header("Health")]
    public int maxHealth;
    public int currentHealth;

    private bool isDead;

    //private int dmgTake;
    //private bool alreadyTookDmg = false;


    private void Start()
    {
        currentHealth = maxHealth;

        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        if (isDead)
        {
            playerMovement.isDead = true;
            playerMovement.enabled = false;
        }

    }

    public void TakeDamagePlayer(int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            gameManagerScript.GameOver();
        }
    }

    public void HealPlayer(int healAmount)
    {
        currentHealth += healAmount;
        healthBar.SetHealth(currentHealth);
    }

    public void SetMaxHP()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Projectile>() && !alreadyTookDmg)
        {
            dmgTake = collision.collider.GetComponent<Projectile>().explosionDamage;
            TakeDamage(dmgTake);
        }

        alreadyTookDmg = true;
        Invoke(nameof(resetDmgTake), 0.01f);
    }
    

    private void resetDmgTake()
    {
        alreadyTookDmg = false;
        dmgTake = 0;
    }
    */
}
