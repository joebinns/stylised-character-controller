using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script to disable cursor visibility during Play mode.
/// </summary>
public class DisableCursor : MonoBehaviour
{
    /// <summary>
    /// Disable cursor visibility during Play mode.
    /// </summary>
    void Start()
    {
        Cursor.visible = false;
    }
}
