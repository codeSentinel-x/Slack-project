using MyUtils.Structs;
using UnityEngine;



[CreateAssetMenu(menuName = "ScriptableObjects/BiomeSO")]
public class BiomeSO : ScriptableObject {
    public TerrainRule[] _terrainRule;
    public float maxHumidity;
    public float maxTemperature;

}

