using UnityEngine;
using UnityEditor;

namespace Generator
{
    [CustomEditor(typeof(MapGenerator))]
    [CanEditMultipleObjects]
    public class MapGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            MapGenerator generator = (MapGenerator)target;
            if (GUILayout.Button("Generate"))
                generator.Generate();
        }
    }
}