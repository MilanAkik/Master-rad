using Assets.Scripts.Models;
using UnityEditor;
using UnityEngine;

using static Assets.Scripts.Editor.EditorUtilities;

namespace Assets.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(LightParameters))]
    public class LightParametersDrawer : PropertyDrawer
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

                var lightColorProp = prop.FindPropertyRelative("lightColor");
                var lightColorPosition = GetPropertyPosition(pos, 1);

                var color = lightColorProp.vector4Value;
                var pickedColor = EditorGUI.ColorField(
                    lightColorPosition,
                    new GUIContent("Light color"),
                    new Color(color.x, color.y, color.z, color.w),
                    true,
                    true,
                    false);

                lightColorProp.vector4Value = new Vector4(
                    pickedColor.r,
                    pickedColor.g,
                    pickedColor.b,
                    pickedColor.a);

                DrawField(pos, 2, prop, "lightPosition", "Light position");

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