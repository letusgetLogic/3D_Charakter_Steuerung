using System.Collections;
using UnityEngine;


[RequireComponent(typeof(LerpMovement))]
public class Spaceship : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float delayAnimUp = 2f;
    [SerializeField] private float delayAnimDown = 3f;
    [SerializeField] private float animTime = 10f;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private AnimationCurve animCurveUp;
    [SerializeField] private AnimationCurve animCurveDown;

    private PlayerController playerController;

    private Vector3 defaultPosition;
    private LerpMovement lerp;

    private Coroutine coroutineUp;
    private Coroutine coroutineDown;


    private void Awake()
    {
        defaultPosition = transform.position;
        lerp = GetComponent<LerpMovement>();
    }

    private void OnEnable()
    {
        if (lerp)
        {
            lerp.OnPosition += EnablePlayerRigidbody;
        }
    }

    private void OnDisable()
    {
        if (lerp)
        {
            lerp.OnPosition -= EnablePlayerRigidbody;
        }
    }

    private void OnTriggerEnter(Collider _collider)
    {
        if (_collider.CompareTag("Player"))
        {
            playerController = _collider.GetComponent<PlayerController>();
            playerController.IsOnSpaceship = true;

            if (coroutineDown != null)
            {
                StopCoroutine(coroutineDown);
                coroutineDown = null;
            }

            coroutineUp = StartCoroutine(FlyToTheMoon(_collider.transform));
        }
    }

    private void OnTriggerExit(Collider _collider)
    {
            if (_collider.CompareTag("Player"))
        {
            playerController = _collider.GetComponent<PlayerController>();
            playerController.IsOnSpaceship = false;

            if (coroutineUp != null)
            {
                StopCoroutine(coroutineUp);
                coroutineUp = null;
            }

            coroutineDown = StartCoroutine(BackToHome(_collider.transform));
        }
    }

    private IEnumerator FlyToTheMoon(Transform _player)
    {
        yield return new WaitForSeconds(delayAnimUp);

        _player.SetParent(transform, true);

        Vector3 targetPosition = targetTransform.position;

        if (lerp)
        {
            lerp.MoveTo(targetPosition, animTime, animCurveUp);
        }
    }

    private IEnumerator BackToHome(Transform _player)
    {
        yield return new WaitForSeconds(delayAnimDown);



        if (lerp)
        {
            lerp.MoveTo(defaultPosition, animTime, animCurveDown);
        }
    }

    private void EnablePlayerRigidbody()
    {
        if (playerController != null)
            playerController.transform.SetParent(null, true);
    }

}
