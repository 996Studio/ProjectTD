using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour
{
    [SerializeField]
    protected Animator animator;
    
    [SerializeField]
    protected NavMeshAgent agent;

    [SerializeField]
    protected float maxHP;
    
    [SerializeField]
    protected float curHP;
    
    [SerializeField]
    protected int atk;

    [SerializeField]
    protected float moveSpeed;

    [SerializeField] 
    private Image HPBarForeground;

    [SerializeField] 
    private GameObject motherbase;
    
    private float updateSpeedSec = 0.3f;
    
    protected bool isDead = false;
    protected bool canTakeDamage;
    private float recycleMultiplier;

    public float RecycleMultiplier
    {
        get => recycleMultiplier;
        set => recycleMultiplier = value;
    }
    
    public event Action<float> OnHealthChanged = delegate(float f) {  };
    
    //Freeze Parameter
    private float freezeCounter;
    
    public int Atk
    {
        get => atk;
    }
    
    public bool IsDead
    {
        get => isDead;
    }
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        canTakeDamage = true;
        //delegate for changing health
        OnHealthChanged += HandleHealthChange;
        HPBarForeground.fillAmount = 0.5f;
    }
    
    void Start()
    {
        agent.SetDestination(MotherBase.Instance.transform.position);
    }
    
    protected virtual void Update()
    {
        FreezeTimer();
    }

    protected void LateUpdate()
    {
        
    }

    public virtual void InitInfo()
    {
        atk = 1;
    }

    public void TakeDamage(int dmg)
    {
        if (canTakeDamage)
        {
            curHP -= dmg;
            //Debug.Log($"{this.gameObject} {curHP}");

            ChangeHealth();
            
            if (curHP <= 0)
            {
                Death();
            }
        }
    }

    public void ChangeHealth()
    {
        float curHealthPct = curHP / maxHP;
        OnHealthChanged(curHealthPct);
    }

    public virtual void Death()
    {
        isDead = true;
        agent.isStopped = true;
        MotherBase.Instance.KillCount++;
        AudioManager.instance.Play(SoundType.SFX, "EnemyDeath");
        
        Destroy(this.gameObject);
    }
    
    private void HandleHealthChange(float pct)
    {
        StartCoroutine(ChangeHPBarPct(pct));
    }

    private IEnumerator ChangeHPBarPct(float pct)
    {
        float preChangePct = HPBarForeground.fillAmount;
        float timeElapsed = 0.0f;
        canTakeDamage = false;

        while (timeElapsed < updateSpeedSec)
        {
            timeElapsed += Time.deltaTime;
            HPBarForeground.fillAmount = Mathf.Lerp(preChangePct, pct, timeElapsed / updateSpeedSec);
            
            yield return null;
        }

        canTakeDamage = true;
        HPBarForeground.fillAmount = pct;
    }

    private void FreezeTimer()
    {
        if (freezeCounter > 0.0f)
        {
            freezeCounter -= Time.deltaTime;
            if (freezeCounter <= 0)
            {
                freezeCounter = 0.0f;
                StopFreeze();
            }
        }
    }

    public void StartFreeze(float freezeFactor, float duration)
    {
        agent.speed *= freezeFactor;
        freezeCounter = duration;
    }

    public void StopFreeze()
    {
        agent.speed = moveSpeed;
    }

    public List<EnemyBase> GetNearestEnemy(int number, float range)
    {
        List<EnemyBase> targetList = new List<EnemyBase>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);
        List<EnemyBase> tempEnemyList = new List<EnemyBase>();
        foreach (var collider in colliders)
        {
            EnemyBase tempEnemy = collider.GetComponent<EnemyBase>();
            if (tempEnemy != this && tempEnemy != null)
            {
                Debug.Log(tempEnemy);
                tempEnemyList.Add(tempEnemy);
            }
        }
        
        //Debug.Log($"unordered {tempEnemyList.Count}");
        if (tempEnemyList.Count == 0)
        {
            return targetList;
        }
        
        var orderedEnemies = tempEnemyList.OrderBy(e => Vector3.Magnitude(e.transform.position - transform.position))
            .ToList();
        //Debug.Log($"ordered {orderedEnemies.Count()}");

        foreach (var nearbyEnemy in orderedEnemies)
        {
            targetList.Add(nearbyEnemy);
            if (targetList.Count >= number)
            {
                return targetList;
            }
        }
        
        return targetList;
    }
}
