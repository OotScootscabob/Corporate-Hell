using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float currentHealth;

    [SerializeField] string[] bloodBurstPoolKeys;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }
    public float RemainingHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
    public UnityEvent OnDied = new UnityEvent();
    public UnityEvent<float> OnExplode = new UnityEvent<float>();

    public void TakeDamage(float damageAmount, bool spawnBlood, Vector3 hitPoint, Vector3 normal, bool canExplode, float explodeAmount)
    {
        if (isDead) return;

        if (spawnBlood && bloodBurstPoolKeys != null)
        {
            foreach (var name in bloodBurstPoolKeys)
            {
                var bloodFX = ObjectPool.DequeueObject<DestroyParticles>(name);
                bloodFX.transform.position = hitPoint;
                bloodFX.transform.rotation = Quaternion.LookRotation(normal);
                bloodFX.gameObject.SetActive(true);
            }
        }

        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, maxHealth);
        if (currentHealth <= 0)
        {
            isDead = true;

            if (canExplode)
            {
                OnExplode.Invoke(explodeAmount);
            }
            else
            {
                OnDied.Invoke();
            }
        }
    }
    public void AddHealth(float amountToAdd)
    {
        currentHealth = Mathf.Clamp(currentHealth + amountToAdd, 0, maxHealth);
    }
}