using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Monica
{
    public class RunMonica
    {
        private readonly MonicaIO _monicaIO;

        public RunMonica(MonicaIO monicaIO)
        {
            _monicaIO = monicaIO;
        }

        public async Task<JObject> RunMonicaAsync(List<string> files)
        {
            var simFile = files.FirstOrDefault(file => Path.GetFileName(file).Contains("sim"));
            var simj = JObject.Parse(File.ReadAllText(simFile ?? "Data/sim.json")); 
            
            var siteFile = files.FirstOrDefault(file => Path.GetFileName(file).Contains("site"));
            var sitej = JObject.Parse(File.ReadAllText(siteFile ?? "Data/site.json"));

            var cropFile = files.FirstOrDefault(file => Path.GetFileName(file).Contains("crop"));
            var cropj = JObject.Parse(File.ReadAllText(cropFile ?? "Data/crop.json"));

            var climateFile = files.FirstOrDefault(file => Path.GetFileName(file).Contains("climate"));
            var climateCsv = File.ReadAllText(climateFile ?? "Data/climate.csv");

            return CreateMonicaEnv(simj, cropj, sitej, climateCsv);
        }

        public JObject CreateMonicaEnv(JObject simj, JObject cropj, JObject sitej, string climateCsv)
        {
            JObject crop_site_sim = new JObject() {
                {"sim", simj},
                {"crop", cropj},
                {"site", sitej},
                {"climate", climateCsv}
            };

            string parametersPath = Environment.GetEnvironmentVariable("MONICA_PARAMETERS");
            
            var envj = _monicaIO.create_env_json_from_json_config(crop_site_sim, parametersPath);
            return envj;
        }
    }

}
