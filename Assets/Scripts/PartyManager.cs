using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private List<Character> members = new List<Character>();
    public List<Character> Members { get { return members; } }

    [SerializeField] private List<Character> selectChars = new List<Character>();
    public List<Character> SelectChars { get { return selectChars; } }

    public static PartyManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach (Character c in members)
        {
            c.CharInit(VFXManager.instance, UIManager.instance);
        }

        SelectSigleHero(0);

        members[0].MagicSkills.Add(new Magic(VFXManager.instance.MagicData[0])); 
        members[0].MagicSkills.Add(new Magic(VFXManager.instance.MagicData[1]));
        members[0].MagicSkills.Add(new Magic(VFXManager.instance.MagicData[2]));

        members[1].MagicSkills.Add(new Magic(VFXManager.instance.MagicData[3]));
        members[1].MagicSkills.Add(new Magic(VFXManager.instance.MagicData[4])); 
        members[1].MagicSkills.Add(new Magic(VFXManager.instance.MagicData[5]));

        InventoryManager.Instance.AddItem(members[0], 0);
        InventoryManager.Instance.AddItem(members[0], 1);
        InventoryManager.Instance.AddItem(members[0], 3);
        InventoryManager.Instance.AddItem(members[0], 6);
        InventoryManager.Instance.AddItem(members[0], 7);
        InventoryManager.Instance.AddItem(members[0], 9);

        InventoryManager.Instance.AddItem(members[1], 0);
        InventoryManager.Instance.AddItem(members[1], 1);
        InventoryManager.Instance.AddItem(members[1], 2);
        InventoryManager.Instance.AddItem(members[1], 4);
        InventoryManager.Instance.AddItem(members[1], 5);
        InventoryManager.Instance.AddItem(members[1], 8);

        UIManager.instance.ShowMagicToggle();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (selectChars.Count > 0)
            {
                selectChars[0].IsMagicMode = true;
                selectChars[0].CurMagicCast = selectChars[0].MagicSkills[0];
            }
        }
    }

    public void SelectSigleHero(int i)
    {
        foreach (Character c in selectChars)
            c.ToggleRingSelection(false);

        selectChars.Clear();

        selectChars.Add(members[i]);
        selectChars[0].ToggleRingSelection(true);
    }

    public void HeroSelectMagicSkill(int i)
    {
        if (selectChars.Count <= 0) 
            return;

        selectChars[0].IsMagicMode = true;
        selectChars[0].CurMagicCast = selectChars[0].MagicSkills[i];
    }
}
