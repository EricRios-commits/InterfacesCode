using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float velocidad = 10;
    private Rigidbody rb;
    private Vector3 movimiento;
    private bool jump = false;
    private InputAction moveAction;
    private InputAction jumpAction;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        moveAction = new InputAction();
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        jumpAction = new InputAction("Jump", binding: "<Keyboard>/space");
        jumpAction.performed += ctx => jump = true;

        moveAction.Enable();
        jumpAction.Enable();
    }

    void OnDisable()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
    }

    void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        movimiento = new Vector3(input.x, 0f, input.y);
    }

    void FixedUpdate()
    {
        rb.MovePosition(transform.position + movimiento * Time.deltaTime * velocidad);

        if (jump)
        {
            rb.AddForce(Vector3.up * velocidad, ForceMode.Impulse);
            jump = false;
        }
    }
}