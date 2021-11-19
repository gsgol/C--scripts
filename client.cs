using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class client : MonoBehaviour
{
	public string Order { get; set; }

	public void giveOrder(string order, GameObject orderCloud)
	{
		if (Cafe.preparedOrders.ContainsKey(order) && Cafe.preparedOrders[order]!=0)
		{
			Cafe.preparedOrders[order] -= 1;
			Cafe.money += Cafe.recipes[order].price;
			GameObject.Find("WalletCoins").GetComponent<Text>().text = Cafe.money.ToString();
			this.gameObject.GetComponent<Renderer>().enabled = false;
			Destroy(orderCloud);
			Destroy(this.gameObject);
			GameObject.Find("cafe").GetComponent<Cafe>().SummonClient();
		}
	}

	public static GameObject createClientSprite(int side, int clientSprite) //0 - left; 3 - right (x coordinate)

	{
		GameObject client = new GameObject(string.Format("client{0}",side));
		client.transform.SetParent(GameObject.Find("foreground").transform);

		client.AddComponent<CanvasRenderer>();
		RectTransform trans = client.AddComponent<RectTransform>();
		trans.localScale = 5 * Vector3.one;
		trans.anchoredPosition = new Vector2(side, -0.6f); // setting position
		trans.sizeDelta = new Vector2(64, 128); // custom size

		SpriteRenderer sprite = client.AddComponent<SpriteRenderer>();
		sprite.sortingLayerName = "foreground";
		Texture2D tex = Resources.Load<Texture2D>(string.Format("sprites/chars/client{0}", clientSprite));
		sprite.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

		client.AddComponent<client>();

		return client;
	}


		public void createOrderImage(string order, int side)
	{
		GameObject orderCloud = new GameObject(string.Format("orderCloud{0}", side));
		orderCloud.transform.SetParent(GameObject.Find("Canvas").transform);
		orderCloud.transform.SetSiblingIndex(0);

		orderCloud.AddComponent<CanvasRenderer>();
		RectTransform trans = orderCloud.AddComponent<RectTransform>();
		trans.localScale = Vector3.one;
		trans.anchoredPosition = new Vector2(((side==0)?-1:1) * 135.0f, 28.4f); // setting position
		trans.eulerAngles = new Vector3(0, (side-1) * 180, 0);
		trans.sizeDelta = new Vector2(64, 48); // custom size

		Image image = orderCloud.AddComponent<Image>();
		Texture2D tex = Resources.Load<Texture2D>("UI/orderCloud");
		image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));


		///////////////////////////
		GameObject imgOrder = new GameObject("order");
		imgOrder.transform.SetParent(GameObject.Find(string.Format("orderCloud{0}", side)).transform);

		imgOrder.AddComponent<CanvasRenderer>();
		RectTransform t = imgOrder.AddComponent<RectTransform>();
		t.localScale = Vector3.one;
		t.anchoredPosition = new Vector2(0f, 2.4f); // setting position
		t.sizeDelta = new Vector2(32, 32); // custom size

		Image orderImg = imgOrder.AddComponent<Image>();
		Texture2D orderTex = Resources.Load<Texture2D>(System.String.Format("recipes/{0}", order));
		orderImg.sprite = Sprite.Create(orderTex, new Rect(0, 0, orderTex.width, orderTex.height), new Vector2(0.5f, 0.5f));

		Button orderButton = imgOrder.AddComponent<Button>();
		imgOrder.GetComponent<Button>().onClick.AddListener(delegate { giveOrder(order, orderCloud); });
	}

}
