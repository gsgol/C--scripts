using UnityEngine;
using System.Collections;
using System;

public class DataMaster : MonoBehaviour
{
    DateTime currentDate;
    DateTime oldDate;
	public static long time_difference;
    void Set_Time_Difference()
    {
        //Store the current time when it starts
        currentDate = System.DateTime.Now;

        //Grab the old time from the player prefs as a long
        long temp = Convert.ToInt64(PlayerPrefs.GetString("sysString"));

        //Convert the old time from binary to a DataTime variable
        DateTime oldDate = DateTime.FromBinary(temp);

        //Use the Subtract method and store the result as a timespan variable
        TimeSpan difference = currentDate.Subtract(oldDate);

		time_difference = Convert.ToInt64(difference);
    }

    void OnApplicationQuit()
    {
        //Savee the current system time as a string in the player prefs class
        PlayerPrefs.SetString("sysString", System.DateTime.Now.ToBinary().ToString());
    }
	bool Is_Possible(string name)
	{
		List<string> keyList = new List<string>(this.recipes[name].ingredients.Keys);
		foreach (string ingredient in keyList)
		{
			if (this.warehouse[ingredient] == 0)
			{
				return false;
			}
		}
		return true;
	
	}

	void OldPrepareAnOrder(string name)
	{
		List<string> keyList = new List<string>(this.recipes[name].ingredients.Keys);

		foreach (string ingredient in keyList)
		{
			this.warehouse[ingredient] -= 1;
		}
		money += this.recipes[name].price;
	}
	
	void Offline_Orders()
	{
		Random rnd = new Random();
		int num_of_clients = rnd.Next(time_difference / (max_interval * 2), time_difference / max_interval);
		for (int i = 0; i < num_of_clients; ++i)
		{
			string name = MakeAnOrder();
			if(Is_Possible(name))
			{
				OldPrepareAnOrder(name);
			}
			else
			{
				break;
			}


		}
	}



}
