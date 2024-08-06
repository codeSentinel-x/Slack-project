using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class NPC_Behaviour : MonoBehaviour {
    public float _food = 100;
    public float _sleep = 100;

    public List<Food> _inventory = new();
    public int _speed;
    public int _viewRange;

    private List<Vector3> _transforms;
    private int _index;

    void Awake() {
        TickCounter.onTick += (x) => {
            if (x % 5 == 0) DecreesFood();
            if (x % 4 == 0) DecreesSleep();
        };
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

    private void LookForBed() {
        Debug.Log($"Creature {gameObject.name} is looking for bed");
    }

    private void GoToSleep() {
        Debug.Log($"Creature {gameObject.name} fallen asleep");
    }

    public void DecreesFood() {
        _food -= 1;
        if (_food <= 0) {
            Die();
            return;
        }
        else if (_food < 30) {
            LookForFood();
        }
    }

    private void LookForFood() {
        if (_inventory.Count > 0) {
            _food += _inventory[0].addition;
            _inventory.RemoveAt(0);
        }
        else {
            if (GetFoodInRange() == null) {
                GoToRandomTile();
            }
        }
    }

    private void GoToRandomTile() {
        Debug.Log($"Creature {gameObject.name} is going to random tile");
    }

    private List<Food> GetFoodInRange() {
        Debug.Log($"Creature {gameObject.name} is looking for food in range");
        return null;
    }

    private void Die() {
        Debug.Log($"Creature named {gameObject.name} died");
    }

    public void Update() {
        if (_index >= _transforms.Count) return;
        if (_transforms.Count > 0) {
            transform.position = Vector2.MoveTowards(transform.position, _transforms[_index], _speed * Time.deltaTime);
            if (transform.position == _transforms[_index]) _index++;
        }
    }
}
public class Food {
    public int addition;
}