using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("GAME START");
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Debug.Log("QUIT");
        //Application.Quit();
    }

}
