using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    #region Fields
    private Vector3 _basePosition = new Vector3(0, 0.1f, 0);
    private Vector3 _baseScale = new Vector3(0.2f, 0.2f, 0.2f);

    private bool isQuitting = false;
    #endregion

    #region Methods
    private void Start()
    {
        
        ResetCube();
    }
    // Make sure some function doesn't get called when game end
    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnBecameInvisible()
    {
        if (!isQuitting)
        {
            ResetCube();
            Debug.Log("Guess Who's Back");
        }
    }
    // Reset to default position
    private void ResetCube()
    {
        transform.rotation = Quaternion.identity;
        transform.localScale = _baseScale;
        transform.position = _basePosition;
    }
    /// <summary>
    /// Make Cube 2x as small
    /// </summary>
    public void Shrink()
    {
        transform.localScale *= 0.5f;
        Debug.Log("I'm Shrinking!!");
    }
    /// <summary>
    /// Make Cube 2x as big
    /// </summary>
    public void Enlarge()
    {
        transform.localScale *= 2;
        Debug.Log("I'm Getting Bigger!!");
    }
    #endregion
}
