using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStatHandler : MonoBehaviour
{
    [SerializeField] private Image[] _shameSlider;
    [SerializeField] private Image[] _decisionsSlider;
    [SerializeField] private Image[] _distanceSlider;

    private StatHandler<float> statHandler;
    private ProgressBarStatHandler _shameStatHandler;
    private ProgressBarStatHandler _distanceStatHandler;
    private ProgressBarStatHandler _decisionsStatHandler;

    private void Start()
    {
        // Create stats
        var shameStat = new Stat<float>("Shame", 100f,EventManager.Instance.OnLost, 0f);
        var distanceStat = new Stat<float>("Distance", 100f, () => { } ,0f);
        var decisionsStat = new Stat<float>("Decisions", 100f, () => { }, 0f);

        // Create the stat handler and add stats
        statHandler = new StatHandler<float>();
        statHandler.AddStat(shameStat);
        statHandler.AddStat(distanceStat);
        statHandler.AddStat(decisionsStat);

        // Create the ProgressBarStatHandler and initialize it with stats and sliders
        _shameStatHandler = new ProgressBarStatHandler(shameStat, _shameSlider);
        _distanceStatHandler = new ProgressBarStatHandler(distanceStat, _decisionsSlider);
        _decisionsStatHandler = new ProgressBarStatHandler(decisionsStat, _distanceSlider);
        
        _shameStatHandler.UpdateProgressBars();
        _distanceStatHandler.UpdateProgressBars();
        _decisionsStatHandler.UpdateProgressBars();

        //EventBus.Subscribe<ModifyShameStat>((stat)=> _shameStatHandler.ModifyStat(stat.Amount));
        //EventBus.Subscribe<ModifyDecisionTimeStat>((stat)=> _decisionsStatHandler.ModifyStat(stat.Amount));
        //EventBus.Subscribe<ModifyDistanceTimeStat>((stat)=> _distanceStatHandler.ModifyStat(stat.Amount));

    }

}
