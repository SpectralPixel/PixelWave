using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float defaultMoveSpeed;
    private CharacterController cController;

    private Vector2 input;
    private Vector3 position;
    private Vector3 movement;
    private Vector3 velocity;

    private float moveSpeed;
    private float jumpHeight;
    private float gravity = -9.81f;
    private float fallSpeed;

    [Space]
    [SerializeField] private Transform cam;
    [SerializeField] private float lookLimit;
    [SerializeField] private float sensitivity;

    private Vector2 camRot = Vector2.zero; // camera rotation

    void Start()
    {
        cController = GetComponent<CharacterController>();
        moveSpeed = defaultMoveSpeed;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        fallSpeed = gravity * 2f;
        jumpHeight = moveSpeed / 3f;

        if (cController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        movement = transform.right * input.x + transform.forward * input.y;

        cController.Move(movement * moveSpeed * Time.fixedDeltaTime);

        velocity.y += fallSpeed * Time.fixedDeltaTime;
        cController.Move(velocity * Time.fixedDeltaTime);
    }

    public void MovePlayer(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (cController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
    }

    // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/QuickStartGuide.html
    public void MoveCamera(InputAction.CallbackContext context)
    {
        camRot += context.ReadValue<Vector2>() * sensitivity * Time.deltaTime;

        camRot.y = Mathf.Clamp(camRot.y, -lookLimit, lookLimit);

        cam.localEulerAngles = new Vector3(-camRot.y, 0, 0);
        transform.localEulerAngles = new Vector3(0, camRot.x, 0);
    }
}