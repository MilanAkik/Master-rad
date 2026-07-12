using Assets.Scripts.Models;
using UnityEditor;
using UnityEngine;

using static Assets.Scripts.Editor.EditorUtilities;

namespace Assets.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(CloudConfig))]
    public class CloudConfigDrawer : PropertyDrawer
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

                var areaParametersProp = prop.FindPropertyRelative("areaParameters");
                var cylinderParametersProp = prop.FindPropertyRelative("cylinderParameters");
                var densityParametersProp = prop.FindPropertyRelative("densityParameters");
                var lightParametersProp = prop.FindPropertyRelative("lightParameters");
                var shapeParametersProp = prop.FindPropertyRelative("shapeParameters");

                var y = pos.y + fieldHeight + padding;

                y = DrawProperty(pos, y, areaParametersProp, "Area parameters");
                y = DrawProperty(pos, y, cylinderParametersProp, "Cylinder parameters");
                y = DrawProperty(pos, y, densityParametersProp, "Density parameters");
                y = DrawProperty(pos, y, lightParametersProp, "Light parameters");
                DrawProperty(pos, y, shapeParametersProp, "Shape parameters");

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

            var areaParametersProp = prop.FindPropertyRelative("areaParameters");
            var cylinderParametersProp = prop.FindPropertyRelative("cylinderParameters");
            var densityParametersProp = prop.FindPropertyRelative("densityParameters");
            var lightParametersProp = prop.FindPropertyRelative("lightParameters");
            var shapeParametersProp = prop.FindPropertyRelative("shapeParameters");

            return fieldHeight
                + padding
                + EditorGUI.GetPropertyHeight(areaParametersProp, includeChildren: true) + padding
                + EditorGUI.GetPropertyHeight(cylinderParametersProp, includeChildren: true) + padding
                + EditorGUI.GetPropertyHeight(densityParametersProp, includeChildren: true) + padding
                + EditorGUI.GetPropertyHeight(lightParametersProp, includeChildren: true) + padding
                + EditorGUI.GetPropertyHeight(shapeParametersProp, includeChildren: true);
        }

        private static float DrawProperty(Rect pos, float y, SerializedProperty property, string displayName)
        {
            var propertyHeight = EditorGUI.GetPropertyHeight(property, includeChildren: true);
            var propertyRect = new Rect(pos.x, y, pos.width, propertyHeight);

            EditorGUI.PropertyField(propertyRect, property, new GUIContent(displayName), includeChildren: true);

            return y + propertyHeight + padding;
        }
    }
}
