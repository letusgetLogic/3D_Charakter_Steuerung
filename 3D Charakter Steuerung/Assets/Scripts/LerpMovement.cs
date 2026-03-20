using System;
using UnityEngine;

[ExecuteAlways]
public class LerpMovement : MonoBehaviour
{
    [Header("Editor Settings")]
    [SerializeField] private Transform definedTargetTransform;
    [SerializeField] private bool isRunningBackward = false;

    /// <summary>
    /// Invoked when the object to moving reached goal.
    /// </summary>
    public Action OnPosition { get; set; }

    private enum Direction
    {
        None,
        Forward,
        Backward
    }
    private Direction moveState = Direction.None;

    private float currentValue = 0f;
    private Vector3 defaultPosition;
    private Vector3 targetPosition;

    private float animTime = 1f;
    private AnimationCurve animCurve;

    private void OnEnable()
    {
        SetDefault();
    }

    private void Update()
    {
        MoveForward();
        MoveBackward();
    }

    /// <summary>
    /// Sets the default values for the movement.
    /// </summary>
    private void SetDefault()
    {
        currentValue = 0f;
        defaultPosition = transform.position;

        if (definedTargetTransform)
            targetPosition = definedTargetTransform.position;
    }

    /// <summary>
    /// Moves to the position and returns the animation time.
    /// </summary>
    /// <param name="_targetPosition"></param>
    /// <returns></returns>
    public float MoveTo(Vector3 _targetPosition, float _time, AnimationCurve _anim)
    {
        SetDefault();

        targetPosition = _targetPosition;
        moveState = Direction.Forward;
        animTime = _time;
        animCurve = _anim;

        return _time;
    }

    /// <summary>
    /// Interpolates up to 1.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void MoveForward()
    {
        if (moveState == Direction.Forward)
        {
            if (currentValue == 1f)
            {
                if (isRunningBackward)
                {
                    moveState = Direction.Backward;
                }
                else
                {
                    moveState = Direction.None;
                    OnPosition?.Invoke();
                }
                return;
            }

            Interpolate(1f);
        }
    }

    /// <summary>
    /// Interpolates down to 0.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void MoveBackward()
    {
        if (moveState == Direction.Backward)
        {
            if (currentValue == 0f)
            {
                moveState = Direction.None;
                OnPosition?.Invoke();
                return;
            }

            Interpolate(0f);
        }
    }

    /// <summary>
    /// Interpolates the value and sets the position.
    /// </summary>
    /// <param name="_target"></param>
    private void Interpolate(float _target)
    {
        currentValue = Mathf.MoveTowards(
            currentValue, _target, Time.deltaTime / animTime);

        Vector3 position = Vector3.Lerp(defaultPosition, targetPosition, animCurve.Evaluate(currentValue));
        SetPosition(position);
    }

    /// <summary>
    /// Sets the position.
    /// </summary>
    /// <param name="rValue"></param>
    private void SetPosition(Vector3 _pos)
    {
        transform.position = _pos;
    }
}