using UnityEngine;

#region Difficulty Settings

[System.Serializable]
public struct DifficultyChosen
{
    public DifficultyTypes Difficulty;
    public DifficultyChosen(DifficultyTypes difficulty) => this.Difficulty = difficulty;

}
#endregion



#region Progress Bars

[System.Serializable]
public struct ModifyShameStat
{
    public float Amount;

    public ModifyShameStat(float amount)
    {
        this.Amount = amount;
    }
}

[System.Serializable]
public struct ModifyDecisionTimeStat
{
    public float Amount;
    public ModifyDecisionTimeStat(float amount) => this.Amount = amount;

}

[System.Serializable]
public struct ModifyDistanceTimeStat
{
    public float Amount;
    public ModifyDistanceTimeStat(float amount) => this.Amount = amount;

}
#endregion

#region Game States
[System.Serializable]
public struct EnterDecisionMakingState { }

[System.Serializable]
public struct ExitDecisionMakingState { }

#endregion


#region Decisions

[System.Serializable]
public struct SetNextDecision 
{
    public DecisionData NextDecision;

    public SetNextDecision(DecisionData next) => this.NextDecision = next;
}

public struct TriggerNextDecision{ }
#endregion