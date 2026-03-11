using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		// Obtenemos todas las escenas habilitadas en el Build Settings
		string[] scenes = EditorBuildSettings.scenes
			.Where(s => s.enabled)
			.Select(s => System.IO.Path.GetFileNameWithoutExtension(s.path))
			.ToArray();

		if (scenes.Length == 0)
		{
			EditorGUI.LabelField(position, label.text, "Añade escenas al Build Settings");
			return;
		}

		// Buscamos cuál es la escena seleccionada actualmente
		int currentIndex = Mathf.Max(0, System.Array.IndexOf(scenes, property.stringValue));

		// Dibujamos el menú desplegable
		currentIndex = EditorGUI.Popup(position, label.text, currentIndex, scenes);

		// Guardamos la escena seleccionada de vuelta en el string
		property.stringValue = scenes[currentIndex];
	}
}