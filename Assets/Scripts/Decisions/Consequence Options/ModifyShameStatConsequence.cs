using UnityEngine;

[CreateAssetMenu(menuName = "Game/Consequences/Modify Shame")]
public class ModifyShameStatConsequence : ConsequenceData
{
    public int amount;
    public override void Execute()
    {
        EventManager.Instance.OnModifyShameStat(amount);
    }
}
