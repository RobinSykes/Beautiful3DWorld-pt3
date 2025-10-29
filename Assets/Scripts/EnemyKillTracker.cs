using UnityEngine;
using UnityEngine.UIElements;

public class KillManager : MonoBehaviour
{
    public static KillManager Instance { get; private set; }

    private int totalKills = 0;
    private int mostKillsInSingleGame = 0;
    private int currentGameKills = 0;
    private Transform player;
    public float killRange = 10f;

    private float totalPlaytime = 0f; // in seconds
    private float sessionStartTime;

    private Label totalKillsLabel;
    private Label mostKillsLabel;
    private Label playtimeLabel;

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

        // Load saved values
        totalKills = PlayerPrefs.GetInt("TotalKills", 0);
        mostKillsInSingleGame = PlayerPrefs.GetInt("MostKills", 0);
        totalPlaytime = PlayerPrefs.GetFloat("TotalPlaytime", 0f);

        // Start session timer
        sessionStartTime = Time.time;

    }

    private void OnEnable()
    {
        TryFindKillCounterUI();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        TryFindKillCounterUI();
    }

    private void Update()
    {
        // Update playtime while game is running
        float sessionPlaytime = Time.time - sessionStartTime;
        float total = totalPlaytime + sessionPlaytime;

        if (playtimeLabel != null)
            playtimeLabel.text = $"Playtime: {FormatPlaytime(total)}";
    }

    private void TryFindKillCounterUI()
    {
        var uiDoc = Object.FindFirstObjectByType<UIDocument>();
        if (uiDoc != null)
        {
            var root = uiDoc.rootVisualElement;
            totalKillsLabel = root.Q<Label>("TotalKillsText");
            mostKillsLabel = root.Q<Label>("MostKillsText");
            playtimeLabel = root.Q<Label>("PlaytimeText");
            UpdateAllLabels();
        }
    }

    public void AddKill(Vector3 enemyPosition)
    {
        // If no player found, skip distance check
        if (player == null)
        {
            Debug.LogWarning("KillManager: No player found, skipping distance check.");
            return;
        }

        // Check if kill was close enough to player
        float distance = Vector3.Distance(player.position, enemyPosition);
        if (distance > killRange)
        {
            Debug.Log($"Kill ignored — enemy too far from player ({distance:F1} > {killRange}).");
            return;
        }

        // Add kill normally
        totalKills++;
        currentGameKills++;

        if (currentGameKills > mostKillsInSingleGame)
        {
            mostKillsInSingleGame = currentGameKills;
            PlayerPrefs.SetInt("MostKills", mostKillsInSingleGame);
        }

        PlayerPrefs.SetInt("TotalKills", totalKills);
        PlayerPrefs.Save();

        UpdateAllLabels();

        Debug.Log($"Kill added at distance {distance:F1}.");
    }

    public void EndGame()
    {
        // Called when player dies or game ends
        SavePlaytime();
        currentGameKills = 0;
    }

    public void SavePlaytime()
    {
        float sessionPlaytime = Time.time - sessionStartTime;
        totalPlaytime += sessionPlaytime;

        PlayerPrefs.SetFloat("TotalPlaytime", totalPlaytime);
        PlayerPrefs.Save();

        sessionStartTime = Time.time; // reset for next run
        Debug.Log($"[KillManager] Saved total playtime: {FormatPlaytime(totalPlaytime)}");
    }

    private void UpdateAllLabels()
    {
        if (totalKillsLabel != null)
            totalKillsLabel.text = $"Total Kills: {totalKills}";

        if (mostKillsLabel != null)
            mostKillsLabel.text = $"Most Kills in One Game: {mostKillsInSingleGame}";

        if (playtimeLabel != null)
            playtimeLabel.text = $"Playtime: {FormatPlaytime(totalPlaytime)}";
    }

    private string FormatPlaytime(float seconds)
    {
        int hrs = Mathf.FloorToInt(seconds / 3600f);
        int mins = Mathf.FloorToInt((seconds % 3600f) / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);

        if (hrs > 0)
            return $"{hrs}h {mins}m {secs}s";
        else if (mins > 0)
            return $"{mins}m {secs}s";
        else
            return $"{secs}s";
    }

    private void OnApplicationQuit()
    {
        SavePlaytime();
    }
}
