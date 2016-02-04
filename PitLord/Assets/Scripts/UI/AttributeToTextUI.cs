using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AttributeToTextUI : MonoBehaviour {

    public GameObject source;

    public bool health;
    public bool stamina;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

        if(health)
        {
            GetComponent<Text>().text = source.GetComponent<Character>().currentHealth.ToString();
        }

        if(stamina)
        {
            GetComponent<Text>().text = source.GetComponent<Character>().currentStamina.ToString();
        }
	}
}
