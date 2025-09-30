using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(RemoveUnderwaterTrees))]

public class RemoveTreesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RemoveUnderwaterTrees underwaterTreescript = (RemoveUnderwaterTrees)target;

        if (GUILayout.Button("Remove Underwater Trees"))
        {
            underwaterTreescript.RemoveTreesUnderWater();
        }
    }
}
