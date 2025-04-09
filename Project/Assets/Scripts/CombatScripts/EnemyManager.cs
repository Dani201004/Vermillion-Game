using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private SceneDependentToggle sceneDependentToggle;

    // Prefabs de enemigos y puntos de aparición
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;

    // Prefabs de jefes
    public GameObject goblinBossPrefab;
    public GameObject slimeBossPrefab;
    public GameObject mushroomBossPrefab;

    // Contador de enemigos y listas para controlar los spawnPoints usados y los enemigos instanciados
    public int enemyCount = 0;
    private List<int> usedSpawnIndices = new List<int>();
    private List<GameObject> randomEnemies = new List<GameObject>();

    // Variables públicas para controlar la generación
    // En la escena de combate asegúrate de que spawnRandomEnemies esté en true y isBossFight en false
    public bool spawnRandomEnemies = true;
    public bool isBossFight = false;

    private void Awake()
    {
        EnemyManager[] managers = FindObjectsOfType<EnemyManager>();

        if (managers.Length > 1)
        {
            // Encuentra el más antiguo y lo destruye
            EnemyManager oldest = managers[0];
            foreach (EnemyManager manager in managers)
            {
                if (manager != this && manager.gameObject.scene.buildIndex < oldest.gameObject.scene.buildIndex)
                {
                    oldest = manager;
                }
            }
            Destroy(oldest.gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        // Invoca InitializeEnemies con un pequeño retraso para asegurarse de que los spawnPoints estén asignados
        Invoke(nameof(InitializeEnemies), 0.5f);
    }

    public void InitializeEnemies()
    {
        //Debug.Log($"EnemyManager iniciado con spawnRandomEnemies: {spawnRandomEnemies}, isBossFight: {isBossFight}");

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        // Solo genera enemigos aleatorios si spawnRandomEnemies es true y no es pelea de jefe
        if (spawnRandomEnemies && !isBossFight)
        {
            int numberOfEnemies = Random.Range(1, spawnPoints.Length + 1);
            //Debug.Log($"Generando {numberOfEnemies} enemigos aleatorios.");

            for (int i = 0; i < numberOfEnemies; i++)
            {
                SpawnEnemy(i);
            }
        }
    }

    void SpawnEnemy(int index)
    {
        // Buscar el objeto que tiene SceneDependentToggle
        SceneDependentToggle sceneDependentToggle = FindObjectOfType<SceneDependentToggle>();
        if (sceneDependentToggle == null)
        {
            Debug.LogWarning("No se encontró un objeto con SceneDependentToggle en la escena.");
            return;
        }

        if (isBossFight)
        {
            return; // No se generan enemigos normales durante pelea de jefe
        }

        if (!sceneDependentToggle.toggleState)
        {
            // Si togglestate es falso, se genera solo un único enemigo y se ignoran llamadas adicionales.
            if (enemyCount > 0)
                return;

            int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            if (enemyPrefab == null)
            {
                Debug.LogError("Enemy prefab is null!");
                return;
            }
            Transform spawnPoint = spawnPoints[randomSpawnIndex];

            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            enemy.name = "Enemy_" + index;
            enemy.transform.rotation = Quaternion.Euler(0, 180, 0);

            enemyCount++;
            randomEnemies.Add(enemy);
        }
        else
        {
            // En este caso, se generan varios enemigos.
            int spawnIndex = GetUnusedSpawnIndex();
            if (spawnIndex == -1)
            {
                Debug.LogError("No available spawn points left!");
                return;
            }

            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            if (enemyPrefab == null)
            {
                Debug.LogError("Enemy prefab is null!");
                return;
            }

            Transform spawnPoint = spawnPoints[spawnIndex];

            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            enemy.name = "Enemy_" + index;
            enemy.transform.rotation = Quaternion.Euler(0, 180, 0);

            usedSpawnIndices.Add(spawnIndex);
            enemyCount++;

            randomEnemies.Add(enemy);
        }
    }



    int GetUnusedSpawnIndex()
    {
        List<int> availableIndices = new List<int>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (!usedSpawnIndices.Contains(i))
            {
                availableIndices.Add(i);
            }
        }

        if (availableIndices.Count == 0)
        {
            Debug.LogError("No hay puntos de spawn disponibles");
            return -1;
        }

        int selectedIndex = availableIndices[Random.Range(0, availableIndices.Count)];
        //Debug.Log($"Seleccionando spawn point: {selectedIndex}");
        return selectedIndex;
    }

    // Métodos para generar jefes específicos
    public void SpawnGoblinBoss() { SpawnBoss(goblinBossPrefab, "Goblin_Boss"); }
    public void SpawnSlimeBoss() { SpawnBoss(slimeBossPrefab, "Slime_Boss"); }
    public void SpawnMushroomBoss() { SpawnBoss(mushroomBossPrefab, "Mushroom_Boss"); }

    public void SpawnBoss(GameObject bossPrefab, string bossName)
    {
        if (spawnPoints.Length < 3)
        {
            Debug.LogError("El array de spawnPoints no contiene al menos 3 elementos.");
            return;
        }

        // Destruye los enemigos aleatorios ya generados
        foreach (GameObject enemy in randomEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        randomEnemies.Clear();
        usedSpawnIndices.Clear();

        Transform spawnPoint = spawnPoints[2];

        GameObject boss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
        boss.name = bossName;
        boss.transform.rotation = Quaternion.Euler(0, 180, 0);

        enemyCount = 1;
        usedSpawnIndices.Add(System.Array.IndexOf(spawnPoints, spawnPoint));

        //Debug.Log("Se ha generado el jefe: " + bossName);
    }

    public int GetEnemyCount()
    {
        return enemyCount;
    }
    public void ReinitializeEnemies()
    {
        // Destruir los enemigos existentes
        foreach (GameObject enemy in randomEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        randomEnemies.Clear();
        usedSpawnIndices.Clear();
        enemyCount = 0;

        // Vuelve a generar enemigos aleatorios
        InitializeEnemies();
    }
}