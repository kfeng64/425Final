using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAttackHitBox : MonoBehaviour
{
    public Animator anim;
    KeyCode spin = KeyCode.R;
    SphereCollider col;
    public Player1Combat playerCombat;
    public Player2Movement opponentMov;
    float delayBetweenHits = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<SphereCollider>();
        col.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(spin) && playerCombat.canAttack) {
            EnableHitBox();
        }


        if (col.enabled == false) {
            opponentMov.isInSpinCollider = false;
        } else {
            if (opponentMov.isInSpinCollider) {
                if (delayBetweenHits <= 0) {
                    opponentMov.SpinHit(transform.forward);
                    
                    opponentMov.sentAirborne = true;
                    delayBetweenHits = 0.25f;
                } else {
                    delayBetweenHits -= Time.deltaTime;
                }
            }
        }
    }

    void EnableHitBox() {
        float attackTime = -1.0f;
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips) {
            switch (clip.name) {
                case "SpinAttack":
                    attackTime = clip.length;
                    break;
            }
        }
        col.enabled = true;
        Invoke("DisableHitBox", attackTime - .5f);

    }

    void DisableHitBox() {
        col.enabled = false;
        
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Player2") {
            opponentMov.isInSpinCollider = true;
        }
    }
}
