using System;
using System.Linq;
using Mas.Rpc;
using Capnp.Rpc;
using Newtonsoft.Json.Linq;
using FBPLib;
using ST = Mas.Rpc.Common.StructuredText;
using System.Collections.Generic;
using Monica;
using System.Text;

namespace Components
{
    [InPort("IN", description = "Structured text output of MONICA")]
    [InPort("SPLIT", description = "Split sections into separated text streams. true | false (default)", type = typeof(String))]
    [OutPort("OUT")]
    [ComponentDescription("Create CSV Text out of structured text")]
    class CreateMonicaCSV : Component
    {
        IInputPort _inPort;
        IInputPort _splitPort;
        bool _split = false;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _splitPort.Receive();
            if (p != null)
            {
                var str = p.Content.ToString()?.ToUpper();
                Drop(p);
                _splitPort.Close();
                if (str != null) _split = str == "TRUE";
            }

            p = _inPort.Receive();
            if (p != null)
            {
                if (p.Content is ST st)
                {
                    Drop(p);
                    if (st == null) return;




                    p = Create(st);
                    _outPort.Send(p);
                }
            }
        }

        private List<string> CreateCSVStrings()
        {
            var res = new List<string>();


            return res;
        }

        private string WriteOutputObj(IEnumerable<OId> outputIds, IEnumerable<JObject> values, char csvSep)
        {
            //using namespace std::string_literals;
            char[] escapeTokens = { '\n', '\"', csvSep };

            var sb = new StringBuilder();

            if (!values.Any())
            {
                foreach (var o in values)
                {
                    var i = 0;
                    var oidsSize = outputIds.Count();
                    foreach (var oid in outputIds)
                    {
                        var csvSep_ = i + 1 == oidsSize ? "" : "" + csvSep;
                        var j = o[oid.OutputName()];
                        if (j != null)
                        {
                            switch (j.Type)
                            {
                                case JTokenType.Float: 
                                    sb.Append(j.Value<float>()).Append(csvSep_);
                                    break;
                                case JTokenType.String: 
                                    sb.Append(j.ToString().IndexOfAny(escapeTokens) == -1
                                    ? j.ToString()
                                    : "\"" + j.ToString() + "\"").Append(csvSep_);
                                    break;
                                case JTokenType.Boolean: 
                                    sb.Append(j.Value<bool>()).Append(csvSep_); 
                                    break;
                                case JTokenType.Array:
                                    {
                                        int jvi = 0;
                                        var jSize = j.Count();
                                        foreach (var jv in j)
                                        {
                                            var csvSep__ = jvi + 1 == jSize ? "" : "" + csvSep;
                                            switch (jv.Type)
                                            {
                                                case JTokenType.Float:
                                                    sb.Append(jv.Value<float>()).Append(csvSep_);
                                                    break;
                                                case JTokenType.String:
                                                    sb.Append(jv.ToString().IndexOfAny(escapeTokens) == -1
                                                    ? jv.ToString()
                                                    : "\"" + jv.ToString() + "\"").Append(csvSep__);
                                                    break;
                                                case JTokenType.Boolean:
                                                    sb.Append(jv.Value<bool>()).Append(csvSep_);
                                                    break;
                                                default:
                                                    sb.Append("UNKNOWN").Append(csvSep__);
                                                    break;
                                            }
                                            ++jvi;
                                        }
                                        sb.Append(csvSep_);
                                        break;
                                    }
                                default: 
                                    sb.Append("UNKNOWN").Append(csvSep_);
                                    break;
                            }
                        }
                        ++i;
                    }
                    sb.Append("\n");
                }
            }
            return sb.ToString();
        }


        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _splitPort = OpenInput("SPLIT");
            _outPort = OpenOutput("OUT");
        }
    }
}
