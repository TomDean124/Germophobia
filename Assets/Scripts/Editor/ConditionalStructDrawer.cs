using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalStructAttribute))]
public class ConditionalStructDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var conditionAttr = attribute as ConditionalStructAttribute;
        // Search relative to the parent struct (e.g., StaticEnemyData)
        var conditionProperty = property.FindPropertyRelative("../" + conditionAttr.conditionField);

        if (conditionProperty == null || conditionProperty.propertyType != SerializedPropertyType.Boolean)
        {
            EditorGUI.LabelField(position, label, new GUIContent("Condition not found or not a boolean"));
            return;
        }

        bool showField = conditionProperty.boolValue;

        if (showField)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var conditionAttr = attribute as ConditionalStructAttribute;
        var conditionProperty = property.FindPropertyRelative("../" + conditionAttr.conditionField);

        if (conditionProperty != null && conditionProperty.boolValue)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        return 0f; // Hide when false
    }
}