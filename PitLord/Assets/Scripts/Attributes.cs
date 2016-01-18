using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class Attributes : MonoBehaviour {
    
    [HideInInspector]
    public int currentHealth;
    public int maxHealth;

    [HideInInspector]
    public int currentStamina;
    public int maxStamina;

    public List<AudioClip> onHit;
    public List<AudioClip> onDeath;
    
    public void ApplyDamage()
    {

    }
}
