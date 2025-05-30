using System;
using TMPro;
using UnityEngine;

public class CoundownTimer : MonoBehaviour
{
    public enum CountdownFormatting { DaysHoursMinutesSeconds, HoursMinutesSeconds, MinutesSeconds, Seconds };
    public CountdownFormatting countdownFormatting = CountdownFormatting.MinutesSeconds;
    public bool showMilliseconds = true;
    public double countdownTime = 0; 

    public TMP_Text countdownText;
    public static double countdownInternal;
    public bool countdownOver = false;
    public bool isStart = false;

    public void EmpezarContador()
    {
        countdownTime = int.Parse(Caso.caso.tiempoRestante) * 60;
        countdownInternal = countdownTime;
        countdownOver = false; 
        isStart = true;
    }

    public void ReiniciarContador()
    {
        countdownInternal = countdownTime;
        countdownOver = false;
    }

    void FixedUpdate()
    {
        if (isStart) 
        {
            if (countdownInternal > 0)
            {
                countdownInternal -= Time.deltaTime;

                if (countdownInternal < 0)
                {
                    countdownInternal = 0;
                }

                countdownText.text = FormatTime(countdownInternal, countdownFormatting, showMilliseconds);
            }
            else
            {
                if (!countdownOver)
                {
                    countdownOver = true;
                }
            }
        }
    }

    public string FormatTime(double time, CountdownFormatting formatting, bool includeMilliseconds)
    {
        string timeText = "";

        int intTime = (int)time;
        int days = intTime / 86400;
        int hoursTotal = intTime / 3600;
        int hoursFormatted = hoursTotal % 24;
        int minutesTotal = intTime / 60;
        int minutesFormatted = minutesTotal % 60;
        int secondsTotal = intTime;
        int secondsFormatted = intTime % 60;
        int milliseconds = (int)(time * 100);
        milliseconds = milliseconds % 100;

        if (includeMilliseconds)
        {
            if (formatting == CountdownFormatting.DaysHoursMinutesSeconds)
            {
                timeText = string.Format("{0:00}:{1:00}:{2:00}:{3:00}:{4:00}", days, hoursFormatted, minutesFormatted, secondsFormatted, milliseconds);
            }
            else if (formatting == CountdownFormatting.HoursMinutesSeconds)
            {
                timeText = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", hoursTotal, minutesFormatted, secondsFormatted, milliseconds);
            }
            else if (formatting == CountdownFormatting.MinutesSeconds)
            {
                timeText = string.Format("{0:00}:{1:00}:{2:00}", minutesTotal, secondsFormatted, milliseconds);
            }
            else if (formatting == CountdownFormatting.Seconds)
            {
                timeText = string.Format("{0:00}:{1:00}", secondsTotal, milliseconds);
            }
        }
        else
        {
            if (formatting == CountdownFormatting.DaysHoursMinutesSeconds)
            {
                timeText = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", days, hoursFormatted, minutesFormatted, secondsFormatted);
            }
            else if (formatting == CountdownFormatting.HoursMinutesSeconds)
            {
                timeText = string.Format("{0:00}:{1:00}:{2:00}", hoursTotal, minutesFormatted, secondsFormatted);
            }
            else if (formatting == CountdownFormatting.MinutesSeconds)
            {
                timeText = string.Format("{0:00}:{1:00}", minutesTotal, secondsFormatted);
            }
            else if (formatting == CountdownFormatting.Seconds)
            {
                timeText = string.Format("{0:00}", secondsTotal);
            }
        }

        return timeText;
    }
}
