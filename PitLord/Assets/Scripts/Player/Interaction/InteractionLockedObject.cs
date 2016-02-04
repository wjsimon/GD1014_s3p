using UnityEngine;
using System.Collections;

public class InteractionLockedObject : Interaction {

    LockedObject lockedObject;

    public InteractionLockedObject(LockedObject obj)
    {
        lockedObject = obj;
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
        lockedObject.Unlock();
    }
}
