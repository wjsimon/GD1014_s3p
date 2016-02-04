using UnityEngine;
using System.Collections;
/// <summary>
/// Wrapps the normalized start and end times for Animations, based on Animation name
/// </summary>
public class AnimationWrapper {

    public string name;
    public float colStart;
    public float colEnd;
    public float cancel;
    public float duration;
    public float romoLength;
    public float koboDuration;
    public float koboLength;

    public AnimationWrapper()
    {
        name = "";
        colStart = 0.0f;
        colEnd = 0.0f;
        cancel = 0.0f;
        duration = 0.0f;
    }

    public AnimationWrapper(string name, float start, float end, float cancel, float duration)
    {
        this.name = name;
        this.colStart = start;
        this.colEnd = end;
        this.cancel = cancel;
        this.duration = duration;
    }

    public AnimationWrapper RomoLength(float l)
    {
        romoLength = l;
        return this;
    }

    public AnimationWrapper Knockback(float length, float duration)
    {
        koboLength = length;
        koboDuration = duration;

        return this;
    }
}
