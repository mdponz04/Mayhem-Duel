using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonScript : MonoBehaviour
{
    #region Serialized Field

    [SerializeField] private GameObject AkPrefab;
    [SerializeField] private GameObject BarretPrefab;
    [SerializeField] private GameObject ColtPrefab;
    [SerializeField] private GameObject GatlingPrefab;
    [SerializeField] private GameObject HandgunPrefab;
    [SerializeField] private GameObject ShotgunPrefab;
    [SerializeField] private GameObject SMAWPrefab;
    [SerializeField] private GameObject UziPrefab;
    [SerializeField] private Transform SpawnPosition;

    #endregion
    public void OnPlayButtonClick()
    {
        // Add the Scene you'd like to load in File --> Build Setting
        SceneManager.LoadScene("TowerDefense", LoadSceneMode.Single);
        // Just in case game start in paused state idk
        Time.timeScale = 1.0f;
        
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }

    public void OnReturnButtonClick()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    // Test Spawn
    public void SpawnAK()
    {
        Instantiate(AkPrefab, SpawnPosition.position + new Vector3(0,1,0), Quaternion.identity);
    }
}
