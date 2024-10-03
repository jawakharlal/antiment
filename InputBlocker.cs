using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApp1
{
    internal class InputBlocker
    {
        private static bool IsCtrl9Pressed()
        {
            return (Control.ModifierKeys == Keys.Control) && (Keyboard.IsKeyDown(Keys.D9));
        }
        private static IntPtr _keyboardHookID = IntPtr.Zero;
        private static IntPtr _mouseHookID = IntPtr.Zero;
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        public static void BlockInput()
        {
            _keyboardHookID = SetKeyboardHook(KeyboardProc);
            _mouseHookID = SetMouseHook(MouseProc);
        }

        public static void UnblockInput()
        {
            if (_keyboardHookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_keyboardHookID);
                _keyboardHookID = IntPtr.Zero;
            }

            if (_mouseHookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_mouseHookID);
                _mouseHookID = IntPtr.Zero;
            }
        }

        private static IntPtr SetKeyboardHook(LowLevelProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr SetMouseHook(LowLevelProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

        // Процедура для блокировки клавиатуры
        private static IntPtr KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)0x100) // WM_KEYDOWN
            {
                int vkCode = Marshal.ReadInt32(lParam); // Получаем код клавиши

                // Принудительно блокируем Esc, Tab, Win, Alt
                if (vkCode == (int)Keys.Escape || vkCode == (int)Keys.Tab ||
                    vkCode == (int)Keys.LWin || vkCode == (int)Keys.RWin ||
                    vkCode == (int)Keys.LMenu || vkCode == (int)Keys.RMenu) // Alt
                {
                    return (IntPtr)1; // Блокируем указанные клавиши
                }

                // Проверяем, нажата ли комбинация Ctrl + 9 напрямую по кодам клавиш
                bool ctrlPressed = (GetAsyncKeyState((int)Keys.LControlKey) & 0x8000) != 0 ||
                                   (GetAsyncKeyState((int)Keys.RControlKey) & 0x8000) != 0;

                if (ctrlPressed && vkCode == (int)Keys.D9)
                {
                    return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam); // Пропускаем комбинацию Ctrl + 9
                }

                return (IntPtr)1; // Блокировать все остальные клавиши
            }
            return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
        }




        // Процедура для блокировки мыши
        private static IntPtr MouseProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                return (IntPtr)1; // Блокировать любое движение и действия мыши
            }
            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }

        // WinAPI функции для хуков
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;
    }
}


