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
        AddAnimation(new AnimationWrapper("P_ShortLight01", 0.3f * 0.7f, 0.9f * 0.7f, 1.2f * 0.7f, 1.66f * 0.7f));
        AddAnimation(new AnimationWrapper("P_ShortLight02", 0.3f * 0.7f, 0.8f * 0.7f, 1.2f * 0.7f, 1.5f * 0.7f));
        AddAnimation(new AnimationWrapper("P_ShortHeavy", 1.0f, 1.5f, 1.8f, 2.0f));
        AddAnimation(new AnimationWrapper("P_GreatLight01", 0.55f, 0.8f, 2.08f, 2.60f));
        AddAnimation(new AnimationWrapper("P_GreatLight02", 0.3f, 0.9f, 1.34f, 1.66f));
        AddAnimation(new AnimationWrapper("P_GreatHeavy01", 0.8f, 1.4f, 2.0f, 3.0f));

        AddAnimation(new AnimationWrapper("E_SwordCombo01", 1.0f, 1.5f, 2.1f, 2.1f));
        AddAnimation(new AnimationWrapper("E_SwordCombo02", 0.10f, 0.52f, 0.67f, 0.67f));
        AddAnimation(new AnimationWrapper("E_SwordCombo03", 0.24f, 0.64f, 1.0f, 1.0f));
        AddAnimation(new AnimationWrapper("E_SwordHeavy01", 1.37f, 2.17f, 3.5f, 3.5f));

        AddAnimation(new AnimationWrapper("E_SpearLight01", 0.24f, 0.84f, 1.0f, 1.0f));
        AddAnimation(new AnimationWrapper("E_SpearLight02", 0.27f, 0.92f, 1.17f, 1.17f));
        AddAnimation(new AnimationWrapper("E_SpearHeavy01", 0.30f, 1.0f, 1.66f, 1.66f));

        AddAnimation(new AnimationWrapper("E_BowLight01", 2.1f, 4.0f, 4.0f, 4.0f));
        AddAnimation(new AnimationWrapper("E_MageSpell01", 0.5f, 0.6f, 4.0f, 4.0f));

        AddAnimation(new AnimationWrapper("B_Combo01_1", 1.0f, 1.5f, 2.5f, 2.5f));
        AddAnimation(new AnimationWrapper("B_Combo01_2", 1.0f, 1.5f, 2.5f, 2.5f));
        AddAnimation(new AnimationWrapper("B_Combo01_3", 1.0f, 1.5f, 2.5f, 2.5f));

        AddAnimation(new AnimationWrapper("B_Combo02_1", 1.0f, 1.5f, 2.5f, 2.5f));
        AddAnimation(new AnimationWrapper("B_Combo02_2", 1.0f, 1.5f, 2.5f, 2.5f));
        AddAnimation(new AnimationWrapper("B_Combo02_3", 1.0f, 1.5f, 2.5f, 2.5f));

        AddAnimation(new AnimationWrapper("B_Combo03_1", 1.0f, 1.5f, 2.5f, 2.5f));
        AddAnimation(new AnimationWrapper("B_Combo03_2", 1.0f, 1.5f, 2.5f, 2.5f));
        AddAnimation(new AnimationWrapper("B_Combo03_3", 1.0f, 1.5f, 2.5f, 2.5f));

        AddAnimation(new AnimationWrapper("B_Heavy_1", 1.0f, 1.5f, 2.5f, 2.5f));
        AddAnimation(new AnimationWrapper("B_Heavy_2", 1.0f, 1.5f, 2.5f, 2.5f));
        AddAnimation(new AnimationWrapper("B_Heavy_3", 1.0f, 1.5f, 2.5f, 2.5f));

        AddAnimation(new AnimationWrapper("B_AOE_1", 1.0f, 1.5f, 2.5f, 2.5f));
        AddAnimation(new AnimationWrapper("B_AOE_2", 1.0f, 1.5f, 2.5f, 2.5f));
        AddAnimation(new AnimationWrapper("B_AOE_3", 1.0f, 1.5f, 2.5f, 2.5f));

        AddAnimation(new AnimationWrapper("B_Projectile", 1.0f, 1.5f, 2.5f, 2.5f)); //colEnd = duration - start
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
