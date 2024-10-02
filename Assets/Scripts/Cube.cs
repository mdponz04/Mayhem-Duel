using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public void Shrink()
    {
        transform.localScale *= 0.5f;
        Debug.Log("I'm Shrinking!!");
    }

    public void Enlarge()
    {
        transform.localScale *= 2;
        Debug.Log("I'm Getting Bigger!!");
    }
}
