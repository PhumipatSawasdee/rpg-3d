using System.Collections.Generic;
using UnityEngine;

public class Npc : Character
{
    [SerializeField] private bool isShopKeeper;
    public bool IsShopKeeper {  get { return isShopKeeper; } }

    [SerializeField] private List<Item> shopItems = new();
    public List<Item> ShopItems { get { return shopItems; } set { shopItems = value; } }

    [SerializeField] private int npcMoney = 3000;
    public int NpcMoney { get { return npcMoney; } set { npcMoney = value; } }

    [SerializeField] private List<Quest> questToGive = new List<Quest>();
    public List<Quest> QuestToGive { get { return questToGive; } set { questToGive = value; } }

    public Quest CheckQuestList(QuestStatus status)
    {
        foreach (Quest quest in questToGive)
        {
            if (quest.Status == status)
                return quest;
        }

        return null;
    }
}
