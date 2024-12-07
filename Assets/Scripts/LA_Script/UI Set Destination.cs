using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISetDestination : MonoBehaviour
{
    public SelectTPDestination selectDestination;

    public void SetDestinationFromIndex(int index)
    {
        selectDestination.SetTpDestination(index);
    }
}
