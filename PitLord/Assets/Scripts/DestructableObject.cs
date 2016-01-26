using UnityEngine;
using System.Collections;

public class DestructableObject : Attributes {

	// Use this for initialization
	void Start () 
    {
        currentHealth = 1;
        block = false;

        StoreTransform temp = new StoreTransform(transform.position, transform.rotation, transform.localScale);
        spawnPoint = new GameObject(gameObject.name + "_Spawn");
        spawnPoint.transform.parent = gameObject.transform;
        spawnPoint.transform.position = temp.position;
        spawnPoint.transform.rotation = temp.rotation;
        spawnPoint.transform.localScale = temp.localScale;

        RegisterObject();
    }
	
	// Update is called once per frame
	void Update () 
    {
	}

    void OnDestroy()
    {
        AudioSource player = new AudioSource();

        player.clip = onDeath[Random.Range(0, onDeath.Count)];
    }
}
