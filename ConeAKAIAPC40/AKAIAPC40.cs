﻿using ConeEngine.Model.Device;
using ConeEngine.Model.Flow;
using ConeEngine.Plugin;
using Newtonsoft.Json.Linq;
using Sanford.Multimedia.Midi;
using Serilog;

namespace ConeAKAIAPC40
{
    public class AKAIAPC40 : Plugin
    {
        public override string Name => "AKAI APC 40";

        public override string ID => "akaiapc40";

        public override string Description => "AKAI APC 40 communication plugin";

        protected string MidiName = "Akai";

        protected OutputDevice od;
        protected InputDevice id;

        public List<AkaiBindNode> ActiveBindNodes = new();

        public override Result Enable(Context ctx)
        {
            od = new OutputDevice(GetMidiOutputDevice(MidiName));
            id = new InputDevice(GetMidiInputDevice(MidiName));

            id.ChannelMessageReceived += OnMidi;
            id.Error += HandleError;
            id.StartRecording();

            od.Error += HandleError;

            od.Send(new SysExMessage(new byte[] { 0xf0, 0x47, 0x00, 0x73, 0x60, 0x00, 0x04, 0x42, 0, 0, 0, 0xF7 }));

            enabled = true;

            return Result.OK;
        }

        public override Result Disable(Context ctx)
        {
            this.Disable();

            return Result.OK;
        }

        protected void Disable()
        {
            enabled = false;

            try { od.Dispose(); } catch { }
            try { id.Dispose(); } catch { }
        }

        public override Result<Device> RegisterDevice(Context ctx, JObject config)
        {
            return Result.VAL<Device>(new AkaiDevice(this));
        }

        private static int GetMidiInputDevice(string partialDeviceName)
        {
            for (int i = 0; i < InputDevice.DeviceCount; i++)
            {
                var info = InputDevice.GetDeviceCapabilities(i);
                if (info.name.Contains(partialDeviceName))
                    return i;
            }
            throw new Exception($"Cannot find input midi device with '{partialDeviceName}' in the name.");
        }
        private static int GetMidiOutputDevice(string partialDeviceName)
        {
            for (int i = 0; i < OutputDeviceBase.DeviceCount; i++)
            {
                var info = OutputDeviceBase.GetDeviceCapabilities(i);
                if (info.name.Contains(partialDeviceName))
                    return i;
            }
            throw new Exception($"Cannot find output midi device with '{partialDeviceName}' in the name.");
        }

        protected virtual void OnMidi(object? o, ChannelMessageEventArgs e)
        {
            var m = e.Message;

            var chs = (m.Bytes[0]).ToString("X");
            var nhs = m.Data1.ToString("X");

            var sign = chs + nhs;
            sign = sign.ToLower();

            var value = m.Data2;

            Log.Verbose("AKAI INPUT::SIGN {0} VAL {1}", sign, value);

            foreach(var bn in ActiveBindNodes)
            {
                if(bn.Signature == sign)
                {
                    bn.OnAkai(value);
                }
            }

            if(sign.StartsWith("9") || sign.StartsWith("8"))
            {
                var down = sign.StartsWith("9");

                foreach(var bn in ActiveBindNodes.Where(bn => bn.Signature.StartsWith("x")))
                {
                    bn.OnAkai(down ? 1 : 0);
                }
            }
        }

        public virtual void SendMidi(string signature, byte value)
        {
            byte[] bytes = { Convert.ToByte(signature.Substring(0, 2), 16),
                             Convert.ToByte(signature.Substring(2,2), 16),
                             value, 0};

            int mint = BitConverter.ToInt32(bytes, 0);

            Log.Verbose("AKAI OUTPUT::SIGN {0} VAL {1}", signature, value);

            try
            {
                od.SendShort(mint);
            }
            catch(Exception ex)
            {
                Log.Warning("AKAI : Could not send midi: {0}. Disabling...", ex.Message);
                this.Disable();
            }
        }

        public override Result Update(Context ctx)
        {
            //throw new NotImplementedException();
            return Result.OK;
        }

        protected void HandleError(object? obj, Sanford.Multimedia.ErrorEventArgs e)
        {
            Log.Warning("AKAI : MIDI device error: {0}. Disabling...", e.Error.Message);

            this.Disable();
        }
    }
}