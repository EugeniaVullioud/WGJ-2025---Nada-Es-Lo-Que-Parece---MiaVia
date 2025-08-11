using UnityEngine;

[CreateAssetMenu(menuName = "Game/Consequences/Lose")]

public class LoseConsequence : ConsequenceData
{
    public override void Execute()
    {
        EventManager.Instance.OnLost();
    }
}
