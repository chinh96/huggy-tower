#if UNITY_EDITOR
using Lance.Common;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// thanks @roboryantron
/// </summary>
/// <summary>
/// Draws the custom enum selector popup for enum fileds using the
/// EnumAttribute.
/// </summary>
[CustomPropertyDrawer(typeof(EnumAttribute))]
public class EnumAttributeDrawer : PropertyDrawer
{
    private const string TYPE_ERROR = "Enum can only be used on enum fields.";

    /// <summary>
    /// Cache of the hash to use to resolve the ID for the drawer.
    /// </summary>
    private int _idHash;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // If this is not used on an eunum, show an error
        if (property.type != "Enum")
        {
            GUIStyle errorStyle = "CN EntryErrorIconSmall";
            Rect r = new Rect(position);
            r.width = errorStyle.fixedWidth;
            position.xMin = r.xMax;
            GUI.Label(r, "", errorStyle);
            GUI.Label(position, TYPE_ERROR);
            return;
        }

        // By manually creating the control ID, we can keep the ID for the
        // label and button the same. This lets them be selected together
        // with the keyboard in the inspector, much like a normal popup.
        if (_idHash == 0) _idHash = "EnumAttributeDrawer".GetHashCode();
        int id = GUIUtility.GetControlID(_idHash, FocusType.Keyboard, position);

        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, id, label);

        GUIContent buttonText;
        // If the enum has changed, a blank entry
        if (property.enumValueIndex < 0 || property.enumValueIndex >= property.enumDisplayNames.Length)
        {
            buttonText = new GUIContent();
        }
        else
        {
            buttonText = new GUIContent(property.enumDisplayNames[property.enumValueIndex]);
        }

        if (DropdownButton(id, position, buttonText))
        {
            Action<int> onSelect = i =>
            {
                property.enumValueIndex = i;
                property.serializedObject.ApplyModifiedProperties();
            };

            SearchablePopup.Show(position, property.enumDisplayNames, property.enumValueIndex, onSelect);
        }

        EditorGUI.EndProperty();
    }

    /// <summary>
    /// A custom button drawer that allows for a controlID so that we can
    /// sync the button ID and the label ID to allow for keyboard
    /// navigation like the built-in enum drawers.
    /// </summary>
    private static bool DropdownButton(int id, Rect position, GUIContent content)
    {
        Event current = Event.current;
        switch (current.type)
        {
            case EventType.MouseDown:
                if (position.Contains(current.mousePosition) && current.button == 0)
                {
                    Event.current.Use();
                    return true;
                }

                break;
            case EventType.KeyDown:
                if (GUIUtility.keyboardControl == id && current.character == '\n')
                {
                    Event.current.Use();
                    return true;
                }

                break;
            case EventType.Repaint:
                EditorStyles.popup.Draw(position, content, id, false);
                break;
        }

        return false;
    }
}

/// <summary>
/// A popup window that displays a list of options and may use a search
/// string to filter the displayed content. 
/// </summary>
public class SearchablePopup : PopupWindowContent
{
    #region -- Constants --------------------------------------------------

    /// <summary> Height of each element in the popup list. </summary>
    private const float ROW_HEIGHT = 16.0f;

    /// <summary> How far to indent list entries. </summary>
    private const float ROW_INDENT = 8.0f;

    /// <summary> Name to use for the text field for search. </summary>
    private const string SEARCH_CONTROL_NAME = "EnumSearchText";

    #endregion -- Constants -----------------------------------------------

    #region -- Static Functions -------------------------------------------

    /// <summary> Show a new SearchablePopup. </summary>
    /// <param name="activatorRect">
    /// Rectangle of the button that triggered the popup.
    /// </param>
    /// <param name="options">List of strings to choose from.</param>
    /// <param name="current">
    /// Index of the currently selected string.
    /// </param>
    /// <param name="onSelectionMade">
    /// Callback to trigger when a choice is made.
    /// </param>
    public static void Show(Rect activatorRect, string[] options, int current, Action<int> onSelectionMade)
    {
        SearchablePopup win = new SearchablePopup(options, current, onSelectionMade);
        PopupWindow.Show(activatorRect, win);
    }

    /// <summary>
    /// Force the focused window to redraw. This can be used to make the
    /// popup more responsive to mouse movement.
    /// </summary>
    private static void Repaint() { EditorWindow.focusedWindow.Repaint(); }

    /// <summary> Draw a generic box. </summary>
    /// <param name="rect">Where to draw.</param>
    /// <param name="tint">Color to tint the box.</param>
    private static void DrawBox(Rect rect, Color tint)
    {
        Color c = GUI.color;
        GUI.color = tint;
        GUI.Box(rect, "", selection);
        GUI.color = c;
    }

