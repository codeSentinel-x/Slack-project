using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyUtils.Structs {

    static class MyRandom {
        public static Color GetColorFormGradient(Gradient g) {
            return g.Evaluate(Random.Range(0f, 1f));
        }
    }




}

