using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class client : MonoBehaviour
{
	public string Order;

	public client(GameObject client, string order)
    {
		this.Order = order;
    }

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

	
		}
	}

	public void createOrderImage(string order)
	{
		GameObject orderCloud = new GameObject("orderCloud");
		orderCloud.transform.SetParent(GameObject.Find("Canvas").transform);
		orderCloud.transform.SetSiblingIndex(0);

		orderCloud.AddComponent<CanvasRenderer>();
		RectTransform trans = orderCloud.AddComponent<RectTransform>();
		trans.localScale = Vector3.one;
		trans.anchoredPosition = new Vector2(155.0f, 28.4f); // setting position
		trans.sizeDelta = new Vector2(64, 48); // custom size

		Image image = orderCloud.AddComponent<Image>();
		Texture2D tex = Resources.Load<Texture2D>("UI/orderCloud");
		image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));


		///////////////////////////
		GameObject imgOrder = new GameObject("order");
		imgOrder.transform.SetParent(GameObject.Find("orderCloud").transform);

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
