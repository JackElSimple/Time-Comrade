using System.Collections.Generic;
using UnityEngine;
using static OpitControllerRewind;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class CloneController : MonoBehaviour
{
    [Header("Referencias Visuales")]
    [SerializeField] private SpriteRenderer characterSprite;

    [Header("Configuraci?n de Movimiento")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Fisicas de Salto")]
    [SerializeField] private float gravityScale = 3f;      // Gravedad base
    [SerializeField] private float fallMultiplier = 1.5f;   // Cae mas rapido de lo que sube
    [SerializeField] private float lowJumpMultiplier = 2f; // Salto corto si sueltas rapido el espacio

    [Header("Deteccion de Suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private bool wantsToJump;

    private List<PlayerInputFrame> recordedInputs = new List<PlayerInputFrame>();
    private int frameNumber;
    private Vector3 initialPosition; //the position of the player when the rewind  is pushed
    private Vector3 initialVelocity; //the vector of movement of the player when the rewind is pushed
    private PlayerInputFrame frame;

    void Awake()
    {
        Debug.Log("Clon despertado");
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if(frameNumber >= recordedInputs.Count)
        {
            transform.position= initialPosition;
            rb.linearVelocity = initialVelocity;
            frameNumber= 0;
        }
        // Inputs
        //horizontalInput = Input.GetAxisRaw("Horizontal"); // A,D
        frame = recordedInputs[frameNumber];


        // Lógica de Flip
        if (frame.horizontal > 0)
        {
            characterSprite.flipX = true; // Mirando a la derecha (D)
        }
        else if (frame.horizontal < 0)
        {
            characterSprite.flipX = false;  // Mirando a la izquierda (A)
        }

        if (frame.jump && isGrounded) // Espacio
        {
           wantsToJump = true;
        }

        // Check de suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        frameNumber++;
    }

    void FixedUpdate()
    {
        ApplyMovement();
        ApplyJump();
        ApplyBetterFall();
    }

    private void ApplyMovement()
    {
        // Movimiento horizontal directo (evita el "deslizamiento" del hielo)
        rb.linearVelocity = new Vector2(frame.horizontal * moveSpeed, rb.linearVelocity.y);
    }

    private void ApplyJump()
    {
        if (wantsToJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            wantsToJump = false;
        }
    }

    private void ApplyBetterFall()
    {
        // Si estas cayendo, aumenta la gravedad
        // Si estas subiendo pero soltaste el boton de salto, frena la subida (salto variable).
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
    public void SetListaInputs(List<PlayerInputFrame> listaInputs)
    {
        recordedInputs = listaInputs;
    }
    public void SetInitialPosition(Vector3 pos)
    {
        initialPosition = pos;
    }
    public void SetInitialVelocity(Vector3 vel)
    {
        initialVelocity = vel;
    }

}