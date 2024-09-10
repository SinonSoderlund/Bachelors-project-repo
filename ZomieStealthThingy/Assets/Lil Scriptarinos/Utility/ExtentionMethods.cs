using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extention methods
/// </summary>
public static class ExtentionMethods
{
    /// <summary>
    /// Two way modulation function for ints (ie underflow as well as overflow)
    /// </summary>
    /// <param name="i">the int being modulated</param>
    /// <param name="mod">the modulation value</param>
    /// <returns></returns>
    public static int mod(this int i, int mod)
    {
        return (i % mod + mod) % mod;
    }
    /// <summary>
    /// Function for converting a Vector3 to a Vector2 (throws away z-axis)
    /// </summary>
    /// <param name="v">The Vector 3 being converted</param>
    /// <returns></returns>
    public static Vector2 ToVector2(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }
    /// <summary>
    /// Function for converting a vector2 to a vector3 (adds zeroed z-axis)
    /// </summary>
    /// <param name="v">the vector2 being converted</param>
    /// <returns></returns>
    public static Vector3 ToVector3(this Vector2 v)
    {
        return new Vector3(v.x, v.y, 0);
    }

    /// <summary>
    /// Return the absolute value of the int
    /// </summary>
    /// <param name="i">this</param>
    /// <returns></returns>
    public static int Abs(this int i)
    {
        return Mathf.Abs(i);
    }
    /// <summary>
    /// Function for converting a float to a vector 3, all axies are set to float value
    /// </summary>
    /// <param name="f">The float being turned into a vector 3</param>
    /// <returns></returns>
    public static Vector3 ToVector3(this float f)
    {
        return new Vector3(f, f, f);
    }
    /// <summary>
    /// Sets this transforms position values to the x and y values of other transform, preserves Z value of this transform
    /// </summary>
    /// <param name="v">this transform</param>
    /// <param name="t">transform to track</param>
    public static void SetXY(this Transform v, Transform t)
    {
        v.position = new Vector3(t.position.x, t.position.y, v.position.z);
    }
}