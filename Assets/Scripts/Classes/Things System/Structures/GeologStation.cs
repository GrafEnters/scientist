using System.Collections.Generic;
using UnityEngine;

public class GeologStation : Building {
    [Header("GeologStation")]
    public ShelfCell[] shelfSells;

    IDictionary<Matter, string> mattersLibrary;
    Matter curMatter;
    ShelfCell curshelfcell;

    /**********/

    private void Start() {
        if (mattersLibrary == null)
            mattersLibrary = new Dictionary<Matter, string>();
    }

    /**********/

    public void NameNewMatter(string name) {
        mattersLibrary.Add(curMatter, name);
        curshelfcell.SetName(name);
        TalkManager.Say(name + " такоe необычное название...");
    }

    public bool IsMaxFilled() {
        if (mattersLibrary == null)
            return false;
        return shelfSells.Length == mattersLibrary.Count;
    }

    public int GetMattersAmount() {
        if (mattersLibrary != null)
            return mattersLibrary.Count;
        else
            return 0;
    }

    public List<Matter> GetMattersList() {
        return new List<Matter>(mattersLibrary.Keys);
    }

    public string GetNameByMatter(Matter newMatter) {
        if (mattersLibrary.ContainsKey(newMatter))
            return mattersLibrary[newMatter];
        else
            return "не нашёл";
    }

    public void PutPreviousMatters(IDictionary<Matter, string> _mattersLibrary, IDictionary<GameObject, string> cubes) {
        mattersLibrary = _mattersLibrary;
        int i = 0;
        foreach (GameObject cube in cubes.Keys) {
            i++;
            shelfSells[i].SetName(cubes[cube]);
            shelfSells[i].PutObject(cube);
        }
    }

    public IDictionary<Matter, string> GetLibrary() {
        return mattersLibrary;
    }

    public IDictionary<GameObject, string> GetAll() {
        IDictionary<GameObject, string> cubes = new Dictionary<GameObject, string>();
        foreach (ShelfCell cell in shelfSells) {
            cubes.Add(cell.GetCube(), cell.GetName());
        }

        return cubes;
    }

    public bool AddMatter(Matter newMatter, ShelfCell shelfCell) {
        if (!mattersLibrary.ContainsKey(newMatter)) {
            curMatter = newMatter;
            curshelfcell = shelfCell;
            TalkManager.Ask(NameNewMatter);
            return true;
        } else
            TalkManager.Say("Такой материал уже есть!");

        return false;
    }
}