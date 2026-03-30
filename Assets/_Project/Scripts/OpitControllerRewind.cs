using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Tilemaps;
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
    private Animator _anim;
    private float horizontalInput;
    private bool isGrounded;
	private bool jumpBuffered;
	private bool isJumpHeld;
	private bool isRecording;
    private Vector3 initialPosition; //the position of the player when the rewind  is pushed
    private Vector3 initialVelocity; //the vector of movement of the player when the rewind is pushed

    private List<PlayerInputFrame> recordedInputs = new List<PlayerInputFrame>();

    public struct PlayerInputFrame //Struct for saving all imputs, at the moment the horizontal and the jump
    {
        public float horizontal;
        public bool jumpPressed;
		public bool jumpHeld;

		public PlayerInputFrame(float h, bool jPressed, bool jHeld)
		{
            horizontal = h;
            jumpPressed = jPressed;
			jumpHeld = jHeld;
        }
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;

        _anim =transform.GetChild(1).GetComponent<Animator>(); //el child 1 es el sprite con la animacion
    }

    void Update()
    {
		if (Time.timeScale == 0f) return;

		// Inputs
		horizontalInput = Input.GetAxisRaw("Horizontal"); // A,D
		isJumpHeld = Input.GetButton("Jump"); // Espacio

		if (Input.GetButtonDown("Jump")){ jumpBuffered = true; }

		HandleVisuals(); // Voltea el sprite
		
		if (Input.GetMouseButtonDown(0))
		{
			SceneController sc = Object.FindAnyObjectByType<SceneController>();
			if (sc != null) sc.GestionarHabilidad();
		}

		_anim.SetFloat("speed", math.abs(rb.linearVelocity.x));

    }

    void FixedUpdate()
    {
		// Check de suelo
		isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

		if (isRecording){ recordedInputs.Add(new PlayerInputFrame(horizontalInput, jumpBuffered, isJumpHeld)); }

		ApplyMovement();
        ApplyJump();
        ApplyBetterFall();

		jumpBuffered = false; // Resetea el buffer de salto despues de procesarlo en ApplyJump()
	}

	private void HandleVisuals()
	{
		if (horizontalInput > 0)
		{
			characterSprite.flipX = true; // Mirando a la derecha (D)
		}
		else if (horizontalInput < 0)
		{
			characterSprite.flipX = false;  // Mirando a la izquierda (A)
		}
	}
	private void ApplyMovement()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void ApplyJump()
    {
        if (jumpBuffered && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
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
        else if (rb.linearVelocity.y > 0 && !isJumpHeld)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }

        
    }

    public void StartRecording()
    {
        Debug.Log("Grabacion comenzada");
		recordedInputs.Clear();
        isRecording = true;
        initialPosition = transform.position;
        initialVelocity = rb.linearVelocity;

    }
    public void FinishRecording()
    {
        Debug.Log("Grabacion terminada");
        isRecording = false;
        rb.linearVelocity = initialVelocity;
        transform.position = initialPosition; 
        
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

	public void CancelRecording()
	{
		Debug.Log("Grabación cancelada: Datos eliminados sin teletransporte.");
		isRecording = false;
		recordedInputs.Clear(); // Limpia la lista de frames grabados
	}
}