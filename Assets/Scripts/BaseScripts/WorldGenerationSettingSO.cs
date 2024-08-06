using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/WorldGenerationSettingSO")]
public class WorldGenerationSettingSO : ScriptableObject {
    public NoiseGenerationDataSO _dataSource;
    public BiomeAssetSO _biomeAssetSource;
    public EnvironmentRuleSO _environmentRuleSource;

}
