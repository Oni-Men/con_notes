using System.Collections.Generic;

namespace App.Domain.Scenario
{
    public sealed class ScenarioPage
    {
        public string text;
        public string background;
        public List<ScenarioCommand> commands;

        public override string ToString()
        {
            return $"speaker: text: {text}, background: {background}, commands: {commands}";
        }
    }
}