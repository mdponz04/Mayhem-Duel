using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XRControllerInput : MonoBehaviour
{
    [SerializeField] private ActionBasedController leftController;
    [SerializeField] private ActionBasedController rightController;
    [SerializeField] private InputActionProperty redSphereAction;
    [SerializeField] private InputActionProperty blueSphereAction;
    [SerializeField] private InputActionProperty malevolentShrineAction;

    private HollowPurpleSkill hollowPurpleSkill;
    private MalevolentShrine malevolentShrine;


    private void Start()
    {
        hollowPurpleSkill = GetComponent<HollowPurpleSkill>();
        malevolentShrine = GetComponent<MalevolentShrine>();

        // Enable the actions
        redSphereAction.action.Enable();
        blueSphereAction.action.Enable();

        // Add listeners for the button presses
        if(malevolentShrineAction != null)
        {
            malevolentShrineAction.action.performed += ctx => OnMalevolentShrineButtonPressed();
        }
        if(hollowPurpleSkill != null)
        {
            redSphereAction.action.performed += ctx => OnRedSphereButtonPressed();
            blueSphereAction.action.performed += ctx => OnBlueSphereButtonPressed();
        }
    }

    private void OnDestroy()
    {
        //// Remove listeners when the script is destroyed
        //redSphereAction.action.performed -= ctx => OnRedSphereButtonPressed();
        //blueSphereAction.action.performed -= ctx => OnBlueSphereButtonPressed();

        //// Disable the actions
        //redSphereAction.action.Disable();
        //blueSphereAction.action.Disable();
    }

    private void OnRedSphereButtonPressed()
    {
        hollowPurpleSkill.SpawnRedSphere();
    }

    private void OnBlueSphereButtonPressed()
    {
        hollowPurpleSkill.SpawnBlueSphere();
    }

    private void OnMalevolentShrineButtonPressed()
    {
        malevolentShrine.DomainExpansion();
    }
}