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
