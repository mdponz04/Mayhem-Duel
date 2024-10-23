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
        Debug.Log("You Clicked Play!");
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
        Debug.Log("You Clicked Quit!");
    }
}
