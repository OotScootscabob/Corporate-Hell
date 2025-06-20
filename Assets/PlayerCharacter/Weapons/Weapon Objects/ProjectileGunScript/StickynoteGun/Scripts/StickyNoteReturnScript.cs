using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class StickyNoteReturnScript : MonoBehaviour
{
    private new Collider collider;
    Rigidbody rb;
    private bool didRecall = false;
    StickyNoteSecondaryFire script;

    private float damageMultiplier = 2;

    private Material[] mat = new Material[2];
    public float dissolveRate = 0.0075f;
    public float refreshRate = 0.025f;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        mat [0] = this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material;
        mat[1] = this.gameObject.transform.GetChild(1).GetComponent<Renderer>().material;
    }
    public void SetScript(StickyNoteSecondaryFire newScript)
    {
        script = newScript;
    }
    public void Recall(Transform caller)
    {
        if (didRecall)
            return;

        rb.isKinematic = false;
        rb.useGravity = false;
        GetComponent<ProjectileDamge>().damage *= damageMultiplier;
        GetComponent<ProjectileDestroyAfterTime>().enabled = true;
        GetComponent<ProjectileDestroyAfterTime>().destroyTime = GetComponent<ProjectileDestroyAfterTime>().maxDestroyTime;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce((caller.position - transform.position).normalized * script.GetComponent<StickyNoteGunScript>().launchForce / 2, ForceMode.Impulse);

        didRecall = true;
        StartCoroutine(SetCollisionWithDelay());
    }
    IEnumerator SetCollisionWithDelay()
    {
        yield return new WaitForSeconds(0.05f);
        collider.enabled = true;
        float spinMagnitude = Random.Range(-script.GetComponent<StickyNoteGunScript>().spinSpeed, script.GetComponent<StickyNoteGunScript>().spinSpeed);
        GetComponent<Rigidbody>().AddTorque(transform.up * spinMagnitude);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (didRecall == false)
            GetComponent<ProjectileDestroyAfterTime>().enabled = false;

        if (didRecall && script != null) 
        {
            script.SubtractShotProjectile(this.gameObject);
            StartCoroutine(Dissolve());
        }
    }
    public void OnTimeDestroy()
    {
        if (script != null)
        {
            script.SubtractShotProjectile(this.gameObject);
            StartCoroutine(Dissolve());
        }
    }
    IEnumerator Dissolve()
    {
        collider.enabled = false;
        GetComponent<ProjectileDestroyAfterTime>().enabled = false;
        yield return new WaitForSeconds(1.5f);

        float counter = 0;

        while (mat[0].GetFloat("_DissolveAmmount") < 1)
        {
            counter += dissolveRate;
            for (int i = 0; i < mat.Length; i++)
            {
                mat[i].SetFloat("_DissolveAmmount", counter);
            }
            yield return new WaitForSeconds(refreshRate);
        }
        ResetBullet();
    }
    private void ResetBullet()
    {
        ObjectPool.EnqueueObject(this, "StickyNote");
        script.AddStickyNoteAmmo();

        mat[0].SetFloat("_DissolveAmmount", 0);
        mat[1].SetFloat("_DissolveAmmount", 0);

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        didRecall = false;

        gameObject.GetComponent<Collider>().enabled = true;
    }
}
