using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class UniquePistolScript : MonoBehaviour
{
    [Header("Weapon Properties")]
    [SerializeField] private float bloom;
    [SerializeField] private float fireRate;
    [SerializeField] private float damage;
    [SerializeField] private float maxDistance;

    [Header("References")]
    Transform playerCam;

    float timeSinceLastShot;

    [Header("Trail")]

    [SerializeField] TrailRenderer bulletTrailPrefab;
    private float fakeBulletSpeed = 1000000f;
    private ObjectPool<TrailRenderer> trailPool;
    private WaitForSeconds trailDelay;

    private LayerMask shootingLayerMask;
    private bool didRelease = true;

    private void Awake()
    {
        playerCam = transform.parent.parent.parent.GetChild(transform.childCount).transform;
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
            trail =>
            {
                trail.transform.parent = null;
            }, trail =>
            {
                trail.enabled = false;
                trail.transform.parent = go.transform;
            }, DestroyTrailData);
        
        trailDelay = new WaitForSeconds(bulletTrailPrefab.time);

        PlayerShoot.shootInput += Shoot;
        PlayerShoot.releaseShoot += ReleaseShoot;
        timeSinceLastShot = 1f / (fireRate / 60f);
    }
    private bool CanShoot() => timeSinceLastShot > 1f / (fireRate / 60f) && didRelease;
    public void Shoot()
    {
        if (CanShoot() && this.isActiveAndEnabled)
        {
            //Bloom
            float x = Random.Range(-bloom, bloom);
            float y = Random.Range(-bloom, bloom);
            float z = Random.Range(-bloom, bloom);
            
            //Direction With Spread
            Vector3 direction = playerCam.forward + new Vector3(x, y, z);
            
            //Raycast and HitscanTrail
            if (Physics.Raycast(playerCam.position, direction, out RaycastHit hitInfo, maxDistance, shootingLayerMask))
            {
                var damageable = hitInfo.transform.GetComponent<HealthController>();
                if (damageable != null)
                    damageable.TakeDamage(damage, true, hitInfo.point, hitInfo.normal, false, 0);
                
                AttachTrail(transform, hitInfo, direction);
            }
            else
            {
                AttachTrail(transform, null, direction);
            }
            timeSinceLastShot = 0f;
            didRelease = false;
        }
    }
    public void ReleaseShoot()
    {
        didRelease = true;
    }
    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;
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
