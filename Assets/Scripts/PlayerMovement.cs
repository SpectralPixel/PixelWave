using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float defaultMoveSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravityMultiplier;

    private CharacterController cController;

    private Vector2 lookInput;
    private Vector2 camRot = Vector2.zero; // camera rotation

    private Vector2 moveInput;
    private Vector3 movement;
    private Vector3 velocity;

    private float moveSpeed;
    private float gravity = -9.81f;
    private float fallSpeed;

    [Space]
    [SerializeField] private Transform cam;
    [SerializeField] private float lookLimit;
    [SerializeField] private float sensitivity;

    void Start()
    {
        cController = GetComponent<CharacterController>();
        moveSpeed = defaultMoveSpeed;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        camRot += lookInput * sensitivity * Time.deltaTime;

        camRot.y = Mathf.Clamp(camRot.y, -lookLimit, lookLimit);

        cam.localEulerAngles = new Vector3(-camRot.y, 0, 0);
        transform.localEulerAngles = new Vector3(0, camRot.x, 0);
    }

    void FixedUpdate()
    {
        fallSpeed = gravity * gravityMultiplier;

        if (cController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        movement = transform.right * moveInput.x + transform.forward * moveInput.y;

        cController.Move(movement * moveSpeed * Time.fixedDeltaTime);

        velocity.y += fallSpeed * Time.fixedDeltaTime;
        cController.Move(velocity * Time.fixedDeltaTime);
    }

    public void MovePlayer(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();
    public void MoveCamera(InputAction.CallbackContext context) => lookInput = context.ReadValue<Vector2>();

    // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/QuickStartGuide.html
    public void Jump(InputAction.CallbackContext context)
    {
        if (cController.isGrounded && context.started)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
    }

}