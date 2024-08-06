using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/EnvironmentRuleSO")]
public class EnvironmentRuleSO : ScriptableObject {
    public int size;
    public Transform _prefab;
    public SpawnRuleSO[] rulesSO;

}
