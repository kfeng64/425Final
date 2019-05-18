using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabHitBox : MonoBehaviour
{
	public Animator anim;
	BoxCollider col;
	public Player1Combat playerCombat;
	public Player2Movement opponent;
	string opponentTag = "Player2";

	// Start is called before the first frame update
	void Start()
    {
		col = GetComponent<BoxCollider>();
		col.enabled = false;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
