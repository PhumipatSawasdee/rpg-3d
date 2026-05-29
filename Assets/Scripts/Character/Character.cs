using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum CharState
{
    Idle,
    Walk,
    WalkToEnemy,
    Attack,
    WalkToMagicCast,
    MagicCast,
    Hit,
    Die,
    WalkToNPC
}

public abstract class Character : MonoBehaviour
{
    protected NavMeshAgent navAgent;

    protected Animator anim;
    public Animator Anim { get { return anim; } }

    [SerializeField] protected Sprite avatarPic;
    public Sprite AvatarPic { get { return avatarPic; } }

    [SerializeField] protected string charName;
    public string CharName { get { return charName; } }

    [SerializeField] protected CharState state;
    public CharState State { get { return state; } }

    [SerializeField] protected GameObject ringSelection;
    public GameObject RingSelection { get { return ringSelection; } }

    [SerializeField] protected int curHP = 10;
    public int CurHP { get { return curHP; } set { curHP = value; } }

    [SerializeField] protected int maxHp = 100;
    public int MaxHP { get { return maxHp; } }

    [SerializeField] protected Character curCharTarget;
    public Character CurCharTarget { get { return curCharTarget; }
        set { curCharTarget = value; } }

    [SerializeField] protected float attackRange = 2f;
    public float AttackRange { get { return attackRange; } }


    [SerializeField] protected int attackDamage = 3;
    public int AttackDamage { get { return attackDamage; } set { attackDamage = value; } }

    [SerializeField] protected float attackCoolDown = 2f;
    [SerializeField] protected float attackTimer = 0f;

    [SerializeField] protected float findingRange = 20f;
    public float FindingRange { get { return findingRange; } }

    [SerializeField] protected List<Magic> magicSkills = new List<Magic>();
    public List<Magic> MagicSkills 
    { get { return magicSkills; } set { magicSkills = value; } }

    [SerializeField] protected Magic curMagicCast = null;
    public Magic CurMagicCast
    { get { return curMagicCast; } set { curMagicCast = value; } }

    [SerializeField] protected bool isMagicMode = false;
    public bool IsMagicMode
    { get { return isMagicMode; } set { isMagicMode = value; } }

    [SerializeField] protected Transform shootPoint; 
    public Transform ShootPoint
    { get { return shootPoint; } set { shootPoint = value; } }

    [Header("Inventory")]
    [SerializeField] protected Item[] inventoryItems;
    public Item[] InventoryItems { get { return inventoryItems; } set { inventoryItems = value; } }

    [SerializeField] protected Item mainWeapon;
    public Item MainWeapon { get { return mainWeapon; } set { mainWeapon = value; } }

    [SerializeField] protected Transform mainWeaponHand;
    [SerializeField] protected GameObject mainWeaponObj;

    [SerializeField] protected int attackPower = 0;

    [SerializeField] protected Item shield;
    public Item Shield { get { return shield; } set { shield = value; } }

    [SerializeField] protected Transform shieldHand;
    [SerializeField] protected GameObject shieldObj;

    [SerializeField] protected int defensePower = 0;
    public int DefensePower { get { return defensePower; } set { defensePower = value; } }

    protected UIManager uiManager;
    protected InventoryManager invManager;
    protected PartyManager partyManager;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    public void CharInit(UIManager uiM, InventoryManager invM, PartyManager partyM)
    {
        uiManager = uiM;
        invManager = invM;
        partyManager = partyM;

        inventoryItems = new Item[InventoryManager.MAXSLOT];
    }

