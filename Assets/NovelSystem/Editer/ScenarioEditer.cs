using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NovelSystem.Data;

namespace NovelSystem.Editer
{

    [CreateAssetMenu(menuName = "Assets/Create Scenario", fileName = "Scenario")]
    public class ScenarioEditer : ScriptableObject
    {
        [SerializeField]
        public List<ScenarioMaster> scenarioData = new List<ScenarioMaster>();
    }
}