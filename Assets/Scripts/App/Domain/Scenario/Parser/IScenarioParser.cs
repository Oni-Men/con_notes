using App.Domain.Scenario;

namespace Domain.Scenario.Parser
{
    public interface IScenarioParser
    {
        public ScenarioRoot TryParseScenario(string data);
    }
}