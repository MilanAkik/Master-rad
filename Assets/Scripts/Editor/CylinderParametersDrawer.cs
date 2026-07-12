using Assets.Scripts.Models;
using UnityEditor;
using UnityEngine;

using static Assets.Scripts.Editor.EditorUtilities;

namespace Assets.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(CylinderParameters))]
    public class CylinderParametersDrawer : PropertyDrawer
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

                var generatorParametersProp = prop.FindPropertyRelative("generatorParameters");
                var generatorProp = prop.FindPropertyRelative("generator");

                var generatorParametersPosition = GetPropertyPosition(pos, 1);
                var generatorParametersHeight = EditorGUI.GetPropertyHeight(generatorParametersProp, includeChildren: true);
                generatorParametersPosition.height = generatorParametersHeight;

                EditorGUI.PropertyField(
                    generatorParametersPosition,
                    generatorParametersProp,
                    new GUIContent("Generator parameters"),
                    includeChildren: true);

                var generatorPosition = new Rect(
                    pos.x,
                    generatorParametersPosition.y + generatorParametersHeight + padding,
                    pos.width,
                    fieldHeight);

                EditorGUI.PropertyField(generatorPosition, generatorProp, new GUIContent("Generator"));

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            if (!prop.isExpanded)
            {
                return fieldHeight;
            }

            var generatorParametersProp = prop.FindPropertyRelative("generatorParameters");
            var generatorParametersHeight = EditorGUI.GetPropertyHeight(generatorParametersProp, includeChildren: true);

            return fieldHeight + generatorParametersHeight + fieldHeight + (2 * padding);
        }
    }
}
