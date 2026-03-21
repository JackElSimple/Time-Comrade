using UnityEngine;

public class RecordingCamera2D : MonoBehaviour, RecordSwitch
{
    [SerializeField] private SpriteRenderer overlay;

    private void OnEnable()
    {
        SceneController.recordingListeners.Add(this);
    }

    private void OnDisable()
    {
        SceneController.recordingListeners.Remove(this);
    }

    public void OnRecordingStart()
    {
        Debug.Log($"{name} received OnRecordingStart");
        if (overlay != null)
        {
            overlay.color = new Color(0, 1, 0, 0.1f);
            overlay.enabled = true;
        }
    }

    public void OnRecordingStop()
    {
        if (overlay != null)
        {
            overlay.enabled = false;
        }
    }
}