using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance; // Singleton

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource musicSource;

    // Referencias a la UI que se actualizar�n en cada escena
    private Slider effectsSlider;
    private Slider musicSlider;
    private Toggle musicToggle;

    private const string MusicVolumeKey = "musicVolume";
    private const string EffectsVolumeKey = "effectsVolume";
    private const float DefaultVolume = 1f;

    private float lastMusicVolume;   // Para mutear/restaurar en cinem�ticas
    private float lastEffectsVolume;

    private void Awake()
    {
        // Configuraci�n de Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Se ejecuta cada vez que se carga una escena.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateUIReferences();
    }

    /// <summary>
    /// Busca en la escena los objetos "MusicSlider", "EffectsSlider" y "MusicToggle",
    /// aunque est�n desactivados o sean hijos profundos, y les asigna sus listeners.
    /// </summary>
    private void UpdateUIReferences()
    {
        // 1) Buscar el objeto "MusicSlider" (aunque est� inactivo/en hijos)
        GameObject musicBarObject = FindInActiveObjectByName("MusicSlider");
        if (musicBarObject != null)
        {
            // Importante: GetComponentInChildren<Slider>(true) para incluir hijos inactivos
            musicSlider = musicBarObject.GetComponentInChildren<Slider>(true);
            if (musicSlider != null)
            {
                musicSlider.onValueChanged.RemoveAllListeners();
                musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
            }
        }
        else
        {
            musicSlider = null;
        }

        // 2) Buscar el objeto "EffectsSlider"
        GameObject effectsBarObject = FindInActiveObjectByName("EffectsSlider");
        if (effectsBarObject != null)
        {
            effectsSlider = effectsBarObject.GetComponentInChildren<Slider>(true);
            if (effectsSlider != null)
            {
                effectsSlider.onValueChanged.RemoveAllListeners();
                effectsSlider.onValueChanged.AddListener(OnEffectsSliderChanged);
            }
        }
        else
        {
            effectsSlider = null;
        }

        // 3) Buscar el objeto "MusicToggle"
        GameObject musicToggleObject = FindInActiveObjectByName("MusicToggle");
        if (musicToggleObject != null)
        {
            musicToggle = musicToggleObject.GetComponentInChildren<Toggle>(true);
            if (musicToggle != null)
            {
                musicToggle.onValueChanged.RemoveAllListeners();
                musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
            }
        }
        else
        {
            musicToggle = null;
        }

        // Sincronizar la UI con los valores guardados
        LoadVolume();
        if (musicSlider != null)
            OnMusicSliderChanged(musicSlider.value);
        if (effectsSlider != null)
            OnEffectsSliderChanged(effectsSlider.value);
    }

    public void OnMusicSliderChanged(float value)
    {
        // Si el toggle est� apagado, se silencia la m�sica
        if (musicToggle != null && !musicToggle.isOn)
        {
            mixer.SetFloat("music", -80f);
            return;
        }

        if (value <= 0.0001f)
        {
            mixer.SetFloat("music", -80f);
        }
        else
        {
            mixer.SetFloat("music", Mathf.Log10(value) * 20);
        }
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
    }

    public void OnEffectsSliderChanged(float value)
    {
        if (value <= 0.0001f)
        {
            mixer.SetFloat("effects", -80f);
        }
        else
        {
            mixer.SetFloat("effects", Mathf.Log10(value) * 20);
        }
        PlayerPrefs.SetFloat(EffectsVolumeKey, value);
    }

    public void OnMusicToggleChanged(bool isOn)
    {
        if (isOn)
        {
            if (musicSlider != null)
                OnMusicSliderChanged(musicSlider.value);
        }
        else
        {
            mixer.SetFloat("music", -80f);
        }
    }

    private void LoadVolume()
    {
        if (!PlayerPrefs.HasKey(MusicVolumeKey))
            PlayerPrefs.SetFloat(MusicVolumeKey, DefaultVolume);
        if (!PlayerPrefs.HasKey(EffectsVolumeKey))
            PlayerPrefs.SetFloat(EffectsVolumeKey, DefaultVolume);

        if (musicSlider != null)
            musicSlider.value = PlayerPrefs.GetFloat(MusicVolumeKey, DefaultVolume);
        if (effectsSlider != null)
            effectsSlider.value = PlayerPrefs.GetFloat(EffectsVolumeKey, DefaultVolume);
    }

    // M�todos para mutear/restaurar (cinem�ticas) y controlar la m�sica en combate

    public void MuteForCinematic()
    {
        mixer.GetFloat("music", out lastMusicVolume);
        mixer.GetFloat("effects", out lastEffectsVolume);
        mixer.SetFloat("music", -80f);
        mixer.SetFloat("effects", -80f);
    }

    public void RestoreAfterCinematic()
    {
        mixer.SetFloat("music", lastMusicVolume);
        mixer.SetFloat("effects", lastEffectsVolume);
    }

    public void StopMusicForCombat()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Pause();
    }

    public void ResumeMusicAfterCombat()
    {
        if (musicSource != null && !musicSource.isPlaying)
            musicSource.Play();
    }

    /// <summary>
    /// Busca un GameObject por nombre en la escena actual,
    /// incluyendo objetos inactivos y jerarqu�as profundas.
    /// Devuelve el primero que coincida.
    /// </summary>
    private GameObject FindInActiveObjectByName(string name)
    {
        var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var rootObj in rootObjects)
        {
            var result = FindInChildrenRecursive(rootObj, name);
            if (result != null)
                return result;
        }
        return null;
    }

    /// <summary>
    /// B�squeda recursiva en todos los hijos (activos o inactivos) de un objeto.
    /// </summary>
    private GameObject FindInChildrenRecursive(GameObject parent, string name)
    {
        if (parent.name == name) return parent;

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            var child = parent.transform.GetChild(i).gameObject;
            var result = FindInChildrenRecursive(child, name);
            if (result != null) return result;
        }
        return null;
    }
}
