using UnityEngine;
using System.Collections;

public class InteractionPotionRefill : Interaction {

    PotionRefillObject refillObject;

    public InteractionPotionRefill(PotionRefillObject obj)
    {
        refillObject = obj;
    }
	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {	
	}

    public override void Execute()
    {
        base.Execute();
        refillObject.Refill();
    }
}
