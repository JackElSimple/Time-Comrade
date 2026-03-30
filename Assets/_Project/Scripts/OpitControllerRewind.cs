using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class OpitControllerRewind : BaseCharacterController
{
    [Header("Cosas Rewind")]
    [SerializeField] private float cloneDistance = 0.5f; // para no complicarse ahora lo de que el clon se atraviese con el jugador, se moverá al jugador esta distancia a la izquierda

    private Animator _anim;
    private float horizontalInput;
	private bool jumpBuffered;
	private bool isJumpHeld;
	private bool isRecording;



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
    protected override void Awake()
    {
		base.Awake();
        _anim =transform.GetChild(1).GetComponent<Animator>(); //el child 1 es el sprite con la animacion
    }

    void Update()
    {
		if (Time.timeScale == 0f) return;

		// Inputs
		horizontalInput = Input.GetAxisRaw("Horizontal"); // A,D
		isJumpHeld = Input.GetButton("Jump"); // Espacio

		if (Input.GetButtonDown("Jump")){ jumpBuffered = true; }

		HandleVisuals(horizontalInput); // Voltea el sprite
		
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

		ApplyMovement(horizontalInput);
        ApplyJump();
        ApplyBetterFall(isJumpHeld);

		jumpBuffered = false; // Resetea el buffer de salto despues de procesarlo en ApplyJump()
	}

    private void ApplyJump()
    {
        if (jumpBuffered && isGrounded)
        {
           ExecuteJump();
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