using UnityEngine;

public class FlamethrowerScript : MonoBehaviour
{
    FlamethrowerHeatManager script;
    Transform playerCam;

    ParticleSystem fire;
    ParticleSystem.EmissionModule em;
    ParticleSystem.ColorOverLifetimeModule parColor;
    Gradient gradient;

    GradientColorKey[] colorKeys;
    GradientAlphaKey[] alphaKeys;
    private void Awake()
    {
        playerCam = transform.parent.parent.parent.GetChild(transform.childCount).transform;

        script = transform.parent.GetComponent<FlamethrowerHeatManager>();

        fire = transform.parent.GetComponent<ParticleSystem>();

        em = fire.emission;
        em.enabled = false;
        parColor = fire.colorOverLifetime;

        gradient = new Gradient();
        colorKeys = new GradientColorKey[3];
        //blue
        colorKeys[0] = new GradientColorKey(new Color(0.239f, 0.533f, 1), 0.2f);
        //orange
        colorKeys[1] = new GradientColorKey(new Color(1, 0.4f, 0f), 0.5f);
        //black
        colorKeys[2] = new GradientColorKey(Color.black, 1f);

        alphaKeys = new GradientAlphaKey[2];
        //opaque
        alphaKeys[0] = new GradientAlphaKey(1.0f, 0.3f);
        //transparent
        alphaKeys[1] = new GradientAlphaKey(0f, 1f);
        gradient.SetKeys(colorKeys, alphaKeys);
        parColor.color = gradient;

        Vector3 targetPoint;
        Ray ray = new Ray(playerCam.position, playerCam.forward);
        targetPoint = ray.GetPoint(10);//This number tells how angled the flamethrower is
        transform.rotation = Quaternion.LookRotation(targetPoint - transform.position);

        PlayerShoot.shootInput += Shoot;
        PlayerShoot.releaseShoot += endShoot;
    }
    private void OnDisable()
    {
        em.enabled = false;
    }
    private void Update()
    {
        float heatRatio = script.GetCurrentHeat() / script.GetMaxTimeToCool();
        float fGradientPosition = Mathf.SmoothStep(0f, 1f, heatRatio) * 0.2f;
        float sGradientPosition = Mathf.SmoothStep(0f, 1f, heatRatio) * 0.5f + 0.001f;

        alphaKeys[0].time = heatRatio * 0.3f;
        alphaKeys[1].time = heatRatio + 0.002f;

        Color lowHeat = new Color(1f, 0.7f, 0.2f);
        Color highHeat = new Color(1f, 0.3f, 0f);
        Color currentColor = Color.Lerp(lowHeat, highHeat, heatRatio);

        colorKeys[1].color = currentColor;
        MoveGradientKeys(0, fGradientPosition);
        MoveGradientKeys(1, sGradientPosition);
    }
    private void MoveGradientKeys(int keyIndex, float newTime)
    {
        colorKeys[keyIndex].time = newTime;
        gradient.SetKeys(colorKeys, gradient.alphaKeys);
        parColor.color = gradient;
    }
    public void Shoot()
    {
        if (!this.isActiveAndEnabled)
            return;

        script.DecreaseHeat();

        if (em.enabled)
            return;

        if (em.enabled == false)
        {
            em.enabled = true;
        }
    }
    public void endShoot()
    {
        if (em.enabled)
        {
            em.enabled = false;
        }
    }
    public ParticleSystem.EmissionModule GetFlamethrowerParticleSystem()
    {
        return em;
    }
}