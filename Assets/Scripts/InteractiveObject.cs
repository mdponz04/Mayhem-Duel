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
    private void Update()
    {
        
    }

    private void HandleInteracting()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        box.GetComponent<Renderer>().material.color = Color.blue;
    }
}
