using ConeEngine.Model.Entry.Bind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeAKAIAPC40
{
    public class AkaiBindNode : BindNode
    {
        public string Signature { get; set; }
        
        public double LastValue { get; set; }

        protected AKAIAPC40 plugin;

        protected bool ButtonSignature = false;

        public AkaiBindNode(string signature, double lastValue, AKAIAPC40 plugin)
        {
            Signature = signature;
            LastValue = lastValue;
            this.plugin = plugin;

            if (signature.ToLower().StartsWith("x"))
                ButtonSignature = true;
        }

        public override double Get()
        {
            return LastValue;
        }

        public override void Set(double value)
        {
            LastValue = value;

            if(!ButtonSignature)
                plugin.SendMidi(Signature, (byte)value);

            if(ButtonSignature)
            {
                var down = value >= 0.5;

                var bsgn = down ? "9" : "8";

                plugin.SendMidi(bsgn + Signature.Remove(0,1), (byte)Math.Round(value));
            }
        }

        public virtual void OnAkai(double value)
        {
            LastValue = value;
            SetPoll(true);
        }
    }
}
