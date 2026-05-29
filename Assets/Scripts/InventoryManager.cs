using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject[] itemPrefabs;
    public GameObject[] ItemPrefabs { get { return itemPrefabs; } set { itemPrefabs = value; } }

    [SerializeField] private ItemData[] itemData;
    public ItemData[] ItemData { get { return itemData; } set { itemData = value; } }

    public const int MAXSLOT = 18;

    public static InventoryManager instance;
    private void Awake()
    {
        instance = this;    
    }

    private void Start()
    {
        if (QuestManager.instance.NPCPerson.Length <= 0)
            return;

        AddItemShopToNPC(1, 0);
        AddItemShopToNPC(1, 3);
        AddItemShopToNPC(1, 4);
        AddItemShopToNPC(1, 5);
        AddItemShopToNPC(1, 6);
        AddItemShopToNPC(1, 10);

        AddItemShopToNPC(2, 1);
        AddItemShopToNPC(2, 2);
        AddItemShopToNPC(2, 7);
        AddItemShopToNPC(2, 8);
        AddItemShopToNPC(2, 9);
        AddItemShopToNPC(2, 14);
        AddItemShopToNPC(2, 15);
        AddItemShopToNPC(2, 16);
        AddItemShopToNPC(2, 17);
        AddItemShopToNPC(2, 18);
        AddItemShopToNPC(2, 19);
        AddItemShopToNPC(2, 20);
        AddItemShopToNPC(2, 21);
        AddItemShopToNPC(2, 22);
        AddItemShopToNPC(2, 23);
        AddItemShopToNPC(2, 24);
    }

    public bool AddItem(Character character, int id)
    {
        Item item = new Item(ItemData[id]);

        for (int i = 0; i < character.InventoryItems.Length; i++)
        {
            if (character.InventoryItems[i] == null)
            {
                character.InventoryItems[i] = item;
                return true;
            }
        }

        Debug.Log("Inventory Full");
        return false;
    }

    public void SaveItemInBag(int index, Item item)
    {
        if (PartyManager.instance.SelectChars.Count == 0)
            return;

        PartyManager.instance.SelectChars[0].InventoryItems[index] = item;

        switch (index)
        {
            case 16:
                PartyManager.instance.SelectChars[0].EquipShield(item);
                break;

            case 17:
                PartyManager.instance.SelectChars[0].EquipWeapon(item);
                break;
        }
    }

    public void RemoveItemInBag(int index)
    {
        if (PartyManager.instance.SelectChars.Count == 0)
            return;

        PartyManager.instance.SelectChars[0].InventoryItems[index] = null;

        switch (index)
        {
            case 16:
                PartyManager.instance.SelectChars[0].UnEquipShield();
                break;

            case 17:
                PartyManager.instance.SelectChars[0].UnEquipWeapon();
                break;
        }
    }

    private void SpawnDropItem(Item item, Vector3 pos)
    {
        int id;

        switch(item.Type)
        {
            case ItemType.Consumable:
                id = 1;
                break;

            default:
                id = 0;
                break;
        }

        GameObject itemObj = Instantiate(ItemPrefabs[id], pos, Quaternion.identity);
        itemObj.AddComponent<ItemPick>();

        ItemPick itemPick = itemObj.GetComponent<ItemPick>();
        itemPick.Init(item, instance, PartyManager.instance);
    }

    public void SpawnDropInventory(Item[] items, Vector3 pos)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                Vector3 ranPos = new Vector3(Random.RandomRange(pos.x - 1.5f, pos.x + 1.5f), pos.y, Random.RandomRange(pos.z - 1.5f, pos.z + 1.5f));
                SpawnDropItem(items[i], ranPos);
            }
        }
    }

    public void DrinkConsumableItem(Item item, int slotId)
    {
        string s = string.Format("Drink: {0}", item.ItemName);
        Debug.Log(s);

        if (PartyManager.instance.SelectChars.Count > 0)
        {
            PartyManager.instance.SelectChars[0].Recover(item.Power);
            RemoveItemInBag(slotId);
        }
    }

    public bool CheckPartyForItem(int id)
    {
        Item item = new Item(itemData[id]);
        Debug.Log(item.ItemName);

        List<Character> party = PartyManager.instance.Members;

        foreach (Character hero in party)
        {
            for (int i = 0; i < hero.InventoryItems.Length; i++)
            {
                if (hero.InventoryItems[i] == null)
                    continue;

                Debug.Log(hero.InventoryItems[i].ItemName);

                if (hero.InventoryItems[i].ID == item.ID)
                    return true;
            }
        }
        return false;
    }

    public bool RemoveItemFromParty(int id)
    {
        Item item = new Item(itemData[id]);
        Debug.Log($"Finding {item.ItemName}");

        List<Character> party = PartyManager.instance.Members;

        foreach (Character hero in party)
        {
            for (int i = 0;i < hero.InventoryItems.Length; i++)
            {
                if (hero.InventoryItems[i] == null)
                    continue;

                if (hero.InventoryItems[i].ID == item.ID)
                {
                    Debug.Log($"Removing {hero.InventoryItems[i].ItemName}");
                    hero.InventoryItems[i] = null;
                    Debug.Log($"Removed {hero.InventoryItems[i]}");
                    return true;
                }
            }
        }
        return false;
    }

    private void AddItemShopToNPC(int npcId, int itemId)
    {
        Item item = new Item(itemData[itemId]);
        QuestManager.instance.NPCPerson[npcId].ShopItems.Add(item);
    }
}
