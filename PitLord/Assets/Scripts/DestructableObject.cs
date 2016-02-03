using UnityEngine;
using System.Collections;

public class DestructableObject : Attributes {

	// Use this for initialization
	protected override void Start () 
    {
        base.Start();

        StoreTransform temp = new StoreTransform(transform.position, transform.rotation, transform.localScale);
        spawnPoint = new GameObject(gameObject.name + "_Spawn");
        spawnPoint.transform.parent = gameObject.transform;
        spawnPoint.transform.position = temp.position;
        spawnPoint.transform.rotation = temp.rotation;
        spawnPoint.transform.localScale = temp.localScale;

        RegisterObject();
    }
	
	// Update is called once per frame
	protected override void Update () 
    {
	}

    protected override void OnDestroy()
    {
        if(onDeath.Count > 0)
        {
            AudioSource player = new AudioSource();
            player.clip = onDeath[Random.Range(0, onDeath.Count)];
            player.Play();
        }

        GameManager.instance.RemoveObject(gameObject);
    }

    protected override void RegisterObject()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().AddObject(gameObject);
    }
}
