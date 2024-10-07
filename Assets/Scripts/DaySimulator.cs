using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaySimulator : MonoBehaviour
{
    public float rotateSpeed = 0.1f;
    // Update is called once per frame
    void Update()
    {
        // Rotate on X axis only
        transform.Rotate(rotateSpeed, 0, 0);
    }
}
