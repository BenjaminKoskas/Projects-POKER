using System;
using System.Collections.Generic;
using UnityEngine;

public class ChipsManager : MonoBehaviour
{
    public Dictionary<string, Chip> chips = new Dictionary<string, Chip>();

    public Chip dealer;
    public Chip BB;
    public Chip SB;

    public static ChipsManager Instance;

    public GameObject chipPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        Chip[] sc_chips = Utils.GetAllInstances<Chip>("Chips");
        foreach (Chip chip in sc_chips)
        {
            if(chip.isDealer)
                dealer = chip;
            else if (chip.isBB)
                BB = chip;
            else if (chip.isSB)
                SB = chip;
            else
                chips.Add(chip.value.ToString(), chip);
        }
    }

    public List<Chip> FindChipsByValue(int n)
    {
        List<Chip> chipToInstantiate = new List<Chip>();
        Dictionary<int, int> v = new Dictionary<int, int>();
        for (int i = 0; i < n + 1; i++)
        {
            if (i == 0)
            {
                v.Add(i, i);
            }
            else
            {
                int a = n + 1;
                foreach (Chip c in chips.Values)
                {
                    if (c.value <= i)
                    {
                        if (a > v[i - c.value] + 1)
                            a = v[i - c.value] + 1;
                    }
                }
                v.Add(i, a);
            }
        }

        List<Chip> chipsToUse = new List<Chip>();
        foreach (Chip chip in chips.Values)
        {
            if (chip.value <= n)
                chipsToUse.Add(chip);
        }

        int x = n;
        for (int d = chipsToUse.Count - 1; d >= 0; d--)
        {
            for (int i = 0; i < v.Count; i++)
            {
                if (chipsToUse[d].value * i > x)
                {
                    x -= chipsToUse[d].value * (i - 1);
                    for (int j = 0; j < i - 1; j++)
                    {
                        chipToInstantiate.Add(chipsToUse[d]);
                    }
                    break;
                }
            }
        }

        return chipToInstantiate;
    }
}
