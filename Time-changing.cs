using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public enum DayCycles // Enum with day and night cycles, you can change or modify with whatever you want
{
    Morning = 0,
    Day = 1,
    Evening = 2,
    Night = 3
}


public class DayNightSystem2D : MonoBehaviour
{
    [Header("Controllers")]

    [Tooltip("Global light 2D component, we need to use this object to place light in all map objects")]
    public UnityEngine.Rendering.Universal.Light2D globalLight; // global light

    [Tooltip("This is a current cycle time, you can change for private float but we keep public only for debug")]
    private int cycleCurrentTime; // current cycle time


    [Tooltip("Enum with multiple day cycles to change over time, you can add more types and modify whatever you want to fits on your project")]
    public DayCycles dayCycle = DayCycles.Morning; // default cycle

    [Header("Cycle Colors")]


    public Color morning; // Eg: 6:00 at 10:00

    [Tooltip("(Mid) Day color, you can adjust based on best color for this cycle")]
    public Color day; // Eg: 10:00 at 18:00

    [Tooltip("Sunset color, you can adjust based on best color for this cycle")]
    public Color evening; // Eg: 18:00 22:00

    [Tooltip("Night color, you can adjust based on best color for this cycle")]
    public Color night; // Eg: 22:00 at 06:00



    [Header("Objects")]
    [Tooltip("Objects to turn on and off based on day night cycles, you can use this example for create some custom stuffs")]
    public UnityEngine.Rendering.Universal.Light2D[] mapLights; // enable/disable in day/night states

    void Start()
    {
        Circle_Movement();
        float time_div = (10 - (DateTime.Now.Minute % 10)) * 60 + (60 - DateTime.Now.Second) * 1.0f;
        InvokeRepeating("Circle_Movement", time_div, 600.0f);
    }

    void Circle_Movement()
    {
        // Update cycle time
        Set_Daycycle();

        float max = 1.0f, curr = 1.0f;

        if (dayCycle == DayCycles.Day && dayCycle == DayCycles.Night)
        {
            max = 28800.0f;
        }
        else
        {
            max = 14400.0f;
        }
        curr = Set_Interval();

        // percent it's an value between current and max time to make a color lerp smooth
        float percent = curr / max;

        // Sunrise state (you can do a lot of stuff based on every cycle state, like enable animals only in sunrise )
        if (dayCycle == DayCycles.Morning)
        {
            ControlLightMaps(false); // disable map light (keep enable only at night)
            globalLight.color = Color.Lerp(morning, day, percent);
        }

        // Mid Day state
        if (dayCycle == DayCycles.Day)
        {
            globalLight.color = Color.Lerp(day, evening, percent);
        }

        // Sunset state
        if (dayCycle == DayCycles.Evening)
        {
            globalLight.color = Color.Lerp(evening, night, percent);
        }

        // Night state
        if (dayCycle == DayCycles.Night)
        {
            ControlLightMaps(true); // enable map lights (disable only in day states)
            globalLight.color = Color.Lerp(night, morning, percent);
        }
    }

    void ControlLightMaps(bool status)
    {
        // loop in light array of objects to enable/disable
        if (mapLights.Length > 0)
            foreach (UnityEngine.Rendering.Universal.Light2D _light in mapLights)
                _light.gameObject.SetActive(status);
    }


    void Set_Daycycle()
    {
        cycleCurrentTime = DateTime.Now.Hour;

        if (cycleCurrentTime >= 6 && cycleCurrentTime < 10)
        {
            dayCycle = DayCycles.Morning;
        }
        else if (cycleCurrentTime >= 10 && cycleCurrentTime < 18)
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
            if (hour < 6)
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
            ans = (hour - 10) * 3600.0f + minute * 60;
        }
        else if (dayCycle == DayCycles.Morning)
        {
            ans = (hour - 6) * 3600.0f + minute * 60;
        }
        else
        {
            ans = (hour - 18) * 3600.0f + minute * 60;
        }

        return ans;
    }
}