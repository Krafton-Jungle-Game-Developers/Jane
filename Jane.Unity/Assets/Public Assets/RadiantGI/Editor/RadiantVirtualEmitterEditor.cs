using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace RadiantGI.Universal {

    [CustomEditor(typeof(RadiantVirtualEmitter))]
    public class RadiantVirtualEmitterEditor : Editor {

        SerializedProperty color, intensity, range;
        SerializedProperty addMaterialEmission, targetRenderer, material, emissionPropertyName, materialIndex;
        SerializedProperty boxCenter, boxSize, boundsInLocalSpace;

        private readonly BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle();


        void OnEnable() {
            color = serializedObject.FindProperty("color");
            intensity = serializedObject.FindProperty("intensity");
            range = serializedObject.FindProperty("range");
            addMaterialEmission = serializedObject.FindProperty("addMaterialEmission");
            targetRenderer = serializedObject.FindProperty("targetRenderer");
            material = serializedObject.FindProperty("material");
            emissionPropertyName = serializedObject.FindProperty("emissionPropertyName");
            materialIndex = serializedObject.FindProperty("materialIndex");
            boxCenter = serializedObject.FindProperty("boxCenter");
            boxSize = serializedObject.FindProperty("boxSize");
            boundsInLocalSpace = serializedObject.FindProperty("boundsInLocalSpace");
        }

        protected virtual void OnSceneGUI() {
            RadiantVirtualEmitter vi = (RadiantVirtualEmitter)target;

            Bounds bounds = vi.GetBounds();
            m_BoundsHandle.center = bounds.center;
            m_BoundsHandle.size = bounds.size;

            // draw the handle
            EditorGUI.BeginChangeCheck();
            m_BoundsHandle.DrawHandle();
            if (EditorGUI.EndChangeCheck()) {
                // record the target object before setting new values so changes can be undone/redone
                Undo.RecordObject(vi, "Change Bounds");

                // copy the handle's updated data back to the target object
                Bounds newBounds = new Bounds();
                newBounds.center = m_BoundsHandle.center;
                newBounds.size = m_BoundsHandle.size;
                vi.SetBounds(newBounds);
            }
        }

        public override void OnInspectorGUI() {

            serializedObject.Update();

            EditorGUILayout.PropertyField(color);
            EditorGUILayout.PropertyField(addMaterialEmission);
            if (addMaterialEmission.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(targetRenderer);
                EditorGUILayout.PropertyField(material);
                EditorGUILayout.PropertyField(emissionPropertyName);
                EditorGUILayout.PropertyField(materialIndex);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(intensity);
            EditorGUILayout.PropertyField(range);
            EditorGUILayout.PropertyField(boxCenter);
            EditorGUILayout.PropertyField(boxSize);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(boundsInLocalSpace, new GUIContent("Local Space"));
            if (EditorGUI.EndChangeCheck()) {
                RadiantVirtualEmitter vi = (RadiantVirtualEmitter)target;
                if (boundsInLocalSpace.boolValue) {
                    boxCenter.vector3Value = Vector3.zero;
                } else {
                    boxCenter.vector3Value = vi.transform.position;
                }
                vi.SetBounds(new Bounds(boxCenter.vector3Value, boxSize.vector3Value));
            }

            serializedObject.ApplyModifiedProperties();

        }

    }


    public static class RadiantVirtualEmitterEditorExtension {

        [MenuItem("GameObject/Create Other/Radiant GI/Virtual Emitter")]
        static void CreateEmitter(MenuCommand menuCommand) {
            GameObject emitter = new GameObject("Radiant Virtual Emitter", typeof(RadiantVirtualEmitter));

            GameObjectUtility.SetParentAndAlign(emitter, menuCommand.context as GameObject);

            Undo.RegisterCreatedObjectUndo(emitter, "Create Virtual Emitter");
            Selection.activeObject = emitter;
        }

    }
}

