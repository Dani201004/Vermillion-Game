using System.Collections;
using System.Collections.Generic; // Necesario para usar List
using System.Linq;
using UnityEngine;

public class BattleCameraController : MonoBehaviour
{
    public Transform player;
    public Transform cameraTransform;
    public float transitionSpeed = 2f;
    public float rotationSpeed = 50f;
    public float smoothReturnSpeed = 2f;
    public float upDownMovementSpeed = 0.2f;
    public float upDownAmplitude = 0.1f;

    private List<Transform> enemies = new List<Transform>();
    private int currentEnemyIndex = 0;
    private Transform targetEnemy;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private bool isLookingAtEnemy = false;
    private bool isRotating = false;
    private bool isReturningToOriginal = false;
    private GameObject tutorialsObject;
    private TurnManager turnManager;

    public GameObject combatMenuPrefab;
    [SerializeField] private GameObject currentCombatMenu;

    [Header("Camera Points")]
    public Transform[] lookTargets; // Objetivos que la cámara debe mirar
    private int currentCameraPointIndex = 0;
    private bool isMovingToPoint = false;

    private Transform customTargetPoint; // Punto de cámara especificado en tiempo real
    private Transform customLookTarget;  // Objetivo especificado en tiempo real

    // Objeto indicador para mostrar de quién es el turno actual (asignar en el Inspector, por ejemplo, la flecha 2D)
    public GameObject indicator;

    [Header("Turn Camera Animation")]
    [Tooltip("Duración del movimiento de cámara (en segundos)")]
    public float cameraTurnAnimationDuration = 1.0f;
    [Tooltip("Amplitud del movimiento vertical de la cámara")]
    public float cameraTurnAnimationAmplitude = 0.5f;

    private void Start()
    {
        turnManager = FindFirstObjectByType<TurnManager>();
        originalCameraPosition = cameraTransform.position;
        originalCameraRotation = cameraTransform.rotation;
        // Llama a ResetTargets después de 0.5 segundos (ajusta el tiempo según necesites)
        Invoke(nameof(ResetTargets), 0.9f);
    }

