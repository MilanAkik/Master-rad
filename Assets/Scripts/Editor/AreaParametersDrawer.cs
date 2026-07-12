using Assets.Scripts.Models;
using UnityEditor;
using UnityEngine;

using static Assets.Scripts.Editor.EditorUtilities;

namespace Assets.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(AreaParameters))]
    public class AreaParametersDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(pos, label, prop);

            prop.isExpanded = EditorGUI.Foldout(
                new Rect(pos.x, pos.y, pos.width, fieldHeight),
                prop.isExpanded,
                label,
                toggleOnLabelClick: true);

            if (prop.isExpanded)
            {
                EditorGUI.indentLevel++;
                DrawField(pos, 1, prop, "areaMin", "Area min");
                DrawField(pos, 2, prop, "areaMax", "Area max");
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return fieldHeight + (prop.isExpanded ? 2 * (fieldHeight + padding) : 0);
        }
    }
}
