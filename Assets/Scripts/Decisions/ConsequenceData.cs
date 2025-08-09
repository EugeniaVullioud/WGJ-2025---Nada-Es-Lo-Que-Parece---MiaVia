using UnityEngine;

public abstract class ConsequenceData : ScriptableObject, IConsequence
{
    public abstract void Execute();
}