    #endregion -- Static Functions ----------------------------------------

    #region -- Helper Classes ---------------------------------------------

    /// <summary>
    /// Stores a list of strings and can return a subset of that list that
    /// matches a given filter string.
    /// </summary>
    private class FilteredList
    {
        /// <summary>
        /// An entry in the filtererd list, mapping the text to the
        /// original index.
        /// </summary>
        public struct Entry
        {
            public int index;
            public string text;
        }

        /// <summary> All posibile items in the list. </summary>
        private readonly string[] _allItems;

        /// <summary> Create a new filtered list. </summary>
        /// <param name="items">All The items to filter.</param>
        public FilteredList(string[] items)
        {
            _allItems = items;
            Entries = new List<Entry>();
            UpdateFilter("");
        }

        /// <summary> The current string filtering the list. </summary>
        public string Filter { get; private set; }

        /// <summary> All valid entries for the current filter. </summary>
        public List<Entry> Entries { get; private set; }

        /// <summary> Total possible entries in the list. </summary>
        public int MaxLength => _allItems.Length;

        /// <summary>
        /// Sets a new filter string and updates the Entries that match the
        /// new filter if it has changed.
        /// </summary>
        /// <param name="filter">String to use to filter the list.</param>
        /// <returns>
        /// True if the filter is updated, false if newFilter is the same
        /// as the current Filter and no update is necessary.
        /// </returns>
        public bool UpdateFilter(string filter)
        {
            if (Filter == filter)
                return false;

            Filter = filter;
            Entries.Clear();

            for (int i = 0; i < _allItems.Length; i++)
            {
                if (string.IsNullOrEmpty(Filter) || _allItems[i].ToLower().Contains(Filter.ToLower()))
                {
                    Entry entry = new Entry { index = i, text = _allItems[i] };
                    if (string.Equals(_allItems[i], Filter, StringComparison.CurrentCultureIgnoreCase))
                        Entries.Insert(0, entry);
                    else
                        Entries.Add(entry);
                }
            }

            return true;
        }
    }

    #endregion -- Helper Classes ------------------------------------------

    #region -- Private Variables ------------------------------------------

    /// <summary> Callback to trigger when an item is selected. </summary>
    private readonly Action<int> _onSelectionMade;

    /// <summary>
    /// Index of the item that was selected when the list was opened.
    /// </summary>
    private readonly int _currentIndex;

    /// <summary>
    /// Container for all available options that does the actual string
    /// filtering of the content.  
    /// </summary>
    private readonly FilteredList _list;

    /// <summary> Scroll offset for the vertical scroll area. </summary>
    private Vector2 _scroll;

    /// <summary>
    /// Index of the item under the mouse or selected with the keyboard.
    /// </summary>
    private int _hoverIndex;

    /// <summary>
    /// An item index to scroll to on the next draw.
    /// </summary>
    private int _scrollToIndex;

    /// <summary>
    /// An offset to apply after scrolling to scrollToIndex. This can be
    /// used to control if the selection appears at the top, bottom, or
    /// center of the popup.
    /// </summary>
    private float _scrollOffset;

    #endregion -- Private Variables ---------------------------------------

    #region -- GUI Styles -------------------------------------------------

    // GUIStyles implicitly cast from a string. This triggers a lookup into
    // the current skin which will be the editor skin and lets us get some
    // built-in styles.

    private static GUIStyle searchBox = "ToolbarSeachTextField";
    private static GUIStyle cancelButton = "ToolbarSeachCancelButton";
    private static GUIStyle disabledCancelButton = "ToolbarSeachCancelButtonEmpty";
    private static GUIStyle selection = "SelectionRect";

    #endregion -- GUI Styles ----------------------------------------------

    #region -- Initialization ---------------------------------------------

    private SearchablePopup(string[] names, int currentIndex, Action<int> onSelectionMade)
    {
        _list = new FilteredList(names);
        this._currentIndex = currentIndex;
        this._onSelectionMade = onSelectionMade;

        _hoverIndex = currentIndex;
        _scrollToIndex = currentIndex;
        _scrollOffset = GetWindowSize().y - ROW_HEIGHT * 2;
    }

    #endregion -- Initialization ------------------------------------------

    #region -- PopupWindowContent Overrides -------------------------------

    public override void OnOpen()
    {
        base.OnOpen();
        // Force a repaint every frame to be responsive to mouse hover.
        EditorApplication.update += Repaint;
    }

