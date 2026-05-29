using UnityEngine;
using UnityEngine.Rendering;

public class Hero : Character
{
    [SerializeField] private int prefabId;
    public int PrefabId { get { return prefabId; } }

    [SerializeField] private int exp;
    public int Exp { get { return exp; } set { exp = value; } }

    [SerializeField] private int level;
    public int Level { get { return level; } set { level = value; } }

    [SerializeField] private int nextExp;
    public int NextExp { get { return nextExp; } set { nextExp = value; } }

    [SerializeField] private int strength;
    public int Strength { get { return strength; } set { strength = value; } }

    [SerializeField] private int dexterity;
    public int Dexterity { get { return dexterity; } set { dexterity = value; } }

    [SerializeField] private int constitution;
    public int Constitution { get { return constitution; } set { constitution = value; } }

    [SerializeField] private int intelligence;
    public int Intelligence { get { return intelligence; } set { intelligence = value; } }

    [SerializeField] private int wisdom;
    public int Wisdom { get { return wisdom; } set { wisdom = value; } }

    [SerializeField] private int charisma;
    public int Charisma { get { return charisma; } set { charisma = value; } }

    void Update()
    {
        switch (state)
        {
            case CharState.Walk:
                WalkUpdate(); 
                break;

            case CharState.WalkToEnemy:
                WalkToEnemyUpdate();
                break;

            case CharState.Attack:
                AttackUpdate();
                break;

            case CharState.WalkToMagicCast:
                WalkToMagicCastUpdate();
                break;

            case CharState.WalkToNPC:
                WalkToNPCUpdate();
                break;
        }
    }

    public void SaveItemInInventory(Item item)
    {
        for (int i = 0; i < InventoryManager.MAXSLOT; i++)
        {
            if (inventoryItems[i] == null)
            {
                InventoryItems[i] = item;
                return;
            }
        }
    }

    public void ReceiveExp(int n)
    {
        exp += n;
        CheckLevel(exp);
    }

    private void UpdateStat()
    {
        attackDamage++;
        defensePower++;
        maxHp++;

        if (strength >= Random.Range(1, 20))
            attackDamage++;

        if (dexterity >= Random.Range(1, 20))
            defensePower++;

        if (constitution >= Random.Range(1, 20))
            maxHp++;
    }

    public void CheckLevel(int exp)
    {
        nextExp = level * 30;

        if (exp >= nextExp)
        {
            level++;
            nextExp = level * 30;
            UpdateStat();

            switch (level)
            {
                case 2:
                    if (MyActions.onCreateMagic != null)
                    {
                        if (prefabId == 0 || prefabId == 1)
                            ActivateNewSkill(0);
                    }
                    break;

                case 3:
                    if (MyActions.onCreateMagic != null)
                    {
                        if (prefabId == 0)
                            ActivateNewSkill(1);

                        else if (prefabId == 1)
                            ActivateNewSkill(2);
                    }
                    break;

                case 5:
                    if (MyActions.onCreateMagic != null)
                    {
                        if (prefabId == 0 || prefabId == 5 || prefabId == 6)
                            ActivateNewSkill(5);

                        else if (prefabId == 1 || prefabId == 2)
                            ActivateNewSkill(9);

                        else if (prefabId == 3 || prefabId == 4)
                            ActivateNewSkill(8);
                    }
                    break;

                case 10:
                    if (MyActions.onCreateMagic != null)
                    {
                        if (prefabId == 0)
                            ActivateNewSkill(3);

                        else if (prefabId == 1)
                            ActivateNewSkill(5);

                        else if (prefabId == 2)
                            ActivateNewSkill(4);

                        else if (prefabId == 3)
                            ActivateNewSkill(5);

                        else if (prefabId == 4)
                            ActivateNewSkill(6);

                        else if (prefabId == 5 || prefabId == 6)
                            ActivateNewSkill(7);
                    }
                    break;
            }
        }
    }

    public void ActivateNewSkill(int magicId)
    {
        Magic magic;
        magic = MyActions.onCreateMagic(magicId);
        magicSkills.Add(magic);
        UIManager.instance.ShowMagicToggles();
    }
}
