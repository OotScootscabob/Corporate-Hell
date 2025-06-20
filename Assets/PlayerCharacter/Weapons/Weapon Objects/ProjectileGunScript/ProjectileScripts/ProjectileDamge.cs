using System.Collections;
using UnityEngine;

public class ProjectileDamge : MonoBehaviour
{
    public float damage;

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        var damageable = other.GetComponent<HealthController>();
        if (damageable != null)
        damageable.TakeDamage(damage, true, collision.contacts[0].point, collision.contacts[0].normal, false, 0);
    }
}
