using System.Collections;
using System.Collections.Generic;
using MyUtils.Classes;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SpawnRuleSO")]
public class SpawnRuleSO : ScriptableObject {
    public Transform _prefab;
    public int size;
    public Rule[] rules;
}

