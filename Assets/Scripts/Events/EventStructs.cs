using UnityEngine;

public interface IEventDataSetter<T>
{
    void Set(T data);
}

#region Difficulty Settings
[System.Serializable]
public struct DifficultyChosen : IEventDataSetter<DifficultyTypes>
{
    public DifficultyTypes Difficulty;

    public DifficultyChosen(DifficultyTypes difficulty) => this.Difficulty = difficulty;

    public void Set(DifficultyTypes difficulty) => this.Difficulty = difficulty;
}
[System.Serializable]

public struct ModifySpeed : IEventDataSetter<float>
{
    public float Amount;

    public ModifySpeed(float amount) => this.Amount = amount;

    public void Set(float amount) => this.Amount = amount;
}

#endregion


#region Progress Bars
[System.Serializable]
public struct ModifyShameStat : IEventDataSetter<float>
{
    public float Amount;

    public ModifyShameStat(float amount) => this.Amount = amount;

    public void Set(float amount) => this.Amount = amount;
}

[System.Serializable]
public struct ModifyDecisionTimeStat : IEventDataSetter<float>
{
    public float Amount;
    public ModifyDecisionTimeStat(float amount) => this.Amount = amount;

    public void Set(float amount) => this.Amount = amount;
}

[System.Serializable]
public struct ModifyDistanceTimeStat : IEventDataSetter<float>
{
    public float Amount;
    public ModifyDistanceTimeStat(float amount) => this.Amount = amount;

    public void Set(float amount) => this.Amount = amount;
}

public struct MaxShameReached { }
#endregion


#region Game States
[System.Serializable]
public struct EnterDecisionMakingState { }

[System.Serializable]
public struct ExitDecisionMakingState { }
#endregion


#region Decisions
[System.Serializable]
public struct SetNextDecision : IEventDataSetter<DecisionData>
{
    public DecisionData NextDecision;

    public SetNextDecision(DecisionData next) => this.NextDecision = next;

    public void Set(DecisionData next) => this.NextDecision = next;
}
public struct OnCorrectlyAnswered : IEventDataSetter<bool>
{
    public bool WasCorrect;

    public OnCorrectlyAnswered(bool answer) => this.WasCorrect = answer;

    public void Set(bool answer) => this.WasCorrect = answer;
}
[System.Serializable]
public struct TriggerNextDecision { }
public struct FinishDecisionMaking { }
#endregion


#region Menu
[System.Serializable]
public struct MenuChosen : IEventDataSetter<MenuType>
{
    public MenuType Menu;
    public MenuChosen(MenuType menu) => this.Menu = menu;

    public void Set(MenuType menu) => this.Menu = menu;
}
#endregion


#region Game Management
[System.Serializable]
public struct ExitGame{ }

[System.Serializable]
public struct LostGame { }

[System.Serializable]
public struct ResetGame { }

[System.Serializable]
public struct WonGame { }

[System.Serializable]
public struct PlayAgain { }
#endregion


#region Visuals
[System.Serializable]
public struct VisualsReady: IEventDataSetter<bool>
{
    public bool Ready;
    public VisualsReady(bool ready) => this.Ready = ready;

    public void Set(bool ready) => this.Ready = ready;
}
#endregion

#region Shame
[System.Serializable]
public struct EmptyShame { }

[System.Serializable]
public struct HalfShame { }

[System.Serializable]
public struct FullShame { }
#endregion


#region Audio
[System.Serializable]
public struct PlaySFX
{
    public AudioClip SFX;
    public float Volume;
    public bool Loop;

    public PlaySFX(AudioClip sfx, float volume = 1f, bool loop = false)
        => (this.SFX, this.Volume, this.Loop) = (sfx, volume, loop);

    public void Set(AudioClip sfx, float volume = 1f, bool loop = false)
        => (this.SFX, this.Volume, this.Loop) = (sfx, volume, loop);
}

[System.Serializable]
public struct SetMasterVolume
{
    public float Volume;
    public SetMasterVolume(float volume = 1f) => this.Volume = volume;

    public void Set(float volume) => this.Volume = volume;
}

public struct SetMusicVolume
{
    public float Volume;
    public SetMusicVolume(float volume = 1f) => this.Volume = volume;

    public void Set(float volume) => this.Volume = volume;
}

public struct SetSFXVolume
{
    public float Volume;
    public SetSFXVolume(float volume = 1f) => this.Volume = volume;

    public void Set(float volume) => this.Volume = volume;
}
[System.Serializable]
public struct ToggleMuteMaster : IEventDataSetter<bool>
{
    public bool Mute;
    public ToggleMuteMaster(bool mute) => this.Mute = mute;

    public void Set(bool ready) => this.Mute = ready;
}
public struct ToggleMuteMusic : IEventDataSetter<bool>
{
    public bool Mute;
    public ToggleMuteMusic(bool mute) => this.Mute = mute;

    public void Set(bool ready) => this.Mute = ready;
}
#endregion