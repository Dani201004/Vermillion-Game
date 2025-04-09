using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject freelookCamera;

    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private float footstepInterval = 0.5f;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isCursorLocked = true;
    private bool canMove = true; // Control de movimiento
    private CharacterController characterController;
    private Animator animator;
    private AudioSource audioSource;
    private float footstepTimer;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Solo procesamos el movimiento si se permite y el CharacterController está activo
        if (canMove && characterController.enabled)
        {
            MoveCharacter();
        }
        HandleCursorVisibilityToggle();
    }

    private void MoveCharacter()
    {
        // Verificar nuevamente por seguridad que el CharacterController esté activo
        if (!characterController.enabled)
            return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(moveX, 0f, moveZ).normalized;
        float currentSpeed = speed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= sprintMultiplier;
            if (animator != null)
            {
                animator.SetBool("running", true);
                animator.SetBool("walking", false);
            }
        }
        else
        {
            if (animator != null)
            {
                animator.SetBool("running", false);
                animator.SetBool("walking", true);
            }
        }

        if (direction.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveX, moveZ) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if (characterController.enabled)
            {
                characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
            }

            HandleFootsteps();
        }
        else
        {
            if (animator != null)
            {
                animator.SetBool("walking", false);
                animator.speed = 1.0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (animator != null)
            {
                animator.speed = 1.0f; // Aseguramos que la animación de salto se reproduzca a velocidad normal
                animator.SetTrigger("Jump");
            }
        }

        velocity.y += gravity * Time.deltaTime;
        if (characterController.enabled)
        {
            characterController.Move(velocity * Time.deltaTime);
        }
    }

    private void HandleFootsteps()
    {
        if (isGrounded && characterController.velocity.magnitude > 0.1f)
        {
            float currentFootstepInterval = Input.GetKey(KeyCode.LeftShift) ? footstepInterval / sprintMultiplier : footstepInterval;

            footstepTimer += Time.deltaTime;
            if (footstepTimer >= currentFootstepInterval)
            {
                audioSource.PlayOneShot(footstepSound);
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    private void HandleCursorVisibilityToggle()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCursorLocked = !isCursorLocked;

            if (isCursorLocked)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                freelookCamera.SetActive(true);
                canMove = true; // Habilitar movimiento
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                freelookCamera.SetActive(false);
                canMove = false; // Deshabilitar movimiento

                // Si se estaba reproduciendo alguna animación, se cancela y se reproduce la animación de Idle.
                if (animator != null)
                {
                    animator.SetBool("walking", false);
                    animator.SetBool("running", false);
                    animator.Play("Idle");
                }
            }
        }
    }
}
