using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("WallRunning")]

    public float wallRunSmoothing = 1;
    public float wallRunZ = 20;

    private Transform head;

    private bool wallrunning;
    private bool wallrunDir;

    Camera cam;
    Rigidbody rb;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        cam = GetComponentInChildren<Camera>();
        rb = transform.parent.GetComponentInChildren<Rigidbody>();
    }

    private void Start()
    {
        wallrunning = false;

        head = Movement.Instance.head;
    }

    private void LateUpdate()
    {
        transform.position = head.position;

        if (wallrunning && Input.GetAxisRaw("Vertical") == 1)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Mathf.LerpAngle(transform.rotation.eulerAngles.z,
                wallrunDir ? wallRunZ : -wallRunZ, Time.deltaTime * wallRunSmoothing));
        }
        else
        {
            if (Mathf.RoundToInt(transform.rotation.eulerAngles.z * 10) != 0)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Mathf.LerpAngle(transform.rotation.eulerAngles.z, 0, Time.deltaTime * wallRunSmoothing * 3f));
            }
        }
    }

    public void StartWallrun(bool dir)
    {
        wallrunning = true;
        wallrunDir = dir;
    }

    public void StopWallrun()
    {
        wallrunning = false;
    }
}