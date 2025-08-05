using UnityEditor;
using UnityEngine;
using static Common_Cave_Dive.GrovalConst_CaveDive;
using static Common_Cave_Dive.GrovalStruct_CaveDive;

[CustomPropertyDrawer(typeof(Character_Data))]
public class CharacterDataEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var objIDProp = property.FindPropertyRelative("Obj_ID");
        var posProp = property.FindPropertyRelative("pos");
        var moveRangeProp = property.FindPropertyRelative("move_range");

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = 4f;
        Rect lineRect = new Rect(position.x, position.y, position.width, lineHeight);

        // ----------------------------
        // Obj_ID の制限付きドロップダウン
        // ----------------------------
        string[] displayOptions = GetLimitedObjIDNames();
        int[] actualEnumValues = GetLimitedObjIDValues();

        int currentValue = objIDProp.enumValueIndex;
        int limitedIndex = System.Array.IndexOf(actualEnumValues, currentValue);
        if (limitedIndex == -1) limitedIndex = 0;

        int selected = EditorGUI.Popup(lineRect, "Obj ID", limitedIndex, displayOptions);
        objIDProp.enumValueIndex = actualEnumValues[selected];
        lineRect.y += lineHeight + spacing;

        // posはGOAL_ARROWまで表示
        if (objIDProp.enumValueIndex <= (int)Obj_ID.GOAL_ARROW)
        {
            EditorGUI.PropertyField(lineRect, posProp);
            lineRect.y += lineHeight + spacing;

            // move_rangeはMINEかSHARKのみ
            if (objIDProp.enumValueIndex == (int)Obj_ID.MINE || objIDProp.enumValueIndex == (int)Obj_ID.SHARK)
            {
                EditorGUI.PropertyField(lineRect, moveRangeProp);
                lineRect.y += lineHeight + spacing;
            }

        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var objIDProp = property.FindPropertyRelative("Obj_ID");

        int lines = 1; // Obj_IDは常に表示

        if (objIDProp.enumValueIndex <= (int)Obj_ID.GOAL_ARROW)
        {
            lines++; // pos

            if (objIDProp.enumValueIndex == (int)Obj_ID.MINE || objIDProp.enumValueIndex == (int)Obj_ID.SHARK)
            {
                lines++; // move_range
            }
        }

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = 4f;

        return lines * (lineHeight + spacing);
    }
    private string[] GetLimitedObjIDNames()
    {
        Obj_ID[] all = (Obj_ID[])System.Enum.GetValues(typeof(Obj_ID));
        int limit = (int)Obj_ID.GOAL - 1;
        string[] names = new string[limit + 1];

        for (int i = 0; i <= limit; i++)
        {
            names[i] = all[i].ToString();
        }

        return names;
    }

    private int[] GetLimitedObjIDValues()
    {
        int limit = (int)Obj_ID.GOAL - 1;
        int[] values = new int[limit + 1];

        for (int i = 0; i <= limit; i++)
        {
            values[i] = i;
        }

        return values;
    }
}