using System;
using System.Collections.Generic;

/// <summary>
/// A handler to manage multiple stats.
/// </summary>
/// <typeparam name="T">The type of the stat value (e.g., float, int, etc.).</typeparam>
public class StatHandler<T> where T : struct, IComparable, IConvertible
{
    private readonly List<Stat<T>> stats;

    /// <summary>
    /// Initializes a new StatHandler.
    /// </summary>
    public StatHandler()
    {
        stats = new List<Stat<T>>();
    }

    /// <summary>
    /// Adds a stat to the handler.
    /// </summary>
    /// <param name="stat">The stat to be added.</param>
    public void AddStat(Stat<T> stat)
    {
        stats.Add(stat);
    }

    /// <summary>
    /// Removes a stat from the handler.
    /// </summary>
    /// <param name="stat">The stat to be removed.</param>
    public void RemoveStat(Stat<T> stat)
    {
        stats.Remove(stat);
    }

    /// <summary>
    /// Finds a stat by its name.
    /// </summary>
    /// <param name="statName">The name of the stat.</param>
    /// <returns>The stat with the given name, or null if not found.</returns>
    public Stat<T> GetStat(string statName)
    {
        return stats.Find(s => s.StatName == statName);
    }

    /// <summary>
    /// Modifies the value of a stat by its name.
    /// </summary>
    /// <param name="statName">The name of the stat to modify.</param>
    /// <param name="amount">The amount to modify the stat by.</param>
    public void ModifyStat(string statName, T amount)
    {
        Stat<T> stat = GetStat(statName);
        if (stat != null)
        {
            stat.ModifyStat(amount);
        }
    }

    /// <summary>
    /// Returns all stats managed by the handler.
    /// </summary>
    public List<Stat<T>> GetAllStats()
    {
        return stats;
    }
}
