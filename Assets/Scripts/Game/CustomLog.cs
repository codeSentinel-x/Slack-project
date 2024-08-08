using UnityEngine;
using System.Collections.Generic;
using System;

namespace MyUtils.Custom {
    public static class CustomLog {
        private static List<MessagesHolder> logMessages = new();

        public static void Log(string message, string tag, bool colWithSame) {

            bool found = false;

            foreach (var m in logMessages) {
                if (m.tag == tag) {
                    m.AddMessage(message, colWithSame);
                    found = true;
                    break;
                }
            }

            if (!found) logMessages.Add(new MessagesHolder(tag, message));
            CustomConsoleWindow.UpdateLog(logMessages);
        }

        public static void ClearLogs() {
            logMessages = new();
        }
    }
}