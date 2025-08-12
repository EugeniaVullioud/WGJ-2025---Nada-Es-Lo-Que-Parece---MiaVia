using UnityEngine;

[CreateAssetMenu(menuName = "Game/Consequences/Lose")]

public class LoseConsequence : ConsequenceData
{
    public bool byWrongStreet = true;
    public override void Execute()
    {
        EventManager.Instance.OnLost(byWrongStreet);
    }
}
