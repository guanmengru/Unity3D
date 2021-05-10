using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//拓展方法，判断是否在攻击范围内
public static class ExtensionMethod
{
    private const float dotThreshold = 0.5f;
    public static bool IsFacingTarget(this Transform transform,Transform target)
    {
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        float dot = Vector3.Dot(transform.forward, vectorToTarget);

        return dot >= dotThreshold;
    }
}
