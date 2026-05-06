using UnityEngine;

public class RecordingCamera2D : MonoBehaviour, RecordSwitch
{
    [SerializeField] private SpriteRenderer overlay;

    public void OnRecordingStart()
    {
        if (overlay != null)
        {
            //overlay.color = new Color(0, 1, 0, 0.02f);
            overlay.color =  new Color(0f, 1f, 1f, 0.02f);
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
    void OnEnable()
    {
        SceneController.recordingListeners.Add(this);
    }

    void OnDisable()
    {
        SceneController.recordingListeners.Remove(this);
    }
}