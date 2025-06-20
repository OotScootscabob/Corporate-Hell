using System.Collections;
using UnityEngine;

public class DestroyParticles : MonoBehaviour
{
    [SerializeField] float destroyTime;
    private ParticleSystem[] particleSystems;

    [SerializeField] string poolKey;

    private void Awake()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }
    private void OnEnable()
    {
        foreach (var ps in particleSystems)
        {
            ps.Clear(true);
            ps.Play();
        }

        StartCoroutine(DestroyTimer(destroyTime));
    }
    private IEnumerator DestroyTimer(float time)
    {
        yield return new WaitForSeconds(time);
        ObjectPool.EnqueueObject(this, poolKey);
    }
}
