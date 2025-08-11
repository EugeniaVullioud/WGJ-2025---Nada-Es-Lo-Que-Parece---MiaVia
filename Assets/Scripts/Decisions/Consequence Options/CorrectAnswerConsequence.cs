using UnityEngine;

[CreateAssetMenu(menuName = "Game/Consequences/Correct")]
public class CorrectAnswerConsequence : ConsequenceData
{
    public override void Execute()
    {
        EventManager.Instance.OnCorrectAnswer(true);
    }
}
