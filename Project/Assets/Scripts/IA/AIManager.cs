using UnityEngine;
using UnityEngine.AI;

public class AIManager : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    private CharacterMovement characterMovement;
    [SerializeField] Transform[] objective;
    [SerializeField] GameObject player;
    [SerializeField] Animator animator;

    [SerializeField] bool detected = false;
    [SerializeField] float speed = 2f;
    [SerializeField] float detectAngle = 45f;
    [SerializeField] float detectDistance = 25f;

    [SerializeField] AudioClip footstepSound; // Sonido de pasos
    [SerializeField] float footstepInterval = 0.5f; // Velocidad de los pasos
    private AudioSource audioSource;
    private float lastStepTime = 0f; // Tiempo del último sonido de paso

    private Vector3 goal;
    private bool isWalking;
    private float timePast;
    private float timeReset;
    private float maxTime = 4f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        agent.speed = speed;
        Invoke("StartRound", 2f);

        player = GameObject.FindWithTag("Player");


        if (player != null)
        {
            characterMovement = player.GetComponent<CharacterMovement>();
        }

        timeReset = Time.time;
        isWalking = false;
        detected = false;
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                characterMovement = player.GetComponent<CharacterMovement>();
            }
        }

        agent.SetDestination(goal);
        animator.SetBool("isWalking", isWalking);

        DetectPlayer();
        UpdateStatus();
        UpdateSpeed();
        PlayFootsteps();

        timePast = Time.time - timeReset;
    }

    void UpdateStatus()
    {
        if (detected)
        {
            goal = player.transform.position;
            isWalking = true;
        }
        else if (!isWalking && timePast >= 3)
        {
            StartRound();
        }
        else if (isWalking && (timePast > 3f || timePast > maxTime))
        {
            StopRound();
        }
    }

    void UpdateSpeed()
    {
        agent.speed = isWalking ? speed : 0f;
    }

    void StartRound()
    {
        if (!detected)
        {
            int n = Random.Range(0, objective.Length);
            isWalking = true;
            goal = objective[n].position;
            timeReset = Time.time;
        }
    }

    void StopRound()
    {
        isWalking = false;
        float interval = Random.Range(2f, 6f);
        Invoke("StartRound", interval);
    }

    void DetectPlayer()
    {
        if (player != null)
        {
            float distPlayer = Vector3.Distance(transform.position, player.transform.position);
            Vector3 vectorToPlayer = player.transform.position - transform.position;
            float angleToPlayer = Vector3.Angle(transform.forward, vectorToPlayer.normalized);

            detected = distPlayer < detectDistance && angleToPlayer < detectAngle;
            if (detected) goal = player.transform.position;
        }
        else
        {
            player = GameObject.FindWithTag("Player");
        }
    }

    void PlayFootsteps()
    {
        if (isWalking && footstepSound != null && audioSource != null)
        {
            if (Time.time - lastStepTime > footstepInterval)
            {
                audioSource.PlayOneShot(footstepSound);
                lastStepTime = Time.time; // Actualiza el tiempo del último sonido
            }
        }
    }
}
