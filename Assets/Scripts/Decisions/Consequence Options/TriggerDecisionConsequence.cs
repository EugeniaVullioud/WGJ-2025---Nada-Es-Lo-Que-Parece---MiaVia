using UnityEngine;

[CreateAssetMenu(menuName = "Game/Consequences/Trigger Decision")]
public class TriggerDecisionConsequence : ConsequenceData
{
    public DecisionData nextDecision;
    public override void Execute()
    {
        return;
        EventManager.Instance.OnSetNextDecision(nextDecision);
    }
}
