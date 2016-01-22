using UnityEngine;
using System.Collections;
/// <summary>
/// Wrapps the normalized start and end times for Animations, based on Animation name
/// </summary>
public class AnimationWrapper {

    public string name;
    public float start;
    public float end;
    public float cancel;

    public AnimationWrapper()
    {
        name = "";
        start = 0.0f;
        end = 0.0f;
    }

    public AnimationWrapper(string name, float start, float end, float cancel)
    {
        this.name = name;
        this.start = start;
        this.end = end;
        this.cancel = cancel;
    }
}
