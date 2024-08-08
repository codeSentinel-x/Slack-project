using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomConsoleWindow : EditorWindow {
    private static List<MessagesHolder> _logMessages = new();
    private Vector2 _scrollPos;
    #region Gui styles
    private GUIStyle _messageStyle;
    private GUIStyle _messageCountStyle;
    private GUIStyle _foldoutStyle = null;
    private GUIStyle _messageBorderStyle;
    private GUIStyle _foldoutBorderStyle;
    #endregion
    private Dictionary<string, MessagesHolder> _foldouts = new();

    [MenuItem("Window/CustomConsole")]
    public static void ShowWindow() {
        GetWindow<CustomConsoleWindow>("Custom console");
    }

    private void OnEnable() {
        InitializeStyles();

    }

    private void InitializeStyles() {
        _messageStyle = new() {
            wordWrap = true,
            richText = true,
            fontSize = 11,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
            normal = { textColor = Color.white }
        };

        _messageCountStyle = new() {
            wordWrap = true,
            richText = true,
            fontSize = 11,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleRight,
            normal = { textColor = Color.white },
        };
        _messageBorderStyle = new() {
            normal = { background = GenerateTexture(2, 2, new Color(0.2f, 0.2f, 0.2f, 1f)) },
            padding = new RectOffset(10, 10, 10, 10),
            margin = new RectOffset(0, 0, 5, 5),
            border = new RectOffset(1, 1, 1, 1),

        };
        _foldoutBorderStyle = new() {
            normal = { background = GenerateTexture(2, 2, new Color(0.1f, 0.1f, 0.1f, 1f)) },
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

        _foldoutStyle ??= new GUIStyle(EditorStyles.foldout) {
            wordWrap = true,
            richText = true,
            fontSize = 10,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
            normal = { textColor = Color.green }
        };

        foreach (var messagesHolder in _logMessages) {
            if (!_foldouts.ContainsKey(messagesHolder.tag)) {
                _foldouts.Add(messagesHolder.tag, messagesHolder);
            }

            GUILayout.BeginVertical(_foldoutBorderStyle, GUILayout.ExpandWidth(false)); //start of box main box
            GUILayout.BeginHorizontal();
            _foldouts[messagesHolder.tag].isEnabled = EditorGUILayout.Foldout(_foldouts[messagesHolder.tag].isEnabled, messagesHolder.tag, _foldoutStyle);
            if (messagesHolder.totalCount > 0) GUILayout.Label($"({messagesHolder.totalCount + 1})     ", _messageCountStyle, GUILayout.ExpandWidth(false));
            GUILayout.Label($" [{FormatTime(messagesHolder.lastOccurrence)}]  ", _messageCountStyle, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();

            if (_foldouts[messagesHolder.tag].isEnabled) {
                for (int i = messagesHolder.messages.Count - 1; i >= 0; i--) {
                    var message = messagesHolder.messages[i];
                    // GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical(_messageBorderStyle, GUILayout.ExpandWidth(true));
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(message.content, _messageStyle, GUILayout.ExpandWidth(true));
                    if (message.count > 0) GUILayout.Label($"({message.count + 1})     ", _messageCountStyle, GUILayout.ExpandWidth(false));
                    GUILayout.Label($"Last occurrence: [{FormatTime(message.lastOccurrence)}]  ", _messageCountStyle, GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    // GUILayout.EndHorizontal();
                }

            }
            GUILayout.EndVertical(); //end of main box


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
    public string FormatTime(float time) {
        int hours, minutes;
        int roundedTime = Mathf.FloorToInt(time);
        hours = roundedTime / 3600;
        if (hours > 0) roundedTime -= hours * 3600;
        minutes = roundedTime / 60;
        if (minutes > 0) roundedTime -= minutes * 60;
        string nullString = "00";
        return $"{(hours > 0 ? Format(hours) : nullString)}:{(minutes > 0 ? Format(minutes) : nullString)}:{(roundedTime > 0 ? Format(roundedTime) : nullString)}";

        static string Format(int v) {
            if (v < 10) return $"0{v}";
            else return $"{v}";
        }
    }

}
public class MessagesHolder {
    public string tag;
    public bool isEnabled = false;
    public List<Message> messages = new();
    public int totalCount = 0;
    public float lastOccurrence = 0;
    public MessagesHolder(string t, string fM) {
        tag = t;
        messages = new() { new Message(fM, 0, Time.time) };
    }
    public void AddMessage(string message, bool collapse) {
        if (collapse) {
            bool found = false;
            foreach (var m in messages) {
                if (m.content == message) {
                    totalCount++;
                    m.count++;
                    m.lastOccurrence = Time.time;
                    lastOccurrence = Time.time;
                    found = true;
                    break;
                }
            }
            if (!found) {
                messages.Add(new Message(message, 0, Time.time));
                totalCount += 1;
                lastOccurrence = Time.time;
            }
        }
        else {
            messages.Add(new Message(message, 0, Time.time));
            totalCount += 1;
            lastOccurrence = Time.time;
        }
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