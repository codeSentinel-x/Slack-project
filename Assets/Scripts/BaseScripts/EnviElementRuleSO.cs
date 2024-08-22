using MyUtils.Classes;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/EnviElementRuleSO")]
public class EnviElementRuleSO : ScriptableObject {
    public Transform _prefab;
    public int _size;
    public bool _useFirstSettingForAllBiomes;
    public Rule[] _rules;
}
