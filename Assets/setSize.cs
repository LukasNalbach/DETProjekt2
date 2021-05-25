using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setSize : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale = new Vector3(1, 1, 1) * Game.Instance.Settings.viewDistance /30;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
