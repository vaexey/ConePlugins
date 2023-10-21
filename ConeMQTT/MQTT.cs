using ConeEngine.Model.Device;
using ConeEngine.Model.Flow;
using ConeEngine.Plugin;
using Newtonsoft.Json.Linq;

namespace ConeMQTT
{
    public class MQTT : Plugin
    {
        public override string Name => "MQTT";
        public override string Description => "Allows to send messages through MQTT";
        public override string ID => "mqttplugin";

        public override Result<Device> RegisterDevice(Context ctx, JObject config)
        {
            var url = config.Value<string>("url");
            var user = config.Value<string>("user");
            var pass = config.Value<string>("password");

            var d = new MQTTDevice(url, user, pass);

            return Result.VAL<Device>(d);
        }

        public override Result Enable(Context ctx)
        {
            enabled = true;

            return base.Enable(ctx);
        }

        public override Result Disable(Context ctx)
        {
            enabled = false;

            return base.Disable(ctx);
        }
    }
}