using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookBook : MonoBehaviour
{

    public void checkWarehouse()
    {
        GameObject[] recipeBlocks = GameObject.FindGameObjectsWithTag("recipeBlock");

        foreach (GameObject block in recipeBlocks)
        {
            bool craftable = true;
            string buttonName = string.Format("cookButton {0}", block.transform.Find("recipeName").GetComponent<Text>().text);
            foreach (Transform materialBlock in block.transform.Find("materials").transform)
            {
                Transform mat = materialBlock.gameObject.transform.Find("materialName");
                string materialName = mat.GetComponent<Text>().text;

                if (GameObject.Find("cafe").GetComponent<Cafe>().isInWarehouse(materialName) == 0)
                {
                    mat.GetComponent<Text>().color = new Color32(164, 17, 17, 255);
                    craftable = false;
                }
                else
                {
                    mat.GetComponent<Text>().color = new Color32(50, 50, 50, 255);
                }

            }
            if (!craftable)
            {
                block.transform.Find(buttonName).GetComponent<BoxCollider2D>().enabled = false;
                block.transform.Find(buttonName).GetComponent<Image>().color = new Color32(111, 111, 111, 255);
            }
            else
            {
                block.transform.Find(buttonName).GetComponent<BoxCollider2D>().enabled = true;
                block.transform.Find(buttonName).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
            
        }
    }


    public void toggleRecipeBook()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            checkWarehouse();
        }
    }
}
