using MyUtils.Structs;
using UnityEngine;



[CreateAssetMenu(menuName = "ScriptableObjects/BiomeSO")]
public class BiomeSO : ScriptableObject {
    public TerrainType[] _terrainTypes;
}
