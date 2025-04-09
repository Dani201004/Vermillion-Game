using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveGameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    private SceneTransition sceneTransition;
    public static SaveGameManager Instance { get; private set; }

    public GameData CurrentGameData => currentGameData;
    private GameData currentGameData;

    // Servicio encargado de las operaciones de archivo
    private SaveFileService fileService;

    // Control de guardado para evitar múltiples guardados simultáneos o en corto período
    private bool isSaving = false;
    [SerializeField] float saveCooldown = 1.0f;
    private float lastSaveTime = -10f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        fileService = new SaveFileService("Saves");
    }

    #region Clases de Datos

    [Serializable]
    public class EquipmentSaveData
    {
        public string characterID;
        public List<string> equipment;  // Aquí se guardará el JSON de cada ítem
        public EquipmentSaveData()
        {
            equipment = new List<string>();
        }
    }

    // NUEVA CLASE: Guarda la referencia (ruta) al archivo de stats de cada personaje
    [Serializable]
    public class CharacterStatsReference
    {
        public string characterName;
        public string statsFilePath;
    }

    [Serializable]
    public class GameData
    {
        public List<CharacterData> characters;
        public Vector3Data position;
        public List<string> inventory;        // JSON de cada ítem del inventario general
        public List<string> currentEquipment; // JSON de cada ítem del equipamiento activo
        public List<EquipmentSaveData> allEquipment;  // JSON de los equipamientos de TODOS los personajes
        public string currentEquipmentID;     // ID del equipamiento actualmente activo (ej. "Adventurer")
        public string sceneName;

        // NUEVA LISTA: Referencias a los saves de stats de los personajes
        public List<CharacterStatsReference> characterStatsReferences;

        public GameData()
        {
            characters = new List<CharacterData>();
            position = new Vector3Data(206, 9, -215);
            inventory = new List<string>();
            currentEquipment = new List<string>();
            allEquipment = new List<EquipmentSaveData>();
            currentEquipmentID = "Adventurer"; // Valor por defecto
            characterStatsReferences = new List<CharacterStatsReference>();
        }

        public void UpdateGameData(Vector3Data position, List<string> inventory, List<string> currentEquipment, List<EquipmentSaveData> allEquipment, string currentEquipmentID, string sceneName)
        {
            this.position = position;
            this.inventory = new List<string>(inventory);
            this.currentEquipment = new List<string>(currentEquipment);
            this.allEquipment = new List<EquipmentSaveData>(allEquipment);
            this.currentEquipmentID = currentEquipmentID;
            this.sceneName = sceneName;
        }

        public void AddCharacter(CharacterData character)
        {
            characters.Add(character);
        }

        public void UpdateCharacter(CharacterData updatedCharacter)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                if (string.Equals(characters[i].playerName, updatedCharacter.playerName, StringComparison.OrdinalIgnoreCase))
                {
                    characters[i] = updatedCharacter;
                    return;
                }
            }
        }

        public CharacterData GetCharacter(string playerName)
        {
            return characters.Find(character => string.Equals(character.playerName, playerName, StringComparison.OrdinalIgnoreCase));
        }
    }

    [Serializable]
    public class CharacterData
    {
        public string playerName;
        public int level;
        public int experience;
        public int maxExperience;
        public int skillPoints;
        public int attack;
        public int magicPower;
        public int armor;
        public float maxHealth;
        public int stamina;
        public int speed;
        public float health;
        public float mana;

        public CharacterData(
            string playerName = "Player",
            int level = 1,
            int experience = 0,
            int maxExperience = 100,
            int skillPoints = 0,
            int attack = 10,
            int magicPower = 0,
            int armor = 5,
            float maxHealth = 100,
            int stamina = 50,
            int speed = 10,
            float health = 100f,
            float mana = 50f)
        {
            this.playerName = playerName;
            this.level = level;
            this.experience = experience;
            this.maxExperience = maxExperience;
            this.skillPoints = skillPoints;
            this.attack = attack;
            this.magicPower = magicPower;
            this.armor = armor;
            this.maxHealth = maxHealth;
            this.stamina = stamina;
            this.speed = speed;
            this.health = health;
            this.mana = mana;
        }

        public void UpdateStats(
            int level,
            int experience,
            int maxExperience,
            int skillPoints,
            int attack,
            int magicPower,
            int armor,
            float maxHealth,
            int stamina,
            int speed,
            float health,
            float mana)
        {
            this.level = level;
            this.experience = experience;
            this.maxExperience = maxExperience;
            this.skillPoints = skillPoints;
            this.attack = attack;
            this.magicPower = magicPower;
            this.armor = armor;
            this.maxHealth = maxHealth;
            this.stamina = stamina;
            this.speed = speed;
            this.health = health;
            this.mana = mana;
        }
    }

    [Serializable]
    public class Vector3Data
    {
        public float x, y, z;
        public Vector3Data(float x, float y, float z)
        {
            this.x = x; this.y = y; this.z = z;
        }
        public Vector3 ToUnityVector3() => new Vector3(x, y, z);
        public static Vector3Data FromUnityVector3(Vector3 vector) => new Vector3Data(vector.x, vector.y, vector.z);
    }

    #endregion

    #region Métodos de Guardado

    public void SaveGame()
    {
        if (Time.time - lastSaveTime < saveCooldown)
        {
            Debug.Log("Guardado en cooldown. Se ignora este intento.");
            return;
        }
        if (isSaving)
        {
            Debug.Log("Ya se está guardando. Se ignora este intento.");
            return;
        }

        isSaving = true;
        lastSaveTime = Time.time;

        try
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.LogError("No se encontró un objeto con la etiqueta 'Player'.");
                return;
            }
            if (currentGameData == null)
            {
                currentGameData = new GameData();
            }

            // --- Recopilación del inventario general ---
            List<string> inventoryData = new List<string>();
            foreach (var item in InventoryManager.GetInventoryItems())
            {
                if (item != null)
                    inventoryData.Add(JsonUtility.ToJson(item));
            }

            // --- Recopilación del equipamiento actualmente activo ---
            List<string> currentEquipmentData = new List<string>();
            foreach (var item in InventoryManager.GetEquipmentItems())
            {
                if (item != null)
                    currentEquipmentData.Add(JsonUtility.ToJson(item));
            }

            // --- Recopilación de TODOS los inventarios de equipamiento ---
            List<EquipmentSaveData> allEquipmentData = new List<EquipmentSaveData>();

            // Inventario de equipamiento para Adventurer
            EquipmentSaveData adventurerData = new EquipmentSaveData();
            adventurerData.characterID = "Adventurer";
            foreach (var item in InventoryManager.equipmentItemsAdventurer)
            {
                if (item != null)
                    adventurerData.equipment.Add(JsonUtility.ToJson(item));
            }
            allEquipmentData.Add(adventurerData);

            // Para Witch
            EquipmentSaveData witchData = new EquipmentSaveData();
            witchData.characterID = "Witch";
            foreach (var item in InventoryManager.equipmentItemsWitch)
            {
                if (item != null)
                    witchData.equipment.Add(JsonUtility.ToJson(item));
            }
            allEquipmentData.Add(witchData);

            // Para Paladin
            EquipmentSaveData paladinData = new EquipmentSaveData();
            paladinData.characterID = "Paladin";
            foreach (var item in InventoryManager.equipmentItemsPaladin)
            {
                if (item != null)
                    paladinData.equipment.Add(JsonUtility.ToJson(item));
            }
            allEquipmentData.Add(paladinData);

            // Para Cleric
            EquipmentSaveData clericData = new EquipmentSaveData();
            clericData.characterID = "Cleric";
            foreach (var item in InventoryManager.equipmentItemsCleric)
            {
                if (item != null)
                    clericData.equipment.Add(JsonUtility.ToJson(item));
            }
            allEquipmentData.Add(clericData);

            // --- Actualización de los datos de la partida ---
            currentGameData.UpdateGameData(
                Vector3Data.FromUnityVector3(player.transform.position),
                inventoryData,
                currentEquipmentData,
                allEquipmentData,
                InventoryManager.GetCurrentEquipmentID(), // Por ejemplo "Adventurer"
                SceneManager.GetActiveScene().name
            );

            // Se genera siempre una ruta nueva para el save (ignorando LastSavePath)
            string saveFilePath = fileService.GenerateSaveFilePath(SceneManager.GetActiveScene().name);

            // Opcional: si quieres que se almacene la ruta del último save en PlayerPrefs, puedes hacerlo,
            // pero no se usará para sobrescribir el archivo anterior.
            PlayerPrefs.SetString("LastSavePath", saveFilePath);
            PlayerPrefs.Save();

            // Captura de pantalla asociada al guardado (opcional)
            string screenshotPath = Path.Combine(fileService.SaveFolderPath, "Screenshot_" +
                Path.GetFileNameWithoutExtension(saveFilePath) + ".png");
            fileService.CaptureScreenshot(screenshotPath);

            string jsonData = JsonUtility.ToJson(currentGameData, true);
            fileService.SaveToFile(saveFilePath, jsonData);
            fileService.ManageSaveFileLimit();

            Debug.Log("Partida guardada correctamente en: " + saveFilePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al guardar la partida: " + ex.Message);
        }
        finally
        {
            isSaving = false;
        }
    }


    // MÉTODO NUEVO: Actualiza la lista de referencias a los saves de los stats de cada personaje.
    private void UpdateCharacterStatsReferences()
    {
        // Reiniciamos la lista
        currentGameData.characterStatsReferences = new List<CharacterStatsReference>();
        // Obtenemos el identificador global del save
        string saveId = PlayerPrefs.GetString("SaveIdentifier", "DefaultSave");
        // Por cada personaje registrado en el GameData, se crea su referencia.
        foreach (var character in currentGameData.characters)
        {
            CharacterStatsReference reference = new CharacterStatsReference();
            reference.characterName = character.playerName;
            // Se asume que el archivo de stats se nombra usando el saveId y el nombre del personaje.
            string fileName = saveId + "_" + character.playerName + ".json";
            reference.statsFilePath = System.IO.Path.Combine(Application.persistentDataPath, "PlayerStatsSaves", fileName);
            currentGameData.characterStatsReferences.Add(reference);
        }
    }

    #endregion

    #region Métodos de Carga y Restauración

    public void LoadGameAndApply()
    {
        currentGameData = LoadGame();
        ApplyGameDataToPlayer();
        RestoreInventoryAndEquipment();

    }

    private GameData LoadGame()
    {
        try
        {
            string saveFilePath = PlayerPrefs.GetString("LastSavePath", "");
            if (!string.IsNullOrEmpty(saveFilePath))
            {
                string jsonData = fileService.LoadFromFile(saveFilePath);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    GameData gameData = JsonUtility.FromJson<GameData>(jsonData);
                    //Debug.Log("Partida cargada desde: " + saveFilePath);
                    return gameData;
                }
            }
            Debug.Log("No se encontró un archivo de guardado. Se creará una nueva partida.");
            return new GameData();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al cargar la partida: " + ex.Message);
            return new GameData();
        }
    }

    public void ApplyGameDataToPlayer()
    {
        if (currentGameData == null)
        {
            Debug.LogError("currentGameData es null. No se pueden aplicar los datos de la partida.");
            return;
        }

        // Asegurarse de que sceneTransition esté asignado
        if (sceneTransition == null)
        {
            sceneTransition = FindObjectOfType<SceneTransition>(true);
            if (sceneTransition == null)
            {
                Debug.LogError("SceneTransition no encontrado en la escena.");
                return;
            }
        }

        // Obtener el nombre de la escena guardado en currentGameData
        string sceneToLoad = currentGameData.sceneName;
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("No se encontró nombre de escena en currentGameData. Se usará una escena por defecto.");
            sceneToLoad = "DefaultScene"; // O el nombre de la escena por defecto que prefieras
        }

        // Usar el SceneTransition para cargar la escena usando el nombre obtenido
        sceneTransition.LoadLevel(sceneToLoad);
    }

    public void RestoreInventoryAndEquipment()
    {
        // --- Restaurar Inventario General ---
        Item[] invItems = InventoryManager.GetInventoryItems();
        for (int i = 0; i < invItems.Length; i++)
            invItems[i] = null;
        int invIndex = 0;
        foreach (string itemJson in currentGameData.inventory)
        {
            if (!string.IsNullOrEmpty(itemJson) && invIndex < invItems.Length)
            {
                invItems[invIndex] = JsonUtility.FromJson<Item>(itemJson);
                invIndex++;
            }
        }

        // --- Restaurar Equipamiento Actual ---
        Item[] currEquipItems = InventoryManager.GetEquipmentItems();
        for (int i = 0; i < currEquipItems.Length; i++)
            currEquipItems[i] = null;
        int currEquipIndex = 0;
        foreach (string itemJson in currentGameData.currentEquipment)
        {
            if (!string.IsNullOrEmpty(itemJson) && currEquipIndex < currEquipItems.Length)
            {
                currEquipItems[currEquipIndex] = JsonUtility.FromJson<Item>(itemJson);
                currEquipIndex++;
            }
        }

        // --- Restaurar TODOS los Inventarios de Equipamiento ---
        foreach (var equipmentData in currentGameData.allEquipment)
        {
            if (equipmentData.characterID == "Adventurer")
            {
                Item[] arr = InventoryManager.equipmentItemsAdventurer;
                for (int i = 0; i < arr.Length; i++) arr[i] = null;
                int idx = 0;
                foreach (string json in equipmentData.equipment)
                {
                    if (!string.IsNullOrEmpty(json) && idx < arr.Length)
                    {
                        arr[idx] = JsonUtility.FromJson<Item>(json);
                        idx++;
                    }
                }
            }
            else if (equipmentData.characterID == "Witch")
            {
                Item[] arr = InventoryManager.equipmentItemsWitch;
                for (int i = 0; i < arr.Length; i++) arr[i] = null;
                int idx = 0;
                foreach (string json in equipmentData.equipment)
                {
                    if (!string.IsNullOrEmpty(json) && idx < arr.Length)
                    {
                        arr[idx] = JsonUtility.FromJson<Item>(json);
                        idx++;
                    }
                }
            }
            else if (equipmentData.characterID == "Paladin")
            {
                Item[] arr = InventoryManager.equipmentItemsPaladin;
                for (int i = 0; i < arr.Length; i++) arr[i] = null;
                int idx = 0;
                foreach (string json in equipmentData.equipment)
                {
                    if (!string.IsNullOrEmpty(json) && idx < arr.Length)
                    {
                        arr[idx] = JsonUtility.FromJson<Item>(json);
                        idx++;
                    }
                }
            }
            else if (equipmentData.characterID == "Cleric")
            {
                Item[] arr = InventoryManager.equipmentItemsCleric;
                for (int i = 0; i < arr.Length; i++) arr[i] = null;
                int idx = 0;
                foreach (string json in equipmentData.equipment)
                {
                    if (!string.IsNullOrEmpty(json) && idx < arr.Length)
                    {
                        arr[idx] = JsonUtility.FromJson<Item>(json);
                        idx++;
                    }
                }
            }
        }

        // --- Restaurar la Referencia al Equipamiento Actual según el ID guardado ---
        switch (currentGameData.currentEquipmentID)
        {
            case "Adventurer":
                InventoryManager.currentEquipmentItems = InventoryManager.CloneEquipmentArray(InventoryManager.equipmentItemsAdventurer);
                break;
            case "Witch":
                InventoryManager.currentEquipmentItems = InventoryManager.CloneEquipmentArray(InventoryManager.equipmentItemsWitch);
                break;
            case "Paladin":
                InventoryManager.currentEquipmentItems = InventoryManager.CloneEquipmentArray(InventoryManager.equipmentItemsPaladin);
                break;
            case "Cleric":
                InventoryManager.currentEquipmentItems = InventoryManager.CloneEquipmentArray(InventoryManager.equipmentItemsCleric);
                break;
            default:
                Debug.LogError("ID de equipamiento desconocido: " + currentGameData.currentEquipmentID);
                break;
        }

        Debug.Log("Inventario y todos los equipamientos restaurados.");
    }

    public void RestorePlayer()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        OnSceneLoaded(activeScene, LoadSceneMode.Single);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currentGameData == null)
        {
            Debug.LogError("currentGameData es null en OnSceneLoaded. No se han cargado los datos.");
            return;
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }
        if (playerPrefab == null)
        {
            Debug.LogError("playerPrefab es null.");
            return;
        }
        GameObject newPlayer = Instantiate(playerPrefab, currentGameData.position.ToUnityVector3(), Quaternion.identity);
        //Debug.Log("Jugador instanciado en: " + newPlayer.transform.position);
    }

    public void LoadGameFromPath(string filePath)
    {
        try
        {
            if (System.IO.File.Exists(filePath))
            {
                string jsonData = System.IO.File.ReadAllText(filePath);
                GameData gameData = JsonUtility.FromJson<GameData>(jsonData);
                currentGameData = gameData;
                //Debug.Log("Partida cargada desde: " + filePath);
                ApplyGameDataToPlayer();
                RestoreInventoryAndEquipment();
            }
            else
            {
                Debug.LogError("El archivo de guardado no existe en la ruta especificada: " + filePath);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al cargar la partida desde el archivo: " + ex.Message);
        }
    }

    public void DeleteAllSaves()
    {
        if (fileService == null)
        {
            Debug.LogWarning("SaveFileService no está inicializado.");
            return;
        }

        string saveFolder = fileService.SaveFolderPath; // Asegúrate de que SaveFolderPath esté definido en SaveFileService.
        if (Directory.Exists(saveFolder))
        {
            try
            {
                // Obtener todos los archivos dentro de la carpeta de guardados.
                string[] files = Directory.GetFiles(saveFolder);
                foreach (string file in files)
                {
                    File.Delete(file);
                    Debug.Log("Archivo de guardado eliminado: " + file);
                }
                // Opcional: borrar la clave del último save almacenada en PlayerPrefs.
                PlayerPrefs.DeleteKey("LastSavePath");
                PlayerPrefs.Save();
                //Debug.Log("Todos los saves han sido eliminados.");
            }
            catch (Exception ex)
            {
                Debug.LogError("Error al borrar los archivos de guardado: " + ex.Message);
            }
        }
        else
        {
            Debug.LogWarning("La carpeta de guardados no existe: " + saveFolder);
        }
    }

    #region Registro de PlayerStats

    /// <summary>
    /// Registra en el global save (GameData) un nuevo PlayerStats, añadiendo:
    /// - Un CharacterData si no existe aún.
    /// - Una referencia (CharacterStatsReference) con la ruta del archivo de stats.
    /// </summary>
    /// <param name="ps">La instancia de PlayerStats a registrar.</param>
    public void RegisterPlayerStats(PlayerStats ps)
    {
        if (currentGameData == null)
        {
            currentGameData = new GameData();
        }

        // Registrar el CharacterData (si no existe)
        var existingCharacter = currentGameData.GetCharacter(ps.characterName);
        if (existingCharacter == null)
        {
            CharacterData newChar = new CharacterData(
                playerName: ps.characterName,
                level: ps.currentLevel,
                experience: ps.currentExperience,
                maxExperience: ps.maxExperience,
                skillPoints: ps.skillPoints,
                attack: ps.Attack,
                magicPower: ps.MagicPower,
                armor: ps.Armor,
                maxHealth: ps.MaxHealth,
                stamina: ps.Stamina,
                speed: ps.Speed,
                health: ps.CurrentHealth,
                mana: ps.MaxMana
            );
            currentGameData.AddCharacter(newChar);
            Debug.Log("[SaveGameManager] Registrado nuevo CharacterData para: " + ps.characterName);
        }

        // Construir la ruta del archivo de stats (aunque el archivo aún no exista)
        string saveId = PlayerPrefs.GetString("SaveIdentifier", "DefaultSave");
        string fileName = saveId + "_" + ps.characterName + ".json";
        string filePath = Path.Combine(Application.persistentDataPath, "PlayerStatsSaves", fileName);

        // Si no existe ya una referencia para este personaje, se añade
        bool exists = false;
        foreach (var reference in currentGameData.characterStatsReferences)
        {
            if (reference.characterName.Equals(ps.characterName, StringComparison.OrdinalIgnoreCase))
            {
                exists = true;
                break;
            }
        }
        if (!exists)
        {
            CharacterStatsReference newRef = new CharacterStatsReference();
            newRef.characterName = ps.characterName;
            newRef.statsFilePath = filePath;
            currentGameData.characterStatsReferences.Add(newRef);
            Debug.Log("[SaveGameManager] Registrada nueva referencia para: " + ps.characterName);
        }
    }
    #endregion

    #endregion
}