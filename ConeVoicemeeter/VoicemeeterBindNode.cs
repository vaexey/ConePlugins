using ConeEngine.Model.Entry.Bind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeVoicemeeter
{
    public class VoicemeeterBindNode : BindNode
    {
        public string Signature { get; set; }
        public double LastValue { get; set; }

        protected Voicemeeter plugin;

        public VoicemeeterBindNode(string signature, double lastValue, Voicemeeter plugin)
        {
            Signature = signature;
            LastValue = lastValue;
            this.plugin = plugin;
        }

        public override double Get()
        {
            return LastValue;
        }

        public override void Set(double value)
        {
            LastValue = value;

            plugin.Client.SetParam(Signature, (float)value);
        }

        public virtual void OnVM(double value)
        {
            if (Math.Abs(value - LastValue) < 0.01)
                return;

            LastValue = value;
            SetChanged(true);
        }
    }
}
