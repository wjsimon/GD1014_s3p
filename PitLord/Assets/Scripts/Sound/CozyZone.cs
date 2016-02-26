using UnityEngine;
using System.Collections;

public class CozyZone : MonoBehaviour
{
    SoundManager manager;
    // Use this for initialization
    void Start()
    {
        if (manager == null) { manager = GameObject.Find("BGM").GetComponent<SoundManager>(); }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter( Collider other )
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            manager.EnterCozy();
        }
    }

    void OnTriggerExit( Collider other )
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            manager.ExitCozy();
        }
    }
}
