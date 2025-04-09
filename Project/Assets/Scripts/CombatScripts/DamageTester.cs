using UnityEngine;

public class DamageTester : MonoBehaviour
{
    private PlayerStats playerStats;

    private void Start()
    {
        // Encuentra al jugador en la escena
        playerStats = FindFirstObjectByType<PlayerStats>();
    }

    private void Update()
    {
        // Simula daño al presionar la tecla "D"
        if (Input.GetKeyDown(KeyCode.P) && playerStats != null)
        {
            playerStats.TakeDamage(2900);
        }
    }
}
