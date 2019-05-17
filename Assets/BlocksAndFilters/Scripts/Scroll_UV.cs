using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll_UV : MonoBehaviour {
    float scrollSpeed_X = 0.05f;
    float scrollSpeed_Y = 0.05f;

    // Start is called before the first frame update
    void Start() {
        
    }

    
    void Update() {
        var offsetX = Time.time * scrollSpeed_X;
        var offsetY = Time.time * scrollSpeed_Y;
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}
