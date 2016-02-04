using UnityEngine;
using System.Collections;

public class OpenLockedObject : Interaction {

    LockedObject lockedObject;

    public OpenLockedObject(LockedObject obj)
    {
        lockedObject = obj;
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
