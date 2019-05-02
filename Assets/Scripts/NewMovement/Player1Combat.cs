using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Combat : MonoBehaviour {

    public Player1Movement player;
    public Player2Movement opponent;
    public GameObject o1, o2;

    public bool canAttack;
    bool isComboing;
    float comboTime;
    int combo;
    public Animator anim;
    string opponentTag = "Player2";
    KeyCode punch = KeyCode.E;
    KeyCode spin = KeyCode.R;

    // Start is called before the first frame update
    void Start() {
        canAttack = true;
        isComboing = false;
        comboTime = 0.0f;
        combo = 0;
        
    }

    // Update is called once per frame
    void Update() {
        
        if (Input.GetKeyDown(punch) && canAttack) {
            Punch();
        }

        if (Input.GetKeyDown(spin) && canAttack) {
            SpinAttack();
        }

        if (isComboing) {
            comboTime -= Time.deltaTime;
        }

        if (isComboing && comboTime <= 0) {
            isComboing = false;
            combo = 0;
        }
    }

    void Punch() {
        player.hasControl = false;
        isComboing = true;
        float attackTime = -1.0f;
        combo++;

        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips) {
            switch (clip.name) {
                case "KB_m_Jab_R":
                    if (combo == 1)
                        attackTime = clip.length;
                    break;
                case "KB_m_Jab_L":
                    if (combo == 2)
                        attackTime = clip.length;
                    break;
                case "KB_p_Uppercut_R":
                    if (combo == 3)
                        attackTime = clip.length;
                    break;
            }
        }

        if (attackTime != -1.0f) {
            canAttack = false;
            Invoke("AttackCoolDown", attackTime / 3.5f);

            comboTime = attackTime;

            if (comboTime > 0) {
                switch (combo) {
                    case 1:
                        anim.Play("KB_m_Jab_R");
                        player.SetHitDist(0.2f, 1, -2);
                        opponent.SetHitDist(0.2f, 1, -2);
                        break;
                    case 2:
                        anim.Play("KB_m_Jab_L");
                        player.SetHitDist(0.2f, 1, -2);
                        opponent.SetHitDist(0.2f, 1, -2);
                        break;
                    case 3:
                        anim.Play("KB_p_Uppercut_R");
                        player.SetHitDist(0.5f, 10, -2);
                        opponent.SetHitDist(0.025f, .5f, 7.0f);
                        opponent.sentAirborne = true;
                        //Physics.IgnoreCollision(o1.GetComponent<CharacterController>(), o2.GetComponent<CharacterController>(), true);
                        //Invoke("ResetCollision", .75f);
                        break;
                }
            }
            player.startedAttack = true;
            if (opponent.isInHitCollider) {
                opponent.KnockBack(transform.forward);
            }
            

            if (comboTime > 0) {
                CancelInvoke("SetHasControlTrue");
            }
            Invoke("SetHasControlTrue", attackTime);
            player.oldPosition = transform.position;
        }
    }

    void SpinAttack() {
        player.hasControl = true;
        isComboing = true;
        float attackTime = -1.0f;
        combo++;

        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips) {
            switch (clip.name) {
                case "SpinAttack":
                    attackTime = clip.length;
                    break;
            }
        }

        if (attackTime != -1.0f) {
            canAttack = false;
            Invoke("AttackCoolDown", attackTime);

            comboTime = attackTime;

            if (comboTime > 0) {
                anim.Play("SpinAttack");
                //opponent.SetHitDist(0.0f, 0.0f, 6.0f); 
            }

            player.startedAttack = true;
            //opponent.KnockBack(transform.forward);
            player.oldPosition = transform.position;
        }
    }

    void SetHasControlTrue() {
        player.hasControl = true;
    }

    void AttackCoolDown() {
        canAttack = true;
    }

    void ResetCollision() {
        Physics.IgnoreCollision(o1.GetComponent<CharacterController>(), o2.GetComponent<CharacterController>(), false);
    }

    private void OnTriggerStay(Collider other) {
        if (Input.GetKeyDown(KeyCode.E) && other.CompareTag(opponentTag)) {
            transform.LookAt(other.transform);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }
}
