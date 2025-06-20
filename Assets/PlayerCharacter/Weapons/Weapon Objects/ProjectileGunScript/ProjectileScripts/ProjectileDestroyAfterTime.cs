using UnityEngine;
using UnityEngine.Events;

public class ProjectileDestroyAfterTime : MonoBehaviour
{
    [SerializeField] public float maxDestroyTime;
    public float destroyTime;
    private void Awake()
    {
        destroyTime = maxDestroyTime;
    }
    void Update()
    {
        if (destroyTime > 0)
        {
            destroyTime -= Time.deltaTime;
        }
        else if (destroyTime < 0)
        {
            OnDestroyed.Invoke();
        }
    }
    public UnityEvent OnDestroyed;
}