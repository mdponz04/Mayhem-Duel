using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TriggerZone>().OnEnterEvent.AddListener(InsideTrash);
    }
    public void InsideTrash(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}
