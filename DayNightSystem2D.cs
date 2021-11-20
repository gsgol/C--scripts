using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public enum DayCycles
{
    Morning = 0,
    Day = 1,
    Evening = 2,
    Night = 3
}


public class DayNightSystem2D : MonoBehaviour
{
    [Header("Controllers")]

    public UnityEngine.Rendering.Universal.Light2D globalLight; // global light

    private int cycleCurrentTime;

    [Tooltip("Enum with multiple day cycles to change over time")]
    private DayCycles dayCycle; 

    [Header("Cycle Colors")]
    public Color morning; // Eg: 5:00 to 9:00
    public Color day; // Eg: 9:00 to 18:00
    public Color evening; // Eg: 18:00 to 22:00
    public Color night; // Eg: 22:00 to 05:00

    [Header("Objects")]
    [Tooltip("Objects to turn on and off based on day night cycles")]
    public UnityEngine.Rendering.Universal.Light2D[] mapLights; 

    void Start()
    {
        Circle_Movement();
        InvokeRepeating("Circle_Movement", 60.0f, 60.0f);
    }

    void Circle_Movement()
    {
        // Update cycle time
        Set_Daycycle();
       // print("cycleTime " + cycleCurrentTime + " daycycle " + dayCycle);

        float max = 1.0f, curr = 1.0f;

        if (dayCycle == DayCycles.Day || dayCycle == DayCycles.Night)
        {
            max = 28800.0f;  //doesnt fucking matter man but пусть будет на потом
        }
        else
        {
            max =7200.0f;
        }
        curr = Set_Interval();

        float percent = curr / max;
        //print(string.Format("{0} / {1} = {2}", curr, max, percent));

        if (dayCycle == DayCycles.Morning)
        {
            if (DateTime.Now.Hour < 7)
            {
                ControlLightMaps(true);
                globalLight.color = Color.Lerp(night, morning, percent);
            }
            else
            {
                ControlLightMaps(false);
                globalLight.color = Color.Lerp(morning, day, percent);
            }
            
        }

        if (dayCycle == DayCycles.Day)
        {
            ControlLightMaps(false);
            globalLight.color = day;
        }

        if (dayCycle == DayCycles.Evening)
        {
            if (DateTime.Now.Hour < 20)
            {
                ControlLightMaps(false);
                globalLight.color = Color.Lerp(day, evening, percent);
            }
            else
            {
           
                ControlLightMaps(true);
                globalLight.color = Color.Lerp(evening, night, percent);
            }
        }

        if (dayCycle == DayCycles.Night)
        {
            ControlLightMaps(true); 
            globalLight.color = night;

        }
    }

    void ControlLightMaps(bool status)
    {
        if (mapLights.Length > 0)
            foreach (UnityEngine.Rendering.Universal.Light2D _light in mapLights)
                _light.gameObject.SetActive(status);
    }


    void Set_Daycycle()
    {
        cycleCurrentTime = DateTime.Now.Hour;

        if (cycleCurrentTime >= 5 && cycleCurrentTime < 9)
        {
            dayCycle = DayCycles.Morning;
        }
        else if (cycleCurrentTime >= 9 && cycleCurrentTime < 18)
        {
            dayCycle = DayCycles.Day;
        }
        else if (cycleCurrentTime >= 18 && cycleCurrentTime < 22)
        {
            dayCycle = DayCycles.Evening;
        }
        else
        {
            dayCycle = DayCycles.Night;
        }
    }


    float Set_Interval()
    {
        int hour = DateTime.Now.Hour;
        int minute = DateTime.Now.Minute;
        float ans = 1.0f;
        if (dayCycle == DayCycles.Night)
        {
            if (hour < 5)
            {
                ans = (hour + 2) * 3600.0f + minute * 60;
            }
            else
            {
                ans = (hour - 22) * 3600.0f + minute * 60;
            }

        }
        else if (dayCycle == DayCycles.Day)
        {
            ans = (hour - 9) * 3600.0f + minute * 60;
        }
        else if (dayCycle == DayCycles.Morning)
        {
            if (hour >= 7)
                ans = (hour - 7) * 3600.0f + minute * 60;
            else
                ans = (hour - 5) * 3600.0f + minute * 60;
        }
        else
        {
            if(hour>=20)
                ans = (hour - 20) * 3600.0f + minute * 60;
            else
                ans = (hour - 18) * 3600.0f + minute * 60;
        }

        return ans;
    }
}