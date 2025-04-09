using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnManager : MonoBehaviour
{
    // Clase auxiliar para almacenar la "foto" de los stats de un jugador
    [System.Serializable]
    public class CombatStatsSnapshot
    {
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

        public CombatStatsSnapshot(PlayerStats stats)
        {
            currentLevel = stats.currentLevel;
            currentExperience = stats.currentExperience;
            maxExperience = stats.maxExperience;
            skillPoints = stats.skillPoints;
            Attack = stats.Attack;
            MagicPower = stats.MagicPower;
            Armor = stats.Armor;
            MaxHealth = stats.MaxHealth;
            Stamina = stats.Stamina;
            Speed = stats.Speed;
            CurrentHealth = stats.CurrentHealth;
            MaxMana = stats.MaxMana;
        }
    }

    // Diccionario que almacena la snapshot inicial de cada PlayerStats
    private Dictionary<PlayerStats, CombatStatsSnapshot> initialPlayerStats = new Dictionary<PlayerStats, CombatStatsSnapshot>();

    // Clase auxiliar para el sistema de turnos
    public class TurnEntry
    {
        public CharacterStats character;
        public float nextTurnTime;

        public TurnEntry(CharacterStats character, float initialTime)
        {
            this.character = character;
            this.nextTurnTime = initialTime;
        }
    }

    private List<TurnEntry> turnEntries = new List<TurnEntry>();

    [Header("UI y Escena")]
    public TextMeshProUGUI turnIndicatorText; // Indicador del próximo turno
    public GameObject battleMenu;             // Menú de batalla (se activa o desactiva)

    [Header("Controladores y Scripts")]
    private BattleCameraController battleCameraController;
    private FleeBehaviour fleeBehaviour;
    public CanvasResize canvasResizeScript;

    // Evento que notifica el cambio de turno
    public event System.Action<CharacterStats> OnTurnChanged;

    [Header("Parámetros de Turnos")]
    [Tooltip("Tiempo base que se sumará al finalizar el turno (se dividirá por Speed).")]
    public float baseDelay = 10f;

    [SerializeField] private Camera secondaryPlayerCamera;
    [SerializeField] private Camera secondaryEnemyCamera;
    private SceneTransitionManager sceneTransitionManager;

    [Header("Combat Results UI")]
    public GameObject combatResultsCanvas;           // Canvas para mostrar los resultados del combate
    public List<TextMeshProUGUI> playerResultsTexts;   // Lista de textos para cada jugador (ej. 4)
    public CombatResultsUI combatResultsUI;            // Referencia al script de resultados

    // Audio
    public AudioSource audioSource;
    public AudioClip magicSound;
    public AudioClip physicalSound;

    public static int damage;
    void Awake()
    {
        battleCameraController = FindFirstObjectByType<BattleCameraController>();
        fleeBehaviour = FindFirstObjectByType<FleeBehaviour>();
    }
    private void Start()
    {
        sceneTransitionManager = FindFirstObjectByType<SceneTransitionManager>();
        if (sceneTransitionManager == null)
        {
            Debug.LogError("SceneTransitionManager no encontrado en la escena.");
        }

        // Buscar y asignar el AudioSource por nombre
        GameObject audioObject = GameObject.Find("Effects sounds");
        if (audioObject != null)
        {
            audioSource = audioObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogWarning(" El objeto 'Effects sounds' no tiene un componente AudioSource.");
            }
        }
        else
        {
            Debug.LogWarning(" No se encontró un objeto llamado 'Effects sounds' en la escena.");
        }
        StartCoroutine(InitializeTurnOrder());
    }

    // Inicializa el sistema de turnos y captura la snapshot inicial de cada PlayerStats
    private IEnumerator InitializeTurnOrder()
    {
        yield return new WaitForSeconds(1f); // Espera un frame para asegurar que todas las entidades estén listas

        CharacterStats[] allEntities = Object.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None);
        turnEntries.Clear();
        initialPlayerStats.Clear();
        // Imprime cuántas entidades se encontraron
        Debug.Log("Se encontraron " + allEntities.Length + " entidades con CharacterStats.");
        foreach (CharacterStats entity in allEntities)
        {
            if (entity is PlayerStats player)
            {
                // Se asume que PlayerStats ya ha cargado sus datos en Start()
                initialPlayerStats[player] = new CombatStatsSnapshot(player);
            }
            // Agregamos la entidad al orden de turnos
            turnEntries.Add(new TurnEntry(entity, 0f));
        }

        // Ordenamos los turnos según el tiempo (en este caso, todos inician en 0)
        turnEntries.Sort((a, b) => a.nextTurnTime.CompareTo(b.nextTurnTime));
        UpdateTurnIndicator();
        HandleTurnUI();


        if (turnEntries.Count > 0)
            OnTurnChanged?.Invoke(turnEntries[0].character);
    }

    public GameObject GetCurrentTurnEntity()
    {
        if (turnEntries.Count > 0)
            return turnEntries[0].character.gameObject;
        return null;
    }

    public List<CharacterStats> GetTurnOrder()
    {
        List<CharacterStats> list = new List<CharacterStats>();
        foreach (TurnEntry entry in turnEntries)
            list.Add(entry.character);
        return list;
    }

    public void EndTurn()
    {
        StartCoroutine(HandleEndTurn());
    }

    private IEnumerator HandleEndTurn()
    {
        if (turnEntries.Count == 0)
        {
            Debug.LogWarning("No hay entidades en el orden de turnos.");
            yield break;
        }

        GameObject currentEntity = GetCurrentTurnEntity();

        // 1️⃣ Movimiento de cámara inicial (si tiene una)
        if (currentEntity != null)
        {
            PlayerCameraController comp = currentEntity.GetComponentInChildren<PlayerCameraController>();
            if (comp != null)
            {
                yield return StartCoroutine(HandleCameraTransition(comp, currentEntity));
            }
        }

        // 2️⃣ Movimiento de cámara del enemigo (si aplica)
        if (secondaryEnemyCamera != null && currentEntity.GetComponent<EnemyStats>() != null)
        {
            yield return StartCoroutine(SwitchToSecondaryCamera(secondaryEnemyCamera));
        }

        AdvanceTurn();
    }

    private IEnumerator HandleCameraTransition(PlayerCameraController playerCamera, GameObject currentEntity)
    {
        yield return StartCoroutine(playerCamera.MoveCameraUp());

        // Identificar si es un jugador o un enemigo
        PlayerStats playerStats = currentEntity.GetComponent<PlayerStats>();
        EnemyStats enemyStats = currentEntity.GetComponent<EnemyStats>();

        // 1️⃣ Sonidos de ataque si hay daño
        if (damage > 0)
        {
            bool isMagic = playerStats != null ? playerStats.isMagic : (enemyStats != null && enemyStats.isMagic);

            if (isMagic)
            {
                if (audioSource != null && magicSound != null)
                    audioSource.PlayOneShot(magicSound);
                else
                    Debug.LogWarning("AudioSource o magicSound no están asignados.");
            }
            else
            {
                if (audioSource != null && physicalSound != null)
                    audioSource.PlayOneShot(physicalSound);
                else
                    Debug.LogWarning("AudioSource o physicalSound no están asignados.");
            }
        }

        // 2️⃣ Movimiento de cámara secundaria (si existe)
        if (secondaryPlayerCamera != null && enemyStats == null)
            yield return StartCoroutine(SwitchToSecondaryCamera(secondaryPlayerCamera));
        else
            yield return StartCoroutine(SwitchToSecondaryCamera(secondaryEnemyCamera));
    }


    private IEnumerator SwitchToSecondaryCamera(Camera cam)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
            mainCamera.gameObject.SetActive(false);

        cam.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        cam.gameObject.SetActive(false);
        if (mainCamera != null)
            mainCamera.gameObject.SetActive(true);
    }

    private void AdvanceTurn()
    {
        TurnEntry currentEntry = turnEntries[0];
        float delay = baseDelay / Mathf.Max(currentEntry.character.Speed, 1);
        currentEntry.nextTurnTime += delay;

        RemoveDeadEntities();
        turnEntries.Sort((a, b) => a.nextTurnTime.CompareTo(b.nextTurnTime));

        if (turnEntries.Count > 0)
            OnTurnChanged?.Invoke(turnEntries[0].character);

        UpdateTurnIndicator();
        HandleTurnUI();

        Debug.Log("Nuevo turno: " + GetCurrentTurnEntity()?.name);
        EndBattle();
    }

    public void RemoveDeadEntities()
    {
        turnEntries.RemoveAll(entry => entry.character.CurrentHealth <= 0);
    }

    private void UpdateTurnIndicator()
    {
        if (turnIndicatorText != null && turnEntries.Count > 0)
        {
            int nextIndex = (turnEntries.Count > 1) ? 1 : 0;
            turnIndicatorText.text = "Proximo turno: " + turnEntries[nextIndex].character.gameObject.name;
        }
        else
        {
            Debug.LogWarning("TurnIndicatorText no asignado o no hay entidades en el orden de turnos.");
        }
    }

    private void HandleTurnUI()
    {
        if (turnEntries.Count == 0)
            return;

        if (battleMenu == null)
            battleMenu = GameObject.FindWithTag("BattleMenu");

        if (turnEntries[0].character is EnemyStats)
        {
            if (battleMenu != null)
                battleMenu.SetActive(false);
            canvasResizeScript?.ToggleResize();
            ExecuteEnemyTurn(turnEntries[0].character as EnemyStats);
        }
        else
        {
            if (battleMenu != null)
                battleMenu.SetActive(true);
            if (battleCameraController != null)
                battleCameraController.ResetCameraPosition();
            canvasResizeScript?.ToggleResize();
        }
    }

    private void ExecuteEnemyTurn(EnemyStats enemyStats)
    {
        if (enemyStats == null)
            return;

        EnemyAI enemyAI = enemyStats.GetComponent<EnemyAI>();
        if (enemyAI != null)
            enemyAI.ExecuteEnemyTurn();
        else
            Debug.LogWarning("No se encontró la IA en el enemigo: " + enemyStats.gameObject.name);
    }

    // Al final del combate, si no quedan enemigos vivos, se delega en CombatResultsUI la presentación de los resultados.
    public void EndBattle()
    {
        bool playerAlive = false;
        bool enemyAlive = false;

        foreach (TurnEntry entry in turnEntries)
        {
            CharacterStats character = entry.character;
            if (character is PlayerStats && character.CurrentHealth > 0)
                playerAlive = true;
            if (character is EnemyStats && character.CurrentHealth > 0)
                enemyAlive = true;
        }

        if (!playerAlive)
        {
            Debug.Log("¡Los jugadores han perdido!");
            fleeBehaviour?.Flee();
            SceneTransitionManager.winnedReturn = false;
        }
        if (!enemyAlive)
        {
            Debug.Log("¡Los jugadores han ganado!");

            // Desactivar TODA la UI de combate
            if (battleMenu != null) battleMenu.SetActive(false);
            if (turnIndicatorText != null) turnIndicatorText.gameObject.SetActive(false);
            if (canvasResizeScript != null) canvasResizeScript.gameObject.SetActive(false);

            // Mostrar resultados de combate
            if (combatResultsUI != null)
            {
                combatResultsUI.DisplayAllCombatResults(initialPlayerStats, playerResultsTexts);
                if (combatResultsCanvas != null) combatResultsCanvas.SetActive(true);

                // Guardar datos de cada jugador
                foreach (PlayerStats player in initialPlayerStats.Keys)
                {
                    player.SavePlayerStats();
                }
            }
            else
            {
                Debug.LogWarning("CombatResultsUI no asignado.");
            }
        }
    }
    public void ExitButton()
    {
        SceneTransitionManager.winnedReturn = true;
        sceneTransitionManager.LoadExplorationScene();
    }

    // Métodos auxiliares para selección, fusiones, etc.
    public CharacterStats SelectPartyMember(string memberName)
    {
        foreach (TurnEntry entry in turnEntries)
        {
            if (entry.character.name.ToLower() == memberName.ToLower() && entry.character.CurrentHealth > 0)
            {
                Debug.Log($"Miembro seleccionado: {entry.character.name}");
                return entry.character;
            }
        }
        Debug.LogWarning("No se encontró un miembro válido con ese nombre.");
        return null;
    }

    private bool IsInTurnOrder(CharacterStats character)
    {
        foreach (TurnEntry entry in turnEntries)
        {
            if (entry.character == character)
                return true;
        }
        return false;
    }

    public void FusionTurn(CharacterStats partner)
    {
        if (turnEntries.Count == 0)
            return;

        CharacterStats activeCharacter = turnEntries[0].character;
        if (partner == activeCharacter || partner.CurrentHealth <= 0 || !IsInTurnOrder(partner))
        {
            Debug.Log("Fusión no válida.");
            return;
        }

        if (partner is PlayerStats playerStats)
        {
            string partnerName = playerStats.characterName.ToLower();
            if (partnerName.Contains("wicht") || partnerName.Contains("explorer"))
            {
                int baseDamage = activeCharacter.Attack + partner.Attack;
                int finalDamage = Mathf.FloorToInt(baseDamage * 1.5f);
                Debug.Log($"Efecto de Fusión: Aumento de Daño. Ataque combinado: {finalDamage} de daño.");
                activeCharacter.Attack = finalDamage;
            }
            else if (partnerName.Contains("paladin"))
            {
                Debug.Log("Efecto de Fusión: Los enemigos recibirán más daño.");
                ApplyEnemyDebuff();
            }
            else if (partnerName.Contains("cleric"))
            {
                Debug.Log("Efecto de Fusión: Reducción de Coste de Mana.");
                ReduceManaCost(activeCharacter);
            }
        }
        else
        {
            Debug.LogError("El compañero no es un PlayerStats.");
        }
    }

    private void ApplyEnemyDebuff()
    {
        foreach (TurnEntry entry in turnEntries)
        {
            if (entry.character is EnemyStats)
            {
                entry.character.Armor -= 10;
                Debug.Log($"{entry.character.gameObject.name} ahora tiene -10 de defensa durante este turno.");
            }
        }
    }

    private void ReduceManaCost(CharacterStats character)
    {
        foreach (Skill skill in character.skills)
        {
            skill.ManaCost = Mathf.FloorToInt(skill.ManaCost * 0.7f);
            Debug.Log($"Coste de maná reducido para la habilidad {skill.Name}.");
        }
    }
}
