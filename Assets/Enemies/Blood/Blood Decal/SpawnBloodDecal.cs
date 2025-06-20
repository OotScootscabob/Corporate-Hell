using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class SpawnBloodDecal : MonoBehaviour
{
    [Header("Decals")]
    [SerializeField] string decalPoolKey;

    [Header("Properties")]
    [SerializeField] float minSize;
    [SerializeField] float maxSize;

    private ParticleSystem ps;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = ps.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            Vector3 pos = collisionEvents[i].intersection;
            Vector3 normal = collisionEvents[i].normal;
            Quaternion rot = Quaternion.LookRotation(normal);

            var decal = ObjectPool.DequeueObject<DecalProjector>(decalPoolKey);
            decal.transform.position = pos + normal * -0.1f;
            decal.transform.rotation = Quaternion.Inverse(rot);
            decal.gameObject.SetActive(true);

            decal.transform.Rotate(Vector3.forward, Random.Range(0f, 360f));

            DecalProjector projector = decal.GetComponent<DecalProjector>();
            float size = Random.Range(minSize, maxSize);
            projector.size = new Vector3(size, size, 0.25f);

            Material mat = new Material(projector.material);
            projector.material = mat;

            mat.SetFloat("_Size", Random.Range(0.5f, 1f));
            mat.SetFloat("_Sharpen", Random.Range(0.1f, 1f));
            mat.SetFloat("_NoiseScale", Random.Range(20f, 50f));
            mat.SetFloat("_Noise_Amp", Random.Range(0.1f, 1f));
            mat.SetFloat("_NoiseOffset", Random.Range(0f, 100f));
        }
    }
}
