public interface SaveListener
{
    void SaveState();
    void LoadState();
	void OnRewindFinished();
}