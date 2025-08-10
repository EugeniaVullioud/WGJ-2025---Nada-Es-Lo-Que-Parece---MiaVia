using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A stat handler that updates a list of progress bars (sliders) in Unity based on the current stat values.
/// </summary>
public class ProgressBarStatHandler
{
    private Stat<float> _stat; // List of stats to manage (typically float stats)
    private Slider[] progressBars; // List of sliders (progress bars) to update

    /// <summary>
    /// Initializes the ProgressBarStatHandler with stats and corresponding sliders.
    /// </summary>
    /// <param name="stat">A list of stats to manage.</param>
    /// <param name="progressBars">A list of sliders (progress bars) that correspond to the stats.</param>
    public ProgressBarStatHandler(Stat<float> stat, Slider[] progressBars)
    {
        this._stat = stat ?? throw new System.ArgumentNullException(nameof(stat), "Stat cannot be null");
        this.progressBars = progressBars ?? throw new System.ArgumentNullException(nameof(progressBars), "Progress bars cannot be null");
    }

    /// <summary>
    /// Updates all progress bars based on the current stat values.
    /// </summary>
    public void UpdateProgressBars()
    {
        // Loop through each progress bar and corresponding stat
        for (int i = 0; i < progressBars.Length; i++)
        {
            Slider slider = progressBars[i];
            // Update the slider value based on the current value of the stat
            slider.value = _stat.CurrentValue / _stat.MaxValue;
        }
    }

    /// <summary>
    /// Modify the stat values by a given amount.
    /// </summary>
    /// <param name="statName">The stat to modify.</param>
    /// <param name="amount">The amount to modify the stat by.</param>
    public void ModifyStat(float amount)
    {      
       _stat.ModifyStat(amount);
        UpdateProgressBars();
    }
   
    /// <summary>
    /// Resets the value of all stats to their initial values.
    /// </summary>
    public void ResetStats()
    {
        _stat.ResetStat();
        UpdateProgressBars();
    }
}
