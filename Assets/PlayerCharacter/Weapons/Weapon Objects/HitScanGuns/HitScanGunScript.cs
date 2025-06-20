using System;
using System.Collections;
using NUnit;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class HitScanGunScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] HitscanGunData gunData;

    Transform playerCam;
    float timeSinceLastShot;

    [Header("HitScan")]

    [SerializeField] TrailRenderer bulletTrailPrefab;
    private float fakeBulletSpeed = 1000000f;
    private ObjectPool<TrailRenderer> trailPool;
    private WaitForSeconds trailDelay;

    LayerMask shootingLayerMask;

    private void Awake()
    {
        if (gunData.hitscan)
        {
            shootingLayerMask = ~(1 << LayerMask.NameToLayer("Player"));
            var go = new GameObject("BulletTrails")
            {
                transform =
            {
                position = Vector3.zero
            }
            };
            trailPool = new ObjectPool<TrailRenderer>(
                    () => Instantiate(bulletTrailPrefab, go.transform),
                    trail => {
                        trail.transform.parent = null;
                    }, trail => {
                        trail.enabled = false;
                        trail.transform.parent = go.transform;
                    }, DestroyTrailData);

            trailDelay = new WaitForSeconds(bulletTrailPrefab.time);
        }

        playerCam = transform.parent.parent.parent.GetChild(transform.childCount).transform;
        PlayerShoot.shootInput += Shoot;
        timeSinceLastShot = 1f / (gunData.fireRate / 60f);
    }
    private bool CanShoot() => timeSinceLastShot > 1f / (gunData.fireRate / 60f);
    public void Shoot()
    {
        if (CanShoot() && this.isActiveAndEnabled)
        {
            for (int i = 0; i < gunData.numOfProjectiles; i++)
            {
                //Bloom
                float x = Random.Range(-gunData.bloom, gunData.bloom);
                float y = Random.Range(-gunData.bloom, gunData.bloom);
                float z = Random.Range(-gunData.bloom, gunData.bloom);

                //Direction With Spread
                Vector3 direction = playerCam.forward + new Vector3(x, y, z);

                if (gunData.hitscan)
                {
                    //Raycast and HitscanTrail
                    if (Physics.Raycast(playerCam.position, direction, out RaycastHit hitInfo, gunData.maxDistance, shootingLayerMask))
                    {
                        var damageable = hitInfo.transform.GetComponent<HealthController>();
                        if (damageable != null)
                        damageable.TakeDamage(gunData.damage, true, hitInfo.point, hitInfo.normal, false, 0);

                        AttachTrail(transform, hitInfo, direction);
                    }
                    else
                    {
                        AttachTrail(transform, null, direction);
                    }
                }
            }
            timeSinceLastShot = 0f;
            OnGunShot();
        }
    }
    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }

    private void OnGunShot()
    {

    }
    private static void DestroyTrailData(TrailRenderer trail)
    {
        Destroy(trail.gameObject);
    }
    //Trail
    public void AttachTrail(Transform spawnLocation, RaycastHit? hit, Vector3 direction)
    {
        var trail = trailPool.Get();
        trail.transform.position = spawnLocation.position;
        trail.transform.rotation = Quaternion.identity;
        trail.Clear();
        trail.enabled = true;

        if (hit.HasValue)
        {
            StartCoroutine(SpawnTrail(trail, hit.Value.point));
 
        }
        else
        {
            StartCoroutine(SpawnTrail(trail, spawnLocation.position + direction * 100));
        }
    }
    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint)
    {
        var startPosition = trail.transform.position;
        var distance = Vector3.Distance(trail.transform.position, hitPoint);
        var remainingDistance = distance;
        while (remainingDistance > 0)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hitPoint, 1 - (remainingDistance / distance));
            remainingDistance -= fakeBulletSpeed * Time.deltaTime;
            yield return null;
        }
        trail.transform.position = hitPoint;
        yield return trailDelay;
        trailPool.Release(trail);
    } 
}
