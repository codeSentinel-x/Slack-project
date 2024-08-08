using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MyUtils.Custom;
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
    private GUIStyle _previewMessageStyle;
    private GUIStyle _messageCountStyle;
    private GUIStyle _foldoutStyle = null;
    private GUIStyle _messageBorderStyle;
    private GUIStyle _foldoutBorderStyle;
    #endregion
    private Dictionary<string, MessagesHolder> _foldouts = new();
    private bool _updateMessages;

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
        _previewMessageStyle = new() {
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
        _foldoutStyle = new GUIStyle() {
            wordWrap = true,
            richText = true,
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
            normal = { textColor = Color.white }

        };
    }

    public static void UpdateLog(List<MessagesHolder> messages) {
        _logMessages = new List<MessagesHolder>(messages);
        var window = GetWindow<CustomConsoleWindow>();
        window.Repaint();
    }

    private void OnGUI() {
        if (!_updateMessages) return;
        GUILayout.Label("Custom Log Messages", EditorStyles.boldLabel);
        _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, false);


        GenerateButtons();
        GenerateFoldouts();

        GUILayout.EndScrollView();
    }

    private void GenerateButtons() {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Show all", GUILayout.MaxWidth(75f))) {
            foreach (var messagesH in _logMessages) {
                messagesH.isEnabled = true;
            }
        }
        if (GUILayout.Button("Hide all", GUILayout.MaxWidth(75f))) {
            foreach (var messagesH in _logMessages) {
                messagesH.isEnabled = false;
            }
        }
        if (GUILayout.Button("Clear", GUILayout.MaxWidth(75f))) {
            _logMessages = new();
            _foldouts = new();
            CustomLog.ClearLogs();
        }
        if (GUILayout.Button("Stop logs", GUILayout.MaxWidth(75f))) {
            _updateMessages = false;
        }
        if (GUILayout.Button("Start logs", GUILayout.MaxWidth(75f))) {
            _updateMessages = true;
        }
        GUILayout.EndHorizontal();
    }
    private void GenerateFoldouts() {
        foreach (var messagesHolder in _logMessages) {
            if (!_foldouts.ContainsKey(messagesHolder.tag)) {
                _foldouts.Add(messagesHolder.tag, messagesHolder);
            }

            GUILayout.BeginVertical(_foldoutBorderStyle, GUILayout.ExpandWidth(false)); //start of box main box
            GUILayout.BeginHorizontal(); //start of foldout label
            _foldouts[messagesHolder.tag].isEnabled = EditorGUILayout.Foldout(_foldouts[messagesHolder.tag].isEnabled, $"|| {messagesHolder.tag} ||", true, _foldoutStyle);
            if (messagesHolder.messages.Count > 0) {
                GUILayout.Label(messagesHolder.messages[^1].content, _previewMessageStyle, GUILayout.ExpandWidth(false));
            }
            if (messagesHolder.totalCount > 0) GUILayout.Label($"({messagesHolder.totalCount + 1})     ", _messageCountStyle, GUILayout.ExpandWidth(false));
            GUILayout.Label($" [{FormatTime(messagesHolder.lastOccurrence)}]  ", _messageCountStyle, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal(); //end of foldout label

            if (_foldouts[messagesHolder.tag].isEnabled) {
                for (int i = messagesHolder.messages.Count - 1; i >= 0; i--) {
                    var message = messagesHolder.messages[i];

                    GUILayout.BeginVertical(_messageBorderStyle, GUILayout.ExpandWidth(true)); //start of message box
                    GUILayout.BeginHorizontal();    //start of message label
                    GUILayout.Label(message.content, _messageStyle, GUILayout.ExpandWidth(true));
                    if (message.count > 0) GUILayout.Label($"({message.count + 1})     ", _messageCountStyle, GUILayout.ExpandWidth(false));
                    GUILayout.Label($"Last occurrence: [{FormatTime(message.lastOccurrence)}]  ", _messageCountStyle, GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();     //end of message label
                    GUILayout.EndVertical();    //end of message box
                }

            }
            GUILayout.EndVertical(); //end of main box


        }

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
        var timeSpan = TimeSpan.FromSeconds(time);
        return $"{timeSpan:hh\\:mm\\:ss}";
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