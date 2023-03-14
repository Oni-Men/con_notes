using App.Domain.Scenario;

namespace Domain.Scenario.Parser
{
    public interface IScenarioParser
    {
        public ScenarioData TryParseScenario(string data);
    }
}