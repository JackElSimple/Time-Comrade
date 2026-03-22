using UnityEngine;

public class RecordingTimeWall : MonoBehaviour, RecordSwitch

{
    void Start()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public void OnRecordingStart()
    {
         GetComponent<Collider2D>().enabled = true;
    GetComponent<SpriteRenderer>().enabled = true;
    }

    public void OnRecordingStop()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }
    
}
