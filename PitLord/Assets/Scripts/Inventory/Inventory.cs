using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory {

    public List<Item> itemList = new List<Item>();

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void AddItem(Item t)
    {
        itemList.Add(t);
    }
    public void AddItem(string name, string desc, Texture2D tex)
    {
        itemList.Add(new Item(name, desc, tex));
    }

    public void RemoveItem(Item t)
    {
        itemList.Remove(t);
    }
    public void RemoveItem(string name)
    {
        for(int i = 0; i < itemList.Count; i++)
        {
            if(itemList[i].name == name)
            {
                RemoveItem(itemList[i]);
            }
        }
    }
}
