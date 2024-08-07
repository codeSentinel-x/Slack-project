using UnityEngine;
using System.Collections.Generic;

namespace MyUtils.Custom {
    public static class CustomLog {
        private static List<Message> logMessages = new();

        public static void Log(string message, bool colWithSame) {

            int count = 0;
            if (colWithSame) {
                foreach (var m in logMessages) {
                    if (m.content == message) {
                        count = ++m.count;
                        m.lastOccurrence = Time.time;
                        break;
                    }
                }
            }
            if (count == 0) logMessages.Add(new Message(message, count, Time.time));
            CustomConsoleWindow.UpdateLog(logMessages);
        }
    }
}