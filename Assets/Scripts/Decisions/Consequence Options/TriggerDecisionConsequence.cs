using UnityEngine;

[CreateAssetMenu(menuName = "Game/Consequences/Trigger Decision")]
public class TriggerDecisionConsequence : ConsequenceData
{
    public DecisionData nextDecision;
    public override void Execute()
    {
        EventManager.Instance.OnSetNextDecision(nextDecision);
    }
}
