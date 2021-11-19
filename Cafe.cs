using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Cafe : MonoBehaviour
{

	public static int min_interval;
	public static int max_interval;
	public static int money;
	public static Dictionary<string, Recipe> recipes;
	public static Dictionary<string, int> warehouse;
	public static Dictionary<string, int> shop;
	public static Dictionary<string, int> preparedOrders;

	public class Recipe
	{

		public int price { get; set; }
		public int preparationTime { get; set; }
		public List<string> ingredients { get; set; }

        public override string ToString()
        {
			string ing = " ";
			foreach(var i in ingredients)
            {
				ing = ing + i + " ";
            }
			return price.ToString() + "," + preparationTime.ToString() + ing;

		}
    }

	void UpdateWarehouse (string name, int amount)
	{
		// добавление на склад определенного кол-ва ингредиентов если такого ингредиента нет в словаре он создается
		int available;
		if (warehouse.TryGetValue(name, out available))
		{
			warehouse[name] += amount;
		}
		else
		{
			warehouse.Add(name, amount);
		}
	}
	public int isInWarehouse(string name)
    {
		return warehouse[name];
	}
	public void checkWarehouse()
	{
		foreach(GameObject materialBlock in GameObject.FindGameObjectsWithTag("materialBlock"))
		{
			Transform mat = materialBlock.gameObject.transform.Find("materialName");
			string materialName = mat.GetComponent<Text>().text;

			int num = GameObject.Find("cafe").GetComponent<Cafe>().isInWarehouse(materialName);
			if (num!=0)
			{
				materialBlock.gameObject.transform.Find("materialAmount").GetComponent<Text>().text = num.ToString();
			}
		}
	}

	public void toggleWarehouse()
	{
		GameObject warehouse = GameObject.Find("warehouse"); 
		if (warehouse.activeSelf)
		{
			gameObject.SetActive(false);
		}
		else
		{
			gameObject.SetActive(true);
			checkWarehouse();
		}
	}

	public void Buy(string name, int amount, int price)
	{
		// покупка определенного числа ингредиентов надо сделать чтобы пользователь сам вводил сколько он хочет купить продуктов
		// если пользователь пытается купить продуктов больше чем может то покупка совершается только на  то число которое может себе позволить пользователь
		if (amount * price <= money)
		{
			UpdateWarehouse(name, amount);
			money -= amount * price;
			GameObject.Find("WalletCoins").GetComponent<Text>().text = money.ToString();
		}
		else
		{
			UpdateWarehouse(name,(int)(money / price));
			money -= (int)(money / price) * price;
			//надо бы вывести соообщение что недостачно средств для покупки всех но я хз как лучше это сделать
		}
	}

	public void BuyMaterials()
	{
		GameObject[] materialBlocks = GameObject.FindGameObjectsWithTag("materialBlock");

		foreach (GameObject block in materialBlocks)
		{
			string materialName = block.transform.Find("materialName").GetComponent<Text>().text;
			Transform mat = block.transform.Find("materialAmount");
			int numOfMaterial = int.Parse(mat.GetComponent<Text>().text);
			if (numOfMaterial != 0)
			{
				Buy(materialName, numOfMaterial, getMaterialPrice(materialName));
			}
		}
	}
	public void resetShop()
	{
		foreach (GameObject materialBlock in GameObject.FindGameObjectsWithTag("materialBlock"))
		{
			Transform mat = materialBlock.gameObject.transform.Find("materialName");
			string materialName = mat.GetComponent<Text>().text;
			materialBlock.gameObject.transform.Find("materialAmount").GetComponent<Text>().text = "0";
		}
	}
	string MakeAnOrder()
	{
		List<string> keyList = new List<string>(recipes.Keys);
		System.Random rnd = new System.Random();
		return keyList[rnd.Next(keyList.Count)];
	}

	public void PrepareAnOrder(string name)
	{

		if(GameObject.Find(String.Format("cookButton {0}", name)).GetComponent<BoxCollider2D>().enabled == true)
        {
			List<string> keyList = recipes[name].ingredients;
			foreach (string ingredient in keyList)
			{
				warehouse[ingredient] -= 1;
			}

			StartCoroutine(ExampleCoroutine());
			System.Collections.IEnumerator ExampleCoroutine()
			{
				GameObject smoke = GameObject.Find("cookingSmoke");
				ParticleSystem ps = smoke.GetComponent<ParticleSystem>();
				ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
				var main = ps.main;
				main.duration = recipes[name].preparationTime;
				ps.Play();
				
				yield return new WaitForSecondsRealtime(recipes[name].preparationTime);

				if (preparedOrders.ContainsKey(name))
					preparedOrders[name] += 1;
				else
					preparedOrders.Add(name, 1);
			}
		}
        else
        {
			List<string> keyList = recipes[name].ingredients;
			Transform materials = GameObject.Find(String.Format("recipeBlock {0}", name)).transform.Find("materials");
			foreach (string ingredient in keyList)
			{
                if (isInWarehouse(ingredient) == 0)
                {
					Text materialText = materials.Find(String.Format("material {0}", ingredient)).transform.Find("materialName").GetComponent<Text>();
					StartCoroutine(NoMaterialNotify(materialText.color, Color.red, materialText));
				}
				
			}
		}
	}
    System.Collections.IEnumerator NoMaterialNotify(Color startColor, Color endColor, Text text)   /// вынести в отдельный скрипт
	{
		float duration = 0.5f; 
		float smoothness = 0.02f; 
		float progress = 0; 
		float increment = smoothness / duration;

		bool dirRight = true;
		float speed = 0.5f;
		float startX = text.transform.position.x;
		while (progress < 0.5f)
		{
			text.color = Color.Lerp(startColor, endColor, progress);
			progress += increment;
			
			text.transform.Translate((dirRight?1:-1) * Vector2.right * speed);
			dirRight = (text.transform.position.x < startX + 1 && text.transform.position.x > startX - 1) ? dirRight : !dirRight;

			yield return new WaitForSeconds(smoothness);
		}
		while (progress < 1)
		{
			text.color = Color.Lerp(endColor, startColor, progress);
			progress += increment;

			yield return new WaitForSeconds(smoothness);
		}
	}
	public void StartClients()
    {
		StartCoroutine(ClientsCoroutine());
		System.Collections.IEnumerator ClientsCoroutine()
		{
            System.Random r = new System.Random();
			//int firstClient = r.Next(30, this.min_interval);
			int firstClient = r.Next(3, 10);

			yield return new WaitForSecondsRealtime(firstClient);

			newClientStream();
			//newClientStream();

		}
	}
	public void newClientStream()  // поток клиентов (пока просто отобржате чела и заказ)
    {
		GameObject newClient = GameObject.Find("client");  // сделать создание gameobject с рандомным спрайтом и тд
		newClient.GetComponent<Renderer>().enabled = true;  //сделать анимацию прихода клиента с одной из сторон
		string order = MakeAnOrder();
		client c = newClient.AddComponent<client>();
		c.Order = order; 
		c.createOrderImage(order);

	}
	void UpdateTimeInterval(int min, int max)
	{
		// изменяем максимальный и минимальный интервал между посетителями 
		max_interval = max;
		min_interval = min;
	}

	public int getMaterialPrice(string name)
    {
		return shop[name];
    }



	void Start()
	{
		min_interval = 2 * 60;
		max_interval = 20 * 60;
		money = int.Parse(GameObject.Find("WalletCoins").GetComponent<Text>().text);
		warehouse = new Dictionary<string, int> {   // сделать чтением и записью в playerPrefs
				{ "Soy Sauce", 0 },
				{ "Cabbage", 0 },
				{ "Salt", 2}
		};

		TextAsset jsonMaterials = Resources.Load<TextAsset>("DB/materials");
		shop = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonMaterials.text);

		TextAsset jsonRecipes = Resources.Load<TextAsset>("DB/recipes");
		recipes = JsonConvert.DeserializeObject<Dictionary<string, Recipe>>(jsonRecipes.text);	

		preparedOrders = new Dictionary<string, int> {  //тоже в playerPrefs, со сроком годности подумать
				{ "Ramen", 0 },
				{ "Miso ramen", 0 }
		};



		StartClients();


	}
}
