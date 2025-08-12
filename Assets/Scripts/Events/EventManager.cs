using System;
using UnityEditor.EditorTools;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    private void Awake() => Instance = this;

    private PoolManager poolManager = new PoolManager();

    /*
    void GenerateEvent<T>(Func<T> createInstance) where T: struct
    {
        // Get the pool for the specific event type
        var pool = poolManager.GetPool<T>(createInstance);


        // Get a pooled struct or create a new one
        var eventData = pool.Get();

        // Publish the event (for simplicity, let's assume EventBus handles it)
        EventBus.Publish(eventData);

        // Return the struct to the pool after use
        //pool.Return(eventData);
    }
    */

    public delegate void RefAction<T>(ref T item);

    private void GenerateEvent<T>(RefAction<T> initializer) where T : struct
    {
        var pool = poolManager.GetPool<T>();
        var e = pool.Get();
        initializer(ref e);
        EventBus.Publish(e);
        pool.Return(e);
    }

    #region Difficulty Settings
    public void OnEasyDifficultyChosen()
        => GenerateEvent<DifficultyChosen>((ref DifficultyChosen e) => e.Set(DifficultyTypes.Easy));

    public void OnMediumDifficultyChosen()
        => GenerateEvent<DifficultyChosen>((ref DifficultyChosen e) => e.Set(DifficultyTypes.Medium));

    public void OnHardDifficultyChosen()
        => GenerateEvent<DifficultyChosen>((ref DifficultyChosen e) => e.Set(DifficultyTypes.Hard));

    public void OnChangeSpeed()
      => GenerateEvent<ModifySpeed>((ref ModifySpeed e) => { });
    #endregion


    #region Progress Bars
    public void OnModifyShameStat(float amount)
        => GenerateEvent<ModifyShameStat>((ref ModifyShameStat e) => e.Set(amount));

    public void OnModifyDistanceStat(float amount)
        => GenerateEvent<ModifyDistanceTimeStat>((ref ModifyDistanceTimeStat e) => e.Set(amount));

    public void OnModifyDecisionTimerStat(float amount)
        => GenerateEvent<ModifyDecisionTimeStat>((ref ModifyDecisionTimeStat e) => e.Set(amount));
    #endregion


    #region Decisions
    public void OnSetNextDecision(DecisionData amount)
        => GenerateEvent<SetNextDecision>((ref SetNextDecision e) => e.Set(amount));

    public void OnSpawnNextDecision()
       => GenerateEvent<SpawnNextDecision>((ref SpawnNextDecision e) => { });
    public void OnTriggerNextDecision()
        => GenerateEvent<TriggerNextDecision>((ref TriggerNextDecision e) => { });

    public void OnFinishDecisionMaking()
       => GenerateEvent<FinishDecisionMaking>((ref FinishDecisionMaking e) => { });

    public void OnEnterDecisionMaking()
     => GenerateEvent<EnterDecisionMakingState>((ref EnterDecisionMakingState e) => { });
    #endregion


    #region Menu
    public void OnMainMenuChosen()
        => GenerateEvent<MenuChosen>((ref MenuChosen e) => e.Set(MenuType.MainMenu));

    public void OnNoneMenuChosen()
        => GenerateEvent<MenuChosen>((ref MenuChosen e) => e.Set(MenuType.None));

    public void OnCreditsMenuChosen()
        => GenerateEvent<MenuChosen>((ref MenuChosen e) => e.Set(MenuType.CreditsMenu));

    public void OnSoundMenuChosen()
        => GenerateEvent<MenuChosen>((ref MenuChosen e) => e.Set(MenuType.SoundMenu));

    public void OnVideoMenuChosen()
        => GenerateEvent<MenuChosen>((ref MenuChosen e) => e.Set(MenuType.VideoMenu));

    public void OnDecisionMenuChosen()
        => GenerateEvent<MenuChosen>((ref MenuChosen e) => e.Set(MenuType.DecisionMenu));

    public void OnInGameMenuChosen()
        => GenerateEvent<MenuChosen>((ref MenuChosen e) => e.Set(MenuType.InGameMenu));

    public void OnEndingMenuChosen()
        => GenerateEvent<MenuChosen>((ref MenuChosen e) => e.Set(MenuType.EndingMenu));

    public void OnHowToPlayMenuChosen()
        => GenerateEvent<MenuChosen>((ref MenuChosen e) => e.Set(MenuType.HowToPlayMenu));

    public void OnDifficultyMenuChosen()
        => GenerateEvent<MenuChosen>((ref MenuChosen e) => e.Set(MenuType.DifficultyMenu));
    #endregion


    #region Game Management
    public void OnCorrectAnswer(bool right)
        => GenerateEvent<OnCorrectlyAnswered>((ref OnCorrectlyAnswered e) => e.Set(right));
    public void OnExitGame()
        => GenerateEvent<ExitGame>((ref ExitGame e) => { });

    public void OnResetGame()
        => GenerateEvent<ResetGame>((ref ResetGame e) => { });
    public void OnWonGame()
       => GenerateEvent<WonGame>((ref WonGame e) => { });
    public void OnPlayAgain()
        => GenerateEvent<PlayAgain>((ref PlayAgain e) => { });

    public void OnLost()
        => GenerateEvent<LostGame>((ref LostGame e)  => { });
    #endregion


    #region Visuals
    public void OnVisualsReady(bool done)
        => GenerateEvent<VisualsReady>((ref VisualsReady e) => e.Set(done));
    #endregion


    #region Audio
    public void OnToggleMute(bool done)
       => GenerateEvent<ToggleMuteMaster>((ref ToggleMuteMaster e) => e.Set(done));
    public void OnMuteMusic()
       => GenerateEvent<ToggleMuteMusic>((ref ToggleMuteMusic e) => e.Set(true));
    public void OnUnMuteMusic()
      => GenerateEvent<ToggleMuteMusic>((ref ToggleMuteMusic e) => e.Set(false));

    public void OnPlaySFX(AudioClip sound, float volume, bool loop)
        => GenerateEvent<PlaySFX>((ref PlaySFX e) => e.Set(sound, volume, loop));

    public void OnSetMasterVolume(float volume)
        => GenerateEvent<SetMasterVolume>((ref SetMasterVolume e) => e.Set(volume));

    public void OnSetMusicVolume(float volume)
        => GenerateEvent<SetMusicVolume>((ref SetMusicVolume e) => e.Set(volume));

    public void OnSetSFXVolume(float volume)
        => GenerateEvent<SetSFXVolume>((ref SetSFXVolume e) => e.Set(volume));
    #endregion

    #region Shame Settings
    public void OnEmptyShame()
        => GenerateEvent<EmptyShame>((ref EmptyShame e) => { });

    public void OnHalfShame()
        => GenerateEvent<HalfShame>((ref HalfShame e) => { });

    #endregion
}
