using UnityEngine;

public interface IMenuController
{
    GameObject Root { get; }
    void OnEnableMenu();
    void OnDisableMenu();

}
[System.Serializable]
public enum MenuType
{
    None,
    MainMenu,
    CreditsMenu,
    SoundMenu,
    VideoMenu,
    DecisionMenu,
    InGameMenu,
    EndingMenu,
    HowToPlayMenu,
    DifficultyMenu,
    LoseMenu, 

}