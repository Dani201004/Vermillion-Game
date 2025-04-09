using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class CombatResultsUI : MonoBehaviour
{
    public void DisplayAllCombatResults(Dictionary<PlayerStats, TurnManager.CombatStatsSnapshot> snapshots, List<TextMeshProUGUI> resultsTexts)
    {
        // Orden predefinido de clases (en minúsculas para comparación)
        string[] classOrder = { "thief", "paladin", "cleric", "mage" };

        // Ordenamos los jugadores según el orden deseado.
        // Si el nombre del jugador no se encuentra en classOrder, se le asigna int.MaxValue para que se coloque al final.
        var orderedPlayers = snapshots
            .OrderBy(kvp =>
            {
                int idx = System.Array.IndexOf(classOrder, kvp.Key.characterName.ToLower());
                return idx < 0 ? int.MaxValue : idx;
            })
            .ToList();

        int index = 0;
        foreach (var kvp in orderedPlayers)
        {
            PlayerStats player = kvp.Key;
            TurnManager.CombatStatsSnapshot initialSnapshot = kvp.Value;

            int levelDiff = player.currentLevel - initialSnapshot.currentLevel;
            int expDiff = player.currentExperience - initialSnapshot.currentExperience;
            int spDiff = player.skillPoints - initialSnapshot.skillPoints;
            int attackDiff = player.Attack - initialSnapshot.Attack;
            int magicDiff = player.MagicPower - initialSnapshot.MagicPower;
            int armorDiff = player.Armor - initialSnapshot.Armor;
            float maxHealthDiff = player.MaxHealth - initialSnapshot.MaxHealth;
            int staminaDiff = player.Stamina - initialSnapshot.Stamina;
            int speedDiff = player.Speed - initialSnapshot.Speed;
            float maxManaDiff = player.MaxMana - initialSnapshot.MaxMana;

            string resultText = "";

            resultText += $"Nivel: {player.currentLevel}";
            if (levelDiff > 0)
                resultText += $" ( +{levelDiff} )";
            resultText += "\n";

            if (player.currentLevel > initialSnapshot.currentLevel)
                resultText += $"Exp: {player.currentExperience}/{player.maxExperience}\n";
            else
            {
                resultText += $"Exp: {player.currentExperience}/{player.maxExperience}";
                if (expDiff > 0)
                    resultText += $" ( +{expDiff} )";
                resultText += "\n";
            }

            resultText += $"Ataque: {player.Attack}";
            if (attackDiff > 0)
                resultText += $" ( +{attackDiff} )";
            resultText += "\n";

            resultText += $"Magia: {player.MagicPower}";
            if (magicDiff > 0)
                resultText += $" ( +{magicDiff} )";
            resultText += "\n";

            resultText += $"Defensa: {player.Armor}";
            if (armorDiff > 0)
                resultText += $" ( +{armorDiff} )";
            resultText += "\n";

            resultText += $"Salud Max: {player.MaxHealth}";
            if (maxHealthDiff > 0)
                resultText += $" ( +{maxHealthDiff} )";
            resultText += "\n";

            resultText += $"Mana Max: {player.MaxMana}";
            if (maxManaDiff > 0)
                resultText += $" ( +{maxManaDiff} )";
            resultText += "\n";

            resultText += $"Stamina: {player.Stamina}";
            if (staminaDiff > 0)
                resultText += $" ( +{staminaDiff} )";
            resultText += "\n";

            resultText += $"Velocidad: {player.Speed}";
            if (speedDiff > 0)
                resultText += $" ( +{speedDiff} )";
            resultText += "\n";

            if (index < resultsTexts.Count)
                resultsTexts[index].text = resultText;
            else
                Debug.Log(resultText);

            index++;
        }
    }

}
