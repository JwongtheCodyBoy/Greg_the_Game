using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KillCounter : MonoBehaviour
{
    [Header("Referemces")]
    public TextMeshProUGUI textDisplay;

    private int killCount = 0;

    private void Start()
    {
        textDisplay.text = killCount.ToString(); 
    }

    public void IncrementCount()
    {
        killCount++;
        textDisplay.text = killCount.ToString();
    }
}
