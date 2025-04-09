using UnityEngine;

public class IndividualBillboard : MonoBehaviour
{
    [Tooltip("Cámara hacia la que debe mirar este elemento")]
    public Camera targetCamera;

    void LateUpdate()
    {
        if (targetCamera != null)
        {
            transform.LookAt(transform.position + targetCamera.transform.rotation * Vector3.forward,
                             targetCamera.transform.rotation * Vector3.up);
        }
    }
}