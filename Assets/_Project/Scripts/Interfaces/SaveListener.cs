public interface SaveListener
{
    void SaveState();
    void LoadState();
    void CancelState();
	void OnRewindFinished();
}
