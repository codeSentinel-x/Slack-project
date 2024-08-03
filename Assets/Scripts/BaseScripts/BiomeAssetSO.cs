using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "ScriptableObjects/BiomeAssetSO")]
public class BiomeAssetSO : ScriptableObject {

    [Tooltip("Scriptable object for desert biome settings.")]
    public BiomeSO DesertSO;

    [Tooltip("Scriptable object for jungle biome settings.")]
    public BiomeSO JungleSO;

    [Tooltip("Scriptable object for savanna biome settings.")]
    public BiomeSO SavannaSO;

    [Tooltip("Scriptable object for tundra biome settings.")]
    public BiomeSO TundraSO;

    [Tooltip("Scriptable object for taiga biome settings.")]
    public BiomeSO TaigaSO;

    [Tooltip("Scriptable object for grassland biome settings.")]
    public BiomeSO GrasslandSO;

    [Tooltip("Scriptable object for swamp biome settings.")]
    public BiomeSO SwampSO;

    [Tooltip("Scriptable object for forest biome settings.")]
    public BiomeSO ForestSO;


    public BiomeSO[] biomesSO;
    public BiomeSO GetBiomeSO2(float temp, float humidity) {
        for (int i = 0; i < biomesSO.Length; i++) {
            if (biomesSO[i].maxTemperature >= temp) {
                if (biomesSO[i].maxHumidity >= humidity) {
                    return biomesSO[i];
                }
            }
        }
        return ForestSO;
    }
    public BiomeSO GetBiomeSO(float temp, float humidity) {
        if (temp > 0.7f) {
            if (humidity < 0.3f) return DesertSO;
            if (humidity > 0.7f) return JungleSO;
            else return SavannaSO;
        }
        else if (temp < 0.3f) {
            if (humidity < 0.3f) return TundraSO;
            else return TaigaSO;
        }
        else {
            if (humidity < 0.3f) return GrasslandSO;
            if (humidity > 0.7) return SwampSO;
            else return ForestSO;
        }
    }
}