    public override void OnClose()
    {
        base.OnClose();
        EditorApplication.update -= Repaint;
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(base.GetWindowSize().x, Mathf.Min(600, _list.MaxLength * ROW_HEIGHT + EditorStyles.toolbar.fixedHeight));
    }

    public override void OnGUI(Rect rect)
    {
        Rect searchRect = new Rect(0, 0, rect.width, EditorStyles.toolbar.fixedHeight);
        Rect scrollRect = Rect.MinMaxRect(0, searchRect.yMax, rect.xMax, rect.yMax);

        HandleKeyboard();
        DrawSearch(searchRect);
        DrawSelectionArea(scrollRect);
    }

    #endregion -- PopupWindowContent Overrides ----------------------------

    #region -- GUI --------------------------------------------------------

    private void DrawSearch(Rect rect)
    {
        if (Event.current.type == EventType.Repaint)
            EditorStyles.toolbar.Draw(rect,
                false,
                false,
                false,
                false);

        Rect searchRect = new Rect(rect);
        searchRect.xMin += 6;
        searchRect.xMax -= 6;
        searchRect.y += 2;
        searchRect.width -= cancelButton.fixedWidth;

        GUI.FocusControl(SEARCH_CONTROL_NAME);
        GUI.SetNextControlName(SEARCH_CONTROL_NAME);
        string newText = GUI.TextField(searchRect, _list.Filter, searchBox);

        if (_list.UpdateFilter(newText))
        {
            _hoverIndex = 0;
            _scroll = Vector2.zero;
        }

        searchRect.x = searchRect.xMax;
        searchRect.width = cancelButton.fixedWidth;

        if (string.IsNullOrEmpty(_list.Filter))
            GUI.Box(searchRect, GUIContent.none, disabledCancelButton);
        else if (GUI.Button(searchRect, "x", cancelButton))
        {
            _list.UpdateFilter("");
            _scroll = Vector2.zero;
        }
    }

    private void DrawSelectionArea(Rect scrollRect)
    {
        Rect contentRect = new Rect(0, 0, scrollRect.width - GUI.skin.verticalScrollbar.fixedWidth, _list.Entries.Count * ROW_HEIGHT);

        _scroll = GUI.BeginScrollView(scrollRect, _scroll, contentRect);

        Rect rowRect = new Rect(0, 0, scrollRect.width, ROW_HEIGHT);

        for (int i = 0; i < _list.Entries.Count; i++)
        {
            if (_scrollToIndex == i && (Event.current.type == EventType.Repaint || Event.current.type == EventType.Layout))
            {
                Rect r = new Rect(rowRect);
                r.y += _scrollOffset;
                GUI.ScrollTo(r);
                _scrollToIndex = -1;
                _scroll.x = 0;
            }

            if (rowRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.ScrollWheel)
                    _hoverIndex = i;
                if (Event.current.type == EventType.MouseDown)
                {
                    _onSelectionMade(_list.Entries[i].index);
                    EditorWindow.focusedWindow.Close();
                }
            }

            DrawRow(rowRect, i);

            rowRect.y = rowRect.yMax;
        }

        GUI.EndScrollView();
    }

    private void DrawRow(Rect rowRect, int i)
    {
        if (_list.Entries[i].index == _currentIndex)
            DrawBox(rowRect, Color.cyan);
        else if (i == _hoverIndex)
            DrawBox(rowRect, Color.white);

        Rect labelRect = new Rect(rowRect);
        labelRect.xMin += ROW_INDENT;

        GUI.Label(labelRect, _list.Entries[i].text);
    }

    /// <summary>
    /// Process keyboard input to navigate the choices or make a selection.
    /// </summary>
    private void HandleKeyboard()
    {
        if (Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.DownArrow)
            {
                _hoverIndex = Mathf.Min(_list.Entries.Count - 1, _hoverIndex + 1);
                Event.current.Use();
                _scrollToIndex = _hoverIndex;
                _scrollOffset = ROW_HEIGHT;
            }

            if (Event.current.keyCode == KeyCode.UpArrow)
            {
                _hoverIndex = Mathf.Max(0, _hoverIndex - 1);
                Event.current.Use();
                _scrollToIndex = _hoverIndex;
                _scrollOffset = -ROW_HEIGHT;
            }

            if (Event.current.keyCode == KeyCode.Return)
            {
                if (_hoverIndex >= 0 && _hoverIndex < _list.Entries.Count)
                {
                    _onSelectionMade(_list.Entries[_hoverIndex].index);
                    EditorWindow.focusedWindow.Close();
                }
            }

            if (Event.current.keyCode == KeyCode.Escape)
            {
                EditorWindow.focusedWindow.Close();
            }
        }
    }

    #endregion -- GUI -----------------------------------------------------
}


#endif