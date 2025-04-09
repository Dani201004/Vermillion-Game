using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private AudioClip buttonClickSound; // Sonido para botones y toggles
    private static UIManager instance;
    private static AudioSource audioSource;

    public static UIManager Instance => instance;

    private void Awake()
    {
        // Asegurar una única instancia
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // No se destruye al cambiar de escena

            // Crear el AudioSource si no existe
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            AssignUIElementsSounds(); // Asignar sonido a botones y toggles en la escena inicial

            // Suscribirse al cambio de escena
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // Evitar duplicados
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignUIElementsSounds(); // Asignar sonidos en cada nueva escena
    }

    // Método público para asignar sonidos, de modo que pueda llamarse tras instanciar clones.
    public void AssignUIElementsSounds()
    {
        // Buscar todos los botones, incluyendo los inactivos
        Button[] buttons = FindObjectsOfType<Button>(true);
        foreach (Button button in buttons)
        {
            button.onClick.RemoveListener(PlayButtonSound); // Evita duplicados
            button.onClick.AddListener(PlayButtonSound);
        }

        // Buscar todos los toggles, incluyendo los inactivos
        Toggle[] toggles = FindObjectsOfType<Toggle>(true);
        foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.RemoveListener(PlayToggleSound);
            toggle.onValueChanged.AddListener(PlayToggleSound);
        }
    }

    public void PlayButtonSound()
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.volume = 0.4f; // Ajusta el volumen a 0.4
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    // Método para reproducir sonido en toggles.
    // Puedes descomentar la siguiente línea si deseas que solo se reproduzca al activar el toggle.
    // if (!value) return;
    public void PlayToggleSound(bool value)
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.volume = 0.4f;
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
}