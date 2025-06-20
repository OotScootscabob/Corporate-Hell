using System.Collections;
using UnityEngine;

public class GrappleRecharge : MonoBehaviour
{
    GrapplingHookSecondaryFire script;
    private float rechargeTime;

    private void Awake()
    {
        script = transform.GetChild(0).GetComponent<GrapplingHookSecondaryFire>();
    }
    private void Update()
    {
        if (script.grapplingCoolDown > 0)
        {
            script.grapplingCoolDown -= Time.deltaTime;
        }
    }
}
