using UnityEngine;

public class ConditionalStructAttribute : PropertyAttribute
{
    public string conditionField;

    public ConditionalStructAttribute(string conditionField)
    {
        this.conditionField = conditionField;
    }
}