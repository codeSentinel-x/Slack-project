using UnityEngine;
using System.Collections.Generic;

namespace MyUtils.Custom {
    public static class CustomLog {
        private static List<Message> logMessages = new();

        public static void Log(string message) {
            int count = 0;
            foreach (var m in logMessages) {
                if (m.content == message) {
                    count = ++m.count;
                    break;
                }
            }
            if (count == 0) logMessages.Add(new Message(message, count));
            CustomConsoleWindow.UpdateLog(logMessages);
        }
    }
}