using System.Collections;
using UnityEngine;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI consejoTexto; // Asigna el Text UI en el Inspector
    [SerializeField] private float cambioIntervalo = 3f; // Tiempo en segundos entre cambios

    // Array de consejos
    private string[] consejos = new string[]
    {
        "Recuerda guardar partida frecuentemente",
        "Los cofres ocultos son una gran fuente de valiosos objetos",
        "los slimes son adorables pero no te despistes, intentaran atacarte",
        "Si algun combate te resulta dificil siempre puedes intentar huir",
        "Disfruta del mundo de Vermillion",
        "Recuerda equiparte los objetos, te seran de gran utilidad para el combate",
        "Si un combate es demasiado dificil deberias subir de nivel y volver a intentarlo",
        "No te pierdas, usa el mapa",
        "En el inventario podras encontrar todos los objetos que te encuentres en tu viaje",
        "Gloria a tyr",
    };

    private void Start()
    {
        if (consejoTexto == null)
        {
            Debug.LogError("No se ha asignado un Text UI en el Inspector.");
            return;
        }

        StartCoroutine(CambiarConsejo());
    }

    private IEnumerator CambiarConsejo()
    {
        while (true)
        {
            // Selecciona un consejo aleatorio del array
            consejoTexto.text = consejos[Random.Range(0, consejos.Length)];
            yield return new WaitForSeconds(cambioIntervalo);
        }
    }

}
