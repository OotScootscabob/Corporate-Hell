using System.Collections;
using UnityEngine;

public class ProjectileStick : MonoBehaviour
{
    private new Collider collider;
    Rigidbody rb;

    private Coroutine stickCoroutine;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = true;
        collider.enabled = false;
        stickCoroutine = StartCoroutine(updatePosition(collision.transform));
    }
    public IEnumerator updatePosition(Transform targetTransform)
    {
        if (targetTransform == null) yield break;

        Vector3 localOffset = targetTransform.InverseTransformPoint(transform.position);

        while (targetTransform != null)
        {
            transform.position = targetTransform.TransformPoint(localOffset);
            yield return new WaitForSeconds(0.02f);
        }

        rb.isKinematic = false;
        rb.useGravity = true;
        collider.enabled = true;
    }
    
    public void StopSticking()
    {
        if(stickCoroutine != null)
        {
            StopCoroutine(stickCoroutine);
            stickCoroutine = null;
        }
    }
}