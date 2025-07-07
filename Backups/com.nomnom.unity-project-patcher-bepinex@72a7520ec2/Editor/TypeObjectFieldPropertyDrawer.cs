using System;
using UnityEditor;
using UnityEngine;

namespace Nomnom.BepInEx.Editor {
    [CustomPropertyDrawer(typeof(TypeObjectFieldAttribute))]
    public sealed class TypeObjectFieldPropertyDrawer: PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            var typeObjectFieldAttribute = (TypeObjectFieldAttribute)attribute;
            var fullTypeName = typeObjectFieldAttribute.FullTypeName;
            var type = Type.GetType(fullTypeName);
            if (type == null) {
                EditorGUI.HelpBox(position, $"Type not found: {fullTypeName}", MessageType.Error);
            } else {
                property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, type, false);
            }
            EditorGUI.EndProperty();
        }
    }
}