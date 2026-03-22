using UnityEngine;

public class MovableObject : MonoBehaviour
{
	[Header("Configuraci�n Inicial")]
	public Vector3 customScale = Vector3.one;
	public Vector3 customRotation = Vector3.zero;

	[Header("Movimiento Simple (Opcional)")]
	public bool rotarConstantemente = false;
	public Vector3 ejeRotacion = new Vector3(0, 0, 1);
	public float velocidadRotacion = 0f;

	// Datos para el sistema de Rewind
	[HideInInspector] public Vector3 savedPosition;
	[HideInInspector] public Quaternion savedRotation;
	[HideInInspector] public Vector3 savedScale;

	private Animator anim;
	void Awake()
	{
		anim = GetComponent<Animator>();
	}
	void Start()
	{
		RecordCurrentState();
	}

	void Update()
	{
		if (PauseMenuHandler.Instance != null && PauseMenuHandler.Instance.isPaused)
			return;

		if (rotarConstantemente)
		{
			transform.Rotate(ejeRotacion * velocidadRotacion * Time.deltaTime);
		}
	}

	// El SceneController llama a estos m�todos
	public void RecordCurrentState()
	{
		savedPosition = transform.position;
		savedRotation = transform.rotation;
		savedScale = transform.localScale;
	}

	public void RestoreState()
	{
		transform.position = savedPosition;
		transform.rotation = savedRotation;
		transform.localScale = savedScale;

	}
}