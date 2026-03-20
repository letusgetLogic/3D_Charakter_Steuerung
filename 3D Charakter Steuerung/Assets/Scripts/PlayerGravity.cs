using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerGravity : MonoBehaviour
{
    private Rigidbody rb;

    public MoonGravity Moon;

    private bool isSettingMoonGravity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Disables velocity + use of gravity on rigidbody.
    /// </summary>
    public void SetMoonGravity()
    {
        if (isSettingMoonGravity == false)
        {
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
            isSettingMoonGravity = true;
        }
    }

    /// <summary>
    /// Enables use of gravity on rigidbody.
    /// </summary>
    public void SetEarthGravity()
    {
        if (isSettingMoonGravity)
        {
            rb.useGravity = true;
            isSettingMoonGravity = false;
        }
    }
}
