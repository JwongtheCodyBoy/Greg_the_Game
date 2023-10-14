using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManagerScript : MonoBehaviour
{
    [Header("References")]
    public GameObject gameOverUI;
    public GameObject cam;
    //private EnemyGunScript enemyGuns;

    [Header("Script References for re-ininitilzation")]
    public SpawnerManager spawnerManager;

    [Header("Global Management Stuff")]
    public int maxNumGuns;

    [Header("Menu stuff")]
    public KeyCode pauseKey = KeyCode.Keypad0;
    public TextMeshProUGUI menuText;
    private bool isPaused;

    private void Start()
    {
        EnemyGunScript.maxNumGuns = maxNumGuns;
    }

    private void Update()
    {
        if (!isPaused && Input.GetKeyDown(pauseKey))
        {
            Debug.Log("pause");
            isPaused = true;
            menuText.text = "PAUSED";
            GameOver();
        }
        else if (isPaused && Input.GetKeyDown(pauseKey))
        {
            Debug.Log("un-pause");
            isPaused = false;
            menuText.text = "YOU DIED";
            Resume();
        }
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        cam.GetComponent<PlayerCam>().enabled = false;
    }

    public void Resume()
    {
        gameOverUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        cam.GetComponent<PlayerCam>().enabled = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        spawnerManager.Initilization();
        EnemyGunScript.currentNumGuns = 0;
    }

    public void MainMenu()
    {
        Debug.Log("Main Menu Button was press");
        SceneManager.LoadScene(0); 
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
