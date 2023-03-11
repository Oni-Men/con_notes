using System.Collections.Generic;

namespace App.Domain.Scenario
{
    public sealed class ScenarioCommand
    {
        public string name;
        public List<string> args;
    }
}