using Assets.Scripts.Models;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    [CustomPropertyDrawer(typeof(ShapeParameters))]
    public class CloudConfigDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(pos, label, prop);
            EditorGUI.PropertyField( pos, prop.FindPropertyRelative("radiusMultiplier"), new GUIContent("Radius Multiplier"));
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }

}