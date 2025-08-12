using UnityEngine.UI;
using UnityEngine;

public class TimerBar : MonoBehaviour
{
    [SerializeField] private Image[] _shameSlider;
    void Start()
    {
       EventBus.Subscribe<ModifyDecisionTimeStat>((stat) => Modify(stat.Amount));

    }
    private void OnEnable()
    {
        for (int i = 0; i < _shameSlider.Length; i++)
        {
            _shameSlider[i].fillAmount = 1;
        }
    }
    void Modify(float amount)
    {
        for (int i = 0; i < _shameSlider.Length; i++)
        {
            _shameSlider[i].fillAmount = 1- amount;
        }
    }
}
