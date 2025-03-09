using UnityEngine;

public class DifficultyButton : MonoBehaviour
{
    public float difficulty;
    private StateManager _stateManager;

    // Start is called before the first frame update
    void Start()
    {
        _stateManager = StateManager.Instance;
    }

    public void SetDifficultyAndLaunch(float difficulty)
    {
        _stateManager.StartGame(difficulty);
    }
}
