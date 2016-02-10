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

    //Name, colStart, colEnd, cancel, duration
    private void Init()
    {
        animations = new Dictionary<string, AnimationWrapper>();

        //PlaceHolder
        AddAnimation(new AnimationWrapper("default", 0.0f, 0.0f, 0.0f, 0.0f));
        AddAnimation(new AnimationWrapper("P_ShortLight01", 0.3f, 0.9f, 1.34f, 1.66f).RomoLength(0.818f));
        AddAnimation(new AnimationWrapper("P_ShortLight02", 0.3f, 1.2f, 1.5f, 1.5f).RomoLength(0.500f));
        AddAnimation(new AnimationWrapper("P_ShortHeavy", 1.0f, 1.5f, 1.8f, 2.0f).RomoLength(0.477f).Knockback(2f, 2f));
        AddAnimation(new AnimationWrapper("E_SwordCombo01", 1.0f, 3.0f, 3.6f, 3.6f).RomoLength(0.197f));
        AddAnimation(new AnimationWrapper("E_SwordCombo02", 0.2f, 0.6f, 2.5f, 2.5f));
        AddAnimation(new AnimationWrapper("E_SwordCombo03", 0.2f, 0.6f, 2.5f, 2.5f));
        AddAnimation(new AnimationWrapper("E_SwordHeavy", 0.2f, 0.6f, 2.5f, 2.5f));
    }


    public static AnimationLibrary Get()
    {
        if (instance == null)
        {
            instance = new AnimationLibrary();
        }

        return instance;
    }

    private void AddAnimation( AnimationWrapper wrapper )
    {
        animations[wrapper.name] = wrapper;
    }

    public AnimationWrapper SearchByName( string name )
    {
        return animations[name];
    }
}
