using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeImage : MonoBehaviour
{

    bool fadeIn = false;
    Image raw;
    Color alphaDummy;

    // Use this for initialization
    void Start()
    {
        raw = GetComponent<Image>();
        alphaDummy = raw.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeIn && raw.color.a != 1)
        {
            alphaDummy.a = FakeLerp(alphaDummy.a, 1, 1.0f);
            raw.color = alphaDummy;
        }
        
        if(!fadeIn && raw.color.a != 0)
        {
            alphaDummy.a = FakeLerp(alphaDummy.a, 0, 1.0f);
            raw.color = alphaDummy;
        }
    }

    public void FadeIn()
    {
        Debug.Log("FadeIN");
        fadeIn = true;
    }

    public void FadeOut()
    {
        Debug.Log("FadeOUT");
        fadeIn = false;
    }
    float FakeLerp( float n, float target, float a )
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
