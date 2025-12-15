using UnityEngine;
using UnityEditor;
using HolidayGameJam;

namespace HolidayGameJamEditor
{
    [CustomEditor(typeof(AIGridSetup))]
    public class AIGridSetupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Grid Calculation", EditorStyles.boldLabel);

            AIGridSetup gridSetup = (AIGridSetup)target;

            if (GUILayout.Button("Recalculate Grid"))
            {
                Undo.RecordObject(gridSetup, "Recalculate Grid");
                gridSetup.RecalculateGrid();
                EditorUtility.SetDirty(gridSetup);
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "1. Assign the target surface (play area floor)\n" +
                "2. Set grid resolution (20x20 or 30x30)\n" +
                "3. Click 'Recalculate Grid' to generate grid data\n" +
                "4. Set spawn height for AI block drops",
                MessageType.Info
            );
        }
    }
}
