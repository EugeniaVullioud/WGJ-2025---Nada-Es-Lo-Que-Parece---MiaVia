using UnityEngine;

public interface IDffficulty
{
    void ShowHint(string text);
    void HideHint();
}

[System.Serializable]
public enum DifficultyTypes
{
    Easy,
    Medium,
    Hard,
}