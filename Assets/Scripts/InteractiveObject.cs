using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private GameObject box;
    
    private void Start()
    {
       
    }


    private void HandleInteracting()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            box.GetComponent<Renderer>().material.color = Color.blue;
        }
    }
}
