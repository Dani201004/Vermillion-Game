using UnityEngine;

public class Frames : MonoBehaviour
{
    public static Frames Instance { get; private set; }

    private void Awake()
    {
        // Implementación del Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Evita que se destruya al cambiar de escena
        }
        else
        {
            Destroy(gameObject); // Si ya existe una instancia, destruye este objeto
            return;
        }

        Application.targetFrameRate = 144; // Cambia a los FPS deseados
    }
}
