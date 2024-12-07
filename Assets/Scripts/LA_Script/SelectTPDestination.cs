
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SelectTPDestination : MonoBehaviour
{
    public TeleportationAnchor[] teleportDestinations;
    private TeleportationAnchor _currentTpAnchor;
    public void Start()
    {
        if (teleportDestinations.Length >= 1)
        {
            _currentTpAnchor = teleportDestinations[0];
        }
    }

    public void SetTpDestination(int index)
    {
        _currentTpAnchor = teleportDestinations[index];
    }

    public void RequestTP()
    {
        _currentTpAnchor.RequestTeleport();
    }

}
