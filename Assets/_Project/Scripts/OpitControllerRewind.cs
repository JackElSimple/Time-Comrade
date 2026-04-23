using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class OpitControllerRewind : BaseCharacterController
{
	private Animator _anim;
	private float horizontal;
	private bool isJumpHeld;
	private float cooldownFootstep =0;
	private bool inAirLastFrame=false;

	protected override void Awake()
	{
		base.Awake();
		_anim = transform.GetChild(1).GetComponent<Animator>(); //el child 1 es el sprite con la animacion

	}

	void Update()
	{
		if (Time.timeScale == 0f) return;

		// Inputs
		horizontal = Input.GetAxisRaw("Horizontal"); // A,D
		isJumpHeld = Input.GetButton("Jump"); // Espacio

		

		if (Input.GetButtonDown("Jump")) { wantsToJump = true; }


		if (Input.GetMouseButtonDown(0))
		{
			if (sc != null) sc.GestionarHabilidad();
		}

		HandleVisuals(horizontal); // Voltea el sprite
		_anim.SetFloat("speed", math.abs(rb.linearVelocity.x));

        if (isGrounded && math.abs(rb.linearVelocity.x) > 0.1 && cooldownFootstep > 0.3)
        {
            doingWalkingSound();
            cooldownFootstep = 0;
        }
        cooldownFootstep += Time.deltaTime;
		if(inAirLastFrame && isGrounded)
		{
			doingLandingSound();
		}
		inAirLastFrame = !isGrounded;

	}

	void FixedUpdate()
	{
		if (Time.timeScale == 0f) return;
		// Check de suelo
		//isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer); Upgraded o applycoyote time
		if (sc != null && sc.isRecording) { recordedInputs.Add(new PlayerInputFrame(horizontal, wantsToJump, isJumpHeld)); }


		ApplyMovement(horizontal);
		ApplyCoyoteTime();
		ApplyJump();
		ApplyBetterFall(isJumpHeld);

	}


	public void StartRecording() //añadir rewind
	{
		Debug.Log("Grabacion comenzada");
		recordedInputs.Clear();
		initialPosition = transform.position;
		initialVelocity = rb.linearVelocity;

	}
	public void FinishRecording() // cambiar por rewind
	{
		Debug.Log("Grabacion terminada");
		rb.linearVelocity = initialVelocity;
		transform.position = initialPosition;

	}

	public void CancelRecording()
	{
		Debug.Log("Grabación cancelada: Datos eliminados sin teletransporte.");
		recordedInputs.Clear(); // Limpia la lista de frames grabados
	}
	public void doingWalkingSound()
	{
        SceneController sc = FindFirstObjectByType<SceneController>();
        sc.ReproducirPisada();
    }
    public void doingLandingSound()
    {
        SceneController sc = FindFirstObjectByType<SceneController>();
        sc.ReproducirAterrizaje();
    }

}