using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(LerpMovement))]
public class Spaceship : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;

    [Header("Settings")]
    [SerializeField] private float delayAnimUp = 2f;
    [SerializeField] private float delayAnimDown = 3f;
    [SerializeField] private float animTime = 10f;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private AnimationCurve animCurveUp;
    [SerializeField] private AnimationCurve animCurveDown;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (coroutineDown != null)
            {
                coroutineDown = null;
            }

            coroutineUp = StartCoroutine(FlyToTheMoon(other.transform));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (coroutineUp != null)
            {
                coroutineUp = null;
            }

            coroutineDown = StartCoroutine(BackToHome(other.transform));
        }
    }

    private IEnumerator FlyToTheMoon(Transform _player)
    {
        yield return new WaitForSeconds(delayAnimUp);

        _player.SetParent(transform);

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
        playerController.transform.SetParent(null);
    }

}
