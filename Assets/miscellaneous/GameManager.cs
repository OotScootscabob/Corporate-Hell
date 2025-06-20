using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public StickyNoteReturnScript bulletPrefab;

    public DestroyParticles BloodSprayBurstAroundFX;
    public DestroyParticles BloodSprayBurstFX;
    public DestroyParticles BloodSprayDeathFX;
    public DestroyParticles BloodSprayExplosionFX;

    public DecalProjector bloodDecal;

    public Rigidbody chunk1;
    public Rigidbody chunk2;
    public Rigidbody chunk3;

    private void Awake()
    {
        SetUpPool();
    }

    private void SetUpPool()
    {
        ObjectPool.SetUpPool(bulletPrefab, 100, "StickyNote");
        ObjectPool.SetUpPool(BloodSprayBurstAroundFX, 500, "BloodSprayBurstAroundFX");
        ObjectPool.SetUpPool(BloodSprayBurstFX, 500, "BloodSprayBurstFX");
        ObjectPool.SetUpPool(BloodSprayDeathFX, 100, "BloodSprayDeathFX");
        ObjectPool.SetUpPool(BloodSprayExplosionFX, 100, "BloodSprayExplosionFX");
        ObjectPool.SetUpPool(bloodDecal, 1000, "BloodDecal");
        ObjectPool.SetUpPool(chunk1, 200, "BloodChunk1");
        ObjectPool.SetUpPool(chunk2, 200, "BloodChunk2");
        ObjectPool.SetUpPool(chunk3, 200, "BloodChunk3");
    }
}
