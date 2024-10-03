using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HollowPurpleSkill : MonoBehaviour
{
    [SerializeField] private GameObject redSpherePrefab;
    [SerializeField] private GameObject blueSpherePrefab;
    [SerializeField] private GameObject hollowPurplePrefab;
    [SerializeField] private ActionBasedController leftController;
    [SerializeField] private ActionBasedController rightController;
    [SerializeField] private float sphereSpeed = 5f;
    [SerializeField] private float combinationDistance = 0.075f;



    private GameObject redSphere;
    private GameObject blueSphere;

    public void SpawnRedSphere()
    {
        if (redSphere == null)
        {
            redSphere = Instantiate(redSpherePrefab, leftController.transform.position, Quaternion.identity);
        }
    }

    public void SpawnBlueSphere()
    {
        if (blueSphere == null)
        {
            blueSphere = Instantiate(blueSpherePrefab, rightController.transform.position, Quaternion.identity);
        }
    }

    private void Update()
    { 
        MoveSphere(redSphere, leftController.transform.position);
        MoveSphere(blueSphere, rightController.transform.position);

        if (redSphere != null && blueSphere != null)
        {
            if (Vector3.Distance(redSphere.transform.position, blueSphere.transform.position) < combinationDistance)
            {
                CombineSpheres();
            }
        }
    }

    private void MoveSphere(GameObject sphere, Vector3 targetTransform)
    {
        Vector3 targetPosition = targetTransform;
        targetPosition*= 1.1f;
        if (sphere != null)
        {
            sphere.transform.position = Vector3.Lerp(sphere.transform.position, targetPosition, Time.deltaTime * sphereSpeed);
        }
    }

    private void CombineSpheres()
    {
        Vector3 combinationPoint = (redSphere.transform.position + blueSphere.transform.position) / 2f;
        GameObject hollowPurple = Instantiate(hollowPurplePrefab, combinationPoint, Quaternion.identity);
        hollowPurple.GetComponent<HollowPurple>().direction = leftController.transform.forward;

        // Add any additional effects or behaviors for the Hollow Purple skill here

        Destroy(redSphere);
        Destroy(blueSphere);
        redSphere = null;
        blueSphere = null;

        // Optionally, destroy the Hollow Purple effect after some time
        Destroy(hollowPurple, 5f);
    }
}