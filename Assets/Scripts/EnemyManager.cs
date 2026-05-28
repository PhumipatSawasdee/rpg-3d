using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<Enemy> monsters;
    public List<Enemy> Monsters { get { return monsters; } }

    [SerializeField] private List<int> dropItemIds;
    public List<int> DropItemIds { get { return dropItemIds; } }

    public static EnemyManager instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach (Character n in monsters)
        {
            n.CharInit(UIManager.instance, InventoryManager.instance, PartyManager.instance);
        }

        RandomAddEnemyInventory();
    }

    private void RandomAddEnemyInventory()
    {
        foreach (Enemy e in monsters)
        {
            for (int i = 0; i < Random.Range(0, 3); i++)
            {
                int randomItem = Random.Range(0, dropItemIds.Count);
                InventoryManager.instance.AddItem(e, randomItem);
            }
        }
    }
}
