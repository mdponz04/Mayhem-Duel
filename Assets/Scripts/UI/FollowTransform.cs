using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] private Transform lookAt;
    [SerializeField] private Transform transformToFollow;

    public float followSpeed = 3f;

    private Transform _thisTransfrom;
    private void Start()
    {
        // Cache this lookup
        _thisTransfrom = transform;
    }

    private void Update()
    {
        //_thisTransfrom.LookAt(lookAt, Vector3.up);

        var newPosition = _thisTransfrom.position;
        var followPosition = transformToFollow.position;
        newPosition.x = Mathf.Lerp(newPosition.x, followPosition.x, followSpeed * Time.deltaTime);
        //newPosition.y = Mathf.Lerp(newPosition.y, followPosition.y, followSpeed * Time.deltaTime);
        newPosition.z = Mathf.Lerp(newPosition.z, (followPosition.z - 0.5f), followSpeed * Time.deltaTime);

        transform.position = newPosition;
    }

}
