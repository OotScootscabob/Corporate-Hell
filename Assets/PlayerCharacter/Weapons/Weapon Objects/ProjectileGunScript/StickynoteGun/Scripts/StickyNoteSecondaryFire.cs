using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StickyNoteSecondaryFire : MonoBehaviour
{
    Transform playerCam;

    private static List<GameObject> shotProjectiles = new List<GameObject>();

    StickyNoteRecharge recharge;

    private void Awake()
    {
        playerCam = transform.parent.parent.parent.GetChild(transform.childCount).transform;
        PlayerShoot.secondShootInput += secondaryShoot;
        recharge = transform.parent.GetComponent<StickyNoteRecharge>();
    }
    public void AddShotProjectiles(GameObject proj)
    {
        shotProjectiles.Add(proj);
        proj.GetComponent<StickyNoteReturnScript>().SetScript(this);
    }
    public void SubtractShotProjectile(GameObject proj)
    {
        shotProjectiles.Remove(proj);
    }
    public void AddStickyNoteAmmo()
    {
        GetComponent<StickyNoteGunScript>().currentAmmo = Mathf.Clamp(GetComponent<StickyNoteGunScript>().currentAmmo + 1, 0 , GetComponent<StickyNoteGunScript>().maxAmmo);
    }
    private void secondaryShoot()
    {
        if ((recharge.GetRecharge() == 0) && isActiveAndEnabled)
        {
            foreach (var shot in shotProjectiles)
            {
                shot.GetComponent<StickyNoteReturnScript>().Recall(playerCam);
                shot.GetComponent<ProjectileStick>().StopSticking();
            }
            recharge.ResetCharge();
        }
    }
}