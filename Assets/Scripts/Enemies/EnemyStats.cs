using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyStats : MonoBehaviour
{
    public string enemyName;
    public float currentHP;
    public float maxHP;

    public GameObject textName;
    public bool isSelected;


    private void Start()
    {
        currentHP = maxHP;
    }
    private void Update()
    {
        if (textName != null)
        {
            textName.transform.LookAt(Camera.main.transform.position);
            textName.transform.Rotate(0, 180, 0);
            textName.GetComponent<TextMeshPro>().color = Color.red;
            textName.GetComponent<TextMeshPro>().text = "" + enemyName;
        }
        if (currentHP <= 0)
            currentHP = 0;
    }
    public void RecieveDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log("Damage done = " + damage);
        Debug.Log("Enemy HP = " + currentHP);
    }
}
