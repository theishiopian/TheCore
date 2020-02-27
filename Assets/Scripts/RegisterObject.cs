using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterObject : MonoBehaviour
{
    public string key;

    // Start is called before the first frame update
    void Awake()
    {
        GlobalVars.RegisterObject(key, this.gameObject);
    }
}
