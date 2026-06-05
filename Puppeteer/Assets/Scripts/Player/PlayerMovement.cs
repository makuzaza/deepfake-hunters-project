using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb;
    private Vector2 input;
    private PlayerControls controls;

    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => input = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => input = Vector2.zero;
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
        controls.Disable();
    }

    void FixedUpdate()
    {
        // Movement
        rb.MovePosition(rb.position + input * speed * Time.fixedDeltaTime);

        // Speed always updates normally
        anim.SetFloat("Speed", input.sqrMagnitude);

        // If moving: update MoveX/MoveY and LastX/LastY
        if (input.sqrMagnitude > 0.01f)
        {
            anim.SetFloat("MoveX", input.x);
            anim.SetFloat("MoveY", input.y);

            anim.SetFloat("LastX", input.x);
            anim.SetFloat("LastY", input.y);
        }
        else
        {
            // If NOT moving: freeze MoveX/MoveY to last direction
            // This prevents the Walk Blend Tree from snapping to Walk Up
            anim.SetFloat("MoveX", anim.GetFloat("LastX"));
            anim.SetFloat("MoveY", anim.GetFloat("LastY"));
        }
    }
}
