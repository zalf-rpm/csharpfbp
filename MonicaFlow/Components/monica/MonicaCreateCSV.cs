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
using Newtonsoft.Json;

namespace Components
{
    [InPort("IN", description = "Structured text output of MONICA")]
    [InPort("OPTS", description = "Json Object with options like " +
        "split (true | false=default) -> split sections into separate text streams, " +
        "csvSep (char default=',') -> the csv separator to use, " +
        "addSSBrackets (true | false=default) -> will add brackets around the sections if split = true," +
        "includeSectionName (true=default | false) -> will add the section name before the header line," +
        "includeHeaderRow (true=default | false) -> include header row to section," +
        "includeUnitsRow (true=default | false) -> include units row to section," +
        "includeAggRows (true | false=default) -> include 2 aggregration rows to section", 
        type = typeof(String))]
    [OutPort("OUT")]
    [ComponentDescription("Create CSV Text out of structured text")]
    class MonicaCreateCSV : Component
    {
        IInputPort _inPort;
        IInputPort _optsPort;
        bool _split = false;
        bool _addSSBrackets = false;
        char _csvSep = ',';
        bool _includeSectionName = true;
        bool _includeHeaderRow = true;
        bool _includeUnitsRow = true;
        bool _includeAggRows = false;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _optsPort.Receive();
            if (p != null)
            {
                var str = p.Content?.ToString();
                Drop(p);
                _optsPort.Close();
                if (str != null)
                {
                    try
                    {
                        var j = JObject.Parse(str);
                        _split = j["split"]?.Value<bool>() ?? _split;
                        _addSSBrackets = j["addSSBrackets"]?.Value<bool>() ?? _addSSBrackets;
                        _includeSectionName = j["includeSectionName"]?.Value<bool>() ?? _includeSectionName;
                        _includeHeaderRow = j["includeHeaderRow"]?.Value<bool>() ?? _includeHeaderRow;
                        _includeUnitsRow = j["includeUnitsRow"]?.Value<bool>() ?? _includeUnitsRow;
                        _includeAggRows = j["includeAggRows"]?.Value<bool>() ?? _includeAggRows;
                        _csvSep = j["csvSep"]?.ToString().DefaultIfEmpty(_csvSep).First() ?? _csvSep;
                    }
                    catch (JsonReaderException) { }

                }
            }

            p = _inPort.Receive();
            if (p != null)
            {
                if (p.Content is ST st)
                {
                    Drop(p);
                    if (st == null) return;

                    if (st.Structure.which == ST.structure.WHICH.Json && !string.IsNullOrEmpty(st.Value))
                    {
                        try
                        {
                            var outputj = JObject.Parse(st.Value);

                            StringBuilder sb = null;
                            var dataj = outputj["data"];
                            var customId = outputj["customId"];
                            if (dataj != null)
                            {
                                if (_addSSBrackets && _split) _outPort.Send(Create(Packet.Types.Open, null));
                                foreach (var sectionj in dataj)
                                {
                                    if (_split) sb = new StringBuilder();
                                    if (sb == null) sb = new StringBuilder();

                                    var sectionName = "\""  + (sectionj["origSpec"]?.ToString() ?? "Unknow section").Replace("\"", "") + "\"";
                                    if(_includeSectionName) sb.Append(sectionName).Append('\n');
                                    var outputIds = sectionj["outputIds"].Select(jt => new OId(jt.Value<JObject>())).ToList();

                                    WriteOutputHeaderRows(sb, outputIds, _csvSep, _includeHeaderRow, _includeUnitsRow, _includeAggRows);
                                    var res = sectionj["results"];
                                    if (res != null && res.Any()) WriteOutput(sb, outputIds, res.Select(jt => (JArray)jt), _csvSep);
                                    else { 
                                        res = sectionj["resultsObj"];
                                        if (res != null && res.Any()) WriteOutputObj(sb, outputIds, res.Select(jt => (JObject)jt), _csvSep);
                                    }
                                    
                                    sb.Append('\n');

                                    if (_split)
                                    {
                                        var outp = Create(sb.ToString());
                                        if (customId != null) outp.Attributes.Add("customId", customId);
                                        _outPort.Send(outp);
                                    }
                                }
                                if (_addSSBrackets && _split) _outPort.Send(Create(Packet.Types.Close, null));
                                if (!_split)
                                {
                                    var outp = Create(sb.ToString());
                                    if (customId != null) outp.Attributes.Add("customId", customId);
                                    _outPort.Send(outp);
                                }
                            }
                        }
                        catch (JsonReaderException e) 
                        {
                            Console.WriteLine("Exception: " + e.Message);
                        }
                    }
                }
            }
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
                for (var k = 0; k < values.First().Count; k++)
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
                            case JTokenType.Integer:
                                sb.Append((int)j).Append(csvSep_);
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
                                            case JTokenType.Integer:
                                                sb.Append((int)jv).Append(csvSep__);
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
                                case JTokenType.Integer:
                                    sb.Append((int)j).Append(csvSep_);
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
                                                case JTokenType.Integer:
                                                    sb.Append((int)jv).Append(csvSep__);
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
            _optsPort = OpenInput("OPTS");
            _outPort = OpenOutput("OUT");
        }
    }
}
