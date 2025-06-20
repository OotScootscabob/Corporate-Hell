using UnityEngine;

public class DeathScript : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] string bloodExplosionPoolKey;
    [SerializeField] private bool alwaysExplode;
    [SerializeField] private bool neverExplode;

    [Header("Chunks")]
    [SerializeField] string[] chunksPoolKeys;
    [SerializeField] int maxNumOfChunks;
    [SerializeField] int minNumOfChunks;
    [SerializeField] float maxChunkSize;
    [SerializeField] float minChunkSize;
    [SerializeField] int defaultScatterForce;

    [Header("Death")]
    [SerializeField] string deathExplosionPoolKey;
    [SerializeField] int maxNumOfBloodSplatter;
    [SerializeField] int minNumOfBloodSplatter;
    public void Explode(float explodeAmount)
    {
        if (neverExplode)
        {
            Death();
            return;
        }

        Collider collider = GetComponent<Collider>();
        collider.enabled = false;

        var effect = ObjectPool.DequeueObject<DestroyParticles>(bloodExplosionPoolKey);
        effect.transform.position = transform.position;
        effect.transform.rotation = Random.rotation;
        effect.gameObject.SetActive(true);

        SetNumOfBloodsplatter(effect.gameObject, maxNumOfBloodSplatter, minNumOfBloodSplatter);

        float scatterForce = explodeAmount/15;

        for (int i = 0; i < Random.Range(minNumOfChunks, maxNumOfChunks + 1); i++)
        {
            var chunk = ObjectPool.DequeueObject<Rigidbody>(chunksPoolKeys[Random.Range(0, chunksPoolKeys.Length)]);
            chunk.transform.position = transform.position;
            chunk.transform.rotation = Random.rotation;
            chunk.gameObject.SetActive(true);

            float chunkSize = Random.Range(minChunkSize, maxChunkSize);
            chunk.transform.localScale = new Vector3(chunkSize, chunkSize, chunkSize);

            Rigidbody rb = chunk.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomForce = ConeDirection(75f) * scatterForce;
                rb.AddForce(randomForce, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * scatterForce, ForceMode.Impulse);
            }
        }

        JibAll();
    }
    Vector3 ConeDirection(float coneAngle)
    {
        float angleInRad = coneAngle * Mathf.Deg2Rad;

        float z = Mathf.Cos(angleInRad * Random.value);
        float theta = Random.Range(0f, Mathf.PI * 2);
        float x = Mathf.Sqrt(1 - z * z) * Mathf.Cos(theta);
        float y = Mathf.Sqrt(1 - z * z) * Mathf.Sin(theta);

        Vector3 localDirection = new Vector3(x, z, y);
        return localDirection.normalized;
    }

    public void Death()
    {
        if (alwaysExplode && !neverExplode)
        {
            Explode(defaultScatterForce);
            return;
        }

        var effect = ObjectPool.DequeueObject<DestroyParticles>(deathExplosionPoolKey);
        effect.transform.position = transform.position;
        effect.transform.rotation = Random.rotation;
        effect.gameObject.SetActive(true);

        SetNumOfBloodsplatter(effect.gameObject, maxNumOfBloodSplatter, minNumOfBloodSplatter);
        
        Ragdoll();
    }
    private void SetNumOfBloodsplatter(GameObject pEffect, int maxNum, int minNum)
    {
        var bloodSystem = pEffect.GetComponentInChildren<SpawnBloodDecal>();
        int burstCount = Random.Range(minNum, maxNum + 1);

        if (bloodSystem != null)
        {
            var pSystem = bloodSystem.GetComponentInParent<ParticleSystem>().emission;
            
            if (pSystem.burstCount > 0)
            {
                var burst = pSystem.GetBurst(0);
                burst.count = new ParticleSystem.MinMaxCurve(burstCount);
                pSystem.SetBurst(0, burst);
            }
        }

    }
    private void Ragdoll()
    {
        Destroy(gameObject);
        //code
    }
    private void JibAll()
    {
        Destroy(gameObject);
        //code
    }
}