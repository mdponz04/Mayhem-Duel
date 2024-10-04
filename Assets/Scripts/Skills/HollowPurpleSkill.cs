using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
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
    private float combinationTime = 7f;
    [SerializeField] private AudioClip hollowPurpleSound;


    private GameObject redSphere;
    private GameObject blueSphere;
    List<GameObject> spheres = new List<GameObject>();
    private bool isCombining = false;

    public void SpawnRedSphere()
    {
        if (redSphere == null)
        {
            redSphere = Instantiate(redSpherePrefab, leftController.transform.position, Quaternion.identity);
            redSphere.GetComponent<MeshRenderer>().enabled = false;
            redSphere.GetChildGameObjects(spheres);
            foreach (GameObject sphere in spheres)
            {
                if(sphere.GetComponent<MeshRenderer>() != null)
                {
                    sphere.GetComponent<MeshRenderer>().enabled = false;
                }
                if(sphere.GetComponentInChildren<ParticleSystem>() != null)
                {
                    sphere.GetComponentInChildren<ParticleSystem>().Stop();
                }    
            }
        }
    }

    public void SpawnBlueSphere()
    {
        if (blueSphere == null)
        {
            blueSphere = Instantiate(blueSpherePrefab, rightController.transform.position, Quaternion.identity);
            blueSphere.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void Update()
    { 
        MoveSphere(redSphere, leftController.transform.position);
        MoveSphere(blueSphere, rightController.transform.position);
        if (redSphere != null && blueSphere != null && !isCombining)
        {
            if (Vector3.Distance(redSphere.transform.position, blueSphere.transform.position) < combinationDistance)
            {
                Vector3 combinationPoint = (redSphere.transform.position + blueSphere.transform.position) / 2;
                StartCoroutine(TriggerHollowPurple(combinationPoint));
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

    private IEnumerator TriggerHollowPurple(Vector3 combinationPoint)
    {
        AudioSource.PlayClipAtPoint(hollowPurpleSound, combinationPoint);
        isCombining = true;
        yield return new WaitForSeconds(0.5f);
        blueSphere.GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(combinationTime/2);
        foreach (GameObject sphere in spheres)
        {
            if (sphere.GetComponent<MeshRenderer>() != null)
            {
                sphere.GetComponent<MeshRenderer>().enabled = true;
            }
            if (sphere.GetComponentInChildren<ParticleSystem>() != null)
            {
                sphere.GetComponentInChildren<ParticleSystem>().Play();
            }
        }

        redSphere.GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(combinationTime/2);
        yield return StartCoroutine(SpinAndMergeSphere(combinationPoint));
        combinationPoint = (redSphere.transform.position + blueSphere.transform.position) / 2;
        GameObject hollowPurple = Instantiate(hollowPurplePrefab, combinationPoint, Quaternion.identity);
        Destroy(redSphere);
        Destroy(blueSphere);
        yield return new WaitForSeconds(1.5f);
        hollowPurple.GetComponent<HollowPurple>().direction = leftController.transform.forward;

        isCombining = false;
        // Add any additional effects or behaviors for the Hollow Purple skill here


        redSphere = null;
        blueSphere = null;

        //Optionally, destroy the Hollow Purple effect after some time
        Destroy(hollowPurple, 6f);
    }

    private IEnumerator SpinAndMergeSphere(Vector3 combinationPoint)
    {
        combinationPoint = (redSphere.transform.position + blueSphere.transform.position) / 2;
        float spinSpeed = 0.25f;
        float mergeSpeed = 0.1f;
        float mergeTime = 5f;

        float elapsedTime = 0f;
        while (elapsedTime < mergeTime)
        {
            float t = elapsedTime / mergeTime;
            float angle = t * 360f;
            float distance = Mathf.Lerp(1, 0, t);

            if (blueSphere != null)
            {
                Vector3 bluePosition = combinationPoint + Quaternion.Euler(0, 0, angle) * Vector3.right * distance * mergeSpeed;
                blueSphere.transform.Rotate(Vector3.forward, spinSpeed);
                blueSphere.transform.position = bluePosition;
            }

            if (redSphere != null)
            {
                Vector3 redPosition = combinationPoint + Quaternion.Euler(0, 0, angle + 180) * Vector3.right * distance * mergeSpeed;
                redSphere.transform.Rotate(Vector3.forward, spinSpeed);
                redSphere.transform.position = redPosition;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}