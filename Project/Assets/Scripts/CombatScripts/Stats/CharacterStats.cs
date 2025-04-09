using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

public abstract class CharacterStats : MonoBehaviour, ICharacterStats
{
    // Stats principales //
    public int Attack { get; set; }
    public int MagicPower { get; set; }
    public int Armor { get; set; }
    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }
    public float CurrentMana { get; set; }
    public float MaxMana { get; set; }
    public int Stamina { get; set; }
    public int Speed { get; set; }

    // Animaciones básicas
    public string basicAnimation { get; private set; } = "Attack 1";
    public string basicAnimation2 { get; private set; } = "Attack 2";

    // Video Player //
    public VideoPlayer videoPlayer;
    public VideoClip[] videoClips1;
    public VideoClip[] videoClips2;

    // Eventos de cambio de salud y mana.
    public event System.Action OnHealthChanged;
    public event System.Action OnManaChanged;

    // Lista de habilidades.
    public Skill[] skills { get; set; }

    // Referencias a otros sistemas.
    private TurnManager turnManager;
    private BattleCameraController cameraController;
    [SerializeField] private SimpleConsole console;
    [SerializeField] private Animator playerAnimator;

    // Arrays de efectos VFX asignados manualmente en el inspector.
    public ParticleSystem[] lightEffect;
    public ParticleSystem[] slashEffect;
    public ParticleSystem[] explosionEffect;
    public ParticleSystem[] deathEffect;

    /// <summary>
    /// Inicializa las estadísticas del personaje y obtiene referencias de otros componentes.
    /// </summary>
    public void InitializeStats(int attack, int magicPower, int armor, int maxHealth, int maxMana, int stamina, int speed, Skill[] skills)
    {
        Attack = Mathf.Clamp(attack, 1, 1000);
        MagicPower = Mathf.Clamp(magicPower, 1, 5000);
        Armor = Mathf.Clamp(armor, 1, 500);
        MaxHealth = Mathf.Clamp(maxHealth, 1, 40000);
        CurrentHealth = MaxHealth;
        MaxMana = Mathf.Clamp(maxMana, 1, 700);
        CurrentMana = MaxMana;
        Stamina = Mathf.Clamp(stamina, 1, 700);
        Speed = Mathf.Clamp(speed, 1, 100);

        basicAnimation = "Attack 1";
        basicAnimation2 = "Attack 2";
        this.skills = skills;

        // Buscar referencias en la escena.
        turnManager = FindFirstObjectByType<TurnManager>();
        cameraController = FindFirstObjectByType<BattleCameraController>();
        console = FindFirstObjectByType<SimpleConsole>();
        videoPlayer = GameObject.FindWithTag("DiceMenu").GetComponent<VideoPlayer>();
    }

    public void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke();
    }

    private void Start()
    {
        // Buscar y asignar el Animator.
        Animator Animator = GetComponent<Animator>();
        if (Animator != null)
        {
            playerAnimator = Animator.GetComponent<Animator>();
            if (playerAnimator == null)
            {
                Debug.LogWarning("El objeto 'Effects sounds' no tiene un componente AudioSource.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto llamado 'Effects sounds' en la escena.");
        }

        // Se asume que los arrays de efectos ya están asignados manualmente en el inspector.

        // Si este objeto tiene el componente PlayerStats, se muestran algunos mensajes.
        PlayerStats playerStats = GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            Debug.Log($"?? PlayerStats encontrado, tiene {playerStats.learnedSkills.Count} habilidades.");
            Debug.Log($"? Copiadas {skills.Length} habilidades desde PlayerStats a CharacterStats.");
        }
        else
        {
            Debug.LogError("? No se encontró PlayerStats en el objeto.");
        }
    }

    /// <summary>
    /// Incrementa la estadística indicada.
    /// </summary>
    public void IncreaseStat(string stat)
    {
        switch (stat)
        {
            case "attack":
                Attack += 10;
                break;
            case "magicPower":
                MagicPower += 10;
                break;
            case "armor":
                Armor += 4;
                break;
            case "maxHealth":
                MaxHealth += 100;
                break;
            case "maxMana":
                MaxMana += 50;
                break;
            case "stamina":
                Stamina++;
                break;
            case "speed":
                if (Speed < 50)
                {
                    Speed += 2;
                }
                break;
        }
    }

    /// <summary>
    /// Método principal para aplicar daño, reproducir efectos y procesar la muerte.
    /// Se utiliza el parámetro effectType ("physical", "magic" o "fire") para reproducir el VFX adecuado.
    /// </summary>
    public virtual void TakeDamage(int damage, string effectType)
    {
        float armorReduction = Mathf.Clamp01((float)Armor / 500f);
        int finalDamage = Mathf.RoundToInt(damage * (1f - armorReduction));
        CurrentHealth = Mathf.Max(CurrentHealth - finalDamage, 0);
        OnHealthChanged?.Invoke();
        Debug.Log($"{gameObject.name} recibió {finalDamage} de daño. Vida restante: {CurrentHealth}");

        // Ejecutar efecto de recibir daño si aún no ha muerto y se especificó un efecto.
        if (CurrentHealth > 0 && !string.IsNullOrEmpty(effectType))
        {
            PlayDamageEffect(effectType);
        }

        // Mostrar daño en la UI.
        CharacterUI charUI = GetComponentInChildren<CharacterUI>();
        if (charUI != null)
        {
            charUI.ShowDamage(finalDamage);
        }

        // Procesar muerte.
        if (CurrentHealth == 0)
        {
            // Reproducir efectos de muerte.
            if (deathEffect != null)
            {
                foreach (var effect in deathEffect)
                {
                    if (effect != null)
                    {
                        effect.Play();
                    }
                }
            }

            if (turnManager != null)
                turnManager.RemoveDeadEntities();

            EnemyStats enemy = this as EnemyStats;
            if (enemy != null)
                enemy.EnemyDie();

            if (cameraController != null)
            {
                cameraController.RemoveTarget(transform);
                if (cameraController.GetTarget() == transform)
                    cameraController.ChangeTarget(1);
            }
            StartCoroutine(DeathEffectPlay());
        }
    }

    private IEnumerator DeathEffectPlay()
    {
        // Reproducir efectos de muerte.
        if (deathEffect != null)
        {
            foreach (var effect in deathEffect)
            {
                if (effect != null)
                {
                    effect.Play();
                }
            }

            // Espera hasta que ninguno de los efectos esté reproduciéndose.
            yield return new WaitUntil(() =>
            {
                foreach (var effect in deathEffect)
                {
                    if (effect != null && effect.isPlaying)
                    {
                        return false;
                    }
                }
                return true;
            });
        }

        // Una vez terminados los efectos, destruir el objeto.
        Destroy(gameObject);
    }

    /// <summary>
    /// Sobrecarga para cumplir con la interfaz ICharacterStats.
    /// Llama a la versión extendida sin especificar efecto.
    /// </summary>
    public void TakeDamage(int damage)
    {
        TakeDamage(damage, "");
    }

    /// <summary>
    /// Ejecuta el efecto de partículas correspondiente al tipo de daño recibido.
    /// </summary>
    /// <param name="effectType">"physical", "magic" o "fire"</param>
    private void PlayDamageEffect(string effectType)
    {
        ParticleSystem[] effectsToPlay = null;
        switch (effectType)
        {
            case "physical":
                effectsToPlay = slashEffect;
                break;
            case "magic":
                effectsToPlay = lightEffect;
                break;
            case "fire":
                effectsToPlay = explosionEffect;
                break;
            default:
                Debug.Log("Tipo de efecto no reconocido: " + effectType);
                break;
        }

        if (effectsToPlay != null)
        {
            foreach (var effect in effectsToPlay)
            {
                if (effect != null)
                {
                    effect.Play();
                }
            }
        }
    }

    /// <summary>
    /// Muestra mensajes en consola con un retraso.
    /// </summary>
    private IEnumerator ShowConsoleWithDelay(string hitText, string damageText, string nameText)
    {
        yield return new WaitForSeconds(0.5f);
        if (console != null)
        {
            console.ShowConsole();
            console.UpdateHitText(hitText);
            console.UpdateDamageText(damageText);
            console.UpdateNameText(nameText);
        }
    }

    /// <summary>
    /// Calcula el daño de un ataque básico (físico o mágico) y activa la animación.
    /// Se encarga únicamente de calcular daño y ejecutar animación, sin reproducir efectos.
    /// </summary>
    public int CalculateBasicAttackDamage(bool useMagic)
    {
        int damage = 0;
        if (useMagic)
        {
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger(basicAnimation);
            }
            else
            {
                Debug.Log("Animator nulo");
            }
            damage = Mathf.RoundToInt(MagicPower * 0.8f); // 80% del poder mágico.
            Debug.Log($"{gameObject.name} realiza un ataque básico mágico causando {damage} de daño.");
        }
        else
        {
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger(basicAnimation2);
            }
            damage = Attack; // 100% del ataque físico.
            Debug.Log($"{gameObject.name} realiza un ataque básico físico causando {damage} de daño.");
        }
        TurnManager.damage = damage;
        return damage;
    }

    /// <summary>
    /// Ejecuta el uso de una habilidad indicada por su índice.
    /// Se verifica que haya suficiente mana, se lanza un dado y se calcula el daño escalado.
    /// La reproducción de efectos se delega al objetivo al recibir daño (se pasa effectType).
    /// </summary>
    public int UseSkill(int skillIndex, bool useMagic)
    {
        string[] physicalAbilities = { "Baluarte de Justicia", "Empujón", "Golpe de Escudo", "Acometida", "Acometida2", "Ataque desde las sombras", "Daga Envenenada", "Danza de sombras", "Degüello", "Finta", "Golpe Contundente", "Instinto asesino", "Puñalada Trapera", "Frenzy", "Cleaver Strike", "Fury Strike", "Sticky attack", "Serious Headbutt", "Headbutt", "Mushroom attack", "Vicious Headbutt", "Mycelus attack", "Infinite Hunger", "Harsh Bite" };

        string[] fireAbilities = { "Bola de Fuego", "Cometa Arcano", "Fuego Feerico", "Ignis", "Unknown gas", "Amaenita Caesara", "Fungus Wrath", "Plea of the Grimy Monarch" };

        string[] lightAbilities = { "Castigo Divino", "Espada Celestial", "Canalización Estelar", "Hipnosis", "Goblin bolt", "Mushroom trouble", "Sovereign Splash" };

        if (skillIndex < 0 || skillIndex >= skills.Length)
        {
            Debug.LogWarning("Índice de habilidad inválido en " + gameObject.name);
            return 0;
        }

        Skill selectedSkill = skills[skillIndex];
        Debug.Log($"{gameObject.name} usa la habilidad: {selectedSkill.Name}");

        if (CurrentMana < selectedSkill.ManaCost)
        {
            Debug.Log($"{gameObject.name} no tiene suficiente mana para usar {selectedSkill.Name}.");
            StartCoroutine(ShowConsoleWithDelay("Fallo...", "", gameObject.name));
            return 0;
        }

        // Descontar el mana y notificar el cambio.
        CurrentMana -= selectedSkill.ManaCost;
        Debug.Log($"{gameObject.name} gasta {selectedSkill.ManaCost} de mana. Mana restante: {CurrentMana}");
        OnManaChanged?.Invoke();

        // Lanzar el dado y actualizar la UI.
        int diceResult = RollDice(selectedSkill.DiceType);
        ChangeResult.ChangeTextResult(diceResult.ToString());
        Debug.Log($"{gameObject.name} obtuvo {diceResult} en el dado (necesitaba: {selectedSkill.RequiredRoll}).");

        PlayDiceVideo(selectedSkill.DiceType, diceResult);

        if (diceResult < selectedSkill.RequiredRoll)
        {
            Debug.Log($"{selectedSkill.Name} falló en {gameObject.name}.");
            StartCoroutine(ShowConsoleWithDelay("Fallo...", "", gameObject.name));
            return 0;
        }

        // Determinar el tipo de efecto para que el objetivo lo reproduzca al recibir daño.
        string effectType = "";
        if (physicalAbilities.Contains(selectedSkill.Name))
        {
            effectType = "physical";
        }
        else if (fireAbilities.Contains(selectedSkill.Name))
        {
            effectType = "fire";
        }
        else if (lightAbilities.Contains(selectedSkill.Name))
        {
            effectType = "magic";
        }
        else
        {
            Debug.Log("No se encontró un sistema de partículas específico para " + selectedSkill.Name);
        }

        // Calcular el daño utilizando la estadística correspondiente.
        int baseStat = useMagic ? MagicPower : Attack;
        int skillDamage = Mathf.RoundToInt(baseStat * selectedSkill.ScalingPercent);

        Debug.Log($"{selectedSkill.Name} de {gameObject.name} impacta causando {skillDamage} de daño.");
        StartCoroutine(ShowConsoleWithDelay("Diana!", $"{selectedSkill.Name}: {skillDamage}", gameObject.name));

        // Ejecutar efectos adicionales propios de la habilidad, si existen.
        if (selectedSkill.skillEffect != null)
        {
            selectedSkill.skillEffect.ExecuteEffects(this);
        }
        TurnManager.damage = skillDamage;

        // El efecto correspondiente se reproducirá en el objetivo al recibir el daño,
        // pasando 'effectType' como parámetro en su método TakeDamage.
        return skillDamage;
    }

    /// <summary>
    /// Reproduce el video correspondiente al dado.
    /// </summary>
    private void PlayDiceVideo(string diceType, int result)
    {
        VideoClip[] clips = null;
        if (diceType == "D6")
            clips = videoClips1;
        else if (diceType == "D20")
            clips = videoClips2;

        if (clips != null && result > 0 && result <= clips.Length)
        {
            videoPlayer.clip = clips[result - 1];
            Debug.Log("Video mostrado");
        }
        else
        {
            Debug.LogWarning("No hay video asignado para este resultado.");
            return;
        }

        videoPlayer.Play();
    }

    /// <summary>
    /// Lanza un dado según el tipo y retorna el resultado.
    /// </summary>
    private int RollDice(string diceType)
    {
        int result = 0;
        switch (diceType)
        {
            case "D6":
                result = Random.Range(1, 6);
                if (videoClips1 != null && videoClips1.Length >= result)
                {
                    videoPlayer.clip = videoClips1[result - 1];
                    videoPlayer.Play();
                }
                else
                {
                    Debug.LogWarning("videoClips1 no configurado correctamente en " + gameObject.name);
                }
                StartCoroutine(HandleDiceRoll());
                break;

            case "D20":
                result = Random.Range(1, 20);
                if (videoClips2 != null && videoClips2.Length >= result)
                {
                    videoPlayer.clip = videoClips2[result - 1];
                    videoPlayer.Play();
                }
                else
                {
                    Debug.LogWarning("videoClips2 no configurado correctamente en " + gameObject.name);
                }
                StartCoroutine(HandleDiceRoll());
                break;

            default:
                Debug.LogWarning($"Tipo de dado '{diceType}' no reconocido en {gameObject.name}.");
                break;
        }
        return result;
    }

    /// <summary>
    /// Mantiene visible el contenedor del dado durante un tiempo determinado.
    /// </summary>
    private IEnumerator HandleDiceRoll()
    {
        yield return new WaitForSeconds(2f);
    }

    /// <summary>
    /// Retorna la cantidad de habilidades disponibles.
    /// </summary>
    public int GetSkillCount()
    {
        return skills.Length;
    }


}
