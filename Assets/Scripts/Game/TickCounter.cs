using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickCounter : MonoBehaviour {
    public static Action<int> onTick;
    public static float tickInterval = 5;
    private int ticks = 1;
    private float counter = 1;
    void Update(){
        counter -= Time.deltaTime;
        if(counter <= 0){
            onTick?.Invoke(ticks);
            ticks += 1;
            if(ticks == 101) ticks = 1;
            counter = tickInterval;
        }
    }
}
