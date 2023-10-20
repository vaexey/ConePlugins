using Microsoft.Win32;
using Serilog;
using System;

namespace ConeVoicemeeter
{
    public class VmClient : IDisposable
    {
        private Action _onClose = null;
        private string GetVoicemeeterDir()
        {
            const string regKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            const string uninstKey = "VB:Voicemeeter {17359A74-1236-5467}";
            var key = $"{regKey}\\{uninstKey}";
            var k = Registry.GetValue(key, "UninstallString", null);
            if (k != null)
            {
                return System.IO.Path.GetDirectoryName(k.ToString());
            }


            const string regKey64 = @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
            const string uninstKey64 = "VB:Voicemeeter {17359A74-1236-5467}";
            var key64 = $"{regKey64}\\{uninstKey64}";
            var k64 = Registry.GetValue(key64, "UninstallString", null);

            if (k64 != null)
            {
                return System.IO.Path.GetDirectoryName(k64.ToString());
            }

            throw new Exception("Voicemeeter not found");
        }
        public VmClient()
        {
            //Find Voicemeeter dir.
            var vmDir = GetVoicemeeterDir();
            VoiceMeeterRemote.LoadDll(System.IO.Path.Combine(vmDir, "VoicemeeterRemote64.dll"));
            var lr = VoiceMeeterRemote.Login();
            switch (lr)
            {
                case VbLoginResponse.OK:
                    Log.Verbose("VM : Attached.");
                    break;
                case VbLoginResponse.AlreadyLoggedIn:
                    Log.Verbose("VM : Attached. Was already logged in");
                    break;
                case VbLoginResponse.OkVoicemeeterNotRunning:
                    //Launch.
                    Log.Verbose("VM : Attached. VM Not running.");
                    break;
                default:
                    throw new InvalidOperationException("Bad response from voicemeeter: " + lr);
            }
        }
        public float GetParam(string n)
        {
            float output = -1;
            VoiceMeeterRemote.GetParameter(n, ref output);
            return output;
        }
        public void SetParam(string n, float v)
        {
            VoiceMeeterRemote.SetParameter(n, v);
        }
        public bool Poll()
        {
            return VoiceMeeterRemote.IsParametersDirty() == 1;
        }
        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                Log.Verbose($"VM : VmClient Disposing {disposing}");
                _onClose?.Invoke();
                VoiceMeeterRemote.Logout();
            }
            disposed = true;
        }
        ~VmClient() { Dispose(false); }
        public void OnClose(Action a)
        {
            _onClose = a;
        }
    }
}
