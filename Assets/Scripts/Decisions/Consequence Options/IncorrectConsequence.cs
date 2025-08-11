using UnityEngine;

[CreateAssetMenu(menuName = "Game/Consequences/Incorrect")]
public class IncorrectConsequence : ConsequenceData
{
    public override void Execute()
    {
        EventManager.Instance.OnCorrectAnswer(false);
    }
}
