using UnityEditor;
using UnityEngine;

namespace Enjlectric.ScriptableData.Editor
{
    /// <summary>
    /// Draws a green box next to the ScriptableData variable being used.
    /// </summary>
    [CustomPropertyDrawer(typeof(HighlightIfUsedAttribute))]
    public class HighlightIfUsedPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.LabelField(position, label);
            EditorGUI.PropertyField(position, property);

            if (EditorPrefs.GetBool("ScriptableDataUseProductionValues", false) == ((HighlightIfUsedAttribute)attribute).highlightForProductionMode)
            {
                EditorGUI.DrawRect(new Rect(position.x - 6, position.y, 4, position.height), Color.green);
            }
        }
    }
}