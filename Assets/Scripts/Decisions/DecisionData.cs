using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Decision")]
public class DecisionData : ScriptableObject
{
    [Tooltip("Unique identifier for referencing this decision.")]
    public string decisionId;

    [Tooltip("Question text shown to the player.")]
    [TextArea] public string prompt;

    [Tooltip("Time window to make a choice.")]
    public float timeLimitSeconds = 5f;

    [Tooltip("Choices available in this decision.")]
    public List<ChoiceOption> choices;

    [Tooltip("If none chosen")]
    public ChoiceOption defaultChoice;

}
