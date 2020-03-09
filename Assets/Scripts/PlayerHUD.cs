using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public Slider expBar;
    float maxExp = 1;
    float currentExp = 0;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        expBar.value = CalculateExp();
        player = GlobalVars.GetObject("player");
    }

    // Update is called once per frame
    void Update()
    {
        currentExp = GlobalVars.xp;
    }

    float CalculateExp()
    {
        return currentExp / maxExp;
    }

}
