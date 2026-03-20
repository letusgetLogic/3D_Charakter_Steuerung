using System.Linq;
using UnityEngine;

public class MoonGravity : MonoBehaviour
{
    [SerializeField]
    private float gravityRadius = 10f;
    [SerializeField]
    private float gravityStrength = -10.0f;
   
    /// <summary>
    /// Add force to rigidbody, rotate body upright against the gravity direction.
    /// </summary>
    /// <param name="_body"></param>
    public void Attract(Transform _body)
    {
        // First we define the up directions of gravity and body.
        Vector3 gravityUp = (_body.position - transform.position).normalized;
        Vector3 bodyUp = _body.up;

        // Then add force to the body with the gravity. The gravity strength defines also the direction to gravityUp.
        _body.GetComponent<Rigidbody>().AddForce(gravityUp * gravityStrength);

        // We calculate the value to rotate the body to the target,
        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * _body.rotation;

        // and smooth lerp the body rotation to the targeted rotation.
        _body.rotation = Quaternion.Slerp(_body.rotation, targetRotation, Time.fixedDeltaTime * 50);
    }

    /// <summary>
    /// Checks overlap of colliders in the gravity sphere.
    /// </summary>
    /// <returns></returns>
    public bool IsInGravitySphere(Collider _searcher, LayerMask _layer)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, gravityRadius, 1 << _layer);
        return colliders.Contains(_searcher);
    }

    private void OnDrawGizmos()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, gravityRadius);

        Gizmos.color = colliders.Length > 0 ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, gravityRadius);
    }
}
