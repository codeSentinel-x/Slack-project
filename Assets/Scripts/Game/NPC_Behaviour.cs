using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using MyUtils.Custom;
public class NPC_Behaviour : MonoBehaviour {
    [Range(0f, 100f)] public float _food = 100;
    [Range(0f, 100f)] public float _sleep = 100;

    public List<Food> _inventory = new();
    public int _speed;
    public int _viewRange;
    public bool _isSleeping;
    public bool _isDead;

    private List<Vector3> _transforms = new();
    private int _index;
    private float _birthTime;

    void Awake() {
        _birthTime = Time.time;
        TickCounter._onTick += (x) => {
            if (_isDead) return;
            if (_isSleeping) {
                if (x % 7 == 0) DecreesFood();
                if (x % 2 == 0) Sleep();
            }
            else {
                if (x % 5 == 0) DecreesFood();
                if (x % 4 == 0) DecreesSleep();

            }
        };
    }

    private void Sleep() {
        _sleep += 1;
        if (_sleep > 15 && _food < 20) {
            WakeUp("Hungry");
        }
        else if (_sleep > 90) {
            WakeUp("Rested");
        }
    }


    private void DecreesSleep() {
        _sleep -= 1;
        if (_sleep <= 0) {
            GoToSleep();
        }
        else if (_sleep < 25) {
            LookForBed();
        }
    }

    private void WakeUp(string reason) {
        Log("waked up.", reason: reason, color: Color.blue, collapse: false);
        _isSleeping = false;
    }
    private void LookForBed() {
        Log("is looking for bed");
    }

    private void GoToSleep() {
        Log("fallen asleep", color: Color.cyan, reason: "Exhaustion", collapse: false);
        _isSleeping = true;
    }

    public void DecreesFood() {
        _food -= 1;
        if (_food <= 0) {
            Die("Starvation");
            return;
        }
        else if (_food < 30 && !_isSleeping) {

            LookForFood();
        }
    }

    private void LookForFood() {
        if (_inventory.Count > 0) {
            _food += _inventory[0].addition;
            Log($"restore {_inventory[0].addition} points of food", reason: $"Used {_inventory[0].name} from inventory", color: Color.green,  collapse: false);
            _inventory.RemoveAt(0);
        }
        else {
            if (GetFoodInRange() == null) {
                GoToRandomTile();
            }
        }
    }

    private void GoToRandomTile() {
        Log("is going to random tile");
    }

    private List<Food> GetFoodInRange() {
        Log("is looking for food in range");
        return null;
    }

    private void Die(string reason) {
        Log("died", reason: reason, color: Color.red, lifetime: true, collapse: false);
        _isDead = true;
    }

    public void Update() {
        if (_index >= _transforms.Count) return;
        if (_transforms.Count > 0) {
            transform.position = Vector2.MoveTowards(transform.position, _transforms[_index], _speed * Time.deltaTime);
            if (transform.position == _transforms[_index]) _index++;
        }
    }
    public void Log(string message, string reason = "", Color color = default, bool lifetime = false, bool bold = false, bool collapse = true) {

        string finalMessage = $"Creature: {gameObject.name} {message}";

        if (color == default) color = Color.white;
        finalMessage = $"<color=#{color.ToHexString()}>||   {finalMessage}   ||   </color><color=#{Color.white.ToHexString()}>";
        if (reason != "") finalMessage += $"Reason: {reason}.   ||   ";
        if (lifetime) finalMessage += $"Lifetime: {Time.time - _birthTime:f2} seconds.   ||   ";
        if (bold) finalMessage = $"<b>{finalMessage}</b>";

        CustomLog.Log(finalMessage, collapse);

    }
}

[Serializable]
public class Food {
    public string name;
    public int addition;
}