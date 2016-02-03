using UnityEngine;
using System.Collections;

public class Item {

    public string name;
    public string desc;
    public Texture2D tex;

    public Item(string name, string desc, Texture2D tex)
    {
        this.name = name;
        this.desc = desc;
        this.tex = tex;
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
}
