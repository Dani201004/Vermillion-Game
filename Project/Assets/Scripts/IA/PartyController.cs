using System.Collections.Generic;
using UnityEngine;

public class PartyController : MonoBehaviour
{
    private string playerTag = "Player";
    private Transform player;

    // El array de nombres que se buscar�n en la escena
    [SerializeField] string[] partyMemberNames = new string[] { "Nombre1", "Nombre2", "Nombre3" };

    // Array que contendr� los Transforms de los compa�eros encontrados
    private Transform[] partyMembers;

    [SerializeField] float followDistance = 2.0f;
    [SerializeField] float acceleration = 5.0f;          // Para SmoothDamp en el movimiento
    [SerializeField] float rotationSmoothTime = 0.3f;    // Tiempo de suavizado para la rotaci�n
    [SerializeField] float formationSpacing = 2.0f;
    [SerializeField] float minimumDistance = 1.0f;       // Para evitar que se api�en
    [SerializeField] float randomnessFactor = 0.2f;      // Variaci�n individual en la respuesta

    // Par�metros para la separaci�n (steering behavior)
    [SerializeField] float separationWeight = 1.0f;
    [SerializeField] float separationThresholdMultiplier = 1.5f;

    // Par�metros para la evitaci�n de obst�culos
    [SerializeField] float obstacleAvoidanceRadius = 1.0f;
    [SerializeField] float obstacleAvoidanceWeight = 2.0f;
    [SerializeField] private LayerMask obstacleMask;

    // Par�metros para adaptar la altura al terreno
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastHeight = 10f;

    // Variables para el suavizado
    private Vector3[] velocities;
    private float[] rotationVelocities;  // Para SmoothDampAngle
    private Vector3 lastPlayerPosition;
    private float[] randomOffsets;

    // Variable para controlar el estado "idle"
    private bool isIdle = false;

    private void Start()
    {
        // Buscar al jugador usando su etiqueta
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found in scene!");
            enabled = false;
            return;
        }

        // Buscar autom�ticamente los compa�eros en la escena usando los nombres especificados
        List<Transform> partyMembersList = new List<Transform>();
        Transform[] allTransforms = FindObjectsOfType<Transform>();
        foreach (Transform t in allTransforms)
        {
            foreach (string name in partyMemberNames)
            {
                if (t.gameObject.name == name)
                {
                    partyMembersList.Add(t);
                    break;
                }
            }
        }

        if (partyMembersList.Count == 0)
        {
            Debug.LogWarning("No se encontraron compa�eros con alguno de los nombres especificados en la escena.");
        }
        partyMembers = partyMembersList.ToArray();

