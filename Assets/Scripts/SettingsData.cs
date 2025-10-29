using UnityEngine;

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

public static class SettingsData
{
    public static Difficulty CurrentDifficulty
    {
        get
        {
            string saved = PlayerPrefs.GetString("GameDifficulty", "Easy");
            return saved switch
            {
                "Medium" => Difficulty.Medium,
                "Hard" => Difficulty.Hard,
                _ => Difficulty.Easy
            };
        }
    }
}
