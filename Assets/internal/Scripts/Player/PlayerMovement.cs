using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{

    private Animator animator;
    private CharacterController controller;
    private PlayerInput input;


    [Header("Config")]
    [SerializeField] private Transform camTarget;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private AudioListener listener;
    [SerializeField] private Camera mainCamera;

    [Header("Camera")]
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float topAngleMax = 70f;
    [SerializeField] private float bottomAngleMax = -30f;
    float currentVelocity;

    [Header("speed")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 5f;

    [Header("Interact")]
    [SerializeField] private float interactDistance = 5f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private float jumpHeight = 8f;
    [SerializeField] private float awaitGravity = 2f;
    bool useGravity = false;


    Vector3 velocity;
    float rotateXAxis = 0f;
    float rotateYAxis = 0f;
    bool isGround = false;
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            virtualCamera.Priority = 1;
            listener.enabled = true;
        }
        else
        {
            virtualCamera.Priority = 0;
            listener.enabled = false;
        }
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
        Invoke(nameof(ActiveUseGravity), awaitGravity);
    }
    private void ActiveUseGravity()
    {
        useGravity = true;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (CursorController.instance != null)
        {
            if (!CursorController.instance.CursorFree())
            {
                return;
            }
        }

        Movement(input.GetOnFoot().Movement.ReadValue<Vector2>());
        Rotation(input.GetOnFoot().MouseRotate.ReadValue<Vector2>());

        if (input.GetOnFoot().Jump.triggered)
        {
            Jump();
        }

        Interact();
    }
    private void LateUpdate()
    {
        if (!IsOwner) return;

        Gravity();
    }
    public void Movement(Vector2 inputValue)
    {
        Vector3 moveDir = new Vector3(inputValue.x, 0f, inputValue.y).normalized;
        float speed = moveDir.magnitude >= 0.1f ? walkSpeed : 0f;
        if (input.GetOnFoot().Run.IsPressed())
        {
            speed = runSpeed;
        }
        animator.SetFloat("Speed", speed);
        if (moveDir.magnitude >= 0.1f)
        {

            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + listener.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, smoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            controller.Move(moveDirection * speed * Time.deltaTime);
        }

    }
    private float ClamAngle(float angle, float minValue, float maxValue)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, minValue, maxValue);
    }
    public void Rotation(Vector2 input)
    {
        Vector3 mouseDir = new(input.x * mouseSensitivity * Time.deltaTime, input.y * mouseSensitivity * Time.deltaTime, 0f);

        rotateYAxis += mouseDir.x;
        rotateXAxis += mouseDir.y;

        rotateXAxis = ClamAngle(rotateXAxis, bottomAngleMax, topAngleMax);
        rotateYAxis = ClamAngle(rotateYAxis, float.MinValue, float.MaxValue);

        camTarget.rotation = Quaternion.Euler(rotateXAxis, rotateYAxis, 0f);
    }
    public void Interact()
    {
        if (InteractController.instance == null)
        {
            return;
        }
        InteractController.instance.ChangeTextView(string.Empty);
        Vector2 centerPoint = new(Screen.width / 2f, Screen.height / 2f);
        Ray ray = mainCamera.ScreenPointToRay(centerPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.TryGetComponent<Interactible>(out var item))
            {
                InteractController.instance.ChangeTextView(item.promptMessage);
            }
        }
    }
    public Camera GetMainCamera()
    {
        return mainCamera;
    }
    private void Gravity()
    {
        if (!useGravity)
        {
            return;
        }
        isGround = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);
        velocity.y += gravity * Time.deltaTime;
        if (isGround && velocity.y <= 0f)
        {
            velocity.y = -2f;
        }
        controller.Move(velocity * Time.deltaTime);
    }
    private void Jump()
    {
        if (isGround)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
