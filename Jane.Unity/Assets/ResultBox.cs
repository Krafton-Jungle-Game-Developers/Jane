using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultBox : MonoBehaviour
{
    [SerializeField] private Image rankBox;
    [SerializeField] private Image tailBox;
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text standingsText;
    [SerializeField] private TMP_Text timeText;

    public void ChangeColor(Color changeColor)
    {
        rankBox.color = changeColor;
        tailBox.color = changeColor;
    }
    public void ChangeRank(string changeText)
    {
        rankText.text = changeText;
    }
    public void ChangeStandings(string changeText)
    {
        standingsText.text = changeText;
    }
    public void RecordTime(long tick)
    {
        TimeSpan time = TimeSpan.FromTicks(tick);
        timeText.text = $"{time.Minutes:00}:{time.Seconds:00}.{time.Milliseconds:000}";
    }
}
