using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathsUtils
{
    public static Quaternion ShortestRotation(Quaternion a, Quaternion b)
    {
        if (Quaternion.Dot(a, b) < 0)
        {
            return a * Quaternion.Inverse(Multiply(b, -1));
        }

        else return a * Quaternion.Inverse(b);
    }

    public static Quaternion Multiply(Quaternion input, float scalar)
    {
        return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    }

    public static Vector3 Invert(Vector3 vec)
    {
        return new Vector3(1 / vec.x, 1 / vec.y, 1 / vec.z);
    }

    public static Vector3 ClampVector3(Vector3 vec, Vector3 min, Vector3 max)
    {
        vec.x = Mathf.Clamp(vec.x, min.x, max.x);
        vec.y = Mathf.Clamp(vec.y, min.y, max.y);
        vec.z = Mathf.Clamp(vec.z, min.z, max.z);

        return vec;
    }

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

    public static Vector3 LerpVector3(Vector3 start, Vector3 end, float t)
    {
        Vector3 current;
        current.x = Mathf.Lerp(start.x, end.x, t);
        current.y = Mathf.Lerp(start.y, end.y, t);
        current.z = Mathf.Lerp(start.z, end.z, t);

        return current;
    }
}