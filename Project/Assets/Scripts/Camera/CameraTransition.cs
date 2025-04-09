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
            Debug.LogWarning("No se encontr� la c�mara FreeLook en la escena.");
        }
    }

    private void Update()
    {
        // Asegurar que la c�mara sigue asignada en caso de cambios en la escena
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
            //Debug.Log("C�mara principal asignada correctamente.");
        }
        else
        {
            Debug.LogError("No se encontr� la c�mara principal en la escena. Aseg�rate de que hay una c�mara con la etiqueta 'MainCamera'.");
        }
    }

    public void TransitionToScene()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("No se ha asignado el Transform de la c�mara. Intentando asignarlo nuevamente...");
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
                Debug.LogError("cameraTransform es null durante la rotaci�n.");
                yield break; // Detener la corrutina si la c�mara es null
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}

