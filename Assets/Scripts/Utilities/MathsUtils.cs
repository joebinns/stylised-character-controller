using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A compilation of commonly used maths functions.
/// </summary>
public static class MathsUtils
{
    /// <summary>
    /// Calculates the shortest rotation between two Quaternions.
    /// </summary>
    /// <param name="a">The first Quaternion, from which the rotation is to be calculated.</param>
    /// <param name="b">The second Quaternion, for which the rotation is the goal.</param>
    /// <returns>The shortest rotation from a to b.</returns>
    public static Quaternion ShortestRotation(Quaternion a, Quaternion b)
    {
        if (Quaternion.Dot(a, b) < 0)
        {
            return a * Quaternion.Inverse(Multiply(b, -1));
        }

        else return a * Quaternion.Inverse(b);
    }

    /// <summary>
    /// Calculates the multiplication of a Quaternion by a scalar, such as to alter the scale of a rotation.
    /// </summary>
    /// <param name="input">The Quaternion rotation to be scaled.</param>
    /// <param name="scalar">The scale by which to multiply the rotation.</param>
    /// <returns>The scale adjusted Quaternion.</returns>
    public static Quaternion Multiply(Quaternion input, float scalar)
    {
        return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    }

    /// <summary>
    /// Calculates the axis-wise inverse of a Vector3.
    /// </summary>
    /// <param name="vec">The Vector3 which axes are to be inversed.</param>
    /// <returns>The axis-inversed Vector3.</returns>
    public static Vector3 Invert(Vector3 vec)
    {
        return new Vector3(1 / vec.x, 1 / vec.y, 1 / vec.z);
    }

    /// <summary>
    /// Clamps the axes of a Vector3 to be between a minimum and a maximum.
    /// </summary>
    /// <param name="vec">The unclamped Vector3.</param>
    /// <param name="min">A Vector3 representing the minimum acceptable axes values.</param>
    /// <param name="max">A Vector3 representing the maximum acceptable axes values.</param>
    /// <returns>The axes-clamped Vector3.</returns>
    public static Vector3 ClampVector3(Vector3 vec, Vector3 min, Vector3 max)
    {
        vec.x = Mathf.Clamp(vec.x, min.x, max.x);
        vec.y = Mathf.Clamp(vec.y, min.y, max.y);
        vec.z = Mathf.Clamp(vec.z, min.z, max.z);

        return vec;
    }

    /// <summary>
    /// Clamps the axes of a Quaternion to be between a minimum and a maximum.
    /// </summary>
    /// <param name="q">The unclamped Quaternion.</param>
    /// <param name="bounds">A Vector3 representing absolute range of the Euler rotation for clamping.</param>
    /// <returns>The axes-clamped Quaternion.</returns>
    public static Quaternion ClampRotation(Quaternion q, Vector3 bounds)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, -bounds.x, bounds.x);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);
        angleY = Mathf.Clamp(angleY, -bounds.y, bounds.y);
        q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

        float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
        angleZ = Mathf.Clamp(angleZ, -bounds.z, bounds.z);
        q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

        return q.normalized;
    }

    /// <summary>
    /// Linearly interpolates a Vector3.
    /// </summary>
    /// <param name="start">The initial Vector3.</param>
    /// <param name="end">The final Vector3.</param>
    /// <param name="t">The current duration of the lerp, typically between 0 and 1.</param>
    /// <returns>The lerped Vector3.</returns>
    public static Vector3 LerpVector3(Vector3 start, Vector3 end, float t)
    {
        Vector3 current;
        current.x = Mathf.Lerp(start.x, end.x, t);
        current.y = Mathf.Lerp(start.y, end.y, t);
        current.z = Mathf.Lerp(start.z, end.z, t);

        return current;
    }
}