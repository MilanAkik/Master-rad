using UnityEditor;
using UnityEngine;

public class EditorUtilities
{

    public static void DrawField(Rect pos, int index, SerializedProperty parent, string fieldName, string displayName)
    {
        var position = GetPropertyPosition(pos, index, EditorGUIUtility.singleLineHeight, 100f);
        var field = parent.FindPropertyRelative(fieldName);
        var content = new GUIContent(displayName);
        EditorGUI.PropertyField(position, field, content);
    }

    public static Rect GetPropertyPosition(Rect pos, int index, float fieldHeight, float padding)
    {
        return new Rect(pos.x, pos.y + index * (fieldHeight + padding), pos.width, fieldHeight);
    }

}
