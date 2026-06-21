using Assets.Scripts.Models;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    [CustomPropertyDrawer(typeof(ShapeParameters))]
    public class ShapeParametersDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(pos, label, prop);
            EditorGUI.PropertyField(EditorUtilities.GetPropertyPosition(pos, 0), prop.FindPropertyRelative("radiusMultiplier"), new GUIContent("Radius multiplier"));
            EditorGUI.PropertyField(EditorUtilities.GetPropertyPosition(pos, 1), prop.FindPropertyRelative("heightMultiplier"), new GUIContent("Height multiplier"));
            EditorGUI.PropertyField(EditorUtilities.GetPropertyPosition(pos, 2), prop.FindPropertyRelative("radiusThreshold"), new GUIContent("Radius threshold"));
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return 3 * EditorGUIUtility.singleLineHeight;
        }

    }

}