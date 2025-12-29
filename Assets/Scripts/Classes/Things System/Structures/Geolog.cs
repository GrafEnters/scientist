using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geolog : Villager {
    [Header("Geolog")]
    public int berryNeeded = 5;

    public Color hatColor;
    public GameObject hat;

    [HideInInspector]
    public GameObject[] geologStation;

    [HideInInspector]
    public GameObject pickaxe, tigel, fuelCube, blacksmith;

    GeologStation GS;
    int stationlevel;
    int firstMet;

    /**********/

    private void Start() {
        stationlevel = 0;
        firstMet = 0;
        GS = geologStation[0].GetComponent<GeologStation>();
        StartCoroutine(CallingPlayer());
        hat.GetComponent<MeshRenderer>().material.SetColor("baseColor", hatColor);
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    private void OnEnable() {
        CallingPlayer();
    }

    /**********/

    protected virtual void CheckLetterAnswer(string answer) {
        if (answer.Length > 0) {
            if (curLetter.GetComponent<Letter>().GetTask().answer == answer)
                VillagerAnswer(true, 2, "Отлично спасибо. Ты очень помог деревне.");
            else {
                VillagerAnswer(false, -1, "Ты сделал всё только хуже");
                signPost.GetComponent<SignPost>().EnqueTask(GenerateTask());
            }

            Destroy(curLetter);
        }
    }

    protected virtual void CheckLetterAnswer(Task task) {
        TalkManager.Ask(CheckLetterAnswer);
    }

    void BringNewMatterCheck(Task task) {
        if (task.hideInt < GS.GetMattersAmount()) {
            VillagerAnswer(true, 2, "Отлично! Чем больше материалов - тем больше у нашей коммуны возможностей.");
            Destroy(curLetter);
        } else
            VillagerAnswer(false, 0, "Пока ты не принесёшь новый материал, я не зачту просьбу выполненной.");
    }

    void BringBerriesCheck() {
        Collider[] colls = Physics.OverlapSphere(transform.position, pickUpRadius, ~IgnoreMask);
        List<GameObject> berries = new List<GameObject>();

        int have = 0;
        foreach (Collider coll in colls) {
            if (coll.gameObject.GetComponent<Thing>() && coll.gameObject.GetComponent<Thing>().thingName == "berry") {
                berries.Add(coll.gameObject);
                have++;
            }

            if (have == berryNeeded)
                break;
        }

        if (have == berryNeeded) {
            foreach (GameObject berry in berries) {
                Destroy(berry);
            }

            pickaxe.SetActive(true);
            TalkManager.Say(
                "Великолепно! Вот тебе моя кирка, с ней ты сможешь добыть любой минерал. Иди поищи чего интересного, а я пока тут всё подготовлю.");
            SpawnStation();
            signPost.GetComponent<SignPost>().EnqueTask(GenerateTask());
            firstMet++;
        } else if (have == 0)
            TalkManager.Say("Оставь их в ящике или положи прямо на землю, я не брезгую.");
        else
            TalkManager.Say("Отлично, осталось ещё " + (berryNeeded - have) + " ягоды");
    }

    void BringPickaxeCheck(Task task) {
        Collider[] colls = Physics.OverlapSphere(transform.position, pickUpRadius, ~IgnoreMask);
        GameObject newPickaxe = null;

        foreach (Collider coll in colls) {
            if (coll.gameObject.GetComponent<Pickaxe>()) {
                newPickaxe = coll.gameObject;
                break;
            }
        }

        if (newPickaxe) {
            Destroy(curLetter);
            Destroy(newPickaxe);
            ChangeStation(stationlevel + 1);
            VillagerAnswer(true, 5, "Отлично! C киркой как-то поспокойнее.");
        } else
            VillagerAnswer(false, 0, "Сначала кирка, потом разговоры.");
    }

    protected override void VillagerAnswer(bool isCorrect, int ChangeRep, string phraze) {
        base.VillagerAnswer(isCorrect, ChangeRep, phraze);

        if (isCorrect)
            signPost.GetComponent<SignPost>().EnqueTask(GenerateTask());
    }

    protected override Task GenerateTask() {
        int rnd = Random.Range(0, 2);
        if (GS.GetMattersAmount() < 2)
            rnd = 0;
        else if (GS.IsMaxFilled() && stationlevel == 0)
            rnd = 2;
        else if (GS.IsMaxFilled() && stationlevel == 1)
            rnd = 3;
        switch (rnd) {
            case 0:
                return new Task {
                    taskType = TaskType.bring,
                    CheckConditionsMet = BringNewMatterCheck,
                    villager = this,
                    hideInt = GS.GetMattersAmount(),
                    text =
                        "Надо изучать мир вокруг! Принеси новый материал в нашу коллекцию. \n- Для того чтобы получить материал, положи предмет в тигель и активируй его.\n- Чтобы добавить материал в коллекцию, положи материал на полку и ПКМ на ярлык под этой полкой ",
                    name = "Клара"
                };
            case 1:
                List<Matter> matterL = GS.GetMattersList();

                Matter a1 = matterL[Random.Range(0, matterL.Count)];
                matterL.Remove(a1);
                Matter a2 = matterL[Random.Range(0, matterL.Count)];
                string answer, name1, name2;
                name1 = GS.GetNameByMatter(a1);
                name2 = GS.GetNameByMatter(a2);

                if (a1.Density > a2.Density)
                    answer = name1;
                else
                    answer = name2;

                return new Task {
                    taskType = TaskType.observe,
                    villager = this,
                    CheckConditionsMet = CheckLetterAnswer,
                    text = "Нужна помощь с учётом здешних материалов. Какой материал тяжелее, " + name1 + " или " + name2 +
                           "?\n- Ответ дай в виде точного названия материала!\n- Можешь дать ответ в любой момент, но попытка у тебя только одна!!",
                    name = "Клара",
                    answer = answer
                };
            case 2:
                return new Task {
                    taskType = TaskType.make,
                    CheckConditionsMet = BringPickaxeCheck,
                    villager = this,
                    text =
                        "Вижу импровизированную полку ты уже заполнил. Я готова потрудиться и сделать новую, но взамен ты должен принести мне кирку. Можешь вернуть мою, можешь у Ефима сделать новую. \n- Как сделаешь кирку, просто брось её рядом со мной, я пойму что она для меня",
                    name = "Клара"
                };
            case 3:
                return new Task {
                    taskType = TaskType.make,
                    CheckConditionsMet = BringPickaxeCheck,
                    villager = this,
                    text =
                        "Вижу импровизированную полку ты уже заполнил. Я готова потрудиться и сделать новую, но взамен ты должен принести мне кирку. Можешь вернуть мою, можешь у Ефима сделать новую. \n- Как сделаешь кирку, просто брось её рядом со мной, я пойму что она для меня",
                    name = "Клара"
                };
        }

        return null;
    }

    public override bool Interact(GameObject handObj = null) {
        if (firstMet == 0) {
            TalkManager.Say("Здравствуй, великодушный. Меня зовут Клара, я была геологом, пока не очутилась здесь.");
            firstMet++;
        } else if (firstMet == 1) {
            TalkManager.Say(
                "Покорять новые миры я не гожусь, но мне есть что тебе предложить. Но пока я не подкреплюсь, я не смогу думать ни о чём кроме еды.");
            firstMet++;
        } else if (firstMet == 2) {
            TalkManager.Say("Принеси мне 5 ягод, а взамен я научу тебя всему, что знаю.");
            firstMet++;
        } else if (firstMet == 3) {
            BringBerriesCheck();
        } else {
            TalkManager.Say("Рада тебя видеть!");
        }

        return false;
    }

    void ChangeStation(int nextStation) {
        stationlevel = nextStation;
        geologStation[nextStation].SetActive(true);
        geologStation[nextStation].GetComponent<GeologStation>().PutPreviousMatters(GS.GetLibrary(), GS.GetAll());
        geologStation[nextStation - 1].SetActive(false);
        GS = geologStation[nextStation].GetComponent<GeologStation>();
    }

    void SpawnStation() {
        transform.position = geologStation[0].transform.position + Vector3.left * 3;
        geologStation[0].SetActive(true);
        GS.CleanAround();
        tigel.SetActive(true);
        blacksmith.SetActive(true);
    }

    IEnumerator CallingPlayer() {
        while (firstMet == 0) {
            switch (Random.Range(0, 2)) {
                case 0:
                    TalkManager.Say("Кто нибудь, поговорите со мной.");
                    break;
                case 1:
                    TalkManager.Say("Мне нужна помощь, кто нибудь.");
                    break;
            }

            yield return new WaitForSeconds(5);
        }
    }
}