using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera != null)
        {
            Vector3 direction = transform.position - mainCamera.transform.position;
            direction.y = 0; // Bloquear la rotación en el eje Y
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
