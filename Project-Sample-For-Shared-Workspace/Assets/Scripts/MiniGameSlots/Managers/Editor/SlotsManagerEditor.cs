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
        bool[] machineFoldOuts = { false, false, false };
        bool resultFoldout = false;

        void OnEnable()
        {
            manager = (SlotsManager)serializedObject.targetObject;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Randomize")) manager.Result = manager.Randomize();

            if(manager.Result != null && manager.slotOptions.Count > 0)
            {

                for (int i = 0; i < 3; i++)
                {
                    EditorGUILayout.Separator();
                    machineFoldOuts[i] = EditorGUILayout.Foldout(machineFoldOuts[i], "Machine " + (i + 1).ToString());
                    if(machineFoldOuts[i])
                    {
                        SlotsManager.SlotOption slot = manager.slotOptions[manager.Indices[i]];
                        GUILayout.Label("Tokens: " + slot.tokens);
                    }
                }

                EditorGUILayout.Separator();
                resultFoldout = EditorGUILayout.Foldout(resultFoldout, "Final Result");
                if(resultFoldout)
                {
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Label("Tokens: " + manager.Result.tokens);
                    GUILayout.Label("Target Accuracy: " + manager.GetTargetAccuracy(manager.Result.tokens));
                    GUILayout.Label("Time to Beat: " + manager.GetTimeToBeat(manager.Result.tokens));

                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
}

