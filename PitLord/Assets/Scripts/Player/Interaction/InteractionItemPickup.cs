using UnityEngine;
using System.Collections;

public class InteractionItemPickup : Interaction {

    ItemPickupObject itemPickup;

    public InteractionItemPickup(ItemPickupObject obj)
    {
        itemPickup = obj;
        Start();
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
        itemPickup.PickUp();
    }
}
