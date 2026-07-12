using Assets.Scripts.Models;
using UnityEditor;
using UnityEngine;

using static Assets.Scripts.Editor.EditorUtilities;

namespace Assets.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(ShapeParameters))]
    public class ShapeParametersDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(pos, label, prop);
            prop.isExpanded = EditorGUI.Foldout( new Rect(pos.x, pos.y, pos.width, EditorUtilities.fieldHeight), prop.isExpanded, label, toggleOnLabelClick: true);
            if (prop.isExpanded)
            {
                EditorGUI.indentLevel++;
                DrawField(pos, 1, prop, "radiusMultiplier", "Radius multiplier");
                DrawField(pos, 2, prop, "heightMultiplier", "Height multiplier");
                DrawField(pos, 3, prop, "radiusThreshold", "Radius threshold");
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return fieldHeight + (prop.isExpanded ? 3 * (fieldHeight + padding) : 0);
        }

    }

}