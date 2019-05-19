using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlotHover : MonoBehaviour
{
	Transform origin;
	Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
		origin = transform.parent;
		offset = new Vector3(2, 0, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

		transform.position +=  Vector3.up * Mathf.Cos(Time.time) * .000001f;

	}
}
