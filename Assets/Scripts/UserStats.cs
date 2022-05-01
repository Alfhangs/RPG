using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStats : MonoBehaviour
{
    public string userName;
    public int level;
    public string userClass;

    public float currentHP;
    public float maxHP;
    public float currentMana;
    public float maxMana;

    public float baseAttackPower;
    public float currentAttackPower;
    public float baseAttackSpeed;
    public float currentAttackSpeed;
    public float baseDodge;
    public float currentDodge;
    public float baseHitPercent;
    public float currentHitPercent;
    public float hpRegenTimer;
    public float hpRegentAmount;
    public float manaRegentTimer;
    public float manaRegentAmount;

    public float currentXP;
    public float maxXP;

    public bool isDead;

    public GameObject selectedUnit;

    EnemyStats enemyStatsScript;

    public bool behindEnemy;
    public bool canAttack;

    public float autoAttackCooldown;
    public float autoAttackCurrentTime;
    public bool canAutoAttack;

    public float doubleClickTimer;
    public bool didDoubleClick;

    public LayerMask raycastLayers;
    public bool inLineOfSight;

    public bool hoverOverActive;
    public string hoverName;

    public float tickTime;

    public GameObject rangeSpellPrefab;

    //GUI Art
    public Texture hpBarTexture;
    public Texture manaBarTexture;
    public Texture barsBackgroundTexture;

    //USER GUI Bars Stats
    public float userHpBarLenght;
    public float percentOfUserHp;
    public float userManaBarLenght;
    public float percentOfUserMana;

    private void OnGUI()
    {
        //if (hoverOverActive)
            //GUI.Label(new Rect(Input.mousePosition.x - 100, Screen.height - Input.mousePosition.y, 100, 20), "" + hoverName);

        //HP and Mana Bars
        GUI.DrawTexture(new Rect(20, 30, 120, 70), barsBackgroundTexture);
        GUI.DrawTexture(new Rect(30, 40, userHpBarLenght, 20), hpBarTexture);
        GUI.DrawTexture(new Rect(30, 65, userManaBarLenght, 20), manaBarTexture);
        GUI.Label(new Rect(50, 40, 200, 20), "" + currentHP + "/" + maxHP);
        GUI.Label(new Rect(50, 65, 200, 20), "" + currentMana + "/" + maxMana);
    }
    private void Update()
    {
        //User bars calculated
        if (currentHP >= maxHP)
        {
            percentOfUserHp = currentHP / maxHP;
            userHpBarLenght = percentOfUserHp * 100;
            percentOfUserMana = currentMana / maxMana;
            userManaBarLenght = percentOfUserMana * 100;
        }
        //make sure mana and hp doesnt exceedd max values
        if (currentHP > maxHP) 
            currentHP = maxHP;
        if (currentMana > maxMana) 
            currentMana = maxMana;
        if (currentHP < 0)
            currentHP = 0;
        if (currentMana < 0)
            currentMana = 0;
        if (Input.GetMouseButtonDown(0))
        {
            SelectTarget(0);
        }
        if (Input.GetMouseButtonDown(1))
        {
            SelectTarget(1);
        }
        if (selectedUnit != null)
        {
            Vector3 toTarget = (selectedUnit.transform.position - transform.position).normalized;
            //Check if player is behind Enemy (Calculate dodge, parry, extra damage, etc)
            if (Vector3.Dot(toTarget, selectedUnit.transform.forward) < 0)
            {
                behindEnemy = false;
            }
            else
            {
                behindEnemy = true;
            }

            //Calculate if the player is facing the enemy and is within attack distance
            float distance = Vector3.Distance(this.transform.position, selectedUnit.transform.position);
            Vector3 targetDirection = selectedUnit.transform.position - transform.position;
            Vector3 forward = transform.forward;
            float angle = Vector3.Angle(targetDirection, forward);
            if (angle > 60.0)
            {
                canAttack = false;
            }
            else
            {
                if (distance < 60)
                {
                    canAttack = true;
                }
                else
                    canAttack = false;
            }
        }
        //AutoAttack
        if (selectedUnit != null && canAttack && canAutoAttack)
        {
            if (autoAttackCurrentTime < autoAttackCooldown)
                autoAttackCurrentTime += Time.deltaTime;
            else
            {
                BasicAttack();
                autoAttackCurrentTime = 0;
            }
            //TODO: Detect of theres an object blocking between selected enemy and player
            RaycastHit hit;
            if (Physics.Linecast(selectedUnit.transform.position, transform.position, out hit, raycastLayers))
            {
                Debug.Log("Blocked");
                inLineOfSight = false;
            }
            else
            {
                Debug.Log("Not blocked");
                inLineOfSight = true;
            }
        }



        //double click detect
        if (doubleClickTimer > 0)
        {
            doubleClickTimer -= Time.deltaTime;
        }
        else
        {
            didDoubleClick = false;
        }

        //Attacks
        if (Input.GetKeyDown("1"))
        {
            //TODO: make sure player is facing enemy and is in range
            if (selectedUnit != null && canAttack && inLineOfSight)
            {
                BasicAttack();
                canAutoAttack = true;

            }
        }

        //TODO: Ranged spell attack
        if (Input.GetKeyDown("2"))
        {
            RangeSpell();
        }
        //TODO: Tooltip popup display
        Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit2;
        if (Physics.Raycast(ray2, out hit2, 10000))
        {
            if (hit2.transform.tag == "Enemy")
            {
                hoverOverActive = true;
                hoverName = hit2.transform.GetComponent<EnemyStats>().enemyName;
            }
            else
                hoverOverActive = false;
        }
    }
    void SelectTarget(int index)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000))
        {
            if (hit.transform.tag == "Enemy")
            {
                selectedUnit = hit.transform.gameObject;
                enemyStatsScript = hit.transform.GetComponent<EnemyStats>();
                if (index == 0 && selectedUnit == null)
                {
                    canAutoAttack = false;
                }
                else if (index == 1)
                {
                    canAutoAttack = true;
                }
            }
            else
            {
                if (selectedUnit != null)
                {
                    if(didDoubleClick == false)
                    {
                        didDoubleClick = true;
                        doubleClickTimer = 0.3f;
                    }
                }
                selectedUnit = null;
                enemyStatsScript = null;
            }
        }
    }
    void BasicAttack()
    {
        enemyStatsScript.RecieveDamage(10);
    }
    void RangeSpell()
    {
        //Cast ranged spell
        Vector3 spawnSpellLoc = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        GameObject clone;
        clone = Instantiate(rangeSpellPrefab, spawnSpellLoc, Quaternion.identity);
        selectedUnit.GetComponent<RangeSpell>().target = selectedUnit;
    }
    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Platform")
        {
            transform.parent = GameObject.Find("PlatformNode").transform;
        }
    }
    private void OnCollisionExit()
    {
        transform.parent = null;
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "PoisonArea")
        {
            tickTime -= Time.deltaTime;
            if(tickTime <= 0)
            {
                this.GetComponent<PlayerMovement>().runSpeed = 5;
                currentHP -= 1;
                tickTime = 2;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "PoisonArea")
        {
            this.GetComponent<PlayerMovement>().runSpeed = 15;
        }
    }
}
