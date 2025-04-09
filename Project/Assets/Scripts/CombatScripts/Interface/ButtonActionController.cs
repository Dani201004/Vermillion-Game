using UnityEngine;
using UnityEngine.UI;

public class ButtonActionHandler : MonoBehaviour
{
    private Button button; // Referencia al bot�n (se obtiene autom�ticamente)
    private BattleCameraController cameraController; // Referencia al controlador de la c�mara

    private void Start()
    {



        cameraController = GameObject.FindWithTag("MainCamera")?.GetComponent<BattleCameraController>();
        button.onClick.AddListener(() =>
        {
            cameraController.FocusOnEnemy();
        });


        //Debug.Log("El script ButtonActionHandler ha sido correctamente inicializado.");
    }
}
