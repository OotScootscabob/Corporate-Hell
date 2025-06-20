using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShotgunSecondaryFire : MonoBehaviour
{
    GameObject player;

    [Header("References")]
    [SerializeField] HitscanGunData gunData;

    [Header("Projectile")]
    [SerializeField] GameObject bullet;

    void Awake()
    {
        player = transform.parent.parent.gameObject;
        PlayerShoot.secondShootInput += secondaryShoot;
        PlayerShoot.secondReleaseShoot += secondaryReleaseShoot;
    }
    public void secondaryShoot()
    {
        if (!isActiveAndEnabled)
            return;

        if (ShotgunSecondaryFireRecharge.GetRechargeTime() != 0)
            return;

        PlayerShoot.SetCanShoot(false);
        ShotgunSecondaryFireRecharge.StartCharge();

        if (ShotgunSecondaryFireRecharge.GetCurrentCharge() == ShotgunSecondaryFireRecharge.GetMinSpread())
        {
            ShotgunSecondaryFireRecharge.StartExtraCharge();

            if (ShotgunSecondaryFireRecharge.GetCurrentExtraCharge() == 0)
            {
                secondaryReleaseShoot();
            }
        }
    }
    public void secondaryReleaseShoot()
    {
        if (!isActiveAndEnabled)
            return;

        if (ShotgunSecondaryFireRecharge.GetRechargeTime() != 0)
            return;

        for (int i = 0; i < (gunData.numOfProjectiles * 2); i++)
        {
            if (!gunData.hitscan)
            {
                float newBloom = Mathf.Clamp(gunData.bloom * (ShotgunSecondaryFireRecharge.GetCurrentCharge() / ShotgunSecondaryFireRecharge.GetMaxCharge()), 0.03f, gunData.bloom);
                Vector3 direction = player.transform.forward;
                Vector3 spread = Vector3.zero;
                spread += player.transform.up * Random.Range(-1f, 1f);
                spread += player.transform.right * Random.Range(-1f, 1f);

                direction += spread.normalized * Random.Range(0f, newBloom);

                GameObject currentBullet = Instantiate(bullet, transform.position, Quaternion.identity);

                currentBullet.transform.forward = direction.normalized;

                currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * gunData.launchForce, ForceMode.Impulse);
                currentBullet.GetComponent<ProjectileDamge>().damage = gunData.damage;

                //ONLY USE UPWARD FORCE FOR GRANADES AND OTHER BOUNCING PROJECTILES/NOT NORMAL BULLETS
                currentBullet.GetComponent<Rigidbody>().AddForce(player.transform.up * gunData.upwardLaunchForce, ForceMode.Impulse);
            }
        }
        ShotgunSecondaryFireRecharge.ResetCharge();
        ShotgunSecondaryFireRecharge.ResetRecharge();
        PlayerShoot.SetCanShoot(true);
    }
}
