using UnityEngine;
using System.Collections;

public class Key : MonoBehaviour {

    public string name;
    public string displayName;
	// Use this for initialization

    public Key(string name)
    {
        this.name = name;
    }
    public Key(string name, string display)
    {
        this.name = name;
        this.displayName = display;
    }
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
}
