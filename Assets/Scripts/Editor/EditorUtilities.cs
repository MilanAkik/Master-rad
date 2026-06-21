using UnityEditor;
using UnityEngine;

public class EditorUtilities
{
    public static Rect GetPropertyPosition(Rect pos, int index)
    {
        return new Rect(pos.x, pos.y + index * EditorGUIUtility.singleLineHeight, pos.width, EditorGUIUtility.singleLineHeight);
    }

}
