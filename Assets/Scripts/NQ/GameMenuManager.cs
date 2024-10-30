using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class GameMenuManager : MonoBehaviour
{
    #region Fields

    [SerializeField]
    GameObject menu;

    [SerializeField]
    GameObject instructionCube;

    [SerializeField]
    InputActionProperty vrLeftShowButton;

    [SerializeField]
    InputActionProperty vrRightShowButton;

    [SerializeField]
    Slider daySlider;

    [SerializeField]
    TextMeshProUGUI sliderValue;

    GameObject lightSource;
    #endregion

    // Update is called once per frame
    void Update()
    {
        // Activate Menu when pauseMenu key is pressed on either controller
        if (vrLeftShowButton.action.WasPressedThisFrame() || vrRightShowButton.action.WasPressedThisFrame()) 
        {
            menu.SetActive(!menu.activeSelf);
        }
        // Activate Menu when "1" is pressed by keyboard
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            menu.SetActive(!menu.activeSelf);
        }
        // Update slider value to user while Menu is active
        if (menu.activeSelf)
        {
            sliderValue.text = daySlider.value.ToString("#.00");
        }
    }

    public void DestroyInstructionCube()
    {
        Destroy(instructionCube);
    }
    // Called when user adjust Slider Value
    public void OnDaySliderValueChange()
    {
        lightSource = GameObject.Find("Directional Light");
        if (lightSource != null)
        {
            lightSource.GetComponent<DaySimulator>().ChangeSpeed(daySlider.value);
        }
    }
}
