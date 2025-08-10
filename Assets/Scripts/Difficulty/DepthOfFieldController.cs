using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthOfFieldController : MonoBehaviour
{
    [SerializeField] private Volume volume;

    private DepthOfField depthOfField;

    private void Awake()
    {
        if (volume == null)
            volume = GetComponent<Volume>();

        if (volume.profile.TryGet<DepthOfField>(out var dof))
        {
            depthOfField = dof;
        }
    }
   
    public void ApplySettings(DepthOfFieldSettings settings)
    {
        depthOfField.gaussianStart.value = settings.start;
        depthOfField.gaussianEnd.value = settings.end;
        depthOfField.gaussianMaxRadius.value = settings.maxRadius;
    }
}
[System.Serializable]
public struct DepthOfFieldSettings
{
    public float start;
    public float end;
    public float maxRadius;
}