using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] heroPrefabs;
    public GameObject[] HeroPrefabs { get { return heroPrefabs; } }

    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (Settings.isNewGame)
        {
            Settings.isNewGame = false;
            GeneratePlayerHero();
            AudioManager.instance.PlayBGM(1);
        }
        
        if (Settings.isWarping)
        {
            Settings.isWarping = false;
            WrapPlayers();
        }
    }

    private void GeneratePlayerHero()
    {
        int i = Settings.playerPrefabId;

        GameObject heroObj = Instantiate(heroPrefabs[i], new Vector3(46f, 10f, 38f), Quaternion.identity);
        heroObj.tag = "Player";

        Character hero = heroObj.GetComponent<Character>();
        PartyManager.instance.Members.Add(hero);

        hero.CharInit(UIManager.instance, InventoryManager.instance, PartyManager.instance);

        InventoryManager.instance.AddItem(hero, 0);

        switch (i)
        {
            case 0:
            case 1:
                InventoryManager.instance.AddItem(hero, 1);
                InventoryManager.instance.AddItem(hero, 2);
                break;

            case 2:
                InventoryManager.instance.AddItem(hero, 16);
                break;

            case 3:
                InventoryManager.instance.AddItem(hero, 15);
                break;

            case 4:
                InventoryManager.instance.AddItem(hero, 15);
                break;

            case 5:
                InventoryManager.instance.AddItem(hero, 3);
                InventoryManager.instance.AddItem(hero, 10);
                break;

            case 6:
                InventoryManager.instance.AddItem(hero, 3);
                InventoryManager.instance.AddItem(hero, 7);
                break;
        }
    }

    private void WrapPlayers()
    {
        PartyManager.instance.LoadAllHeroData();
    }
}
