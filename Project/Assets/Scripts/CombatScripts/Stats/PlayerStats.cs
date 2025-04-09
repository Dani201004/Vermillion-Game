using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats
{
    public string characterName;
    public bool TurnAvailable;
    public Button fusionButton; // Presente en el primer script
    public bool isMagic { get; private set; }
    public SkillTree_Manager skillTreeManager;

    // *** Esta lista se utiliza para guardar y cargar (no modificarla, se usa en el guardado del primer script) ***
    public List<Skill> learnedSkills = new List<Skill>();

    // Datos de experiencia y nivel
    public int currentExperience = 0;
    public int maxExperience = 6;
    public int currentLevel;
    private CharacterUI charUI;
    public string Name { get; private set; }

    public int skillPoints;
    private bool statsLoaded = false;


    // -----------------------------
    // FUNCIONALIDAD AGREGADA DEL SEGUNDO SCRIPT
    // -----------------------------

    // Array de ScriptableObjects que definen habilidades (asignar desde el Inspector)
    [SerializeField] private SkillDescScript[] skillDataArray;



    // -----------------------------
    // MÉTODOS DEL CICLO DE VIDA
    // -----------------------------

    void Awake()
    {
        if (skillTreeManager == null)
        {
            SkillTree_Manager[] managers = Resources.FindObjectsOfTypeAll<SkillTree_Manager>();
            foreach (SkillTree_Manager manager in managers)
            {
                if (string.Equals(manager.characterName, this.characterName, StringComparison.OrdinalIgnoreCase))
                {
                    skillTreeManager = manager;
                    Debug.Log($"[PlayerStats] Se asignó SkillTree_Manager para {characterName}.");
                    break;
                }
            }
            if (skillTreeManager == null)
            {
                Debug.LogWarning($"[PlayerStats] No se encontró SkillTree_Manager para {characterName}.");
            }
        }
        Debug.Log($"[PlayerStats] Inicio del PlayerStats para: {characterName}");
        LoadSkills();
        Debug.Log("[PlayerStats] Habilidades cargadas (si existen) mediante LoadSkills.");
        LoadCharacterData();
        Debug.Log("[PlayerStats] Se ha ejecutado LoadCharacterData para cargar/inicializar datos del personaje.");

        // Nueva funcionalidad: cargar las definiciones de habilidades desde ScriptableObjects
        LoadSkillsFromScriptableObjects();

        charUI = GetComponentInChildren<CharacterUI>();

        TurnManager tm = FindFirstObjectByType<TurnManager>();
        if (tm != null)
        {
            tm.OnTurnChanged += OnTurnChanged;
            Debug.Log("[PlayerStats] Suscrito al evento OnTurnChanged.");
        }
        else
        {
            Debug.LogWarning("[PlayerStats] TurnManager no encontrado en la escena.");
        }
        skills = learnedSkills.ToArray();
        // Se asignan las habilidades (por ejemplo, para que el CharacterStats las use)
        // Se muestra en consola la información de cada habilidad cargada

    }
    private void Update()
    {
        foreach (Skill nombre in learnedSkills)
        {
            Debug.Log("Skill: " + nombre.Name);
        }
    }

    void OnDestroy()
    {
        TurnManager tm = FindFirstObjectByType<TurnManager>();
        if (tm != null)
        {
            tm.OnTurnChanged -= OnTurnChanged;
            Debug.Log("[PlayerStats] Cancelada la suscripción al evento OnTurnChanged.");
        }
    }

    private void OnTurnChanged(CharacterStats currentTurnCharacter)
    {
        if (charUI != null)
        {
            bool isMyTurn = (currentTurnCharacter == this);
            charUI.SetTurnIndicator(isMyTurn);
            Debug.Log($"[PlayerStats] Turno cambiado. ¿Es mi turno? {isMyTurn}");
        }
    }

    public bool GetMagic() => isMagic;

    private void InitializeCharacterStats()
    {
        Debug.Log($"[PlayerStats] Inicializando estadísticas para {characterName}.");
        switch (characterName.ToLower())
        {
            case "paladin":
                InitializeStats(attack: 150, magicPower: 0, armor: 80, maxHealth: 800, maxMana: 50, stamina: 100, speed: 30, skills: new Skill[0]);
                isMagic = false;
                break;
            case "witch":
                InitializeStats(attack: 0, magicPower: 300, armor: 25, maxHealth: 400, maxMana: 300, stamina: 70, speed: 50, skills: new Skill[0]);
                isMagic = true;
                break;
            case "cleric":
                InitializeStats(attack: 0, magicPower: 180, armor: 70, maxHealth: 600, maxMana: 200, stamina: 80, speed: 35, skills: new Skill[0]);
                isMagic = true;
                break;
            case "thief":
                InitializeStats(attack: 170, magicPower: 0, armor: 35, maxHealth: 500, maxMana: 50, stamina: 120, speed: 100, skills: new Skill[0]);
                isMagic = false;
                break;
            default:
                InitializeStats(attack: 100, magicPower: 0, armor: 20, maxHealth: 500, maxMana: 100, stamina: 50, speed: 50, skills: new Skill[0]);
                break;
        }
        Debug.Log($"[PlayerStats] Estadísticas inicializadas para {characterName}: Ataque: {Attack}, Defensa: {Armor}, Salud Máxima: {MaxHealth}");
    }

    /// <summary>
    /// Intenta cargar los datos del personaje desde su archivo exclusivo.
    /// Si falla, se inicializan los valores predeterminados y se guarda una primera vez.
    /// </summary>
    private void LoadCharacterData()
    {
        Debug.Log($"[PlayerStats] Intentando cargar datos para {characterName}.");

        if (!LoadPlayerStats())
        {
            Debug.LogWarning($"[PlayerStats] No se encontraron datos guardados para {characterName}. Inicializando estadísticas predeterminadas.");
            InitializeCharacterStats();
            ChangePlayerData();  // Guarda los datos inicializados.
        }
        statsLoaded = true;
    }

    public void HandleExperienceChange(int newExperience)
    {
        Debug.Log($"[PlayerStats] Experiencia actual: {currentExperience}. Incrementando en: {newExperience}");
        currentExperience += newExperience;
        if (currentExperience >= maxExperience)
        {
            Debug.Log($"[PlayerStats] Experiencia alcanzada: {currentExperience} >= {maxExperience}. Subiendo de nivel.");
            LevelUp();
        }
    }

    /// <summary>
    /// Actualiza los datos del personaje y los guarda en su archivo exclusivo.
    /// </summary>
    public void ChangePlayerData()
    {
        Debug.Log("[PlayerStats] Guardando datos del jugador...");
        SavePlayerStats();
        Debug.Log("[PlayerStats] Datos del jugador actualizados.");
    }

    private void LevelUp()
    {
        Debug.Log($"[PlayerStats] Nivel actual: {currentLevel}. Procesando subida de nivel.");

        // Comprobar y obtener la experiencia de misiones almacenada en PlayerPrefs
        int rewardExperience = PlayerPrefs.GetInt("ExperienciaMisiones", 0);
        Debug.Log($"[PlayerStats] Experiencia de misiones guardada: {rewardExperience}");

        currentLevel++;
        currentExperience -= maxExperience;

        maxExperience += maxExperience / 2;

        // Sumar la experiencia de misiones al currentExperience
        currentExperience += rewardExperience;

        if (currentExperience == maxExperience)
        {
            LevelUp();
        }

        // Opcional: Si ya se ha usado esa experiencia, se puede reiniciar para evitar sumarla de nuevo
        PlayerPrefs.SetInt("ExperienciaMisiones", 0);
        PlayerPrefs.Save();

        Debug.Log($"[PlayerStats] Nuevo nivel: {currentLevel}. Nueva experiencia: {currentExperience}/{maxExperience}");

        if (currentLevel % 2 == 0)
        {
            skillPoints++;
            if (skillTreeManager != null)
            {
                skillTreeManager.skillPoints = skillPoints;
                Debug.Log($"[PlayerStats] Se ha actualizado skillTreeManager.skillPoints a: {skillTreeManager.skillPoints}");
            }
            Debug.Log($"[PlayerStats] Se ha otorgado un punto de habilidad. Total: {skillPoints}");
        }

        for (int i = 0; i < 5; i++)
        {
            IncreaseStatBasedOnClass();
        }

        CurrentHealth = MaxHealth;
        CurrentMana = MaxMana;
        Debug.Log($"[PlayerStats] Subida de nivel completada para {characterName}. Salud actualizada: {CurrentHealth}/{MaxHealth}, Mana: {CurrentMana}/{MaxMana}");
        ChangePlayerData();
    }

    private void IncreaseStatBasedOnClass()
    {
        Debug.Log($"[PlayerStats] Incrementando estadística basada en la clase: {characterName}");
        switch (characterName.ToLower())
        {
            case "paladin":
                RandomIncreaseStat(new[] { "armor", "maxHealth", "attack" }, new[] { 0.4f, 0.4f, 0.2f });
                break;
            case "witch":
                RandomIncreaseStat(new[] { "magicPower", "maxMana", "maxHealth" }, new[] { 0.5f, 0.3f, 0.2f });
                break;
            case "cleric":
                RandomIncreaseStat(new[] { "attack", "maxHealth", "maxMana" }, new[] { 0.4f, 0.4f, 0.2f });
                break;
            case "thief":
                RandomIncreaseStat(new[] { "attack", "speed", "speed" }, new[] { 0.4f, 0.4f, 0.2f });
                break;
        }
    }

    private void RandomIncreaseStat(string[] stats, float[] probabilities)
    {
        float roll = UnityEngine.Random.value;
        float cumulative = 0f;
        Debug.Log($"[PlayerStats] Random roll: {roll}");
        for (int i = 0; i < stats.Length; i++)
        {
            cumulative += probabilities[i];
            if (roll <= cumulative)
            {
                Debug.Log($"[PlayerStats] Incrementando estadística: {stats[i]} (roll: {roll} <= cumulative: {cumulative})");
                IncreaseStat(stats[i]);
                return;
            }
        }
    }

    // -----------------------------
    // MÉTODOS DE MANEJO DE HABILIDADES
    // -----------------------------

    /// <summary>
    /// Carga las habilidades guardadas. Se intenta primero usar el SkillTree_Manager (si existe);
    /// en caso contrario se usa PlayerPrefs.
    /// </summary>
    public void LoadSkills()
    {
        string key = "SkillTree" + characterName; // La clave en PlayerPrefs
        string savedSkills = PlayerPrefs.GetString(key, ""); // Obtener las habilidades guardadas
        List<string> loadedSkills = new List<string>(savedSkills.Split(','));

        // Eliminar posibles strings vacíos
        loadedSkills.RemoveAll(s => string.IsNullOrEmpty(s));

        if (loadedSkills.Count > 0)
        {
            foreach (string skillName in loadedSkills)
            {
                AddSkill(skillName);
            }
            Debug.Log($" Habilidades cargadas en PlayerStats para {characterName}: {string.Join(", ", loadedSkills)}");
        }
        else
        {
            Debug.Log($" No se encontraron habilidades guardadas para {characterName} en PlayerPrefs.");
        }
    }

    /// <summary>
    /// Añade una habilidad al personaje, tanto en la lista de nombres (para guardado) como en la lista runtime de objetos Skill.
    /// </summary>
    public void AddSkill(string skillName)
    {
        if (!string.IsNullOrEmpty(skillName) && !learnedSkills.Any(skill => skill.Name == skillName))  // Verifica si la habilidad no está aprendida
        {
            // Buscar la habilidad en el array de SkillDescScript
            Skill skill = FindSkillByName(skillName);
            Debug.Log("skill :" + skill);
            if (skill != null)
            {
                learnedSkills.Add(skill);  // Añadir la habilidad completa a learnedSkills
                Debug.Log($"Se ha agregado la habilidad: {skillName} - Costo de Mana: {skill.ManaCost} - Tipo de Dado: {skill.DiceType}");
            }
            else
            {
                Debug.Log($"No se encontró la habilidad: {skillName}");
            }
        }
        else
        {
            Debug.Log($"Habilidad {skillName} ya estaba aprendida.");
        }
    }

    /// <summary>
    /// Carga las habilidades desde los ScriptableObjects (skillDataArray) basándose en los nombres guardados en learnedSkills.
    /// </summary>
    private void LoadSkillsFromScriptableObjects()
    {
        if (skillDataArray == null || skillDataArray.Length == 0)
        {
            Debug.LogError("? skillDataArray está vacío o no asignado en el Inspector.");
            return;
        }

        skills = new Skill[skillDataArray.Length];

        for (int i = 0; i < skillDataArray.Length; i++)
        {
            if (skillDataArray[i] == null || skillDataArray[i].Data == null)
            {
                Debug.LogError($"? Error en skillDataArray[{i}]. Verifica el Inspector.");
                continue;
            }

            if (learnedSkills.Any(skill => skill.Name == skillDataArray[i].Data.Name))  // ? Solo habilidades aprendidas
            {
                skills[i] = skillDataArray[i].Data;
                Debug.Log($"? Habilidad CARGADA: {skills[i].Name} - Costo de Mana: {skills[i].ManaCost} - Tipo de Dado: {skills[i].DiceType}");
            }
        }
    }

    /// <summary>
    /// Busca una habilidad en el array skillDataArray por su nombre.
    /// </summary>
    private Skill FindSkillByName(string skillName)
    {
        if (skillDataArray == null)
            return null;
        foreach (var skillDesc in skillDataArray)
        {
            if (skillDesc != null && skillDesc.Data != null && skillDesc.Data.Name == skillName)
            {
                return skillDesc.Data;
            }
        }
        return null;
    }

    // -----------------------------
    // REGIÓN DE GUARDADO/CARGA (NO MODIFICAR)
    // -----------------------------

    [Serializable]
    public class PlayerStatsData
    {
        public string saveId;         // Identificador global del save
        public string characterName;
        public int currentLevel;
        public int currentExperience;
        public int maxExperience;
        public int skillPoints;
        public int Attack;
        public int MagicPower;
        public int Armor;
        public float MaxHealth;
        public int Stamina;
        public int Speed;
        public float CurrentHealth;
        public float MaxMana;
        public List<Skill> learnedSkills;
        public bool isMagic;
    }

    /// <summary>
    /// Guarda los datos del personaje en un archivo exclusivo (en la carpeta "PlayerStatsSaves").
    /// </summary>
    public void SavePlayerStats()
    {
        // Obtén el identificador global del save; si no existe, usa "DefaultSave"
        string saveId = PlayerPrefs.GetString("SaveIdentifier", "DefaultSave");

        PlayerStatsData data = new PlayerStatsData
        {
            saveId = saveId,
            characterName = characterName,
            currentLevel = currentLevel,
            currentExperience = currentExperience,
            maxExperience = maxExperience,
            skillPoints = skillPoints,
            Attack = Attack,
            MagicPower = MagicPower,
            Armor = Armor,
            MaxHealth = MaxHealth,
            Stamina = Stamina,
            Speed = Speed,
            CurrentHealth = CurrentHealth,
            MaxMana = MaxMana,
            isMagic = isMagic
        };
        skillTreeManager.skillPoints = skillPoints;

        string json = JsonUtility.ToJson(data, true);
        string folder = Path.Combine(Application.persistentDataPath, "PlayerStatsSaves");
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        // El nombre del archivo incluye el saveId para diferenciar entre distintos saves
        string fileName = saveId + "_" + characterName + ".json";
        string filePath = Path.Combine(folder, fileName);
        File.WriteAllText(filePath, json);
        Debug.Log("[PlayerStats] Datos guardados en: " + filePath);
    }

    /// <summary>
    /// Versión original de carga (basada en PlayerPrefs).
    /// </summary>
    public bool LoadPlayerStats()
    {
        string saveId = PlayerPrefs.GetString("SaveIdentifier", "DefaultSave");
        string folder = Path.Combine(Application.persistentDataPath, "PlayerStatsSaves");
        string fileName = saveId + "_" + characterName + ".json";
        string filePath = Path.Combine(folder, fileName);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerStatsData data = JsonUtility.FromJson<PlayerStatsData>(json);
            if (data != null)
            {
                // Opcional: verificar que el saveId coincida con el actual
                if (!string.Equals(data.saveId, saveId, StringComparison.Ordinal))
                {
                    Debug.LogWarning("[PlayerStats] El saveId en los datos no coincide con el actual.");
                    return false;
                }
                currentLevel = data.currentLevel;
                currentExperience = data.currentExperience;
                maxExperience = data.maxExperience;
                skillPoints = data.skillPoints;
                Attack = data.Attack;
                MagicPower = data.MagicPower;
                Armor = data.Armor;
                MaxHealth = data.MaxHealth;
                Stamina = data.Stamina;
                Speed = data.Speed;
                CurrentHealth = data.MaxHealth;
                MaxMana = data.MaxMana;
                CurrentMana = data.MaxMana;
                isMagic = data.isMagic;
                Debug.Log("[PlayerStats] Datos cargados desde: " + filePath);
                skillTreeManager.skillPoints = data.skillPoints;
                return true;
            }
        }
        return false;
    }
}
