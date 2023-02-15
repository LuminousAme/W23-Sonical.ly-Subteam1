using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MiniGameSlots
{
    [CustomEditor(typeof(SlotsManager))]
    public class SlotsManagerEditor : Editor
    {
        SlotsManager manager;
        SlotsManager.SlotOption result;
        bool[] machineFoldOuts = { false, false, false };
        bool resultFoldout = false;

        void OnEnable()
        {
            manager = (SlotsManager)serializedObject.targetObject;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Randomize")) result = manager.Randomize();

            if(result != null && manager.slotOptions.Count > 0)
            {

                for (int i = 0; i < 3; i++)
                {
                    EditorGUILayout.Separator();
                    machineFoldOuts[i] = EditorGUILayout.Foldout(machineFoldOuts[i], "Machine " + (i + 1).ToString());
                    if(machineFoldOuts[i])
                    {
                        EditorGUILayout.BeginHorizontal();

                        SlotsManager.SlotOption slot = manager.slotOptions[manager.Indices[i]];
                        GUILayout.Label("Tokens: " + slot.tokens);
                        GUILayout.Label("Target Accuracy: " + slot.targetAccuracy);

                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.Separator();
                resultFoldout = EditorGUILayout.Foldout(resultFoldout, "Final Result");
                if(resultFoldout)
                {
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Label("Tokens: " + result.tokens);
                    GUILayout.Label("Target Accuracy: " + result.targetAccuracy);

                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
}

