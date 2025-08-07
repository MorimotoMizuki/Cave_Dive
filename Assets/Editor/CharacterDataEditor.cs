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
        var isStartUpRightProp = property.FindPropertyRelative("is_start_up_right");

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = 4f;

        Rect lineRect = new Rect(position.x, position.y, position.width, lineHeight);

        // Obj_ID 制限付きポップアップ
        string[] displayOptions = GetLimitedObjIDNames();
        int[] actualEnumValues = GetLimitedObjIDValues();

        int currentValue = objIDProp.enumValueIndex;
        int currentEnumValue = (int)((Obj_ID)currentValue);
        int limitedIndex = System.Array.IndexOf(actualEnumValues, currentEnumValue);
        if (limitedIndex == -1) limitedIndex = 0;

        int selected = EditorGUI.Popup(lineRect, "Obj ID", limitedIndex, displayOptions);
        objIDProp.enumValueIndex = actualEnumValues[selected];
        int selectedEnumValue = actualEnumValues[selected];
        lineRect.y += lineHeight + spacing;

        // pos 表示
        EditorGUI.PropertyField(lineRect, posProp);
        lineRect.y += lineHeight + spacing;

        // MINE or SHARK のときのみ追加表示
        if (selectedEnumValue == (int)Obj_ID.MINE || selectedEnumValue == (int)Obj_ID.SHARK)
        {
            EditorGUI.PropertyField(lineRect, moveRangeProp);
            lineRect.y += lineHeight + spacing;

            EditorGUI.PropertyField(lineRect, isStartUpRightProp);
            lineRect.y += lineHeight + spacing;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var objIDProp = property.FindPropertyRelative("Obj_ID");

        int currentValue = objIDProp.enumValueIndex;
        int currentEnumValue = (int)((Obj_ID)currentValue);

        int lines = 2; // Obj_ID + pos

        if (currentEnumValue == (int)Obj_ID.MINE || currentEnumValue == (int)Obj_ID.SHARK)
        {
            lines += 2; // move_range + is_start_up_right
        }

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = 4f;

        return lines * lineHeight + (lines - 1) * spacing;
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