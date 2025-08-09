using System.Collections.Generic;
using UnityEngine;

public class ConsequenceRunner : MonoBehaviour
{
    public static void Execute(List<ConsequenceData> consequences)
    {
        foreach (var consequence in consequences)
        {
            consequence.Execute();
        }
    }
}
