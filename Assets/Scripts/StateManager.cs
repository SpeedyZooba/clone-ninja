using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StateManager : MonoBehaviour
{
    // Main instance of the state manager
    public static StateManager Instance { get; private set; }

    #region Game Elements
    // Prefabs can be referenced with the script attached to them
    // They can be used the following way when you know what type of GameObject you're working with:
    [SerializeField] private List<Target> Targets;
    [SerializeField] private Blade Blade;
    #endregion
    #region Menu
    [SerializeField] private GameObject TitleScreen;
    [SerializeField] private GameObject PauseScreen;
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private TextMeshProUGUI TimeText;
    [SerializeField] private TextMeshProUGUI GameOverText;
    [SerializeField] private Button RestartButton;
    #endregion
    #region Game Status
    public bool IsActive;
    public bool IsPaused;
    [SerializeField] private float _spawnRate;
    [SerializeField] private int _score;
    [SerializeField] private int _time;
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            // For persistence between scenes, you could place a DontDestroyOnLoad(gameObject) here
        }
    }

    void Start()
    {
        Blade.SetManager(this);
    }

    // Note that you should handle user input in Update()
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
    }

    // As it is necessary for optimization, you should use Start() to cache the components, transforms etc. you use often
    public void StartGame(float difficulty)
    {
        TitleScreen.gameObject.SetActive(false);
        ScoreText.gameObject.SetActive(true);
        TimeText.gameObject.SetActive(true);
        IsActive = true;
        IsPaused = false;
        _spawnRate = difficulty;
        _score = 0;
        _time = 60;
        ScoreText.text = "Score: 0";
        TimeText.text = "Time: " + _time;
        StartCoroutine(SpawnTarget());
        StartCoroutine(Timer());
    }

    public void UpdateScore(int points)
    {
        _score += points;
        if (_score < 0)
        {
            EndGame();
        }
        ScoreText.text = "Score: " + _score;
    }

    public void PauseGame()
    {
        if (!IsPaused)
        {
            IsPaused = true;
            // Time scale is the rate at which time passes, setting it to 0 would pause time which would pause physics calculations as a result.
            Time.timeScale = 0;
            PauseScreen.gameObject.SetActive(true);
        }
        else
        {
            IsPaused = false;
            Time.timeScale = 1;
            PauseScreen.gameObject.SetActive(false);
        }
    }

    public void EndGame()
    {
        IsActive = false;
        GameOverText.gameObject.SetActive(true);
        RestartButton.gameObject.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void CheckTime()
    {
        TimeText.text = "Time: " + _time;
        if (_time <= 0)
        {
            EndGame();
        }
    }

    private IEnumerator SpawnTarget()
    {
        while (IsActive && !IsPaused)
        {
            yield return new WaitForSeconds(_spawnRate);
            int index = Random.Range(0, Targets.Count);
            // Instantiate returns an instantiated GameObject, so you can do initializations here as well
            var targetInst = Instantiate(Targets[index].gameObject);
            targetInst.GetComponent<Target>().SetManager(this);
        }
    }

    private IEnumerator Timer()
    {
        while (IsActive && !IsPaused)
        {
            yield return new WaitForSeconds(1);
            _time--;
            CheckTime();
        }
    }
}
