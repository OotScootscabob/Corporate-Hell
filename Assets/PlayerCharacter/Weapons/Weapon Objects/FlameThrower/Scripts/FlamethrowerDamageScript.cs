using UnityEngine;

public class FlamethrowerDamageScript : MonoBehaviour
{
    FlamethrowerHeatManager script;

    public float minFireDamage;
    public float maxFireDamage;
    private void Awake()
    {
        script = GetComponent<FlamethrowerHeatManager>();
    }
    private void OnParticleCollision(GameObject other)
    {
        float finalDamage = Mathf.Clamp(minFireDamage + ((script.GetCurrentHeat() / script.GetMaxTimeToCool()) * (maxFireDamage - minFireDamage)), minFireDamage, maxFireDamage);
        var damageable = other.transform.GetComponent<HealthController>();
        if (damageable != null)
            damageable.TakeDamage(finalDamage, false, Vector3.zero, Vector3.zero, false, 0);
    }
}
