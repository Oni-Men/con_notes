using App.Domain.Scenario;
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

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                scenarioView.CancelScenarioPlay();
            }
        }
        
        public void OnClickCloseButton()
        {
            scenarioView.CancelScenarioPlay();
        }
    }
}