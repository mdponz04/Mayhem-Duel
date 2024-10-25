using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] private List<GameObject> breakablePieces;
    private float timer;
    private float timeToBreak = 2f;


    private void Start()
    {
        foreach (var piece in breakablePieces)
        {
            piece.SetActive(false);
        }
    }

    private void Break()
    {
        timer += Time.deltaTime;

        if(timer > timeToBreak)
        {
            foreach (var piece in breakablePieces)
            {
                piece.SetActive(true);
                piece.transform.parent = null;
            }

            gameObject.SetActive(false);
        }
    }
}
