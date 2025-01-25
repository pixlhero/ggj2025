using System.Collections;
using UnityEngine;
using DG.Tweening;

public class DoorInteractible : MonoBehaviour
{
    [SerializeField] private float slideDistance = 1f;
    [SerializeField] private float slideDuration = 1f;

    private bool _isDoorOpen = false;
    private Vector3 _closedPosition;

    private Coroutine _autoCloseCoroutine;

    private void Awake()
    {
        _closedPosition = transform.position;
    }

    public void ToggleDoor()
    {
        if (!_isDoorOpen)
        {
            // Slide the door to the right by slideDistance
            transform.DOMove(_closedPosition + new Vector3(0f, 0f, slideDistance), slideDuration);
            _isDoorOpen = true;

            // Start a coroutine that closes the door after 2 seconds
            if (_autoCloseCoroutine != null) StopCoroutine(_autoCloseCoroutine);
            _autoCloseCoroutine = StartCoroutine(CloseDoorAfterDelay(2f));
        }
        else
        {
            // Slide the door back to its closed position
            transform.DOMove(_closedPosition, slideDuration);
            _isDoorOpen = false;

            // If the door was manually closed, there's no need to auto-close
            if (_autoCloseCoroutine != null)
            {
                StopCoroutine(_autoCloseCoroutine);
                _autoCloseCoroutine = null;
            }
        }
    }

    private IEnumerator CloseDoorAfterDelay(float delay)
    {
        // Wait for X seconds (2 seconds in our example)
        yield return new WaitForSeconds(delay);

        // Slide the door back to its closed position
        transform.DOMove(_closedPosition, slideDuration);
        _isDoorOpen = false;

        // Nullify the coroutine reference
        _autoCloseCoroutine = null;
    }
}
