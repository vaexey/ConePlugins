using ConeEngine.Model.Device;
using ConeEngine.Model.Entry.Bind;
using ConeEngine.Model.Flow;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeMQTT
{
    public class MQTTDevice : Device
    {
        public string URL { get; set; }

        protected string username { get; set; }
        
        // TODO: INSECURE
        protected string password { get; set; }

        protected MqttFactory factory = new();

        public MQTTDevice(string url, string user, string pass)
        {
            URL = url;
            username = user;
            password = pass;
        }

        public override Result<BindNode> CreateBindNode(Context ctx, JObject config)
        {
            var sign = config.Value<string>("sign");

            var bn = new MQTTNode(this, sign);

            return Result.VAL<BindNode>(bn);
        }

        public async Task SendMessage(string sign, double val)
        {
            using (var client = factory.CreateMqttClient())
            {
                var opt = new MqttClientOptionsBuilder()
                    .WithTcpServer(URL)
                    .WithCredentials(username, password)
                    .Build();

                await client.ConnectAsync(opt, CancellationToken.None);

                var msg = new MqttApplicationMessageBuilder()
                    .WithTopic($"cone/{sign}")
                    .WithPayload(val.ToString())
                    .Build();

                await client.PublishAsync(msg, CancellationToken.None);
                await client.DisconnectAsync();

                Log.Verbose("Finished sending MQTT packet to {0}", $"cone/{sign}");
            }
        }
    }
}
