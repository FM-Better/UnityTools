#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorTool
{
    internal class HierarchyTool
    {
        private static HierarchyTool _instance;

        private static Event CurrentEvent => Event.current;
        private float _nameDistance;

        private Object _selectComponent;
        private readonly GUIContent _content;
        private readonly HierarchyIcon _icon;

        private HierarchyTool()
        {
            _icon = new HierarchyIcon();
            _content = new GUIContent();
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyOnGUI;
        }

        [InitializeOnLoadMethod]
        private static void Enable() => _instance = new HierarchyTool();

        private void HierarchyOnGUI(int instanceId, Rect selectionRect)
        {
            _icon.Dispose();
            _icon.ID = instanceId;
            _icon.IconRect = selectionRect;
            _icon.Go = (GameObject)EditorUtility.InstanceIDToObject(_icon.ID);
            
            if (_icon.Go == null) 
                return;
            
            _icon.Display();
            _icon.NameRect = _icon.IconRect;
            
            var nameStyle = new GUIStyle(TreeView.DefaultStyles.label);
            _icon.NameRect.width = nameStyle.CalcSize(new GUIContent(_icon.Go.name)).x;
            _icon.NameRect.x += 8;
            _nameDistance = _icon.NameRect.x + _icon.NameRect.width;
            _nameDistance += 8;
            
            ShowComponent();
        }

        private void ShowComponent()
        {
            var components = _icon.Go.GetComponents(typeof(Component)).ToList<Object>();

            var count = components.Count;
            _nameDistance += 4;

            for (int i = 0; i < count; ++i)
            {
                var component = components[i];
                if (component == null) continue;
                var type = component.GetType();
                var rect = ComponentPosition(_icon.NameRect, 12, ref _nameDistance);

                ComponentIcon(component, type, rect);
                _nameDistance += 2;
            }
        }

        private void ComponentIcon(Object component, Type componentType, Rect rect)
        {
            if (CurrentEvent.type == EventType.Repaint)
            {
                var image = EditorGUIUtility.ObjectContent(component, componentType).image;
                var tooltip = componentType.Name;
                _content.tooltip = tooltip;
                GUI.Box(rect, _content, GUIStyle.none);
                GUI.DrawTexture(rect, image, ScaleMode.ScaleToFit);
            }

            if (rect.Contains(CurrentEvent.mousePosition))
            {
                if (CurrentEvent.type == EventType.MouseDown)
                {
                    if (CurrentEvent.button == 0)
                    {
                        _selectComponent = component;
                        UtilityRef.DisplayObjectContextMenu(rect, component, 0);
                        CurrentEvent.Use();
                        return;
                    }
                }
            }
            
            if (_selectComponent != null && CurrentEvent.type == EventType.MouseDown && CurrentEvent.button == 0 &&
                !rect.Contains(CurrentEvent.mousePosition))
            {
                _selectComponent = null;
            }
        }

        private static Rect ComponentPosition(Rect rect, float width, ref float result)
        {
            rect.xMin = 0;
            rect.x += result;
            rect.width = width;
            result += width;
            return rect;
        }

        private class HierarchyIcon
        {
            public int ID;
            public Rect IconRect;
            public Rect NameRect;
            public GameObject Go;

            public void Display()
            {
                var pos = CurrentEvent.mousePosition;
                var isHover = pos.x >= 0 && pos.x <= IconRect.xMax + 16 && pos.y >= IconRect.y &&
                              pos.y < IconRect.yMax;
                if (!isHover) return;
                var rect = new Rect(32 + 0.5f, IconRect.y, 16, IconRect.height);
                var isShow = EditorGUI.Toggle(rect, GUIContent.none, Go.activeSelf);
                var active = Go.activeSelf;
                Go.SetActive(isShow);
                if (active != Go.activeSelf)
                {
                    EditorUtility.SetDirty(Go);
                }
            }

            public void Dispose()
            {
                ID = int.MinValue;
                IconRect = Rect.zero;
                NameRect = Rect.zero;
                Go = null;
            }
        }
    }
}
#endif