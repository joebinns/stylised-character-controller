using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gizmos = Popcron.Gizmos;

/// <summary>
/// Utilities for Popcorn.Gizmos, which are gizmos that can work even in compiled games.
/// </summary>
public static class GizmoUtils
{
    // Source: https://forum.unity.com/threads/draw-3d-arc-in-gizmos.994453/
    public static Vector3 DrawArc(Vector3 center, Vector3 point, Vector3 axis, float revFactor1 = 0f, float revFactor2 = 1f, int segments = 48, Color color = default)
    {
        segments = Mathf.Max(1, segments);

        var rad1 = revFactor1 * 2f * Mathf.PI;
        var rad2 = revFactor2 * 2f * Mathf.PI;
        var delta = rad2 - rad1;

        var fsegs = (float)segments;
        var inv_fsegs = 1f / fsegs;

        var vdiff = point - center;
        var length = vdiff.magnitude;
        vdiff.Normalize();

        var prevPoint = point;
        var nextPoint = Vector3.zero;

        if (Mathf.Abs(rad1) >= 1E-6f) prevPoint = pivotAround(center, axis, vdiff, length, rad1);

        for (var seg = 1f; seg <= fsegs; seg++)
        {
            nextPoint = pivotAround(center, axis, vdiff, length, rad1 + delta * seg * inv_fsegs);
            Gizmos.Line(prevPoint, nextPoint, color);
            //Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }

        return nextPoint;
    }

    public static Vector3 pivotAround(Vector3 center, Vector3 axis, Vector3 dir, float radius, float radians)
      => center + radius * (Quaternion.AngleAxis(radians * Mathf.Rad2Deg, axis) * dir);
}
