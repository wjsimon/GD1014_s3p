using UnityEngine;
using System.Collections;

/// <summary>
/// Because Unity sucks major dick.
/// </summary>
public class StoreTransform {

    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;

    public StoreTransform(Vector3 pos, Quaternion rot, Vector3 local)
    {
        position = pos;
        rotation = rot;
        localScale = local;
    }
}

public static class TransformExtension
{
    public static Transform GetRayCastTarget(this Transform trans)
    {
        return trans.FindChild("RayCastTarget").transform;
    }

    public static Transform FindChildRecursive(this Transform trans, string name)
    {
        for(int i = 0; i < trans.childCount; i++)
        {
            Transform current = trans.GetChild(i);
            if (current.name == name)
                return current;

            Transform next = current.FindChildRecursive(name);
            if (next != null) { return next; }
        }

        return null;
    }
}
