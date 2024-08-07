using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomConsoleWindow : EditorWindow {
    private static List<string> _logMessages = new();
    private Vector2 _scrollPos;


    [MenuItem("Window/CustomConsole")]
    public static void ShowWindow() {
        GetWindow<CustomConsoleWindow>("Custom console");
    }

    public static void UpdateLog(List<string> messages) {
        _logMessages = new List<string>(messages);
        var window = GetWindow<CustomConsoleWindow>();
        window.Repaint();
    }

    private void OnGUI() {
        GUILayout.Label("Custom Log Messages", EditorStyles.boldLabel);
        _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, false);
        foreach (var message in _logMessages) {
            GUILayout.Label(message);
        }
    }
}