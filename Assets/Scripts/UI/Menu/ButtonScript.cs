using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonScript : MonoBehaviour
{
    public void OnPlayButtonClick()
    {
        // Add the Scene you'd like to load in File --> Build Setting
        SceneManager.LoadScene("TowerDefense", LoadSceneMode.Single);
        // Just in case game start in paused state idk
        Time.timeScale = 1.0f;
        
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }

    public void OnReturnButtonClick()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
