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

    public void SetMoonGravity()
    {
        if (isSettingMoonGravity == false)
        {
            //rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
            isSettingMoonGravity = true;
        }
    }

    public void SetEarthGravity()
    {
        if (isSettingMoonGravity)
        {
            rb.useGravity = true;
            isSettingMoonGravity = false;
        }
    }
}
