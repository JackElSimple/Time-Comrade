using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class OpitControllerRewind : MonoBehaviour
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

    [Header("Cosas Rewind")]
    [SerializeField] private float cloneDistance = 0.5f; // para no complicarse ahora lo de que el clon se atraviese con el jugador, se moverá al jugador esta distancia a la izquierda

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private bool wantsToJump;
    private bool isRecording;
    private Vector3 initialPosition; //the position of the player when the rewind  is pushed
    private Vector3 initialVelocity; //the vector of movement of the player when the rewind is pushed

    private List<PlayerInputFrame> recordedInputs = new List<PlayerInputFrame>();

    public struct PlayerInputFrame //Struct for saving all imputs, at the moment the horizontal and the jump
    {
        public float horizontal;
        public bool jump;

        public PlayerInputFrame(float h, bool j)
        {
            horizontal = h;
            jump = j;
        }
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal"); // A,D

        
        // Lógica de Flip
        if (horizontalInput > 0)
        {
            characterSprite.flipX = true; // Mirando a la derecha (D)
        }
        else if (horizontalInput < 0)
        {
            characterSprite.flipX = false;  // Mirando a la izquierda (A)
        }

        if (Input.GetButtonDown("Jump") && isGrounded) // Espacio
        {
            wantsToJump = true;
        }

        // Check de suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isRecording)
        {
            float h = Input.GetAxisRaw("Horizontal");
            bool j = Input.GetKey(KeyCode.Space);

            recordedInputs.Add(new PlayerInputFrame(h, j));

        }
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
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
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

    
    public void StartRecording()
    {
        Debug.Log("Grabacion comenzada");
        recordedInputs = new List<PlayerInputFrame>();//we empty the list when we start another recording
        isRecording = true;
        initialPosition = transform.position;
        initialVelocity = rb.linearVelocity;

    }
    public void FinishRecording()
    {
        Debug.Log("Grabacion terminada");
        isRecording = false;
        //transform.position = initialPosition; //terminar
        rb.linearVelocity = initialVelocity;
        if (rb.linearVelocityX > 0)
        {
            transform.position = initialPosition - new Vector3(cloneDistance, 0, 0); 
        }
        else
        {
            transform.position = initialPosition + new Vector3(cloneDistance, 0, 0);
        }

     }

    public Vector3 getInitialPosition() {  
        return initialPosition;
    }
    public Vector3 getInitialVelocity(){
        return initialVelocity; 
     }
    public List<PlayerInputFrame> getImputsList() {
        return recordedInputs;
    }


}