    public void ResetTargets()
    {
        enemies.Clear(); // Limpiamos la lista de enemigos
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy").Select(enemy => enemy.transform)); // Llenamos la lista

        // Imprimimos la cantidad de enemigos encontrados
        //Debug.Log($"Número de enemigos encontrados: {enemies.Count}");

        // Recorremos la lista e imprimimos el nombre de cada enemigo
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                Debug.Log("Enemigo: " + enemy.name);
            }
        }

        if (enemies.Count > 0)
        {
            targetEnemy = enemies[currentEnemyIndex];
            isLookingAtEnemy = false;
            isRotating = false;
        }
    }


    public void RemoveTarget(Transform enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);

            // Si el objetivo actual es el que se eliminó, actualizar el objetivo
            if (targetEnemy == enemy)
            {
                if (enemies.Count > 0)
                {
                    enemies.RemoveAll(e => e == null);

                    // Ajustamos el índice para que no se salga de los límites de la lista
                    currentEnemyIndex = Mathf.Min(currentEnemyIndex, enemies.Count - 1);

                    // Cambiar al siguiente enemigo en la lista
                    targetEnemy = enemies[currentEnemyIndex];
                }
                else
                {
                    // Si no hay más enemigos, establecemos el targetEnemy como null
                    targetEnemy = null;
                }
            }
        }
    }

    private void Update()
    {
        // Cambiar el objetivo enemigo
        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeTarget(-1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            ChangeTarget(1);
        }

        // Para pruebas, se puede usar la tecla R para resetear la cámara manualmente.
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCameraPosition();
        }

        if (isLookingAtEnemy && isRotating && targetEnemy != null)
        {
            RotateCameraTowardsEnemy();
        }

        // Transición hacia un punto específico (si se requiere)
        if (isMovingToPoint)
        {
            TransitionToCustomCameraPoint();
        }
        else
        {
            MoveCameraSlightly();
        }
        enemies.RemoveAll(e => e == null);

        // Recorre la lista de enemigos (se conserva la lógica original)
        foreach (var enemy in enemies)
        {
            if (enemy == null)
            {
                if (enemy == targetEnemy)
                {
                    ChangeTarget(-1);
                }
            }
        }
    }

    public void ChangeTarget(int direction)
    {
        if (enemies.Count == 0) return;

        currentEnemyIndex += direction;

        if (currentEnemyIndex < 0)
        {
            currentEnemyIndex = enemies.Count - 1;
        }
        else if (currentEnemyIndex >= enemies.Count)
        {
            currentEnemyIndex = 0;
        }

        targetEnemy = enemies[currentEnemyIndex];
        isLookingAtEnemy = true;
    }

    private void RotateCameraTowardsEnemy()
    {
        Vector3 directionToEnemy = targetEnemy.position - player.position;
        float angle = Mathf.Atan2(directionToEnemy.z, directionToEnemy.x) * Mathf.Rad2Deg;
        cameraTransform.RotateAround(player.position, Vector3.up, rotationSpeed * Time.deltaTime);
        cameraTransform.LookAt(targetEnemy);
    }

    public void ResetCameraPosition()
    {
        isReturningToOriginal = true;
        isRotating = false;
        isLookingAtEnemy = false;
        isMovingToPoint = false;

        // Posicionar la cámara en su posición y rotación originales de forma inmediata.
        cameraTransform.position = originalCameraPosition;
        cameraTransform.rotation = originalCameraRotation;

        // Configurar el indicador para mostrar el turno actual.
        if (turnManager != null)
        {
            GameObject currentEntity = turnManager.GetCurrentTurnEntity();
            if (currentEntity != null && indicator != null)
            {
                indicator.transform.SetParent(currentEntity.transform, false);
                indicator.transform.localPosition = new Vector3(0, 2f, 0);
                indicator.SetActive(true);
            }
            else if (indicator != null)
            {
                indicator.SetActive(false);
            }
        }

        ResetCombatMenu();
        // Se elimina la llamada a la coroutine de animación (CameraTurnAnimation)
    }

    private void MoveCameraSlightly()
    {
        float newY = originalCameraPosition.y + Mathf.Sin(Time.time * upDownMovementSpeed) * upDownAmplitude;
        cameraTransform.position = new Vector3(cameraTransform.position.x, newY, cameraTransform.position.z);
    }

    private void TransitionToCustomCameraPoint()
    {
        if (customTargetPoint == null || customLookTarget == null) return;

        // Transición de posición
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, customTargetPoint.position, transitionSpeed * Time.deltaTime);

        // Transición de rotación (mirando al objetivo)
        Vector3 directionToLookTarget = customLookTarget.position - cameraTransform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToLookTarget);
        cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, transitionSpeed * Time.deltaTime);

        // Finaliza la transición cuando está cerca del punto y la rotación está alineada
        if (Vector3.Distance(cameraTransform.position, customTargetPoint.position) < 0.1f &&
            Quaternion.Angle(cameraTransform.rotation, targetRotation) < 0.1f)
        {
            isMovingToPoint = false; // Transición finalizada
        }
    }

    public IEnumerator MoveToCameraPoint(Transform cameraPoint, Transform lookTarget)
    {
        float duration = 1.8f; // Duración del movimiento más lento
        float elapsedTime = 0.5f;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget.position - cameraPoint.position);

        while (elapsedTime < duration)
        {
            // Calcular el porcentaje (asegurando que no exceda 1.0)
            float t = Mathf.Clamp01(elapsedTime / duration);

            // Interpolación de posición y rotación
            transform.position = Vector3.Lerp(startPosition, cameraPoint.position, t);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Asegurar posición y rotación finales
        transform.position = cameraPoint.position;
        transform.rotation = targetRotation;

        yield return new WaitForSeconds(0.3f);
    }

    void ResetCombatMenu()
    {
        // Destruir el menú de combate actual si existe
        if (currentCombatMenu != null)
        {
            Destroy(currentCombatMenu);
        }

        currentCombatMenu = Instantiate(combatMenuPrefab, cameraTransform);
        currentCombatMenu.transform.localPosition = Vector3.zero;
    }

    public void FocusOnEnemy()
    {
        if (enemies.Count > 0)
        {
            ChangeTarget(1);
            isReturningToOriginal = false;
            isRotating = true;
            isLookingAtEnemy = true;
            if (tutorialsObject != null)
            {
                tutorialsObject.SetActive(true);
            }
        }
    }

    public void StopCameraRotation()
    {
        isRotating = false;
        isLookingAtEnemy = false;
    }

    // Seleccionador de enemigo y daño
    public Transform GetTarget()
    {
        return targetEnemy;
    }

    /// <summary>
    /// Este método debe llamarse cuando se acabe el turno del personaje.
    /// Ejecuta el reseteo de la cámara y la animación de movimiento vertical.
    /// </summary>
    public void OnTurnEnd()
    {

        ResetCameraPosition();
    }





}
