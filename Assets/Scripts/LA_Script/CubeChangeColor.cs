using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeChangeColor : MonoBehaviour
{
    [SerializeField] public MeshRenderer meshRender;
    [SerializeField] public Material[] mats;
    private int _currentMat = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeColor()
    {
        _currentMat++;
        if(_currentMat >= mats.Length)
        {
            _currentMat = 0;
        }
        meshRender.material = mats[ _currentMat ];
    }
}
