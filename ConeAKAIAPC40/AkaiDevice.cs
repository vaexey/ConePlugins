using ConeEngine.Model.Device;
using ConeEngine.Model.Entry.Bind;
using ConeEngine.Model.Flow;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeAKAIAPC40
{
    public class AkaiDevice : Device
    {
        protected AKAIAPC40 plugin;

        public AkaiDevice(AKAIAPC40 plugin)
        {
            this.plugin = plugin;
        }

        public override Result<BindNode> CreateBindNode(Context ctx, JObject config)
        {
            var sign = config.Value<string>("sign");

            var bn = new AkaiBindNode(sign, 0, plugin);

            plugin.ActiveBindNodes.Add(bn);

            return Result.VAL<BindNode>(bn);
        }
    }
}
