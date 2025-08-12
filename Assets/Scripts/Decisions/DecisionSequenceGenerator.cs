using System.Collections.Generic;
using UnityEngine;
using static WeighterSideObjectSpawner;

public class DecisionSequenceGenerator : MonoBehaviour
{
    [SerializeField] private List<DecisionData> _decisionPrefabs; // assign in inspector
    [SerializeField] private int _sequenceLength = 40;
    [SerializeField] private int _noRepeatCount = 12;
    [SerializeField] private WeighterSideObjectSpawner _spawner;

    private Queue<DecisionData> _currentSequence;
    private int _index;

    public void ResetQueue()
    {
        return;
        _currentSequence.Clear();
        GenerateSequence();
    }
    public void GenerateSequence()
    {
        List<DecisionData> sequence = new List<DecisionData>();
        List<SpawnablePrefab> prefab = new List<SpawnablePrefab>();
        DecisionData last = null;

        // First part: no consecutive repeats
        for (int i = 0; i < _noRepeatCount/2; i++)
        {
            DecisionData next = GetRandomDifferent(last);
            sequence.Add(next);
            last = next;
        }
        _spawner.SetPrefabs(sequence);

        for (int i = _noRepeatCount / 2; i < _noRepeatCount; i++)
        {
            DecisionData next = GetRandomDifferent(last);
            sequence.Add(next);
            last = next;
        }

        // Fill the rest (can have repeats)
        for (int i = _noRepeatCount; i < _sequenceLength; i++)
        {
            sequence.Add(GetRandomAny());
        }
        _currentSequence = new Queue<DecisionData>(sequence);
        _index = 0;

        //EventManager.Instance.OnSetNextDecision(GetNextDecision());

    }

    public DecisionData GetNextDecision()
    {
        if (_currentSequence == null || _currentSequence.Count == 0)
            GenerateSequence();

        _index++;
        return _currentSequence.Dequeue();
    }

    private DecisionData GetRandomAny()
    {
        return _decisionPrefabs[Random.Range(0, _decisionPrefabs.Count)];
    }

    private DecisionData GetRandomDifferent(DecisionData last)
    {
        DecisionData next;
        do
        {
            next = GetRandomAny();
        }
        while (next == last);
        return next;
    }
}
