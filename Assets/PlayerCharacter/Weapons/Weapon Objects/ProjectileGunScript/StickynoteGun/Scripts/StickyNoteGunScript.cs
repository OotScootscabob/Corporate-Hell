using System.Collections;
using UnityEngine;

public class StickyNoteGunScript : MonoBehaviour
{
    Transform playerCam;

    float timeSinceLastShot;

    [Header("Projectile")]
    [SerializeField] GameObject bullet;

    [Header("Properties")]
    [SerializeField] private float fireRate;
    [SerializeField] public int maxAmmo;
    [SerializeField] float damage;

    [Header("Bloom")]
    [SerializeField] private float bloom;
    [SerializeField] float bloomIntensityY;
    [SerializeField] float bloomIntensityX;

    [Header("Launching")]
    [SerializeField] public float launchForce;
    [SerializeField] public float spinSpeed;

    public int currentAmmo;
    private void Awake()
    {
        playerCam = Camera.main.transform;
        PlayerShoot.shootInput += Shoot;
        timeSinceLastShot = 1f / (fireRate / 60f);
        
        currentAmmo = maxAmmo;
    }
    private bool CanShoot() => timeSinceLastShot > 1f / (fireRate / 60f);
    public void Shoot()
    {
        if (CanShoot() && this.isActiveAndEnabled)
        {
            if (currentAmmo < 1)
                return;
            
            Vector3 direction = playerCam.forward;
            Vector3 spread = Vector3.zero;
            
            spread += playerCam.up * Random.Range(-bloomIntensityY, bloomIntensityY);
            spread += playerCam.right * Random.Range(-bloomIntensityX, bloomIntensityX);
            
            direction += spread.normalized * Random.Range(0f, bloom);

            StickyNoteReturnScript currentBullet = ObjectPool.DequeueObject<StickyNoteReturnScript>("StickyNote");

            if (currentBullet != null)
            {
                currentBullet.transform.position = transform.position;
                currentBullet.transform.rotation = playerCam.rotation;
                currentBullet.gameObject.SetActive(true);

                GetComponent<StickyNoteSecondaryFire>().AddShotProjectiles(currentBullet.gameObject);

                currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * launchForce, ForceMode.Impulse);

                float spinMagnitude = Random.Range(-spinSpeed, spinSpeed);
                currentBullet.GetComponent<Rigidbody>().AddTorque(currentBullet.transform.up * spinMagnitude);

                currentBullet.GetComponent<ProjectileDamge>().damage = damage;
                currentAmmo -= 1;
                timeSinceLastShot = 0f;
            }
        }
    }
    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }
}
