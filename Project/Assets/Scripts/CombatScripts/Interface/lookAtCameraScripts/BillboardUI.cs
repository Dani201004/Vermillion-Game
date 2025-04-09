using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        // Hace que el Canvas se oriente hacia la cámara
        transform.LookAt(transform.position + mainCam.transform.rotation * Vector3.forward,
                         mainCam.transform.rotation * Vector3.up);
    }
}
