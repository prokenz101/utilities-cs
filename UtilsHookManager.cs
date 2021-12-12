using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace utilities_cs {
    public class HookExistsException : Exception { }
    public class HookManager {
        static Dictionary<string, KeyboardHook> keyboardHooks = new();

        public static void AddHook(string id, KeyboardHook hook) {
            if (!keyboardHooks.ContainsKey(id)) {
                keyboardHooks.Add(id, hook);
            } else { throw new HookExistsException(); }
        }
        public static void AddHook(string id, ModifierKeys modifiers, Keys keys, Action onPressed, Action? onFail = null) {
            KeyboardHook hook = new();
            hook.KeyPressed += delegate {
                onPressed();
            };
            try {
                hook.RegisterHotKey(modifiers, keys);
            } catch (InvalidOperationException) {
                onFail?.Invoke();
                return;
            }
            AddHook(id, hook);
        }

        public static void UnregisterAllHooks() {
            foreach (KeyboardHook hook in keyboardHooks.Values) {
                hook.Dispose();
            }
            keyboardHooks.Clear();
        }

        public static void UnregisterHook(string id) {
            keyboardHooks[id].Dispose();
            keyboardHooks.Remove(id);
        }
    }
}