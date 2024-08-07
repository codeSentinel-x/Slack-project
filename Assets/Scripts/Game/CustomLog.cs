using UnityEngine;
using System.Collections.Generic;

namespace MyUtils.Custom {
    public static class CustomLog {
        private static List<string> logMessages = new();

        public static void Log(string message) {
            logMessages.Add(message);
            CustomConsoleWindow.UpdateLog(logMessages);
        }
    }
}