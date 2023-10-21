using ConeEngine.Model.Entry.Bind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeMQTT
{
    public class MQTTNode : BindNode
    {
        public double LastValue { get; set; } = 0;
        public string Signature { get; set; } = "";

        protected MQTTDevice device;
        protected bool diff = true;

        public MQTTNode(MQTTDevice dev, string signature)
        {
            device = dev;
            Signature = signature;
        }

        public override double Get()
        {
            return LastValue;
        }

        public override void Set(double value)
        {
            LastValue = value;

            _ = device.SendMessage(Signature, value);
        }


        public override bool Diff()
        {
            if(diff)
            {
                diff = false;
                return true;
            }    

            return false;
        }
    }
}
