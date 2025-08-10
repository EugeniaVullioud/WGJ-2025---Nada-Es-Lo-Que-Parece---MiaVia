using System;
using System.Collections.Generic;
using UnityEngine;

// <summary>
/// Represents a generic stat with a name, a current value, and a maximum value.
/// The stat can be of any type that supports the basic arithmetic operations.
/// </summary>
/// <typeparam name="T">The type of the stat value (e.g., float, int, etc.).</typeparam>
[System.Serializable]
public class Stat<T> where T : struct, IComparable, IConvertible
{
    /// <summary>
    /// The name of the stat.
    /// </summary>
    public string StatName { get; private set; }

    /// <summary>
    /// The current value of the stat.
    /// </summary>
    public T CurrentValue { get; private set; }

    /// <summary>
    /// The maximum value the stat can have.
    /// </summary>
    public T MaxValue { get; private set; }

    Action _onComplete =delegate{ };
    /// <summary>
    /// Initializes a new stat with a name, maximum value, and an optional current value (defaulting to 0).
    /// </summary>
    /// <param name="name">The name of the stat.</param>
    /// <param name="max">The maximum value of the stat.</param>
    /// <param name="current">The current value of the stat (defaults to 0).</param>
    public Stat(string name, T max, Action onComplete, T current = default(T))
    {
        StatName = name;
        MaxValue = max;
        _onComplete = onComplete;
        CurrentValue = (Comparer<T>.Default.Compare(current, max) > 0) ? max : current;
    }

    /// <summary>
    /// Modifies the current value of the stat by a given amount, clamping it between 0 and the max value.
    /// </summary>
    /// <param name="amount">The amount to change the stat by.</param>
    public void ModifyStat(T amount)
    {
        if (typeof(T) == typeof(float))
        {
            float current = Convert.ToSingle(CurrentValue);
            float max = Convert.ToSingle(MaxValue);
            float delta = Convert.ToSingle(amount);
            float clamped = Mathf.Clamp(current + delta, 0, max);
            CurrentValue = (T)(object)clamped;
            if (Math.Abs(max - clamped) < 0.05) _onComplete();
        }
        else if (typeof(T) == typeof(int))
        {
            int current = Convert.ToInt32(CurrentValue);
            int max = Convert.ToInt32(MaxValue);
            int delta = Convert.ToInt32(amount);
            int clamped = Mathf.Clamp(current + delta, 0, max);
            CurrentValue = (T)(object)clamped;
            if (Math.Abs(max - clamped) < 0.05) _onComplete();

        }
        else
        {
            throw new InvalidOperationException($"Unsupported type {typeof(T)} in ModifyStat");
        }
    }

    /// <summary>
    /// Resets the stat's current value to zero or the default value for the type.
    /// </summary>
    public void ResetStat()
    {
        CurrentValue = default(T);
    }
}
