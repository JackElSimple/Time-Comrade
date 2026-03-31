using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public abstract class BaseCharacterController : MonoBehaviour
{
	[Header("Referencias Visuales")]
	[SerializeField] protected SpriteRenderer characterSprite;

	[Header("Configuracion de Movimiento")]
	[SerializeField] protected float moveSpeed = 8f;
	[SerializeField] protected float jumpForce = 12f;

	[Header("Fisicas de Salto")]
	[SerializeField] protected float gravityScale = 3f;
	[SerializeField] protected float fallMultiplier = 1.5f;
	[SerializeField] protected float lowJumpMultiplier = 2f;

	[Header("Deteccion de Suelo")]
	[SerializeField] protected Transform groundCheck;
	[SerializeField] protected float groundCheckRadius = 0.2f;
	[SerializeField] protected LayerMask groundLayer;

	protected Rigidbody2D rb;
	protected bool isGrounded;
	protected bool wantsToJump;
	protected Vector3 initialPosition;
	protected Vector3 initialVelocity;
	protected List<PlayerInputFrame> recordedInputs = new List<PlayerInputFrame>();
	protected SceneController sc;


	public struct PlayerInputFrame //Struct for saving all imputs
	{
		public float horizontalInput;
		public bool jumpPressedInput;
		public bool jumpHeldInput;

		public PlayerInputFrame(float h, bool jPressed, bool jHeld)
		{
			horizontalInput = h;
			jumpPressedInput = jPressed;
			jumpHeldInput = jHeld;
		}
	}

	protected virtual void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		sc = Object.FindAnyObjectByType<SceneController>();

		rb.gravityScale = gravityScale;
		rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		rb.freezeRotation = true;

	}

	protected void ApplyMovement(float horizontal)
	{
		rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
	}

	protected void ApplyJump()
	{
		if (wantsToJump && isGrounded)
		{
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
		}
		wantsToJump = false;
	}

	protected void ApplyBetterFall(bool jumpHeld)
	{
		if (rb.linearVelocity.y < 0)
			rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
		else if (rb.linearVelocity.y > 0 && !jumpHeld)
			rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
	}

	protected void HandleVisuals(float h)
	{
		if (h > 0) characterSprite.flipX = true;
		else if (h < 0) characterSprite.flipX = false;
	}
}