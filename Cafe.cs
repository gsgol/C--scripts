using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Cafe : MonoBehaviour
{

	int min_interval;
	int max_interval;
	int money;
	public Dictionary<string, Recipe> recipes;
	public Dictionary<string, int> warehouse;
	public Dictionary<string, int> shop;

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
		if (this.warehouse.TryGetValue(name, out available))
		{
			this.warehouse[name] += amount;
		}
		else
		{
			this.warehouse.Add(name, amount);
		}
	}
	public int isInWarehouse(string name)
    {
		return this.warehouse[name];
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
		if (amount * price <= this.money)
		{
			UpdateWarehouse(name, amount);
			this.money -= amount * price;
			GameObject.Find("WalletCoins").GetComponent<Text>().text = this.money.ToString();
		}
		else
		{
			UpdateWarehouse(name,(int)(this.money / price));
			this.money -= (int)(this.money / price) * price;
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
		// выбирается 1 из рецептов из словаря
		List<string> keyList = new List<string>(this.recipes.Keys);
		System.Random rnd = new System.Random();
		return keyList[rnd.Next(keyList.Count)];
	
	}

	public void PrepareAnOrder(string name)
	{

		if(GameObject.Find(String.Format("cookButton {0}", name)).GetComponent<BoxCollider2D>().enabled == true)
        {
			List<string> keyList = this.recipes[name].ingredients;
			foreach (string ingredient in keyList)
			{
				print(ingredient);
				this.warehouse[ingredient] -= 1;
			}

			StartCoroutine(ExampleCoroutine());
			System.Collections.IEnumerator ExampleCoroutine()
			{
				GameObject smoke = GameObject.Find("cookingSmoke");
				smoke.GetComponent<Renderer>().enabled = true;
				print(1);

				yield return new WaitForSecondsRealtime(this.recipes[name].preparationTime);

				print(2);
				smoke.GetComponent<Renderer>().enabled = false;

				money += this.recipes[name].price;
				GameObject.Find("WalletCoins").GetComponent<Text>().text = this.money.ToString();
			}
		}
        else
        {
			//globalLight.color = Color.Lerp(sunrise, morning, percent)

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
		GameObject client = GameObject.Find("client");  // сделать создание gameobject с рандомным спрайтом и тд
		client.GetComponent<Renderer>().enabled = true;  //сделать анимацию прихода клиента с одной из сторон
		string order = MakeAnOrder();


		/////////////вынести в отдельный метод
		GameObject imgObject = new GameObject("order");
		imgObject.transform.SetParent(GameObject.Find("Canvas").transform);
		imgObject.transform.SetSiblingIndex(0);

		imgObject.AddComponent<CanvasRenderer>();
		RectTransform trans = imgObject.AddComponent<RectTransform>();
		trans.localScale = Vector3.one;
		trans.anchoredPosition = new Vector2(195.4f, 26.7f); // setting position
		trans.sizeDelta = new Vector2(32, 32); // custom size

		Image image = imgObject.AddComponent<Image>();
		Texture2D tex = Resources.Load<Texture2D>(String.Format("recipes/{0}", order));
		image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
		/////////////////////
	}
	void UpdateTimeInterval(int min, int max)
	{
		// изменяем максимальный и минимальный интервал между посетителями 
		this.max_interval = max;
		this.min_interval = min;
	}

	public int getMaterialPrice(string name)
    {
		return this.shop[name];
    }

	void Start()
	{
		this.min_interval = 2 * 60;
		this.max_interval = 20 * 60;
		this.money = int.Parse(GameObject.Find("WalletCoins").GetComponent<Text>().text);
		this.warehouse = new Dictionary<string, int> {
				{ "Soy Sauce", 0 },
				{ "Cabbage", 0 },
				{ "Salt", 0}
		};

		using (StreamReader r = new StreamReader("Assets/DB/materials.json"))
		{
			string json = r.ReadToEnd();
			this.shop = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);

		}

		using (StreamReader r = new StreamReader("Assets/DB/recipes.json"))
		{
			string json = r.ReadToEnd();
			this.recipes = JsonConvert.DeserializeObject<Dictionary<string, Recipe>>(json);
		}

		StartClients();


	}
}
