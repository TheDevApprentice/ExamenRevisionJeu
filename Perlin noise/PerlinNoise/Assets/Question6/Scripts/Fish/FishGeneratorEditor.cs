using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FishGenerator))]
public class FishGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FishGenerator fishGen = (FishGenerator)target;

        if (DrawDefaultInspector())
        {
            if (fishGen.AutoUpdate)
            {
                fishGen.GenerateFishes();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            fishGen.GenerateFishes();
        }
    }
}
