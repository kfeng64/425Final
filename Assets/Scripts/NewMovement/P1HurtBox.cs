using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P1HurtBox : MonoBehaviour {

    string opponentBody = "BodyCollider2";
    public Player1Movement p1;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    void OnTriggerStay(Collider collision) {
        
        if (collision.gameObject.tag == opponentBody) {
            p1.PushOutFromOtherCollider();
        }

    }
}
