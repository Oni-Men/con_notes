using System.Collections.Generic;
using System.Text;

namespace App.Domain.Scenario
{
    public sealed class ScenarioRoot
    {
        public int id;
        public string title;
        public List<ScenarioPage> pages;

        public List<ScenarioPage> GetPages()
        {
            return pages;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"ScenarioID: {id}");
            foreach (var page in pages)
            {
                sb.AppendLine(page.ToString());
            }

            return sb.ToString();
        }
    }
}