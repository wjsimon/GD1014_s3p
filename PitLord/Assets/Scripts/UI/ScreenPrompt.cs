using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenPrompt : MonoBehaviour {

    public bool showPrompt;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void TogglePrompt()
    {
        showPrompt = !showPrompt;
        transform.FindChild("Prompt").gameObject.SetActive(showPrompt);
    }
}
