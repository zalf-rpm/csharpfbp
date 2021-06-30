using FBPLib;
using System.Collections.Generic;
using System.Linq;

namespace Components
{
    [InPort("IN", description = "string with env vars to be replaced", type = typeof(string))]
    [OutPort("OUT", type = typeof(string))]
    [ComponentDescription("Replace environment variables in string. Env vars in string are of the form ${VAR_NAME}")]
    class ReplaceEnvVars : Component
    {
        IInputPort _inPort;
        OutputPort _outPort;

        public override void Execute()
        {
            var p = _inPort.Receive();
            if (p == null) return;
           
            var str = p.Content.ToString();
            Drop(p);
            List<string> envVars = new();
            var start = 0;
            while (true)
            {
                var i = str.IndexOf("${", start);
                if (i == -1) break;
                var varStart = i + 2;
                var e = str.IndexOf("}", varStart);
                if (e == -1) break;
                var afterVarEnd = e;
                if (varStart < afterVarEnd) envVars.Add(str[varStart..afterVarEnd]);
                start = e + 1;
            }

            foreach(var envVar in envVars)
            {
                str = str.Replace("${" + envVar + "}", System.Environment.GetEnvironmentVariable(envVar) ?? "${" + envVar + "}");
            }

            _outPort.Send(Create(str));
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _outPort = OpenOutput("OUT");
        }
    }
}
