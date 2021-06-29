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

                    if()

                    for (const auto&d : output.data)
		{
			out << "\"" << replace(d.origSpec, "\"", "") << "\"" << endl;
                        writeOutputHeaderRows(out, d.outputIds, csvSep, includeHeaderRow, includeUnitsRow, includeAggRows);
                        writeOutput(out, d.outputIds, d.results, csvSep);
			out << endl;

                    }



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

        private void WriteOutputHeaderRows(StringBuilder sb,
            IEnumerable<OId> outputIds, char csvSep,
            bool includeHeaderRow,
            bool includeUnitsRow,
            bool includeTimeAgg)
        {
            var oss1 = new StringBuilder();
            var oss2 = new StringBuilder();
            var oss3 = new StringBuilder();
            var oss4 = new StringBuilder();

            //using namespace std::string_literals;
            char[] escapeTokens = { '\n', '\"', csvSep };

            var j = 0;
            var oidsSize = outputIds.Count();
            foreach (var oid in outputIds)
            {
                int fromLayer = oid.FromLayer, toLayer = oid.ToLayer;
                bool isOrgan = oid.IsOrgan();
                bool isRange = oid.IsRange() && oid.LayerAggOp == OId.OP.NONE;
                if (isOrgan)
                    toLayer = fromLayer = (int)oid.Organ; // organ is being represented just by the value of fromLayer currently
                else if (isRange)
                { fromLayer++; toLayer++; } // display 1-indexed layer numbers to users

                else
                    toLayer = fromLayer; // for aggregated ranges, which aren't being displayed as range

                for (int i = fromLayer; i <= toLayer; i++)
                {
                    var oss11 = new StringBuilder();
                    if (isOrgan)
                        oss11.Append(string.IsNullOrEmpty(oid.DisplayName) ? oid.Name + "/" + oid.ToString(oid.Organ) : oid.DisplayName);
                    else if (isRange)
                        oss11.Append(string.IsNullOrEmpty(oid.DisplayName) ? oid.Name + "_" + i : oid.DisplayName);
                    else
                        oss11.Append(string.IsNullOrEmpty(oid.DisplayName) ? oid.Name : oid.DisplayName);
                    var csvSep_ = j + 1 == oidsSize && i == toLayer ? "" : csvSep.ToString();
                    oss1.Append(oss11.ToString().IndexOfAny(escapeTokens) == -1 ? oss11.ToString() : "\"" + oss11.ToString() + "\"").Append(csvSep_);
                    var os2 = "[" + oid.Unit + "]";
                    oss2.Append(os2.IndexOfAny(escapeTokens) == -1 ? os2 : "\"" + os2 + "\"").Append(csvSep_);
                    var os3 = "m:" + oid.ToString(includeTimeAgg);
                    oss3.Append(os3.IndexOfAny(escapeTokens) == -1 ? os3 : "\"" + os3 + "\"").Append(csvSep_);
                    var os4 = "j:" + oid.JsonInput.Replace("\"", "");
                    oss4.Append(os4.IndexOfAny(escapeTokens) == -1 ? os4 : "\"" + os4 + "\"").Append(csvSep_);
                }
                ++j;
            }

            if (includeHeaderRow) sb.Append(oss1).Append('\n');
            if (includeUnitsRow) sb.Append(oss2).Append('\n');
            if (includeTimeAgg) sb.Append(oss3).Append('\n').Append(oss4).Append('\n');
        }

        private void WriteOutput(StringBuilder sb, IEnumerable<OId> outputIds, IEnumerable<JArray> values, char csvSep)
        {
            //using namespace std::string_literals;
            char[] escapeTokens = { '\n', '\"', csvSep };

            if (values.Any())
            {
                for (var k = 0; k < values.First().Count(); k++)
                {
                    var  i = 0;
                    var oidsSize = outputIds.Count();
                    foreach (var oid in outputIds)
                    {
                        var csvSep_ = i + 1 == oidsSize ? "" : csvSep.ToString();
                        var j = values.ElementAt(i).ElementAt(k);
                        switch (j.Type)
                        {
                            case JTokenType.Float:
                                sb.Append((float)j).Append(csvSep_);
                                break;
                            case JTokenType.String:
                                sb.Append(j.ToString().IndexOfAny(escapeTokens) == -1 ? j : $"\"{j}\"").Append(csvSep_);
                                break;
                            case JTokenType.Boolean:
                                sb.Append((bool)j).Append(csvSep_);
                                break;
                            case JTokenType.Array:
                                {
                                    int jvi = 0;
                                    var jSize = j.Count();
                                    foreach (var jv in j)
                                    {
                                        var csvSep__ = jvi + 1 == jSize ? "" : csvSep.ToString();
                                        switch (jv.Type)
                                        {
                                            case JTokenType.Float:
                                                sb.Append((float)jv).Append(csvSep__);
                                                break;
                                            case JTokenType.String:
                                                sb.Append(j.ToString().IndexOfAny(escapeTokens) == -1 ? jv : $"\"{jv}\"").Append(csvSep__);
                                                break;
                                            case JTokenType.Boolean:
                                                sb.Append((bool)jv).Append(csvSep__);
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
                        ++i;
                    }
                    sb.Append('\n');
                }
            }
        }

        private void WriteOutputObj(StringBuilder sb, IEnumerable<OId> outputIds, IEnumerable<JObject> values, char csvSep)
        {
            //using namespace std::string_literals;
            char[] escapeTokens = { '\n', '\"', csvSep };

            if (values.Any())
            {
                foreach (var o in values)
                {
                    var i = 0;
                    var oidsSize = outputIds.Count();
                    foreach (var oid in outputIds)
                    {
                        var csvSep_ = i + 1 == oidsSize ? "" : csvSep.ToString();
                        var j = o[oid.OutputName()];
                        if (j != null)
                        {
                            switch (j.Type)
                            {
                                case JTokenType.Float: 
                                    sb.Append((float)j).Append(csvSep_);
                                    break;
                                case JTokenType.String:
                                    sb.Append(j.ToString().IndexOfAny(escapeTokens) == -1 ? j : $"\"{j}\"").Append(csvSep_);
                                    break;
                                case JTokenType.Boolean: 
                                    sb.Append((bool)j).Append(csvSep_); 
                                    break;
                                case JTokenType.Array:
                                    {
                                        int jvi = 0;
                                        var jSize = j.Count();
                                        foreach (var jv in j)
                                        {
                                            var csvSep__ = jvi + 1 == jSize ? "" : csvSep.ToString();
                                            switch (jv.Type)
                                            {
                                                case JTokenType.Float:
                                                    sb.Append((float)jv).Append(csvSep__);
                                                    break;
                                                case JTokenType.String:
                                                    sb.Append(j.ToString().IndexOfAny(escapeTokens) == -1 ? jv : $"\"{jv}\"").Append(csvSep__);
                                                    break;
                                                case JTokenType.Boolean:
                                                    sb.Append((bool)jv).Append(csvSep__);
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
                    sb.Append('\n');
                }
            }
        }


        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _splitPort = OpenInput("SPLIT");
            _outPort = OpenOutput("OUT");
        }
    }
}
