using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DifficultyManager: MonoBehaviour
{
    [Header("DOF Presets")]
    public DepthOfFieldSettings easySettings = new DepthOfFieldSettings { start = 5f, end = 15f, maxRadius = 1f };
    public DepthOfFieldSettings mediumSettings = new DepthOfFieldSettings { start = 3f, end = 12f, maxRadius = 2f };
    public DepthOfFieldSettings hardSettings = new DepthOfFieldSettings { start = 1f, end = 8f, maxRadius = 4f };

    [Header("DOF Presets")]
    public DepthOfFieldController _visualController;

    private void Start()
    {
        EventBus.Subscribe<DifficultyChosen>((e)=> SetDifficulty(e.Difficulty));
    }
    public void SetDifficulty(DifficultyTypes difficulty)
    {

        DepthOfFieldSettings settings = difficulty switch
        {
            DifficultyTypes.Easy => easySettings,
            DifficultyTypes.Medium => mediumSettings,
            DifficultyTypes.Hard => hardSettings,
            _ => easySettings
        };

        _visualController.ApplySettings(settings);
    }
}

[System.Serializable]
public enum DifficultyTypes
{
    Easy,
    Medium,
    Hard,
}