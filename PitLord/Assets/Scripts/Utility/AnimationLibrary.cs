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
    public List<AnimationWrapper> animations;

    private AnimationLibrary()
    {
        Init();
    }

    private void Init()
    {
        animations = new List<AnimationWrapper>();

        //PlaceHolder
        AddAnimation(new AnimationWrapper("LightAttack1", 0.2f, 0.4f, 0.0f)); //0.0f cancel = always cancel, 1.0f cancel = can't cancel
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
        animations.Add(wrapper);
    }

    public AnimationWrapper SearchByName( string name )
    {
        int count = 0;
        AnimationWrapper wrapper = new AnimationWrapper();

        for (int i = 0; i < animations.Count; i++)
        {
            if (animations[i].name == name)
            {
                count++;
                wrapper = animations[i];
            }
        }

        //Returns the Animation going by name specified
        if(count == 1)
        {
            Debug.Log("Animations found; data returned");
            return wrapper;
        }

        //Returns null if there's multiple or no animations by name specified

        Debug.Log(count + " animations found for name specified. Get it together dude.");
        return null;
    }
}
