﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResourceBar : MonoBehaviour
{

    public Image bar;
    public GameObject target;

    public string id;
    public bool lerp;
    public float lerpFactor = 1;
    float currentLerp;

    //public bool drop;

    float scaleFactor;

    Vector3 local;

    // Use this for initialization
    void Start()
    {
        local = bar.rectTransform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        float currentScale = bar.rectTransform.localScale.x;

        switch (id)
        {
            case "health":
                scaleFactor = ((float)target.GetComponent<Attributes>().currentHealth / (float)target.GetComponent<Attributes>().maxHealth);
                break;

            case "stamina":
                scaleFactor = ((float)target.GetComponent<Attributes>().currentStamina / (float)target.GetComponent<Attributes>().maxStamina);
                break;
        }

        if (!lerp)
        {
            bar.rectTransform.localScale = new Vector3(local.x * scaleFactor, local.y, local.z);
        }

        if (lerp)
        {
            bar.rectTransform.localScale = new Vector3(FakeLerp(bar.rectTransform.localScale.x, local.x * scaleFactor, lerpFactor), local.y, local.z);

            //Debug.Log((Mathf.Lerp(bar.rectTransform.localScale.x, local.x * scaleFactor, currentLerp)) + " " + scaleFactor);
            /*   
               if (!((Mathf.Lerp(bar.rectTransform.localScale.x, local.x * scaleFactor, currentLerp)) < (scaleFactor + 0.1)) && scaleFactor < 1)
               {
                   currentLerp = lerpFactor * Time.deltaTime;
                   bar.rectTransform.localScale = new Vector3(Mathf.Lerp(bar.rectTransform.localScale.x, local.x * scaleFactor, currentLerp), local.y, local.z);
               }
               else
               {
                   //Debug.Log("FakeLerp");
                   bar.rectTransform.localScale = new Vector3(FakeLerp(bar.rectTransform.localScale.x, local.x * scaleFactor, lerpFactor), local.y, local.z);
               }
            /**/
        }
    }

    float FakeLerp(float n, float target, float a)
    {
        if (n == target)
            return n;
        else
        {
            float dir = Mathf.Sign(target - n);
            n += a * Time.deltaTime * dir;
            return (dir == Mathf.Sign(target - n)) ? n : target;
        }
    }
}