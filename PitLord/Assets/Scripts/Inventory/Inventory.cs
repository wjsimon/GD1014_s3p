using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Inventory {

    public List<string> items = new List<string>();
    public List<string> keys = new List<string>();
    public List<string> upgrades = new List<string>();

    public Dictionary<string, string> displayNames = new Dictionary<string, string>()
    {
        {"name", "displayname"},
        {"test", "testDisplay"},
    };

	// Use this for initialization
	public void Start () {
        //Load();
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void AddItem(string t )
    {
        items.Add(t);
        //Save();
    }
    public void RemoveItem( string t )
    {
        items.Remove(t);
        //Save();
    }

    /*
    public void Save()
    {
        string keyNames = "";
        string itemNames = "";

        for (int i = 0; i < keys.Count; i++)
        {
            keyNames += keys[i].name + ",";
        }
        for (int i = 0; i < items.Count; i++)
        {
            itemNames += items[i].name + ",";
        }

        PlayerPrefs.SetString("keys", keyNames);
        PlayerPrefs.SetString("items", itemNames);
        PlayerPrefs.Save();
    }
    /**/

    /*
    public void Load()
    {
        string keyNames = PlayerPrefs.GetString("keys");
        string itemNames = PlayerPrefs.GetString("items");

        Debug.Log(itemNames);

        string[] keyArray = keyNames.Split(new string[]{","}, System.StringSplitOptions.RemoveEmptyEntries);
        string[] itemArray = itemNames.Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);

        keys = new List<Key>();
        for(int i = 0; i < keyArray.Length; i++)
        {
            keys.Add(new Key(keyArray[i]));
        }

        items = new List<Item>();
        for (int i = 0; i < itemArray.Length; i++)
        {
            items.Add(new Item(itemArray[i]));
        }
    }
    /**/
}
