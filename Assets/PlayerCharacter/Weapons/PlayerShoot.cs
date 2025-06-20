using System;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public static Action shootInput;
    public static Action secondShootInput;
    public static Action releaseShoot;
    public static Action secondReleaseShoot;

    private Boolean shot;
    private Boolean secondaryShoot;

    public static Boolean canShoot = true;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (canShoot)
            {
                shot = true;
                shootInput?.Invoke();
            }
        }

        if (Input.GetMouseButton(1))
        {
            secondaryShoot = true;
            secondShootInput?.Invoke();
        }

        if (!Input.GetMouseButton(0))
        {
            if (shot)
            {
                releaseShoot?.Invoke();
                shot = false;
            }
        }

        if (!Input.GetMouseButton(1))
        {
            if (secondaryShoot)
            {
                secondReleaseShoot?.Invoke();
                secondaryShoot = false;
            }
        }
    }
    public static void SetCanShoot(bool inCanShoot)
    {
        canShoot = inCanShoot;
    }
}
