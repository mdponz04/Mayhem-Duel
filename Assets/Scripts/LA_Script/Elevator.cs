using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.DualShock.LowLevel;

public class Elevator : MonoBehaviour
{
    private bool _isAtBottom = true;
    private bool _isAnimating = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        StartCoroutine(ElevatorMove());
    }

    public IEnumerator ElevatorMove()
    {
        if (_isAnimating) { yield return null; }
        _isAnimating = true;
        Vector3 currentPos = transform.position;
        Vector3 destination;
        if (_isAtBottom) {
            destination = currentPos + Vector3.up;
        }
        else
        {
            destination = currentPos - Vector3.up;
        }

        yield return Move(currentPos, destination);
        _isAtBottom = !_isAtBottom;

        _isAnimating = false;
    }

    private IEnumerator Move(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        float duration = 0.125f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            transform.localPosition = Vector3.Lerp(from, to, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = to;
    }
}
