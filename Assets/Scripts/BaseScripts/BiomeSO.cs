using MyUtils.Structs;
using UnityEngine;



[CreateAssetMenu(menuName = "ScriptableObjects/BiomeSO")]
public class BiomeSO : ScriptableObject {
    public TerrainRuleAssetType terrainRuleAssetType;
    public TerrainRule[] _terrainRule;

}
