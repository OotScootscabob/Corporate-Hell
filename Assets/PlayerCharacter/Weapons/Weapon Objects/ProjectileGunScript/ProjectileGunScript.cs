using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProjectileGunScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] HitscanGunData gunData;

    GameObject player;
    float timeSinceLastShot;

    [Header("Projectile")]
    [SerializeField] GameObject bullet;

    public int currentAmmo;
    private void Awake()
    {
        player = transform.parent.parent.gameObject;
        PlayerShoot.shootInput += Shoot;
        timeSinceLastShot = 1f / (gunData.fireRate / 60f);

        if (gunData.useAmmo)
        {
            currentAmmo = gunData.maxAmmo;
        }
    }
    private bool CanShoot() => timeSinceLastShot > 1f / (gunData.fireRate / 60f);
    public void Shoot()
    {
        if (CanShoot() && this.isActiveAndEnabled)
        {
            for (int i = 0; i < gunData.numOfProjectiles; i++)
            {
                if (gunData.useAmmo && currentAmmo < 1)
                    return;

                if (!gunData.hitscan)
                {
                    Vector3 direction = player.transform.forward;
                    Vector3 spread = Vector3.zero;

                    spread += player.transform.up * Random.Range(-gunData.bloomRatioY, gunData.bloomRatioY);
                    spread += player.transform.right * Random.Range(-gunData.bloomRatioX, gunData.bloomRatioX);

                    direction += spread.normalized * Random.Range(0f, gunData.bloom);

                    GameObject currentBullet = Instantiate(bullet, transform.position, Quaternion.identity);

                    currentBullet.transform.forward = direction.normalized;

                    currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * gunData.launchForce, ForceMode.Impulse);
                    float spinDirection = Random.Range(-gunData.spinSpeed, gunData.spinSpeed);
                    currentBullet.GetComponent<Rigidbody>().AddTorque(player.transform.up * spinDirection, ForceMode.Impulse);

                    currentBullet.GetComponent<ProjectileDamge>().damage = gunData.damage;
                    
                    //ONLY USE UPWARD FORCE FOR GRANADES AND OTHER BOUNCING PROJECTILES/NOT NORMAL BULLETS
                    currentBullet.GetComponent<Rigidbody>().AddForce(player.transform.up * gunData.upwardLaunchForce, ForceMode.Impulse);
                }
                if (gunData.useAmmo)
                {
                    currentAmmo -= 1;
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
}