        // Inicializar arrays para el suavizado y variaciones individuales
        velocities = new Vector3[partyMembers.Length];
        rotationVelocities = new float[partyMembers.Length];
        randomOffsets = new float[partyMembers.Length];
        for (int i = 0; i < partyMembers.Length; i++)
        {
            randomOffsets[i] = Random.Range(1.0f - randomnessFactor, 1.0f + randomnessFactor);
        }
        lastPlayerPosition = player.position;
    }

    private void Update()
    {
        // Detectar la pulsaci�n de la tecla Control para alternar el estado idle
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isIdle = !isIdle;
        }

        if (isIdle)
        {
            // Si est� en estado idle, detenemos las actualizaciones de posici�n y forzamos la animaci�n idle.
            SetIdleAnimation();
        }
        else
        {
            // Si no, ejecutamos el comportamiento normal de seguimiento.
            FollowPlayer();
            lastPlayerPosition = player.position;
        }
    }

    // M�todo para forzar la animaci�n de reposo
    private void SetIdleAnimation()
    {
        foreach (Transform member in partyMembers)
        {
            Animator animator = member.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("walking", false);
                animator.speed = 1.0f;
            }
        }
    }

    private void FollowPlayer()
    {
        Vector3 forwardDirection = player.forward;
        Vector3 rightDirection = player.right;

        for (int i = 0; i < partyMembers.Length; i++)
        {
            Transform member = partyMembers[i];
            Animator animator = member.GetComponent<Animator>();

            // 1. Formaci�n: calcular posici�n deseada en formaci�n detr�s del jugador
            Vector3 formationOffset = rightDirection * (formationSpacing * (i - (partyMembers.Length - 1) / 2.0f))
                                        - forwardDirection * followDistance;
            Vector3 formationTarget = player.position + formationOffset;

            // 2. Separaci�n: calcular fuerza de separaci�n entre compa�eros
            Vector3 separationForce = Vector3.zero;
            for (int j = 0; j < partyMembers.Length; j++)
            {
                if (i == j) continue;
                Transform other = partyMembers[j];
                float dist = Vector3.Distance(member.position, other.position);
                if (dist < minimumDistance * separationThresholdMultiplier)
                {
                    separationForce += (member.position - other.position).normalized / dist;
                }
            }
            separationForce *= separationWeight;
            separationForce.y = 0; // Aplicar solo en XZ

            // 3. Combinar formaci�n y separaci�n
            Vector3 desiredPosition = formationTarget + separationForce;
            if (IsCollidingWithPlayer(member))
            {
                desiredPosition = member.position;
            }

            // 3.1 Evitar obst�culos: calcular fuerza repulsiva basada en obst�culos cercanos
            Vector3 obstacleAvoidanceForce = Vector3.zero;
            Collider[] obstacles = Physics.OverlapSphere(member.position, obstacleAvoidanceRadius, obstacleMask);
            foreach (Collider obs in obstacles)
            {
                Vector3 closestPoint = obs.ClosestPoint(member.position);
                Vector3 avoidanceDir = (member.position - closestPoint).normalized;
                float distance = Vector3.Distance(member.position, closestPoint);
                if (distance < obstacleAvoidanceRadius)
                {
                    obstacleAvoidanceForce += avoidanceDir * ((obstacleAvoidanceRadius - distance) / obstacleAvoidanceRadius);
                }
            }
            obstacleAvoidanceForce *= obstacleAvoidanceWeight;
            desiredPosition += obstacleAvoidanceForce;

            // 4. Suavizar el movimiento con SmoothDamp
            float smoothTime = (1.0f / acceleration) * randomOffsets[i];
            Vector3 newPosition = Vector3.SmoothDamp(member.position, desiredPosition, ref velocities[i], smoothTime);

            // 4.1 Adaptar la altura al terreno mediante raycast
            Vector3 rayOrigin = new Vector3(newPosition.x, newPosition.y + raycastHeight, newPosition.z);
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, raycastHeight * 2f, groundLayer))
            {
                newPosition.y = hit.point.y;
            }

            member.position = newPosition;

            // 5. Actualizar animaci�n seg�n el movimiento
            float distanceMoved = Vector3.Distance(member.position, desiredPosition);
            if (animator != null)
            {
                bool isWalking = distanceMoved > 0.01f;
                animator.SetBool("walking", isWalking);
                animator.speed = isWalking ? 1.0f + distanceMoved : 1.0f;
            }

            // 6. Rotaci�n suave usando SmoothDampAngle
            Vector3 lookDirection = (desiredPosition - member.position);
            lookDirection.y = 0;
            if (lookDirection.magnitude < 0.1f)
            {
                lookDirection = player.forward;
            }
            float targetAngle = Quaternion.LookRotation(lookDirection).eulerAngles.y;
            targetAngle = (targetAngle + 180f) % 360f;
            float currentAngle = member.rotation.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref rotationVelocities[i], rotationSmoothTime);
            member.rotation = Quaternion.Euler(0, smoothAngle, 0);
        }

        AvoidClumping();
    }

    private void AvoidClumping()
    {
        // Refuerzo extra de separaci�n para evitar que se api�en los compa�eros
        for (int i = 0; i < partyMembers.Length; i++)
        {
            for (int j = i + 1; j < partyMembers.Length; j++)
            {
                Transform memberA = partyMembers[i];
                Transform memberB = partyMembers[j];

                float dist = Vector3.Distance(memberA.position, memberB.position);
                if (dist < minimumDistance)
                {
                    Vector3 direction = (memberA.position - memberB.position).normalized;
                    Vector3 separation = direction * (minimumDistance - dist) * 0.5f;
                    separation.y = 0;
                    memberA.position += separation;
                    memberB.position -= separation;
                }
            }
        }
    }

    private bool IsCollidingWithPlayer(Transform member)
    {
        Collider memberCollider = member.GetComponent<Collider>();
        Collider playerCollider = player.GetComponent<Collider>();
        if (memberCollider != null && playerCollider != null)
        {
            return memberCollider.bounds.Intersects(playerCollider.bounds);
        }
        return false;
    }
}
