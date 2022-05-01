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

    public bool isDead;
    public float respawnTime;
    public GameObject respawnPointLoc;

    public bool inCombat;
    public float wanderTime;
    public float movementSpeed;

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
        if (!isDead)
        {
            if (wanderTime > 0)
            {
                transform.Translate(Vector3.forward * movementSpeed);
                wanderTime -= Time.deltaTime;
            }
            else
            {
                wanderTime = Random.Range(5, 15);
                Wander();
            }
        }
        if (currentHP <= 0 && !isDead)
        {
            isDead = true;
            currentHP = 0;
            respawnPointLoc.SendMessage("SpawnEnemy");
            Destroy(this.gameObject);
            //StartCoroutine(Death());
        }
    }
    void Wander()
    {
        transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
    }
    public void RecieveDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log("Damage done = " + damage);
        Debug.Log("Enemy HP = " + currentHP);
    }
    IEnumerator Death()
    {
        yield return new WaitForSeconds(respawnTime);
        //Send message
        Destroy(this.gameObject);
    }
}
