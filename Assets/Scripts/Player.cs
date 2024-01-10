using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float Hp;
    public float AttackRange = 2;
    public float MoveSpeed = 20;
    public float SimpleAttackDamage = 1;
    public float SimpleAttackSpeed = 1;
    public float HeavyAttackDamage = 3;
    public float HeavyAttackSpeed = 2;

    private float lastAttackTime = 0;
    private bool isDead = false;
    private Rigidbody rb;
    private bool isAttacking = false;
    [SerializeField] private Image HeavyAttackImage;
    public Animator AnimatorController;
    float BeginHeavyCooldown = 0;
    Enemie closestEnemie;

    private void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move();
        if (isDead)
        {
            return;
        }

        if (Hp <= 0)
        {
            Die();
            return;
        }
        closestEnemie = FindClosestEnemy();
        var closestDistance = Vector3.Distance(transform.position, closestEnemie.transform.position);
        if (closestDistance > AttackRange)
        {
            HeavyAttackImage.gameObject.transform.GetChild(0).GetComponent<Text>().text = "Not\navailable";
        }
        else
        {
            HeavyAttackImage.gameObject.transform.GetChild(0).GetComponent<Text>().text = "Heavy\nattack";
        }

        if (HeavyAttackImage.fillAmount == 0)
        {
            BeginHeavyCooldown = Time.time;
            HeavyAttackImage.fillAmount += 0.001f;
        }
        else if (HeavyAttackImage.fillAmount > 0 && HeavyAttackImage.fillAmount <= 1)
        {
            HeavyAttackImage.fillAmount = (Time.time - BeginHeavyCooldown) / HeavyAttackSpeed;
        }
    }

    private void Die()
    {
        isDead = true;
        isAttacking = true;
        Destroy(rb);
        AnimatorController.SetTrigger("Die");

        SceneManager.Instance.GameOver();
    }

    private void Move()
    {
        Vector3 MovementVector = Vector3.zero;
        if (!isAttacking && !isDead)
        {
            if (Input.GetKey(KeyCode.W))
            {
                MovementVector.z += MoveSpeed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                MovementVector.z -= MoveSpeed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                MovementVector.x -= MoveSpeed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                MovementVector.x += MoveSpeed;
            }
        }
        if (MovementVector != Vector3.zero)
        {
            AnimatorController.SetFloat("Speed", MoveSpeed);
            rb.AddForce(MovementVector);
            transform.transform.rotation = Quaternion.LookRotation(MovementVector + transform.position);
        }
        else
        {
            AnimatorController.SetFloat("Speed", 0);
        }
    }

    public void SimpleAttack()
    {
        if (Time.time - lastAttackTime > SimpleAttackSpeed && !isDead)
        {
            isAttacking = true;
            AnimatorController.SetTrigger("Attack");
            lastAttackTime = Time.time;

            if (closestEnemie != null)
            {
                var distance = Vector3.Distance(transform.position, closestEnemie.transform.position);
                if (distance <= AttackRange)
                {
                    transform.transform.rotation = Quaternion.LookRotation(closestEnemie.transform.position - transform.position);
                    closestEnemie.Hp -= SimpleAttackDamage;
                    if (closestEnemie.Hp <= 0)
                    {
                        Hp += 1;
                    }
                }
            }
            StartCoroutine(WaitForAttackToFinish());
        }
    }

    public void HeavyAttack()
    {
        if (Time.time - lastAttackTime > SimpleAttackSpeed && !isDead && HeavyAttackImage.fillAmount == 1)
        {
            var closestDistance = Vector3.Distance(transform.position, closestEnemie.transform.position);
            if (closestDistance > AttackRange)
            {
                return;
            }
            else
            {
                isAttacking = true;
                AnimatorController.SetTrigger("HeavyAttack");

                lastAttackTime = Time.time;


                if (closestEnemie != null)
                {
                    var distance = Vector3.Distance(transform.position, closestEnemie.transform.position);
                    if (distance <= AttackRange)
                    {
                        transform.transform.rotation = Quaternion.LookRotation(closestEnemie.transform.position - transform.position);
                        closestEnemie.Hp -= HeavyAttackDamage;
                        if (closestEnemie.Hp <= 0)
                        {
                            Hp += 1;
                        }
                    }
                }
                StartCoroutine(WaitForAttackToFinish());
                HeavyAttackImage.fillAmount = 0;
            }
        }
    }

    public IEnumerator WaitForAttackToFinish()
    {
        yield return new WaitForSeconds(1);
        isAttacking = false;
    }

    public Enemie FindClosestEnemy()
    {
        Enemie closestEnemie = null;
        float closestDistance = 0;
        var enemies = SceneManager.Instance.Enemies;
        for (int i = 0; i < enemies.Count; i++)
        {
            var enemie = enemies[i];
            if (enemie == null)
            {
                continue;
            }

            if (closestEnemie == null)
            {
                closestEnemie = enemie;
                continue;
            }

            var distance = Vector3.Distance(transform.position, enemie.transform.position);
            closestDistance = Vector3.Distance(transform.position, closestEnemie.transform.position);

            if (distance < closestDistance)
            {
                closestEnemie = enemie;
            }
        }
        return closestEnemie;
    }
}
