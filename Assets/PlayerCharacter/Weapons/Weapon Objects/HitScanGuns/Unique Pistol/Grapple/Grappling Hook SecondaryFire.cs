using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GrapplingHookSecondaryFire : MonoBehaviour
{
    Transform playerCam;

    [Header("Grappling")]
    [SerializeField] private float maxGrappleDistance;
    [SerializeField] private float grappleDelayTime;
    [SerializeField] private float grappleForce;
    [SerializeField] private float upwardGrappleForce;

    [Header("Cooldown")]
    [SerializeField] public float maxGrapplingCoolDown;
    [HideInInspector] public float grapplingCoolDown;

    private Vector3 grapplePoint;
    private LineRenderer lr;
    private Rigidbody rb;
    private LayerMask shootingLayerMask;
    private bool isReturning;
    private bool pulling;
    private bool grappleOut;

    [HideInInspector] public Transform gunTip;

    private Coroutine updatePositionCoroutine;

    private void Awake()
    {
        grapplingCoolDown = 0;

        isReturning = false;
        pulling = false;

        gunTip = transform.GetChild(0).transform;
        rb = transform.parent.parent.parent.parent.GetChild(0).GetComponent<Rigidbody>();
        playerCam = transform.parent.parent.parent.GetChild(transform.childCount).transform;
        shootingLayerMask = ~(1 << LayerMask.NameToLayer("Player"));
        lr = GetComponent<LineRenderer>();
        PlayerShoot.secondShootInput += StartGrapple;
        PlayerShoot.secondReleaseShoot += EndGrapple;
    }
    private void StartGrapple()
    {
        if (grapplingCoolDown > 0) return;

        if (!this.isActiveAndEnabled) return;

        if (grappleOut) return;

        grapplingCoolDown = maxGrapplingCoolDown;
        grappleOut = true;
        lr.enabled = true;

        RaycastHit hit;
        if (Physics.Raycast(playerCam.position, playerCam.forward, out hit, maxGrappleDistance, shootingLayerMask))
        {
            grapplePoint = hit.point;
            updatePositionCoroutine = StartCoroutine(updatePosition(hit.transform.gameObject));
        }
        else
        {
            grapplePoint = playerCam.position + playerCam.forward * maxGrappleDistance;
            Invoke(nameof(EndGrapple), grappleDelayTime);
        }
    }
    public IEnumerator ExecuteGrapple()
    {
        pulling = true;
        yield return new WaitForSeconds(grappleDelayTime);

        while (pulling)
        {
            rb.AddForce((grapplePoint - playerCam.transform.position).normalized * grappleForce, ForceMode.Impulse);
            rb.AddForce(playerCam.transform.up.normalized * upwardGrappleForce, ForceMode.Impulse);
            yield return new WaitForSeconds(0.02f);
        }
    }
    private IEnumerator updatePosition(GameObject collided)
    {
        if (collided == null) yield break;

        Transform targetTransform = collided.transform;
        Vector3 localOffset = targetTransform.InverseTransformPoint(grapplePoint);

        while (!isReturning)
        {
            grapplePoint = targetTransform.TransformPoint(localOffset);
            yield return new WaitForSeconds(0.02f);
        }
    }
    private void EndGrapple()
    {
        if (updatePositionCoroutine != null)
        {
            StopCoroutine(updatePositionCoroutine);
            updatePositionCoroutine = null;
        }

        pulling = false;
        isReturning = true;
        grappleOut = true;
    }
    private void OnDisable()
    {
        EndGrapple();
        grappleOut = false;
    }
    public bool IsGrappling()
    {
        return grappleOut;
    }
    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
    public bool IsReturning()
    {
        return isReturning;
    }
    public bool IsPulling()
    {
        return pulling;
    }
    public void Reset()
    {
        if (updatePositionCoroutine != null)
        {
            StopCoroutine(updatePositionCoroutine);
            updatePositionCoroutine = null;
        }

        grappleOut = false;
        isReturning = false;
        lr.enabled = false;
        grapplePoint = Vector3.zero;
    }
}
