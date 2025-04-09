using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public CharacterStats attacker;
    public CharacterStats defender;

    public void ExecuteBasicAttack(bool useMagic)
    {
        int damage = attacker.CalculateBasicAttackDamage(useMagic);
        defender.TakeDamage(damage);

        Debug.Log($"{attacker.name} infligi� {damage} de da�o a {defender.name}");
    }
}
