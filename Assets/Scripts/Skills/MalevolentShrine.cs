using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class MalevolentShrine : MonoBehaviour
{
    [SerializeField] private GameObject malevolentShrinePrefab;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject spherebound;
    [SerializeField] private AudioClip shrineSound;
    [SerializeField] private AudioClip slashSound;

    [Header("Animation Attribute")]
    [SerializeField] private float riseHeight = 5f;
    [SerializeField] private float riseTime = 5f;
    [SerializeField] private float distance = 5f;

    private Light shrineLight;
    private ParticleSystem particleSystem;
    bool canExecute = true;


    private void Awake()
    {
        malevolentShrinePrefab = Instantiate(malevolentShrinePrefab, player.transform.position + (Vector3.down * riseHeight), Quaternion.identity);
        Deactive();
    }
    private void Deactive()
    {
        List<GameObject> gameObjects = new List<GameObject>();
        malevolentShrinePrefab.GetChildGameObjects(gameObjects);
        spherebound = gameObjects[gameObjects.Count - 1];
        shrineLight = gameObjects[gameObjects.Count - 1].GetComponentInChildren<Light>();
        particleSystem = gameObjects[gameObjects.Count - 2].GetComponent<ParticleSystem>();
        particleSystem.Stop();
        shrineLight.enabled = false;
        spherebound.SetActive(false);
    }

    private void Update()
    {

    }

    public void DomainExpansion()
    {
        if (!canExecute) return;
        AudioManager.instance.PlayAudioClip(shrineSound);
        StartCoroutine(RiseShrine());
    }

    private IEnumerator RiseShrine()
    {
        canExecute = false;
        malevolentShrinePrefab.transform.position = player.transform.position + (Vector3.down * riseHeight);
        yield return new WaitForSeconds(5f);
        float elapsedTime = 0f;
        Vector3 startPosition = player.transform.position + (Vector3.down * riseHeight);
        malevolentShrinePrefab.SetActive(true);
        Deactive();
        while (elapsedTime < riseTime)
        {
            malevolentShrinePrefab.transform.position = Vector3.Lerp(startPosition, startPosition + (Vector3.up * riseHeight), elapsedTime / riseTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        spherebound.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayAudioClip(slashSound);
        shrineLight.enabled = true;
        particleSystem.Play();
        yield return new WaitForSeconds(15f);
        malevolentShrinePrefab.SetActive(false);
        canExecute = true;
        yield break;
    }

}
