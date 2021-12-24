using UnityEngine;
using UnityEditor;
using PathCreation;

namespace PathCreation.Examples
{
    [CustomEditor(typeof(PathFollower), true)]
    public class PathFollowerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PathFollower pathFollower = (PathFollower)target;
            for (int i = 0; i < 3; i++)
            {
                pathFollower.rotateScale[i] = (int)Mathf.Clamp01(pathFollower.rotateScale[i]);
            }
        }
    }
}