using System.Collections.Generic;
using UnityEngine;

public class CloneController : BaseCharacterController
{
    private int frameNumber;
    private PlayerInputFrame currentFrame;

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
			wantsToJump = currentFrame.jumpPressedInput;

			ApplyMovement(currentFrame.horizontalInput);
			ApplyJump();
			ApplyBetterFall(currentFrame.jumpHeldInput);
			HandleVisuals(currentFrame.horizontalInput);

			frameNumber++;
		}
		else { ReiniciarBucle(); }
	}
	
	private void ReiniciarBucle()
	{
		frameNumber = 0;       
		rb.linearVelocity = initialVelocity;
		transform.position = initialPosition;
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