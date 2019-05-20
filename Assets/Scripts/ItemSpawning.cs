using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawning : MonoBehaviour
{
	float timer;
	public float spawnRate;
	public GameObject item;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < 0) {
			timer = spawnRate;
			Instantiate(item, new Vector3(Random.Range(-10, 10), 10, Random.Range(-15, 15)), Quaternion.identity);
		} else {
			timer -= Time.deltaTime;
		}
    }
}
