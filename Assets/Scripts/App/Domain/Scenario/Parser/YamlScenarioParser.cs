using System.IO;
using Domain.Scenario;
using Domain.Scenario.Parser;
using YamlDotNet.Serialization;

namespace App.Domain.Scenario.Parser
{
    public class YamlScenarioParser : IScenarioParser
    {
        public ScenarioRoot TryParseScenario(string data)
        {
            using var reader = new StringReader(data);
            var deserializer = new Deserializer();
            var deserialized = deserializer.Deserialize<ScenarioRoot>(reader);
            return deserialized;
        }
    }
}