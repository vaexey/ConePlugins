using ConeEngine.Model.Device;
using ConeEngine.Model.Entry.Bind;
using ConeEngine.Model.Flow;
using ConeEngine.Plugin;
using Newtonsoft.Json.Linq;
using Serilog;

namespace ConeVoicemeeter
{
    public class Voicemeeter : Plugin
    {
        public override string Name => "Voicemeeter API";

        public override string Description => "Voicemeeter audio console API";

        public override string ID => "voicemeeter";

        public VmClient Client { get; set; } = new();

        public VoicemeeterDevice Device { get; set; }
        protected bool hasGivenDevice = false;

        public List<VoicemeeterBindNode> ActiveBindNodes = new();

        public Voicemeeter()
        {
            Device = new(this);
        }

        public override Result Enable(Context ctx)
        {
            enabled = true;

            return base.Enable(ctx);
        }

        public override Result Disable(Context ctx)
        {
            enabled = false;
            hasGivenDevice = false;
            ActiveBindNodes.Clear();

            return base.Disable(ctx);
        }

        public override Result Update(Context ctx)
        {
            if(Client.Poll())
            {
                Log.Verbose("VM : Poll received.");

                foreach (var bn in ActiveBindNodes)
                    bn.OnVM(Client.GetParam(bn.Signature));
            }

            return base.Update(ctx);
        }

        public override Result<Device> RegisterDevice(Context ctx, JObject config)
        {
            if (hasGivenDevice)
                return Result.Error<Device>("Voicemeeter can have only one device.");

            hasGivenDevice = true;

            return Result.VAL<Device>(Device);
        }
    }

    public class VoicemeeterDevice : Device
    {
        protected Voicemeeter vm;

        public VoicemeeterDevice(Voicemeeter vm)
        {
            this.vm = vm;
        }

        public override Result<BindNode> CreateBindNode(Context ctx, JObject config)
        {
            var bn = new VoicemeeterBindNode(
                config.Value<string>("sign"),
                0,
                vm
            );

            vm.ActiveBindNodes.Add(bn);

            return Result.VAL<BindNode>(bn);
        }
    }
}