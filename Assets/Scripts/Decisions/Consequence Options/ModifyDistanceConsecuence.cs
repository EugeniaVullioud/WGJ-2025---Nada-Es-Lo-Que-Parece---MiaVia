using UnityEngine;

[CreateAssetMenu(menuName = "Game/Consequences/Modify Distance")]
public class ModifyDistanceConsecuence : ConsequenceData
{
    public int amount;
    public override void Execute()
    {
        EventManager.Instance.OnModifyDistanceStat(amount);
    }
}
