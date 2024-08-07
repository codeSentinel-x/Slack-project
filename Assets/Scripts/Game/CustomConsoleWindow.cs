using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

public class CustomConsoleWindow : EditorWindow {
    private static List<Message> _logMessages = new();
    private Vector2 _scrollPos;
    private GUIStyle _messageStyle;
    private GUIStyle _messageCountStyle;
    private GUIStyle _borderStyle;


    [MenuItem("Window/CustomConsole")]
    public static void ShowWindow() {
        GetWindow<CustomConsoleWindow>("Custom console");
    }

    private void OnEnable() {
        _messageStyle = new GUIStyle {
            wordWrap = true,
            richText = true,
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
        };
        _messageCountStyle = new GUIStyle {
            wordWrap = true,
            richText = true,
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleRight,
            normal = { textColor = Color.white },
        };
        _borderStyle = new GUIStyle {
            normal = { background = GenerateTexture(2, 2, Color.black) },
            padding = new RectOffset(10, 10, 10, 10),
            margin = new RectOffset(0, 0, 5, 5),
            border = new RectOffset(1, 1, 1, 1)
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

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(_borderStyle, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label(message.content, _messageStyle, GUILayout.ExpandWidth(true));
            if (message.count > 0) GUILayout.Label($"({message.count + 1})     ", _messageCountStyle, GUILayout.ExpandWidth(false));
            GUILayout.Label($"Last occurrence: [{message.lastOccurrence:f2}]  ", _messageCountStyle, GUILayout.ExpandWidth(false));

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

        }
        GUILayout.EndScrollView();
    }
    private Texture2D GenerateTexture(int w, int h, Color c) {
        Texture2D t = new(w, h);
        Color32[] p = new Color32[w * h];

        for (int i = 0; i < p.Length; i++) {
            p[i] = c;
        }
        t.SetPixels32(p);
        t.Apply();
        return t;
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