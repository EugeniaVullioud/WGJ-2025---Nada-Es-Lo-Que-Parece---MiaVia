using UnityEngine;

public class DecisionTrigger : MonoBehaviour
{
    [Tooltip("Decision asset to trigger when player is close.")]
    public DecisionData decisionToTrigger;

    [Tooltip("Layer mask for objects that can trigger the decision.")]
    public LayerMask triggeringLayers;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        // Check if the other object's layer is in the triggeringLayers mask
        if (((1 << other.gameObject.layer) & triggeringLayers) != 0)
        {            
            hasTriggered = true;
            DecisionController.InEventQueue++;               
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & triggeringLayers) != 0)
        {
            hasTriggered = false;
        }
    }
}
