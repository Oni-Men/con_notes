using System.IO;
using Domain.Scenario.Parser;
using YamlDotNet.Serialization;

namespace App.Domain.Scenario.Parser
{
    public class YamlScenarioParser : IScenarioParser
    {
        public ScenarioData TryParseScenario(string data)
        {
            using var reader = new StringReader(data);
            var deserializer = new Deserializer();
            var deserialized = deserializer.Deserialize<ScenarioData>(reader);
            return deserialized;
        }
    }
}