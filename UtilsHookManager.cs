using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace utilities_cs {
    public class HookExistsException : Exception { }
    public class HookManager {
        static Dictionary<string, KeyboardHook> keyboardHooks = new();

        /// <summary>
        /// Adds the ID and a hook to a dictionary that keeps track of all keyboardhooks.
        /// If given ID already exits, HookExistsException will be raised.
        /// </summary>
        /// <param name="id">The ID of the keyboard hook.</param>
        /// <param name="hook">The keyboard hook being added to the dict.</param>
        public static void AddHook(string id, KeyboardHook hook) {
            if (!keyboardHooks.ContainsKey(id)) {
                keyboardHooks.Add(id, hook);
            } else { throw new HookExistsException(); }
        }

        /// <summary>
        /// Overload of original AddHook method.
        /// Allows you to add ModifierKeys and Regular Keys, onPressed method and onFail method.
        /// </summary>
        /// <param name="id">The ID of the keyboard hook.</param>
        /// <param name="modifiers">Modifier keys, like Ctrl or Alt.</param>
        /// <param name="keys">Main key pressed. Like F8 or X.</param>
        /// <param name="onPressed">Runs onPressed method if keyboardhook is pressed.</param>
        /// <param name="onFail">Runs onFail method if an error occurs while registering hook.</param>
        public static void AddHook(
                string id,
                ModifierKeys[] modifiers,
                Keys keys, Action onPressed,
                Action? onFail = null
            ) {
            KeyboardHook hook = new();
            hook.KeyPressed += delegate {
                onPressed();
            };
            try {
                if (modifiers.Length > 1) {
                    hook.RegisterHotKey(modifiers[0] | modifiers[1], keys);
                } else {
                    hook.RegisterHotKey(modifiers[0], keys);
                }
            } catch (InvalidOperationException) {
                onFail?.Invoke();
                return;
            }
            AddHook(id, hook);
        }

        /// <summary>Unregisteres all hooks registered.</summary>
        public static void UnregisterAllHooks() {
            foreach (KeyboardHook hook in keyboardHooks.Values) {
                hook.Dispose();
            }
            keyboardHooks.Clear();
        }

        /// <summary>Unregisters a specific hook using the ID.</summary>
        /// <param name="id">The ID of the hook that is going to be unregistered.</param>
        public static void UnregisterHook(string id) {
            keyboardHooks[id].Dispose();
            keyboardHooks.Remove(id);
        }
    }
}