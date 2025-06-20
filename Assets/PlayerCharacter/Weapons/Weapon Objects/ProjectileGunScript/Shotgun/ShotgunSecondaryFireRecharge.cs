using Unity.VisualScripting;
using UnityEngine;

public class ShotgunSecondaryFireRecharge : MonoBehaviour
{
    private static float currentCharge;
    private static float maxCharge = 2f;
    private static float currentExtraCharge;
    private static float maxExtraCharge = 1f;
    private static float minSpread = 0f;

    private static float maxRechargeTime = 5f;
    private static float rechargeTime;

    private void Awake()
    {
        currentCharge = maxCharge;
        currentExtraCharge = maxExtraCharge;
        rechargeTime = 0;
    }
    public static void StartCharge()
    {
        currentCharge = Mathf.Clamp(currentCharge - Time.deltaTime, minSpread, maxCharge);
    }
    public static void StartExtraCharge()
    {
        currentExtraCharge = Mathf.Clamp(currentCharge - Time.deltaTime, 0, maxExtraCharge);
    }
    public static void ResetCharge()
    {
        currentCharge = maxCharge;
        currentExtraCharge = maxExtraCharge;
    }
    public static float GetCurrentCharge()
    {
        return currentCharge;
    }
    public static float GetCurrentExtraCharge()
    {
        return currentExtraCharge;
    }
    public static float GetMinSpread()
    {
        return minSpread;
    }
    public static float GetMaxCharge()
    {
        return maxCharge;
    }
    public static float GetRechargeTime()
    {
        return rechargeTime;
    }
    public static void Recharge()
    {
        rechargeTime = Mathf.Clamp(rechargeTime - Time.deltaTime, 0, maxRechargeTime);
    }
    public static void ResetRecharge()
    {
        rechargeTime = maxRechargeTime;
    }
    private void FixedUpdate()
    {
        if (rechargeTime != 0)
        {
            Recharge();
        }
    }
}
