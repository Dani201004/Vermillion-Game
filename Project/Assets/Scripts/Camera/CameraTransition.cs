using System.Collections;
using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    private Transform cameraTransform;
    [SerializeField] public float rotationSpeed = 360f;
    [SerializeField] public float transitionDuration = 1f;

    private GameObject freeLookCamera;

    private void Start()
    {
        AssignCameraTransform();

        freeLookCamera = GameObject.FindWithTag("FreeLookCamera");
        if (freeLookCamera == null)
        {
            Debug.LogWarning("No se encontró la cámara FreeLook en la escena.");
        }
    }

    private void Update()
    {
        // Asegurar que la cámara sigue asignada en caso de cambios en la escena
        if (cameraTransform == null)
        {
            AssignCameraTransform();
        }

        if (freeLookCamera == null)
        {
            freeLookCamera = GameObject.FindWithTag("FreeLookCamera");
        }
    }

    private void AssignCameraTransform()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            cameraTransform = mainCamera.transform;
            //Debug.Log("Cámara principal asignada correctamente.");
        }
        else
        {
            Debug.LogError("No se encontró la cámara principal en la escena. Asegúrate de que hay una cámara con la etiqueta 'MainCamera'.");
        }
    }

    public void TransitionToScene()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("No se ha asignado el Transform de la cámara. Intentando asignarlo nuevamente...");
            AssignCameraTransform();
            if (cameraTransform == null) return;
        }

        if (freeLookCamera != null)
        {
            freeLookCamera.SetActive(false);
        }

        StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            if (cameraTransform != null)
            {
                cameraTransform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            }
            else
            {
                Debug.LogError("cameraTransform es null durante la rotación.");
                yield break; // Detener la corrutina si la cámara es null
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}

