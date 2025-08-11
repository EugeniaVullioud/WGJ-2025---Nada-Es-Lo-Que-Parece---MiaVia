using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [Header("Menu References")]
    public MainMenuController mainMenu;
    public CreditsMenuController creditsMenu;
    public SoundMenuController soundMenu;
    public VideoMenuController videoMenu;
    public DecisionMenuController decisionMenu;
    public InGameMenuController inGameMenu;
    public EndingMenuController endingMenu;
    public HowToPlayMenuController howToPlayMenu;
    public DifficultyMenuController difficultyMenu;
    public LoseMenuController loseMenu;

    private IMenuController currentMenu;

    private Dictionary<MenuType, IMenuController> menuMap;

    private void Awake()
    {
        // Initialize dictionary mapping MenuType to controller
        menuMap = new Dictionary<MenuType, IMenuController>
        {
            { MenuType.MainMenu, mainMenu },
            { MenuType.CreditsMenu, creditsMenu },
            { MenuType.SoundMenu, soundMenu },
            { MenuType.VideoMenu, videoMenu },
            { MenuType.DecisionMenu, decisionMenu },
            { MenuType.EndingMenu, endingMenu },
            { MenuType.HowToPlayMenu, howToPlayMenu },
            { MenuType.DifficultyMenu, difficultyMenu },
            { MenuType.LoseMenu, loseMenu },
            { MenuType.InGameMenu, inGameMenu },

        };

        //ShowMenu(MenuType.MainMenu);

    }
    private void Start()
    {
        EventBus.Subscribe<MenuChosen>((menu) => ShowMenu(menu.Menu));
        EventBus.Subscribe<PlayAgain>((menu) => ShowMenu(MenuType.InGameMenu));
        EventBus.Subscribe<EnterDecisionMakingState>((menu) => ShowMenu(MenuType.DecisionMenu));
        EventBus.Subscribe<FinishDecisionMaking>((menu) => ShowMenu(MenuType.InGameMenu));

        EventManager.Instance.OnMainMenuChosen();
    }
    public void ShowMenu(MenuType menu)
    {

        // Get new menu controller or null
        menuMap.TryGetValue(menu, out IMenuController newMenu);
        if (newMenu != currentMenu)
        {
            // Disable current menu
            currentMenu?.OnDisableMenu();
        }
        currentMenu = newMenu;

        // Set active states for all menus in dictionary
        foreach (var kvp in menuMap)
        {
            bool shouldBeActive = (kvp.Value == currentMenu);
            if (!shouldBeActive) kvp.Value.OnDisableMenu();
        }

        currentMenu?.OnEnableMenu();
    }
}
