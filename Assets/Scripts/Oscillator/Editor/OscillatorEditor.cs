using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Oscillator), true)]
public class OscillatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Oscillator oscillator = (Oscillator) target;
        for (int i = 0; i < 3; i++)
        {
            oscillator.forceScale[i] = (int)Mathf.Clamp01(oscillator.forceScale[i]);
        }
    }
}
