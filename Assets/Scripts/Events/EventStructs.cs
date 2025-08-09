using UnityEngine;

#region Difficulty Settings

[System.Serializable]
public struct DifficultyChosen
{
    public DifficultyTypes Difficulty;

    public DifficultyChosen(DifficultyTypes difficulty)
    {
        this.Difficulty = difficulty;
    }
}
#endregion
