using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

public class CustomConsoleWindow : EditorWindow {
    private static List<Message> _logMessages = new();
    private Vector2 _scrollPos;
    private GUIStyle messageStyle;
    private GUIStyle messageCountStyle;



    [MenuItem("Window/CustomConsole")]
    public static void ShowWindow() {
        GetWindow<CustomConsoleWindow>("Custom console");
    }

    private void OnEnable() {
        messageStyle = new GUIStyle {
            wordWrap = true,
            richText = true,
            fontSize = 15,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
        };
        messageStyle = new GUIStyle {
            wordWrap = true,
            richText = true,
            fontSize = 15,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleRight,
        };

    }
    public static void UpdateLog(List<Message> messages) {
        _logMessages = new List<Message>(messages);
        var window = GetWindow<CustomConsoleWindow>();
        window.Repaint();
    }

    private void OnGUI() {
        GUILayout.Label("Custom Log Messages", EditorStyles.boldLabel);
        _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, false);
        foreach (var message in _logMessages) {
            GUILayout.Label(message.content, messageStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label(message.count.ToString(), messageCountStyle, GUILayout.ExpandWidth(true));


        }
        GUILayout.EndScrollView();
    }

}
public class Message {
    public Message(string s, int c) {
        content = s;
        count = c;
    }
    public Message(string s, int c, float l) {
        content = s;
        count = c;
        lastOccurrence = l;
    }
    public int count;
    public float lastOccurrence;
    public string content;
}