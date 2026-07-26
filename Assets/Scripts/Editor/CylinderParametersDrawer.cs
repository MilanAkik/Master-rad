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

                var generatorProp = prop.FindPropertyRelative("generator");
                var generatorParametersProp = prop.FindPropertyRelative("generatorParameters");
                var cylinderCountProp = generatorParametersProp.FindPropertyRelative("cylinderCount");
                var randomSeedProp = generatorParametersProp.FindPropertyRelative("randomSeed");
                var radiusMultiplierProp = generatorParametersProp.FindPropertyRelative("radiusMultiplier");
                var heightMultiplierProp = generatorParametersProp.FindPropertyRelative("heightMultiplier");

                EditorGUI.PropertyField(GetPropertyPosition(pos, 1), generatorProp, new GUIContent("Generator"));
                EditorGUI.PropertyField(GetPropertyPosition(pos, 2), cylinderCountProp, new GUIContent("Cylinder count"));
                EditorGUI.PropertyField(GetPropertyPosition(pos, 3), randomSeedProp, new GUIContent("Random seed"));
                EditorGUI.PropertyField(GetPropertyPosition(pos, 4), radiusMultiplierProp, new GUIContent("Radius multiplier"));
                EditorGUI.PropertyField(GetPropertyPosition(pos, 5), heightMultiplierProp, new GUIContent("Height multiplier"));

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return fieldHeight + (prop.isExpanded ? 5 * (fieldHeight + padding) : 0);
        }
    }
}
