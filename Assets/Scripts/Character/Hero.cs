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

        Magic magic;

        if (nextExp > exp)
        {
            level++;
            nextExp = level * 30;
            UpdateStat();

            switch (level)
            {
                case 5:
                    if (MyActions.onCreateMagic != null)
                    {
                        magic = MyActions.onCreateMagic(0);
                        magicSkills.Add(magic);
                        uiManager.ShowMagicToggles();
                    }
                    break;

                case 10:
                    if (MyActions.onCreateMagic != null)
                    {
                        magic = MyActions.onCreateMagic(1);
                        magicSkills.Add(magic);
                        uiManager.ShowMagicToggles();
                    }
                    break;
            }
        }
    }
}
