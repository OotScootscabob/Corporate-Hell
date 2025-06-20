using System.Collections;
using NUnit.Framework.Internal;
using UnityEngine;

public class FlamethrowerBombScript : MonoBehaviour
{
    [Header("General")]

    [SerializeField] private float timeUntilExplode = 2f;
    [SerializeField] private float maxExplosionRadius;
    [SerializeField] private float minExplosionRadius;
    [SerializeField] private float maxExplosionForce;
    [SerializeField] private float minExplosionForce;
    [SerializeField] private float maxUpwardsModifier;
    [SerializeField] private float minUpwardsModifier;

    [Header("Effects")]
    [SerializeField] private GameObject explosion;

    [HideInInspector] public GameObject player;
    [HideInInspector] public float damage;
    [HideInInspector] public float maxDamage;

    [SerializeField] GameObject test;

    private Collider collision;

    private void Awake()
    {
        collision = GetComponent<Collider>();
        StartCoroutine(WaitForExplode());
        StartCoroutine(WaitToHitPlayer());
    }
    private void Start()
    {
        Physics.IgnoreCollision(collision, player.transform.GetChild(0).GetComponent<Collider>(), true);
    }
    public void Explode()
    {
        //explosion logic
        float explosionRadius = (minExplosionRadius + ((damage / maxDamage) * (maxExplosionRadius - minExplosionRadius)));

        Collider[] collider = Physics.OverlapSphere(transform.position, explosionRadius);
        GameObject ps = Instantiate(explosion, transform.position, Quaternion.identity);
        ps.transform.localScale = new Vector3(explosionRadius/5, explosionRadius/5, explosionRadius/5);

        //CHECK FOR EXPLOSION RADIUS
        //GameObject ungus = Instantiate(test, transform.position, Quaternion.identity);
        //ungus.transform.localScale = Vector3.one * explosionRadius * 2f;

        foreach (Collider hitCollider in collider)
        {
            float explosionForce = (minExplosionForce + ((damage / maxDamage) * (maxExplosionForce - minExplosionForce)));
            Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float upwardsModifier = (minUpwardsModifier + ((damage / maxDamage) * (maxUpwardsModifier - minUpwardsModifier)));

                if (hitCollider.transform.root.CompareTag("Player"))
                {
                    rb.AddExplosionForce(explosionForce * 5, transform.position, explosionRadius, upwardsModifier);
                }
                else
                {
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier);
                }
            }

            var damageable = hitCollider.transform.GetComponent<HealthController>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage, false, Vector3.zero, Vector3.zero, true, explosionForce);
            }
        }
        Destroy(this.gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }
    IEnumerator WaitForExplode()
    {
        yield return new WaitForSeconds(timeUntilExplode);
        Explode();
    }
    IEnumerator WaitToHitPlayer()
    {
        yield return new WaitForSeconds(0.25f);
        Physics.IgnoreCollision(collision, player.transform.GetChild(0).GetComponent<Collider>(), true);
    }
}
