using System.Collections;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoadGameMenu : MonoBehaviour
{
    [SerializeField] private GameObject saveButtonPrefab; // Prefab del bot�n
    [SerializeField] private Transform saveListContainer; // Contenedor donde se mostrar�n los botones
    [SerializeField] private string saveFolder = "Saves"; // Carpeta de guardado
    [SerializeField] private GameObject loadMenuPanel; // Panel que contiene el men� de carga

    private SaveGameManager saveGameManager;

    private void Start()
    {
        // Obtener el SaveGameManager
        saveGameManager = FindFirstObjectByType<SaveGameManager>();
        if (saveGameManager == null)
        {
            Debug.LogError("SaveGameManager no encontrado.");
        }

        // Inicializar el men� de carga
        PopulateSaveList();
    }

    private void PopulateSaveList()
    {
        // Limpiar el contenedor de botones previos
        foreach (Transform child in saveListContainer)
        {
            Destroy(child.gameObject);
        }

        string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFolder);
        if (!Directory.Exists(saveFolderPath))
        {
            Debug.Log("No se han encontrado partidas guardadas.");
            return;
        }

        // Obtener los archivos de guardado
        string[] saveFiles = Directory.GetFiles(saveFolderPath, "*.json");
        foreach (string saveFilePath in saveFiles)
        {
            // Extraer el nombre del archivo sin la extensi�n
            string saveFileName = Path.GetFileNameWithoutExtension(saveFilePath);

            // Crear un bot�n para esta partida
            GameObject saveButton = Instantiate(saveButtonPrefab, saveListContainer);
            // Usar TextMeshProUGUI en lugar de Text
            saveButton.GetComponentInChildren<TextMeshProUGUI>().text = saveFileName;

            // Asignar la imagen de la captura de pantalla
            string screenshotPath = Path.Combine(saveFolderPath, "Screenshot_" + saveFileName + ".png");
            if (File.Exists(screenshotPath))
            {
                StartCoroutine(AssignScreenshot(saveButton, screenshotPath));
            }

            // A�adir evento al bot�n
            saveButton.GetComponent<Button>().onClick.AddListener(() => LoadGame(saveFilePath));
        }
    }

    private IEnumerator AssignScreenshot(GameObject saveButton, string screenshotPath)
    {
        yield return null; // Esperar un frame antes de asignar la imagen

        // Buscar espec�ficamente el objeto hijo con el componente Image
        Image buttonImage = saveButton.transform.Find("Image")?.GetComponent<Image>(); // Asume que el objeto hijo se llama "Image"
        if (buttonImage == null)
        {
            Debug.LogError("No se encontr� un componente Image en el hijo del bot�n.");
            yield break;
        }

        Texture2D screenshotTexture = LoadTexture(screenshotPath);
        if (screenshotTexture == null)
        {
            Debug.LogError("La textura es nula, no se asignar�.");
            yield break;
        }

        // Crear el Sprite y asignarlo al bot�n
        Sprite sprite = Sprite.Create(
            screenshotTexture,
            new Rect(0, 0, screenshotTexture.width, screenshotTexture.height),
            new Vector2(0.5f, 0.5f)
        );

        if (sprite == null)
        {
            Debug.LogError("Error al crear el Sprite.");
        }
        else
        {
            //Debug.Log("Sprite creado y asignado al bot�n.");
            buttonImage.sprite = sprite;
        }
    }

    private void LoadGame(string saveFilePath)
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogError("El archivo de guardado no existe: " + saveFilePath);
            return;
        }

        string jsonData = File.ReadAllText(saveFilePath);
        SaveGameManager.GameData loadedData = JsonUtility.FromJson<SaveGameManager.GameData>(jsonData);

        if (loadedData == null || string.IsNullOrEmpty(loadedData.sceneName))
        {
            Debug.LogError("Error al cargar la partida o la escena es inv�lida.");
            return;
        }

        // Cargar la escena correspondiente
        StartCoroutine(LoadSceneAndApplyData(loadedData, saveFilePath));
    }

    private IEnumerator LoadSceneAndApplyData(SaveGameManager.GameData loadedData, string saveFilePath)
    {
        // Aplicar los datos una vez que la escena haya terminado de cargar
        saveGameManager.LoadGameFromPath(saveFilePath);


        yield return null;
    }

    private Texture2D LoadTexture(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning("No se encontr� la imagen en: " + path);
            return null;
        }

        //Debug.Log("Imagen encontrada en: " + path);

        byte[] fileData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!texture.LoadImage(fileData))
        {
            Debug.LogError("Error al cargar la imagen en: " + path);
            return null;
        }

        //Debug.Log("Imagen cargada correctamente, dimensiones: " + texture.width + "x" + texture.height);
        return texture;
    }
}