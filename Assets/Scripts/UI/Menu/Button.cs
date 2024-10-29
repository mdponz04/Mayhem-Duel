using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Button : MonoBehaviour
{
    public void OnPlayButtonClick()
    {
        // Add the Scene you'd like to load in File --> Build Setting
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
       
    }
}
