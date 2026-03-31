using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class OpitControllerRewind : BaseCharacterController
{
    [Header("Cosas Rewind")]
    [SerializeField] private float cloneDistance = 0.5f; // para no complicarse ahora lo de que el clon se atraviese con el jugador, se moverá al jugador esta distancia a la izquierda

    private Animator _anim;
    private float horizontal;
	private bool isJumpHeld;


	protected override void Awake()
    {
		base.Awake();
        _anim =transform.GetChild(1).GetComponent<Animator>(); //el child 1 es el sprite con la animacion

    }

    void Update()
    {
		if (Time.timeScale == 0f) return;

		// Inputs
		horizontal = Input.GetAxisRaw("Horizontal"); // A,D
		isJumpHeld = Input.GetButton("Jump"); // Espacio

		if (Input.GetButtonDown("Jump")){ wantsToJump = true; }

		
		if (Input.GetMouseButtonDown(0))
		{
			if (sc != null) sc.GestionarHabilidad();
		}

		HandleVisuals(horizontal); // Voltea el sprite
		_anim.SetFloat("speed", math.abs(rb.linearVelocity.x));
    }

    void FixedUpdate()
    {
		// Check de suelo
		isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
		if (sc != null && sc.isRecording) { recordedInputs.Add(new PlayerInputFrame(horizontal, wantsToJump, isJumpHeld)); }

		ApplyMovement(horizontal);
		ApplyJump();
		ApplyBetterFall(isJumpHeld);

	}


    public void StartRecording()
    {
        Debug.Log("Grabacion comenzada");
		recordedInputs.Clear();
		initialPosition = transform.position;
        initialVelocity = rb.linearVelocity;

    }
    public void FinishRecording()
    {
        Debug.Log("Grabacion terminada");
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
		recordedInputs.Clear(); // Limpia la lista de frames grabados
	}
}