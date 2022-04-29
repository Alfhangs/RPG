using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    public BasicStats[] allClassStats;
    public bool classSelectWindow;
    public GameObject user;

    private void OnGUI()
    {
        if (classSelectWindow)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 150, 200, 40), "CLASS 1"))
            {
                AssingBaseStats(0);
                classSelectWindow = false;
            }
            if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 40), "CLASS 2"))
            {
                AssingBaseStats(1);
                classSelectWindow = false;
            }
        }
    }
    void AssingBaseStats(int index)
    {
        var comp = user.GetComponent<UserStats>();

        comp.userClass = allClassStats[index].userClass;
        comp.baseAttackPower = allClassStats[index].baseAttackPower;
        comp.baseAttackSpeed = allClassStats[index].baseAttackSpeed;
    }
}
