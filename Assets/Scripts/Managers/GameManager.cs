using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private bool visualsReady = false;

    // Public flag to check if visuals are ready
    public bool VisualsReady => visualsReady;

    private void Start()
    {
        EventBus.Subscribe<ExitGame>((_) => ExitGame());
        EventBus.Subscribe<MaxShameReached>((_) => LostGame());
        EventBus.Subscribe<ResetGame>((_) => { visualsReady = false; EventManager.Instance.OnVisualsReady(false); });
    }

    // Call this to start visuals initialization (e.g. loading assets, shaders, setup)
    public void InitializeVisuals()
    {
        visualsReady = false;
        EventManager.Instance.OnVisualsReady(visualsReady);
        StartCoroutine(InitializeVisualsRoutine());
    }

    private IEnumerator InitializeVisualsRoutine()
    {
        // Simulate some async visual init work here:
        yield return new WaitForSeconds(5f); // Simulate delay

        visualsReady = true;

        EventManager.Instance.OnVisualsReady(visualsReady);
    }

    public void ExitGame()
    {
        // Depending on platform:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void LostGame()
    {
        EventManager.Instance.OnLost(false);
    }
}
