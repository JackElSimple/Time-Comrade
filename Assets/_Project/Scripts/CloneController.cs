using System.Collections.Generic;
using UnityEngine;
using static OpitControllerRewind;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class CloneController : MonoBehaviour
{
    [Header("Referencias Visuales")]
    [SerializeField] private SpriteRenderer characterSprite;

    [Header("Configuracion de Movimiento")]
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

    [Header("Rewind Settings")]
    [SerializeField] private float delayTime = 1.0f;

	private Rigidbody2D rb;
    private bool isGrounded;
    private bool wantsToJump;

    private List<PlayerInputFrame> recordedInputs = new List<PlayerInputFrame>();
    private int frameNumber;
    private Vector3 initialPosition; //the position of the player when the rewind  is pushed
    private Vector3 initialVelocity; //the vector of movement of the player when the rewind is pushed
    private PlayerInputFrame currentFrame;
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
		if (Time.timeScale == 0f) return;
    }

    void FixedUpdate()
    {
		if (recordedInputs.Count == 0) return;

		if (frameNumber < recordedInputs.Count)
		{
			currentFrame = recordedInputs[frameNumber];

			isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

			if (currentFrame.jumpPressed && isGrounded) wantsToJump = true;

			ApplyMovement();
			ApplyJump();
			ApplyBetterFall();
			HandleVisuals();

			frameNumber++;
		}
		else
		{
			ReiniciarBucle();
		}
	}
	private void ReiniciarBucle()
	{
		frameNumber = 0;       
		rb.linearVelocity = initialVelocity;
		transform.position = initialPosition;
	}
	private void ApplyMovement()
    {
        rb.linearVelocity = new Vector2(currentFrame.horizontal * moveSpeed, rb.linearVelocity.y);
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
		if (rb.linearVelocity.y < 0)
			rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
		else if (rb.linearVelocity.y > 0 && !currentFrame.jumpHeld) 
			rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
	}
	private void HandleVisuals()
	{
		if (currentFrame.horizontal > 0) characterSprite.flipX = true;
		else if (currentFrame.horizontal < 0) characterSprite.flipX = false;
	}

	public void SetData(List<PlayerInputFrame> inputs, Vector3 pos, Vector3 vel)
	{
		// Crear una nueva lista para que no apunte a la del jugador que se va a borrar
		recordedInputs = new List<PlayerInputFrame>(inputs);
		initialPosition = pos;
		initialVelocity = vel;
		transform.position = pos;
	}

}