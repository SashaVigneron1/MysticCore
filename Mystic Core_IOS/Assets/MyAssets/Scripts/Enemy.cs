using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float maxHealth;
    float currHealth;

    [Header("Damage")]
    [SerializeField] Transform arrowStartPlaceHolder;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] float range;
    [SerializeField] float damage;
    [SerializeField] float secondsPerAttack;
    [SerializeField] float attackingDuration;
    float attackTimer;
    float attackingTimer;

    [Header("Movement")]
    [SerializeField] float movementSpeed;
    [SerializeField] NavMeshAgent agent;


    [Header("Other")]
    [SerializeField] GameObject coinsReceivedCanvas;
    [SerializeField] TextMeshProUGUI coinsReceivedText;
    [SerializeField] Animator animator;
    [SerializeField] public Transform particleOffset;

    MysticTower mysticTower;

    EnemyManager enemyManager;

    enum EnemyState
    {
        Idle,
        Moving,
        Shooting,
        Dead
    }
    EnemyState enemyState = EnemyState.Idle;

    private void Start()
    {
        agent.speed = movementSpeed;
        attackTimer = secondsPerAttack;
    }


    private void Update()
    {
        if (!isStopped)
        {
            switch (enemyState)
            {
                case EnemyState.Idle:
                    //Walk to tower
                    if (agent) agent.SetDestination(mysticTower.transform.position);
                    animator.SetBool("isRunning", true);
                    enemyState = EnemyState.Moving;

                    break;
                case EnemyState.Moving:
                    //General
                    Vector2 thisPosition = new Vector2(this.transform.position.x, this.transform.position.z);
                    Vector2 towerPosition = new Vector2(mysticTower.transform.position.x, mysticTower.transform.position.z);
                    float distanceToTower = Vector2.Distance(thisPosition, towerPosition);

                    // Movement is handled by navmeshagent
                    // If too close: stop running
                    if (distanceToTower <= agent.stoppingDistance)
                    {
                        animator.SetBool("isRunning", false);
                    }

                    // Attack
                    attackTimer += Time.deltaTime;
                    if (attackTimer >= secondsPerAttack)
                    {
                        if (distanceToTower <= range)
                        {
                            attackTimer = 0.0f;

                            enemyState = EnemyState.Shooting;
                            animator.SetBool("isShooting", true);

                            agent.SetDestination(agent.transform.position);
                        }
                    }
                    break;
                case EnemyState.Shooting:
                    attackingTimer += Time.deltaTime;
                    if (attackingTimer >= attackingDuration)
                    {
                        attackingTimer = 0.0f;

                        GameObject arrowObj = Instantiate(arrowPrefab, arrowStartPlaceHolder.position, Quaternion.identity);
                        Arrow arrow = arrowObj.GetComponent<Arrow>();
                        arrow.SetDirection((mysticTower.transform.position + new Vector3(0, 2, 0)) - arrow.transform.position);
                        arrow.SetDamage(damage);

                        animator.SetBool("isShooting", false);
                        enemyState = EnemyState.Moving;

                        agent.SetDestination(mysticTower.transform.position);
                    }

                    break;
                case EnemyState.Dead:
                    agent.SetDestination(agent.transform.position);
                    break;
            }
        }

        

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isStopped)
        {
            if (other.tag == "Projectile" && !animator.GetBool("isDead"))
            {
                Projectile projectile = other.gameObject.GetComponent<Projectile>();
                if (projectile)
                {
                    //Take Damage
                    float health = currHealth;
                    currHealth -= projectile.GetDamage();
                    float receivedDamage = health - currHealth;
                    float lifesteal = receivedDamage * projectile.GetLifesteal();
                    mysticTower.AddHealth(lifesteal);
                    projectile.SpawnExplosion();

                    if (currHealth <= 0)
                    {
                        float value = FindObjectOfType<GameManager>().AddEnemyMoney();
                        coinsReceivedCanvas.SetActive(true);
                        coinsReceivedText.text = ((int)value).ToString();

                        animator.SetBool("isDead", true);
                        enemyManager.DestroyEnemy(this);
                        enemyState = EnemyState.Dead;
                    }

                    //Remove Projectile
                    Destroy(other.gameObject);
                }


            }
        }
        
    }

    #region Mutators
    public void SetEnemyManager(EnemyManager manager)
    {
        enemyManager = manager;
    }
    public void SetTowerTransform(MysticTower tower)
    {
        mysticTower = tower;
    }
    public void SetMaxHealth(float value)
    {
        maxHealth = value;
    }
    public void SetDamage(float value)
    {
        damage = value;
    }
    public void SetSecondsPerAttack(float value)
    {
        secondsPerAttack = value;
    }
    public void SetMovementSpeed(float value)
    {
        movementSpeed = value;
        if (agent) agent.speed = value;
    }
    #endregion

    bool isStopped = false;
    public void Stop(bool value)
    {
        isStopped = value;
        agent.isStopped = value;
    }
}
