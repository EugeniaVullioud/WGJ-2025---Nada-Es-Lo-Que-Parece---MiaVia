using UnityEngine;

[CreateAssetMenu(menuName = "Game/Consequences/Show UI Message")]
public class ShowUIMessageConsequence : ConsequenceData
{
    public string message;
    public override void Execute()
    {
        DecisionService.Instance.ShowMessage(message);
    }
}
