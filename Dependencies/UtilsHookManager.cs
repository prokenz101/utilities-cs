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
            if (keyboardHooks.ContainsKey(id)) {
                keyboardHooks[id].Dispose();
                keyboardHooks.Remove(id);
            }
        }
    }

#nullable disable

    public sealed class KeyboardHook : IDisposable {
        //* Registers a hot key with Windows.
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        //* Unregisters the hot key with Windows.
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// Represents the window that is used internally to get the messages.
        /// </summary>
        private class Window : NativeWindow, IDisposable {
            private static int WM_HOTKEY = 0x0312;

            public Window() {
                //* create the handle for the window.
                this.CreateHandle(new CreateParams());
            }

            /// <summary>
            /// Overridden to get the notifications.
            /// </summary>
            /// <param name="m"></param>
            protected override void WndProc(ref Message m) {
                base.WndProc(ref m);

                //* check if we got a hot key pressed.
                if (m.Msg == WM_HOTKEY) {
                    //* get the keys.
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

                    //* invoke the event to notify the parent.
                    if (KeyPressed != null)
                        KeyPressed(this, new KeyPressedEventArgs(modifier, key));
                }
            }

            public event EventHandler<KeyPressedEventArgs> KeyPressed;

            #region IDisposable Members

            public void Dispose() {
                this.DestroyHandle();
            }

            #endregion
        }

        private Window _window = new Window();
        private int _currentId;

        public KeyboardHook() {
            //* register the event of the inner native window.
            _window.KeyPressed += delegate (object sender, KeyPressedEventArgs args) {
                if (KeyPressed != null)
                    KeyPressed(this, args);
            };
        }

        /// <summary>
        /// Registers a hot key in the system.
        /// </summary>
        /// <param name="modifier">The modifiers that are associated with the hot key.</param>
        /// <param name="key">The key itself that is associated with the hot key.</param>
        public void RegisterHotKey(ModifierKeys modifier, Keys key) {
            //* increment the counter.
            _currentId = _currentId + 1;

            //* register the hot key.
            if (!RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key))
                throw new InvalidOperationException("Couldn’t register the hot key.");
        }

        /// <summary>
        /// A hot key has been pressed.
        /// </summary>
        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        #region IDisposable Members

        public void Dispose() {
            //* unregister all the registered hot keys.
            for (int i = _currentId; i > 0; i--) {
                UnregisterHotKey(_window.Handle, i);
            }

            //* dispose the inner native window.
            _window.Dispose();
        }

        #endregion
    }

    /// <summary>
    /// Event Args for the event that is fired after the hot key has been pressed.
    /// </summary>
    public class KeyPressedEventArgs : EventArgs {
        private ModifierKeys _modifier;
        private Keys _key;

        internal KeyPressedEventArgs(ModifierKeys modifier, Keys key) {
            _modifier = modifier;
            _key = key;
        }

        public ModifierKeys Modifier {
            get { return _modifier; }
        }

        public Keys Key {
            get { return _key; }
        }
    }

    /// <summary>
    /// The enumeration of possible modifiers.
    /// </summary>
    [Flags]
    public enum ModifierKeys : uint {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }
}