using UnityEngine;

[System.Serializable]
public class Skill
{
    public string Name;            // --Nombre de la habilidad
    public float ScalingPercent;   // --Porcentaje de escalado (0.0 a 1.0)
    public bool UseMagic;          // --Si utiliza magia o ataque físico
    public string DiceType;        // --Tipo de dado ("D6", "D20", etc.)
    public int RequiredRoll;       // --Resultado mínimo necesario para acertar
    public int ManaCost;          // --Mana necesario para lanzar la habilidad

    public string attackAnimationTrigger;
    [SerializeField] public SkillEffect skillEffect;
}