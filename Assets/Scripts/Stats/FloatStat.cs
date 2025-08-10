using System;


/// <summary>
/// A concrete implementation of a Stat with a float type value.
/// </summary>
public class FloatStat : Stat<float>
{
    public FloatStat(string name, float max, Action onComplete, float current = 0) : base(name, max,  onComplete, current) { }

}
