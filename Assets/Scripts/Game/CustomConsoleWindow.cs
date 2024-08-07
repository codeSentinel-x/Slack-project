using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

public class CustomConsoleWindow : EditorWindow {
    private static List<MessagesHolder> _logMessages = new();
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
    public static void UpdateLog(List<MessagesHolder> messages) {
        _logMessages = new List<MessagesHolder>(messages);
        var window = GetWindow<CustomConsoleWindow>();
        window.Repaint();
    }

    private void OnGUI() {
        GUILayout.Label("Custom Log Messages", EditorStyles.boldLabel);
        _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, false);

        foreach (var messageHandler in _logMessages) {

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(_borderStyle, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();
            EditorGUILayout.BeginFoldoutHeaderGroup();
            foreach (var message in messageHandler.messages) {
                GUILayout.Label(message.content, _messageStyle, GUILayout.ExpandWidth(true));
                if (message.count > 0) GUILayout.Label($"({message.count + 1})     ", _messageCountStyle, GUILayout.ExpandWidth(false));
                GUILayout.Label($"Last occurrence: [{message.lastOccurrence:f2}]  ", _messageCountStyle, GUILayout.ExpandWidth(false));
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
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
public class MessagesHolder {
    public string tag;
    public List<Message> messages = new();
    public int totalCount = 0;
    public MessagesHolder(string t, string fM) {
        tag = t;
        messages = new() { new Message(fM, 0, Time.time) };
    }
    public void AddMessage(string message) {
        var count = 0;
        foreach (var m in messages) {
            if (m.content == message) {
                count = totalCount = m.count + 1;
                m.count++;
                m.lastOccurrence = Time.realtimeSinceStartup;
                break;
            }
        }
        if (count == 0) messages.Add(new Message(message, count, Time.time));
    }
}
public class Message {
    public Message(string c, int i, float l) {
        content = c;
        count = i;
        lastOccurrence = l;
    }
    public string content;
    public int count;
    public float lastOccurrence;
}