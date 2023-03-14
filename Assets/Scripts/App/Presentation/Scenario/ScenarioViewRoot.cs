using App.Domain.Scenario;
using Cysharp.Threading.Tasks;
using Database.Impl;
using UnityEngine;

namespace App.Presentation.Scenario
{
    public class ScenarioViewRoot : MonoBehaviour
    {
        public struct ScenarioViewParam
        {
            public ScenarioData scenarioData;
        }
        
        [SerializeField]
        private ScenarioView scenarioView;

        public void Initialize(ScenarioViewParam param)
        {
            scenarioView.Initialize(param.scenarioData);
        }
    }
}