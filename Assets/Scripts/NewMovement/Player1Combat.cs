using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Combat : MonoBehaviour {

    public Player1Movement p1;
    public Player2Movement p2;
    public GameObject o1, o2;

    bool canAttack;
    bool isComboing;
    float comboTime;
    int combo;
    public Animator anim;

    // Start is called before the first frame update
    void Start() {
        canAttack = true;
        isComboing = false;
        comboTime = 0.0f;
        combo = 0;
        
    }

    // Update is called once per frame
    void Update() {
        
        if (Input.GetKeyDown(KeyCode.E)) {
            if (canAttack) {
                Punch();
            }
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
        p1.hasControl = false;
        isComboing = true;
        float attackTime = 0.0f;
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
                default:
                    attackTime = -1;
                    break;
            }
        }
        

        if (attackTime != -1) {
            canAttack = false;
            Invoke("AttackCoolDown", attackTime / 3.5f);

            comboTime = attackTime;

            if (comboTime > 0) {
                switch (combo) {
                    case 1:
                        anim.Play("KB_m_Jab_R");
                        p1.SetHitDist(0.2f, 1, -2);
                        p2.SetHitDist(0.2f, 1, -2);
                        break;
                    case 2:
                        anim.Play("KB_m_Jab_L");
                        p1.SetHitDist(0.2f, 1, -2);
                        p2.SetHitDist(0.2f, 1, -2);
                        break;
                    case 3:
                        anim.Play("KB_p_Uppercut_R");
                        p1.SetHitDist(0.5f, 10, -2);
                        p2.SetHitDist(0.025f, .5f, 10.0f);
                        Physics.IgnoreCollision(o1.GetComponent<CharacterController>(), o2.GetComponent<CharacterController>(), true);
                        Invoke("ResetCollision", .75f);
                        break;
                }
            }
            p1.startedAttack = true;
            p2.KnockBack(transform.forward);

            if (comboTime > 0) {
                CancelInvoke("SetHasControlTrue");
            }
            Invoke("SetHasControlTrue", attackTime);
            p1.oldPosition = transform.position;
        }
    }

    void SetHasControlTrue() {
        p1.hasControl = true;
    }

    void AttackCoolDown() {
        canAttack = true;
    }

    void ResetCollision() {
        Physics.IgnoreCollision(o1.GetComponent<CharacterController>(), o2.GetComponent<CharacterController>(), false);
    }

    private void OnTriggerStay(Collider other) {
        if (Input.GetKeyDown(KeyCode.E) && other.CompareTag("Player2")) {
            transform.LookAt(other.transform);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }
}
