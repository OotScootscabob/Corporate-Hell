using UnityEngine;

public class StickyNoteRecharge : MonoBehaviour
{
    public float maxRecharge = 1.25f;
    private float currentRecharge;

    private void Awake()
    {
        currentRecharge = 0;
    }
    public void ResetCharge()
    {
        currentRecharge = maxRecharge;
    }
    public float GetRecharge()
    {
        return currentRecharge;
    }
    private void Update()
    {
        if (currentRecharge != 0)
        {
            currentRecharge = Mathf.Clamp(currentRecharge - Time.deltaTime, 0, maxRecharge);
        }
    }
}