    public void SetState(CharState s)
    {
        state = s;

        if (state == CharState.Idle)
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
        }
    }

    public void WalkToPosition(Vector3 dest)
    {
        if (navAgent != null)
        {
            navAgent.SetDestination(dest);
            navAgent.isStopped = false;
        }

        SetState(CharState.Walk);
    }

    protected void WalkUpdate()
    {
        float distance = Vector3.Distance(transform.position, navAgent.destination);
        //Debug.Log(distance);

        if (distance <= navAgent.stoppingDistance) 
            SetState(CharState.Idle);
    }

    protected void WalkToNPCUpdate()
    {
        float distance = Vector3.Distance(transform.position, curCharTarget.transform.position);

        if (distance <= 2f)
        {
            navAgent.isStopped = true;
            SetState(CharState.Idle);

            Npc npc = curCharTarget.GetComponent<Npc>();

            if (npc != null)
            {
                if (npc.IsShopKeeper)
                    uiManager.PrepareShopPanel(npc, this.GetComponent<Hero>());
                else
                {
                    foreach (Quest quest in npc.QuestToGive)
                    {
                        if (npc.CheckQuestList(QuestStatus.Reject) != null || npc.CheckQuestList(QuestStatus.New) != null || npc.CheckQuestList(QuestStatus.InProgress) != null)
                        {
                            Debug.Log("Has quest to give");
                            uiManager.PrepareDialogueBox(npc);
                        }
                        else
                            Debug.Log("Not has quest to give");
                    }
                }
            }
            else
            {
                bool isInParty = false;
                foreach (Character character in partyManager.Members)
                {
                    if (character.charName == curCharTarget.GetComponent<Character>().charName)
                    {
                        isInParty = true;
                        Debug.Log($"{character.charName} is already in party");
                    }
                }

                if (!isInParty)
                {
                    Hero hero = curCharTarget.GetComponent<Hero>();
                    uiManager.PrepareHeroJoinParty(hero);
                }
            }
        }
    }

    public void ToggleRingSelection(bool flag) => ringSelection.SetActive(flag);

    public void ToAttackCharacter(Character target)
    {
        if (curHP <= 0 || state == CharState.Die) return;

        curCharTarget = target;
        navAgent.SetDestination(target.transform.position);
        navAgent.isStopped = false;

        if (isMagicMode)
            SetState(CharState.WalkToMagicCast);
        else
            SetState(CharState.WalkToEnemy);
    }

    public void WalkToEnemyUpdate()
    {
        if (curCharTarget == null)
        {
            SetState(CharState.Idle);
            return;
        }

        navAgent.SetDestination(curCharTarget.transform.position);
        float distance = Vector3.Distance(transform.position, curCharTarget.transform.position);

        if (distance <= attackRange)
        {
            SetState(CharState.Attack);
            Attack();
        }
    }

    protected void WalkToMagicCastUpdate()
    {
        if (curCharTarget == null || curMagicCast == null)
        {
            SetState(CharState.Idle);
            return;
        }

        navAgent.SetDestination(curCharTarget.transform.position);

        float distance = Vector3.Distance(transform.position, 
            curCharTarget.transform.position);

        if (distance <= curMagicCast.Range)
        {
            navAgent.isStopped = true;
            SetState(CharState.MagicCast);

            MagicCast(curMagicCast);
        }
    }

    public void ToTalkToNPC(Character npc)
    {
        if (curHP <= 0 || state == CharState.Die)
            return;

        curCharTarget = npc;

        navAgent.SetDestination(npc.transform.position);
        navAgent.isStopped = false;

        SetState(CharState.WalkToNPC);
    }

    public bool IsMyEnemy(string targetTag)
    {
        string myTag = gameObject.tag;

        if ((myTag == "Hero" || myTag == "Player") && targetTag == "Enemy")
        {
            //Debug.Log($"{myTag} : Found enemy : {targetTag}");
            return true;
        }

        if (myTag == "Enemy" && (targetTag == "Hero" || targetTag == "Player"))
        {
            //Debug.Log($"{myTag} : Found player : {targetTag}");
            return true;
        }

        //Debug.Log($"{myTag} : Not found enemy : {targetTag}");
        return false;
    }

    protected void Attack()
    {
        transform.LookAt(curCharTarget.transform);
        anim.SetTrigger("Attack");

        float n = Random.Range(0, 4);
        anim.SetFloat("AttackValue", n);

        AttackLogic();
    }

    protected void AttackUpdate()
    {
        if (curCharTarget == null) return;

        if (curCharTarget.CurHP <= 0)
        {
            SetState(CharState.Idle);
            curCharTarget = null;
            return;
        }
        navAgent.isStopped = true;

        attackTimer += Time.deltaTime;
        if (attackTimer > attackCoolDown)
        {
            attackTimer = 0f;
            Attack();
        }

        float distance = Vector3.Distance(transform.position, curCharTarget.transform.position);

        if (distance > attackRange)
        {
            SetState(CharState.WalkToEnemy);
            navAgent.SetDestination(curCharTarget.transform.position);
            navAgent.isStopped = false;
        }
    }

    protected void AttackLogic()
    {
        Character target = curCharTarget.GetComponent<Character>();

        if (target != null)
        {
            int damageAfter = attackDamage + attackPower;
            target.ReviceDamage(damageAfter);
        } 
    }

    protected void MagicCastLogic(Magic magic)
    {
        Character target = curCharTarget.GetComponent<Character>();

        if (target != null)
            target.ReviceDamage(magic.Power);
    }

    private IEnumerator ShootMagicCast(Magic curMagicCast)
    {
        if (MyActions.onShootMagic != null)
            MyActions.onShootMagic(curMagicCast.ShootID,
                ShootPoint.position,
                curCharTarget.ShootPoint.position,
                curMagicCast.ShootTime);

        yield return new WaitForSeconds(curMagicCast.ShootTime);

        MagicCastLogic(curMagicCast);
        isMagicMode = false;

        SetState(CharState.Idle);

        if (uiManager != null)
            uiManager.IsOnCurToggleMagic(false);
    }

    private void MagicCast(Magic curMagicCast)
    {
        transform.LookAt(curCharTarget.transform);
        anim.SetTrigger("MagicAttack");

        StartCoroutine(LoadMagicCast(curMagicCast));
    }

    private IEnumerator LoadMagicCast(Magic curMagicCast)
    {
        if (MyActions.onLoadMagic != null)
            MyActions.onLoadMagic(curMagicCast.LoadID,
                ShootPoint.position,
                curMagicCast.LoadTime);

        yield return new WaitForSeconds(curMagicCast.LoadTime);

        StartCoroutine(ShootMagicCast(curMagicCast));
    }

    public void ReviceDamage(int damage)
    {
        if (curHP <= 0 || state == CharState.Die) 
            return;

        int damageAfter = damage - defensePower;

        if (damageAfter < 0)
            damageAfter = 0;

        curHP -= damageAfter;

        if (curHP <= 0)
        {
            curHP = 0;
            Die();
        }
    }

    protected virtual IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    public void Recover(int n)
    {
        curHP += n;

        if (curHP > maxHp)
            curHP = maxHp;
    }

    public void EquipWeapon(Item item)
    {
        mainWeaponObj = Instantiate(invManager.ItemPrefabs[item.PrefabID], mainWeaponHand);

        mainWeaponObj.transform.localPosition = new Vector3(8.5f, 4f, 3f);
        mainWeaponObj.transform.Rotate(90f, 0f, 180f, Space.Self);

        attackPower += item.Power;
        mainWeapon = item;
    }

    public void UnEquipWeapon()
    {
        if (mainWeapon != null)
        {
            attackPower -= shield.Power;
            mainWeapon = null;
            Destroy(mainWeaponObj);
        }
    }

    public void EquipShield(Item item)
    {
        shieldObj = Instantiate(invManager.ItemPrefabs[item.PrefabID], shieldHand);

        shieldObj.transform.localPosition = new Vector3(-8.5f, -4f, 3f);
        shieldObj.transform.Rotate(-90f, 0f, 180f, Space.Self);

        defensePower += item.Power;
        shield = item;
    }

    public void UnEquipShield()
    {
        if (shield != null)
        {
            defensePower -= shield.Power;
            shield = null;
            Destroy(shieldObj);
        }
    }

    protected virtual void Die()
    {
        navAgent.isStopped = true;
        SetState(CharState.Die);

        anim.SetTrigger("Die");
        invManager.SpawnDropInventory(inventoryItems, transform.position);
        StartCoroutine(DestroyObject());
    }
}
