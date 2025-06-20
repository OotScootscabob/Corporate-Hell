using System;
using UnityEngine;

public class FlamethrowerHeatManager : MonoBehaviour
{
    public float rateToHeat = 0.4f;

    public float maxTimeToCool = 5f;
    private float currentHeat;

    public float maxSecondaryRecharge;
    private float secondaryRecharge;

    private void Awake()
    {
        currentHeat = maxTimeToCool;
        secondaryRecharge = 0;
    }
    public void IncreaseHeat()
    {
        currentHeat = Mathf.Clamp(currentHeat + (Time.deltaTime * rateToHeat), 0, maxTimeToCool);
    }
    public void DecreaseHeat()
    {
        currentHeat = Mathf.Clamp(currentHeat - Time.deltaTime, 0, maxTimeToCool);
    }
    public void DecreaseHeatWithMultiplier(float amount)
    {
        currentHeat = Mathf.Clamp(currentHeat - Time.deltaTime * amount, 0, maxTimeToCool);
    }
    public float GetCurrentHeat()
    {
        return currentHeat;
    }
    public float GetMaxTimeToCool()
    {
        return maxTimeToCool;
    }
    public void SetCurrentHeat(float inHeat)
    {
        currentHeat = inHeat;
    }

    //SecondaryFire
    public float GetRecharge()
    {
        return secondaryRecharge;
    }
    public void ResetRecharge()
    {
        secondaryRecharge = maxSecondaryRecharge;
    }
    private void FixedUpdate()
    {
        if (transform.GetChild(0).GetComponent<FlamethrowerScript>().GetFlamethrowerParticleSystem().enabled == false)
        {
            IncreaseHeat();
        }

        if (secondaryRecharge != 0)
        {
            secondaryRecharge = Mathf.Clamp(secondaryRecharge - Time.deltaTime, 0f, maxSecondaryRecharge);
        }
    }
}
