using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEditor;
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
    private bool isTouched = false;

    public void SpawnRedSphere()
    {
        if (isCombining) return;
        Vector3 spawnPosition = leftController.transform.position + leftController.transform.forward * 0.5f;
        if (redSphere == null)
        {
            redSphere = Instantiate(redSpherePrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            redSphere.SetActive(true);
            redSphere.transform.position = spawnPosition;
        }
        redSphere.GetComponent<MeshRenderer>().enabled = false;
        redSphere.GetChildGameObjects(spheres);
        foreach (GameObject sphere in spheres)
        {
            if (sphere.GetComponent<MeshRenderer>() != null)
            {
                sphere.GetComponent<MeshRenderer>().enabled = false;
            }
            if (sphere.GetComponentInChildren<ParticleSystem>() != null)
            {
                sphere.GetComponentInChildren<ParticleSystem>().Stop();
            }
        }
    }

    public void SpawnBlueSphere()
    {
        if (isCombining) return;
        Vector3 spawnPosition = rightController.transform.position + rightController.transform.forward * 0.5f;
        if (blueSphere == null)
        {
            blueSphere = Instantiate(blueSpherePrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            blueSphere.SetActive(true);
            blueSphere.transform.position = spawnPosition;
        }
        blueSphere.GetComponent<MeshRenderer>().enabled = false;
    }

    private void Update()
    {
        Vector3 spawnPositionRed = leftController.transform.position + leftController.transform.forward * 0.5f;
        Vector3 spawnPositionBlue = rightController.transform.position + rightController.transform.forward * 0.5f;
        MoveSphere(redSphere, spawnPositionRed);
        MoveSphere(blueSphere, spawnPositionBlue);
        if (redSphere != null && blueSphere != null && !isCombining && redSphere.activeSelf && blueSphere.activeSelf)
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
        //targetPosition*= 1.1f;
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
        spheres.Clear();
        redSphere.GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(combinationTime/2);
        combinationPoint = (redSphere.transform.position + blueSphere.transform.position) / 2;
        GameObject hollowPurple = Instantiate(hollowPurplePrefab, combinationPoint, Quaternion.identity);
        hollowPurple.GetComponent<HollowPurple>().isScaling = true;
        hollowPurple.GetComponent<MeshRenderer>().enabled = false;
        hollowPurple.GetChildGameObjects(spheres);
        spheres[0].GetComponent<MeshRenderer>().enabled = false;
        spheres[1].GetComponent<ParticleSystem>().Stop();
        yield return StartCoroutine(SpinAndMergeSphere(spheres[1].GetComponent<ParticleSystem>(), hollowPurple));
        hollowPurple.GetComponent<MeshRenderer>().enabled = true;
        spheres[0].GetComponent<MeshRenderer>().enabled = true;
        redSphere.SetActive(false);
        blueSphere.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        hollowPurple.GetComponent<HollowPurple>().direction = leftController.transform.forward;
        hollowPurple.GetComponent<HollowPurple>().isScaling = false;

        // Add any additional effects or behaviors for the Hollow Purple skill heress

        //Optionally, destroy the Hollow Purple effect after some time
        spheres.Clear();
        Destroy(hollowPurple, 6f);
        isCombining = false;
    }

    private IEnumerator SpinAndMergeSphere(ParticleSystem purple, GameObject hollows)
    {
        Vector3 redPoint = rightController.transform.position + rightController.transform.forward * 0.5f;
        Vector3 bluePoint = leftController.transform.position + leftController.transform.forward * 0.5f;
        Vector3 combinationPoint = (bluePoint + redPoint) / 2;
        Vector3 redStartPosition = redSphere.transform.position;
        Vector3 blueStartPosition = blueSphere.transform.position;
        float mergeTime = 5f; // Adjust this value to control the speed of the merge
        float elapsedTime = 0f;

        while (elapsedTime < mergeTime)
        {
            float t = elapsedTime / mergeTime;
            float smoothT = SmoothStep(t); // Apply easing function
            redPoint = rightController.transform.position + rightController.transform.forward * 0.5f;
            bluePoint = leftController.transform.position + leftController.transform.forward * 0.5f;
            combinationPoint = (bluePoint + redPoint) / 2;
            if (blueSphere != null)
            {
                blueSphere.transform.position = Vector3.Lerp(blueStartPosition, combinationPoint, smoothT);
            }

            if (redSphere != null)
            {
                redSphere.transform.position = Vector3.Lerp(redStartPosition, combinationPoint, smoothT);
            }

            if (purple != null)
            {
                hollows.transform.position = combinationPoint;
            }

            if(Vector3.Distance(redSphere.transform.position, blueSphere.transform.position) < 0.075f && !isTouched)
            {
                isTouched = true;
                purple.Play();
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the spheres are exactly at the combination point at the end
        if (blueSphere != null) blueSphere.transform.position = combinationPoint;
        if (redSphere != null) redSphere.transform.position = combinationPoint;
    }

    private float SmoothStep(float t)
    {
        // Smoothstep function for easing
        return t * t * (3f - 2f * t);
    }
}