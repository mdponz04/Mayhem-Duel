using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class Menus : MonoBehaviour
{
    #region Fields

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private TextMeshProUGUI gameOverMessage;

    [SerializeField] private InputActionProperty vrLeftPauseButton;

    [SerializeField] private InputActionProperty vrRightPauseButton;

    bool isPaused = false;
    #endregion
    private void Start()
    {
        // Test Game Over Menu
        //GameOver("You Won!");
    }
    // Update is called once per frame
    void Update()
    {
        // Make Menu follow player's movement (WIP)
        //Vector3 vHeadPos = Camera.main.transform.position;
        //Vector3 vGazeDir = Camera.main.transform.forward;
        //this.transform.position = (vHeadPos + vGazeDir * 3.0f) + new Vector3(0.0f, -.40f, 0.0f);
        //Vector3 vRot = Camera.main.transform.eulerAngles; vRot.z = 0;
        //this.transform.eulerAngles = vRot;

        // Activate Menu when pauseMenu key is pressed on either controller
        if (vrLeftPauseButton.action.WasPressedThisFrame() || 
            vrRightPauseButton.action.WasPressedThisFrame() || 
            Input.GetKeyDown(KeyCode.Alpha1))
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            if (isPaused)
            {
                UnPause();
            } else
            {
                Pause();
            }
        }
        //// Activate Menu when "1" is pressed by keyboard
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    pauseMenu.SetActive(!pauseMenu.activeSelf);
        //}

    }

    private void Pause()
    {
        isPaused = true;
        Time.timeScale = 0.0f;
        Debug.Log("Pausing");
    }

    private void UnPause()
    {
        isPaused = false;
        Time.timeScale = 1;
        Debug.Log("Continuing");
    }
    /// <summary>
    /// Bring up the Game Over Menu
    /// </summary>
    /// <param name="message">Set the title for the menu, should be 'You Won!' or 'You Lost!'</param>
    public void GameOver(string message)
    {
        //Time.timeScale = 0;
        gameOverMenu.SetActive(!pauseMenu.activeSelf);
        gameOverMessage.text = message;
    }

}
