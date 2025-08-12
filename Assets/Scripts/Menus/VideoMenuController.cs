using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoMenuController : MonoBehaviour, IMenuController
{
    public GameObject Root => gameObject;

    [Header("UI Elements")]
    public VideoPlayer videoPlayer;
    public CanvasGroup loadingTextGroup;
    public Button startButton;
    public string videoFileName;

    public float minLoadingTime = 2f;
    public float vidLenght = 25f;

    private Coroutine videoRoutine;
    private bool _readyToPlay = false;

    private void Awake()
    {
        SetUp();
    }
    private void Start()
    {
        EventBus.Subscribe<VisualsReady>((ready) => { _readyToPlay = ready.Ready; });

        startButton.onClick.AddListener(() => { videoPlayer.Stop(); StopCoroutine(videoRoutine); });

    }
    public void SetUp()
    {
        if (videoPlayer)
        {
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            videoPlayer.url = videoPath;
        }
    }
    public void OnEnableMenu()
    {
        gameObject.SetActive(true);
        startButton.gameObject.SetActive(false);
        loadingTextGroup.alpha = 0f;
        startButton.interactable = false;

        EventManager.Instance.OnVisualsReady(true);
        _readyToPlay = true;

        if (videoRoutine != null)  StopCoroutine(videoRoutine);
        videoRoutine = StartCoroutine(PlayVideoAndWaitRoutine());


    }

    public void OnDisableMenu()
    {
        if (videoRoutine != null)
            StopCoroutine(videoRoutine);

        videoPlayer.Stop();
        gameObject.SetActive(false);
    }

    private IEnumerator PlayVideoAndWaitRoutine()
    {
        // Play video
        videoPlayer.Play();
        // Wait for video to finish

        yield return new WaitForSeconds(minLoadingTime);


        startButton.gameObject.SetActive(true);
        startButton.interactable = true;

        yield return new WaitForSeconds(vidLenght - minLoadingTime -0.5f);

        startButton.onClick.Invoke();
    }



    private IEnumerator FadeOutCanvasGroup(CanvasGroup cg, float duration)
    {
        float elapsed = 0f;
        float startAlpha = cg.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            yield return null;
        }
        cg.alpha = 0f;

        startButton.gameObject.SetActive(true);
        startButton.interactable = true;
    }

}
