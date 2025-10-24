using UnityEngine;
using UnityEngine.UIElements;

public class KillManager : MonoBehaviour
{
    public static KillManager Instance { get; private set; }

    private int killCount = 0;
    private Label killLabel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        //Load saved kills when game starts
        killCount = PlayerPrefs.GetInt("KillCount", 0);
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
            killLabel = root.Q<Label>("KillCountText");
            UpdateKillText();
        }
        else
        {
            killLabel = null;
        }
    }

    public void AddKill()
    {
        killCount++;
        PlayerPrefs.SetInt("KillCount", killCount); //Save it
        PlayerPrefs.Save();
        UpdateKillText();
        Debug.Log($"+1 Kill Total: {killCount}");
    }

    private void UpdateKillText()
    {
        if (killLabel != null)
        {
            killLabel.text = $"Total Enemies Killed: {killCount}";
        }
    }

    public void ResetKills()
    {
        killCount = 0;
        PlayerPrefs.SetInt("KillCount", 0); //Also reset saved value
        PlayerPrefs.Save();
        UpdateKillText();
    }

    public int GetKills() => killCount;
}
