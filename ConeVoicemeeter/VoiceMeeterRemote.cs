using System;
using System.Runtime.InteropServices;

namespace ConeVoicemeeter
{
    public enum VbLoginResponse
    {
        OK = 0,
        OkVoicemeeterNotRunning = 1,
        NoClient = -1,
        AlreadyLoggedIn = -2,
    }

    public static class VoiceMeeterRemote
    {
        [DllImport("VoicemeeterRemote64.dll", EntryPoint = "VBVMR_Login")]
        public static extern VbLoginResponse Login();
        [DllImport("VoicemeeterRemote64.dll", EntryPoint = "VBVMR_Logout")]
        public static extern VbLoginResponse Logout();

        [DllImport("VoicemeeterRemote64.dll", EntryPoint = "VBVMR_SetParameterFloat")]
        public static extern int SetParameter(string szParamName, float value);
        [DllImport("VoicemeeterRemote64.dll", EntryPoint = "VBVMR_GetParameterFloat")]
        public static extern int GetParameter(string szParamName, ref float value);
        [DllImport("VoicemeeterRemote64.dll", EntryPoint = "VBVMR_GetParameterStringA")]
        public static extern int GetParameter(string szParamName, ref string value);

        [DllImport("VoicemeeterRemote64.dll", EntryPoint = "VBVMR_IsParametersDirty")]
        public static extern int IsParametersDirty();
        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string dllToLoad);
        private static IntPtr? _dllHandle;
        public static void LoadDll(string dllPath)
        {
            if (!_dllHandle.HasValue)
            {
                _dllHandle = LoadLibrary(dllPath);

                var e = Marshal.GetLastWin32Error().ToString();

                var m = int.MaxValue;
            }
        }
    }
}
