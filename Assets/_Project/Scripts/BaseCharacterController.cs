using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public abstract class BaseCharacterController : MonoBehaviour
{
	[Header("Referencias Visuales")]
	[SerializeField] protected SpriteRenderer characterSprite;

	[Header("Configuracion de Movimiento")]
	[SerializeField] protected float moveSpeed = 8f;
	[SerializeField] protected float jumpForce = 9f;

	[Header("Fisicas de Salto")]
	[SerializeField] protected float gravityScale = 4f;
	[SerializeField] protected float fallMultiplier = 1.5f;
	[SerializeField] protected float lowJumpMultiplier = 2f;

	[Header("Deteccion de Suelo")]
	[SerializeField] protected Transform groundCheck;
	[SerializeField] protected float groundCheckRadius = 0.2f;
	[SerializeField] protected LayerMask groundLayer;
    protected ContactFilter2D groundFilter;
    protected readonly Collider2D[] results = new Collider2D[4];

    [Header("Coyote time")]
    [SerializeField] protected float coyoteTime = 0.1f;
    protected float coyoteTimeCounter = 0.05f;
    protected bool isGrounded => coyoteTimeCounter > 0f;

    protected Rigidbody2D rb;
	//protected bool isGrounded;
	protected bool wantsToJump;
	public Vector3 initialPosition { get; protected set; }
	public  Vector3 initialVelocity{ get; protected set; }
	public List<PlayerInputFrame> recordedInputs { get; protected set; } = new List<PlayerInputFrame>(); 
	protected SceneController sc;

    protected float deltaX = 0;

	protected int framesArrastrar = 0;
	protected Rigidbody2D cloneReference;

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

        groundFilter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = groundLayer,
            useNormalAngle = true,
            minNormalAngle = 35f,
            maxNormalAngle = 145f
        };

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
	protected void ApplyCoyoteTime() //also it maintains the clone
	{
        int count = rb.GetContacts(groundFilter, results);
        bool isGroundedRaw = count > 0;
        if (isGroundedRaw)
        {
            var onPlatform = results[0].gameObject;
            if (onPlatform.transform.parent != null)
            {
                if (onPlatform.transform.parent.gameObject != null)
                {
                    onPlatform = onPlatform.transform.parent.gameObject;//si tiene padre obtiene el padre (para coger a opit a partir de la plataforma) }

                }
            }
            if (onPlatform.CompareTag("Clone")) //si est� encima del clon va con el pegado
            {
                var prb = onPlatform.GetComponent<Rigidbody2D>();
				cloneReference = prb;
                rb.linearVelocityX += prb.linearVelocityX;
				framesArrastrar = 3;
            }
        }
		if (framesArrastrar > 0) //it will detect as opit is on the clone "framesArrastrar" frames more
		{
			try
			{
                var prb = cloneReference.GetComponent<Rigidbody2D>();
                rb.linearVelocityX += prb.linearVelocityX;
				if (rb.linearVelocityY <= prb.linearVelocityY) {
					rb.linearVelocityY = prb.linearVelocityY;

                }
                framesArrastrar -= 1;
            }
			catch (System.Exception)
			{
                framesArrastrar -= 1;
                throw;
			}      
        }
        if (isGroundedRaw)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }

		if (isGroundedRaw) { rb.gravityScale = 0; }
		else
		{
			rb.gravityScale = gravityScale;
		}

		
    }

}