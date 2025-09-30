using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(RemoveSnowTrees))]

public class RemoveSnowTreesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RemoveSnowTrees abovesnowScript = (RemoveSnowTrees)target;

        if (GUILayout.Button("Remove Snow Trees"))
        {
            abovesnowScript.RemovesnowTrees();
        }
    }
}
