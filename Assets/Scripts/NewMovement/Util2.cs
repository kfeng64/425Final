using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util2 : MonoBehaviour {

    private static float timer;

    private void Update() {
        if (timer > 0) {
            ProgressTimer();
        }
    }

    public static void StartTimer(float val) {
        if (timer == 0) {
            timer = val;
        }
    }

    public static bool TimerStatus() {
        if (timer <= 0) {
            return false;
        } else {
            return true;
        }
    }

    private static void ProgressTimer() {
        timer -= Time.deltaTime;
        if (timer <= 0) {
            timer = 0;
        }
    }

}