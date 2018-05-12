using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MinMax))]
public class MinMaxPropertyDrawer : PropertyDrawer
{
    Rect minRect, maxRect, sliderRect;
    SerializedProperty pMin, pMax, lMin, lMax, diff;
    float minVal, maxVal;
    bool maxChanged, minChanged;
    int gap = 10;
    float f = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUIUtility.labelWidth = 70;

        minRect = new Rect(
            position.x,
            position.y,
            position.width / 2f - gap,
            position.height / 2f);

        maxRect = new Rect(
            position.x + position.width / 2f + gap,
            position.y,
            position.width / 2f - gap,
            position.height / 2f);

        sliderRect = new Rect(
            position.x,
            position.y + position.height / 2f,
            position.width,
            position.height / 2f);

        pMin = property.FindPropertyRelative("pivotMin");
        pMax = property.FindPropertyRelative("pivotMax");

        lMin = property.FindPropertyRelative("minLimit");
        lMax = property.FindPropertyRelative("maxLimit");

        diff = property.FindPropertyRelative("diff");
        
        EditorGUI.PropertyField(minRect, pMin, new GUIContent("Min Value"));
        EditorGUI.PropertyField(maxRect, pMax, new GUIContent("Max Value"));

        minVal = pMin.floatValue;
        maxVal = pMax.floatValue;

        EditorGUI.MinMaxSlider(sliderRect, ref minVal, ref maxVal, 0, 10);

        pMin.floatValue = Mathf.Round(minVal);
        pMax.floatValue = Mathf.Round(maxVal);
        
        if (maxVal > lMax.floatValue)
        {
            maxVal = lMax.floatValue;
            pMax.floatValue = maxVal;
        }

        if (minVal < lMin.floatValue)
        {
            minVal = lMin.floatValue;
            pMin.floatValue = minVal;
        }

        if(minVal > maxVal - diff.floatValue)
        {
            if(minVal - diff.floatValue >= lMin.floatValue)
            {
                minVal = maxVal - diff.floatValue;
                pMin.floatValue = maxVal - diff.floatValue;
            }
            else
            {
                maxVal = minVal + diff.floatValue;
                pMax.floatValue = minVal + diff.floatValue;
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * 2f;
    }
}
