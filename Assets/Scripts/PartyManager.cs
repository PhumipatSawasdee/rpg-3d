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

        members[0].MagicSkills.Add(new Magic(0, "Power Glow", 10f, 20, 3f, 3f, 2, 1)); 
        members[0].MagicSkills.Add(new Magic(1, "Bubble", 15f, 20, 2f, 2f, 1, 3));
        members[0].MagicSkills.Add(new Magic(2, "Firework", 8f, 25, 3f, 3f, 0, 6));

        members[1].MagicSkills.Add(new Magic(0, "Fire Ball", 10f, 35, 3f, 2f, 1, 0));
        members[1].MagicSkills.Add(new Magic(1, "Explosion Body", 4f, 45, 4f, 3f, 2, 5)); 
        members[1].MagicSkills.Add(new Magic(2, "Electricity", 12f, 25, 2f, 2f, 2, 4));

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
