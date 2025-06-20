using UnityEngine;

public class FlamethrowerSecondaryFire : MonoBehaviour
{
    Transform playerCam;

    FlamethrowerHeatManager script;

    [Header("Projectile")]
    [SerializeField] GameObject projectile;
    [SerializeField] private float launchForce;
    [SerializeField] private float upwardLaunchForce;

    [Header("Damage Properties")]
    [SerializeField] private float minFireDamage;
    [SerializeField] private float maxFireDamage;

    private float startHeat, endHeat;
    private bool charging = false;

    void Awake()
    {
        script = transform.parent.GetComponent<FlamethrowerHeatManager>();

        playerCam = transform.parent.parent.parent.GetChild(transform.childCount).transform;
        PlayerShoot.secondShootInput += secondaryShoot;
        PlayerShoot.secondReleaseShoot += secondaryReleaseShoot;
    }
    private bool DontStart() => (!this.isActiveAndEnabled) || (script.GetRecharge() != 0);
    public void secondaryShoot()
    {
        if (DontStart()) return;

        if (charging == false)
            startHeat = script.GetCurrentHeat();

        charging = true;

        PlayerShoot.SetCanShoot(false);

        script.DecreaseHeatWithMultiplier(3); 
    }
    public void secondaryReleaseShoot()
    {
        if (DontStart()) return;

        charging = false;

        endHeat = script.GetCurrentHeat();

        float finalDamage = Mathf.Clamp(minFireDamage + (((startHeat - endHeat) / script.GetMaxTimeToCool()) * (maxFireDamage - minFireDamage)), minFireDamage, maxFireDamage);
        Debug.Log(finalDamage);

        Vector3 direction = playerCam.forward;

        GameObject currentBullet = Instantiate(projectile, transform.position, Quaternion.identity);

        FlamethrowerBombScript bulletScript = currentBullet.GetComponent<FlamethrowerBombScript>();
        if (bulletScript != null)
        {
            bulletScript.damage = finalDamage;
            bulletScript.maxDamage = maxFireDamage;
            bulletScript.player = transform.parent.parent.parent.parent.gameObject;
        }

        currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * launchForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(playerCam.up * upwardLaunchForce, ForceMode.Impulse);

        script.ResetRecharge();
        PlayerShoot.SetCanShoot(true);

        startHeat = 0;
        endHeat = 0;
    }
}
