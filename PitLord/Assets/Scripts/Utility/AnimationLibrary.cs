using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Stores custom AnimationWrappers and allows them to be searched by name, allows user to have all Animation Start/End Times for normalized time in this library,
/// so tweaks are more easily distributed if scripts utilize the Library references instead of hard values.
/// 
/// Adjust Animation normalized times via Library to globally implement changes to normalized times.
/// </summary>
public class AnimationLibrary
{
    private static AnimationLibrary instance;
    public Dictionary<string, AnimationWrapper> animations;

    private AnimationLibrary()
    {
        Init();
    }

    private void Init()
    {
        animations = new Dictionary<string, AnimationWrapper>();

        //PlaceHolder
        AddAnimation(new AnimationWrapper("LightAttack1", 0.2f, 0.4f, 1.0f)); //0.0f cancel = always cancel, 1.0f cancel = can't cancel
        AddAnimation(new AnimationWrapper("HeavyAttack1", 0.3f, 0.45f, 1.0f));
    }


    public static AnimationLibrary Get()
    {
        if (instance == null)
        {
            instance = new AnimationLibrary();
        }

        return instance;
    }

    private void AddAnimation(AnimationWrapper wrapper)
    {
        animations[wrapper.name] = wrapper;
    }

    public AnimationWrapper SearchByName( string name )
    {
        return animations[name];
    }
}
