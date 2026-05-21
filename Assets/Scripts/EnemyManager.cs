using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<Enemy> monsters;
    public List<Enemy> Monsters { get { return monsters; } }

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

        InventoryManager.instance.AddItem(monsters[0], 0);
        InventoryManager.instance.AddItem(monsters[0], 1);
        InventoryManager.instance.AddItem(monsters[0], 2);
    }
}
