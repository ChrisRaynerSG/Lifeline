using System;
using TMPro;
using UnityEngine;

public class DayController : MonoBehaviour{
    public GameObject Sun;
    public float time;
    public int hour;
    public int minute;

    public float sunSpeed = 0.25f;

    public static event Action<float> OnTimeOfDayChanged; // Event to notify subscribers about the time of day change

    // will want to add design pattern for showing time of day elsewhere later but for now will display here.

    public TextMeshProUGUI timeOfDayText;

    public void Start()
    {
        Sun.transform.rotation = Quaternion.Euler(0, 0, 0);
        timeOfDayText.text = "00:00";
        time = 90;
    }

    public void Update()
    {
        if(Time.timeScale == 0)
        {
            return; // Skip the update if the game is paused
        }

        Sun.transform.Rotate(Vector3.right * Time.deltaTime * sunSpeed, Space.World);
        if (Sun.transform.rotation.eulerAngles.x >= 360)
        {
            Sun.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        time += Time.deltaTime * sunSpeed;
        // how do I get the time in hours and minutes from 360?
        if (time >= 360)
        {
            time = 0;
        }
        hour = (int)(time / 15); // 360 degrees / 24 hours = 15 degrees per hour
        minute = (int)((time % 15) * 4); // 30 degrees per hour / 60 minutes = 0.5 degrees per minute
        
        timeOfDayText.text = hour.ToString("00") + ":" + minute.ToString("00");
        //do i just do the calculation for hours and minutes here and notify subscribers from here or do I send the time and let them do the calculation? 
        OnTimeOfDayChanged?.Invoke(time); // Notify subscribers about the time of day change

    }
}