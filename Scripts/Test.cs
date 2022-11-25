namespace com.wao.core
{
#if UNITY_EDITOR
    using System;
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;

    public class ListToPopupAttribute : PropertyAttribute
    {
        public Type myType;
        public string propertyName;
        public ListToPopupAttribute(Type myType, string propertyName)
        {
            this.myType = myType;
            this.propertyName = propertyName;
        }
    }
    [CustomPropertyDrawer(typeof(ListToPopupAttribute))]
    public class ListToPropertyDrawer : PropertyDrawer
    {
        private int _selectedIndex;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ListToPopupAttribute atb = attribute as ListToPopupAttribute;
            List<string> stringList = null;
            if (atb.myType.GetField(atb.propertyName) != null)
            {
                stringList = atb.myType.GetField(atb.propertyName).GetValue(atb.myType) as List<string>;
            }
            if (stringList != null)
            {
                _selectedIndex = Math.Max(stringList.IndexOf(property.stringValue),0);
                _selectedIndex = EditorGUI.Popup(position, property.name, _selectedIndex, stringList.ToArray());
                property.stringValue = stringList[_selectedIndex];
            }
            else
                EditorGUI.PropertyField(position, property, label);
        }
    }
    [ExecuteInEditMode]
    public class Test : MonoBehaviour
    {
        public static List<string> myList;
        [ListToPopup(typeof(Test),"myList")]
        public string popup;
        private void Awake()
        {
            myList = new List<string> { "1", "2", "3" };
        }
    }
#endif
}
