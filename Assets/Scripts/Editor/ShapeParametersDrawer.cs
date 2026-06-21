using Assets.Scripts.Models;
using UnityEditor;
using UnityEngine;

using static EditorUtilities;

namespace Assets.Scripts.Utilities
{
    [CustomPropertyDrawer(typeof(ShapeParameters))]
    public class ShapeParametersDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(pos, label, prop);
            DrawField(pos, 0, prop, "radiusMultiplier", "Radius multiplier");
            DrawField(pos, 1, prop, "heightMultiplier", "Height multiplier");
            DrawField(pos, 2, prop, "radiusThreshold", "Radius threshold");
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return 3 * (EditorUtilities.fieldHeight + EditorUtilities.padding);
        }

    }

}