using UnityEngine;
using UnityEngine.UIElements;

public class KillManager : MonoBehaviour
{
    public static KillManager Instance { get; private set; }

    private int totalKills = 0;                // All-time kills (saved)
    private int currentGameKills = 0;          // Kills in this session
    private int mostKillsInSingleGame = 0;     // Highest kills ever achieved in one game

    private Label totalKillsLabel;
    private Label mostKillsLabel;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved data
        totalKills = PlayerPrefs.GetInt("TotalKills", 0);
        mostKillsInSingleGame = PlayerPrefs.GetInt("MostKillsInSingleGame", 0);

        currentGameKills = 0; // reset for each run
    }

    private void OnEnable()
    {
        TryFindKillCounterUI();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        TryFindKillCounterUI();
    }

    private void TryFindKillCounterUI()
    {
        var uiDoc = Object.FindFirstObjectByType<UIDocument>();
        if (uiDoc != null)
        {
            var root = uiDoc.rootVisualElement;
            totalKillsLabel = root.Q<Label>("TotalKillsText");     // In UI Builder: Label name = TotalKillsText
            mostKillsLabel = root.Q<Label>("MostKillsText");       // In UI Builder: Label name = MostKillsText
            UpdateKillText();
        }
        else
        {
            totalKillsLabel = null;
            mostKillsLabel = null;
        }
    }

    public void AddKill()
    {
        totalKills++;
        currentGameKills++;

        // Check for new record
        if (currentGameKills > mostKillsInSingleGame)
        {
            mostKillsInSingleGame = currentGameKills;
            PlayerPrefs.SetInt("MostKillsInSingleGame", mostKillsInSingleGame);
        }

        // Save total kills persistently
        PlayerPrefs.SetInt("TotalKills", totalKills);
        PlayerPrefs.Save();

        UpdateKillText();

        Debug.Log($"+1 Kill Total: {totalKills}, Current Game: {currentGameKills}, Most in Single Game: {mostKillsInSingleGame}");
    }

    private void UpdateKillText()
    {
        if (totalKillsLabel != null)
            totalKillsLabel.text = $"Total Kills: {totalKills}";

        if (mostKillsLabel != null)
            mostKillsLabel.text = $"Most Kills in One Game: {mostKillsInSingleGame}";
    }

    public void ResetAllData()
    {
        totalKills = 0;
        currentGameKills = 0;
        mostKillsInSingleGame = 0;

        PlayerPrefs.SetInt("TotalKills", 0);
        PlayerPrefs.SetInt("MostKillsInSingleGame", 0);
        PlayerPrefs.Save();

        UpdateKillText();
    }

    public void ResetCurrentGameKills()
    {
        currentGameKills = 0;
    }

    public int GetTotalKills() => totalKills;
    public int GetMostKillsInSingleGame() => mostKillsInSingleGame;
    public int GetCurrentGameKills() => currentGameKills;
}
