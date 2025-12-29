using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elder : Villager {
    [Header("Elder")]
    public int logNeeded = 3;

    public Color hatColor;
    public GameObject hat;

    [HideInInspector]
    public GameObject campfire, elderHouse, crate, axe, geolog;

    int hintsGiven, firstMet;
    Queue<string> hints;

    /**********/

    private void Start() {
        firstMet = 0;
        StartCoroutine(CallingPlayer());
        GetNewHints();
        hat.GetComponent<MeshRenderer>().material.SetColor("baseColor", hatColor);
    }

    private void OnDisable() {
        //	StopAllCoroutines();
    }

    private void OnEnable() {
        CallingPlayer();
    }

    /**********/

    public override bool Interact(GameObject handObj = null) {
        if (firstMet == 0) {
            TalkManager.Say("Наконец-то. Надеюсь ты порядочнее предыдущего. Меня зовут Карл,a твоё имя меня не интересует.");
            firstMet++;
        } else if (firstMet == 1) {
            TalkManager.Say("Надо тут освоиться. Костёр я развёл, но сил больше не осталось. Вот тебе мой топор. Подними его на клавишу F.");
            campfire.SetActive(true);
            campfire.GetComponent<Building>().CleanAround();
            axe.SetActive(true);
            firstMet++;
        } else if (firstMet == 2 && handObj && handObj.GetComponent<Thing>().thingName == "axe") {
            TalkManager.Say("Отлично сидит в руке, да? Будем считать что да. Теперь принеси 3 бревна, построим основу для деревни");
            firstMet++;
        } else if (firstMet == 2) {
            TalkManager.Say("Мне нужно три бревна. Так что не задерживайся, путник.");
            firstMet++;
        } else if (firstMet == 3) {
            BringLogsCheck();
        } else {
            string hint = hints.Dequeue();
            hintsGiven++;
            hints.Enqueue(hint);
            TalkManager.Say(hint);
        }

        return false;
    }

    protected override Task GenerateTask() {
        return new Task {
            taskType = TaskType.bring,
            CheckConditionsMet = BringMeLetter,
            text =
                "Раз я здесь самый первый, то возьму на себя роль старейшины. А все старейшины любят когда в их деревне покой, и всякие падающие с неба люди не кричат почём зря. Впредь, если кому-то нужна твоя помощь - он оставит письмо. Принеси мне это письмо, кстати.\n- ПКМ на меня с письмом в руке завершит задание",
            name = "Карл",
            villager = this
        };
    }

    void BringLogsCheck() {
        Collider[] colls = Physics.OverlapSphere(transform.position, pickUpRadius, ~IgnoreMask);
        GameObject[] logs = new GameObject[logNeeded];

        int have = 0;
        foreach (Collider coll in colls) {
            if (coll.gameObject.GetComponent<Thing>() && coll.gameObject.GetComponent<Thing>().thingName == "log") {
                logs[have] = coll.gameObject;
                have++;
            }
        }

        if (have == logNeeded) {
            foreach (GameObject log in logs) {
                Destroy(log);
            }

            elderHouse.SetActive(true);
            elderHouse.GetComponent<Building>().CleanAround();
            crate.SetActive(true);
            TalkManager.Say(
                "Вот тебе ящик, с ним будет поудобнее. Клавишей Е сможешь достать из за спины, а нажав 1, повесишь инструмент на крючок. ");
            firstMet++;
            SpawnGeolog();
            signPost.SetActive(true);
            signPost.GetComponent<Building>().CleanAround();
            signPost.GetComponent<SignPost>().EnqueTask(GenerateTask());
        } else if (have == 0)
            TalkManager.Say("Положи бревна рядом со мной, я пойму что они для меня.");
        else
            TalkManager.Say("Отлично, осталось ещё " + (logNeeded - have) + " бревна");
    }

    void BringMeLetter(Task task) {
        VillagerAnswer(true, 1, "Видишь как просто! Не забывай почаще проверять письма на этой стойке.");
        Destroy(curLetter);
    }

    void SpawnGeolog() {
        transform.position = elderHouse.transform.position - Vector3.forward * 1.5f;
        geolog.SetActive(true);
    }

    IEnumerator CallingPlayer() {
        while (firstMet == 0) {
            switch (Random.Range(0, 3)) {
                case 0:
                    TalkManager.Say("Эй, новенький. Подойди и нажми на меня ПКМ, познакомимся.");
                    break;
                case 1:
                    TalkManager.Say("Игроооок, ау. Нажми на меня ПКМ, я в долгу не останусь.");
                    break;
                case 2:
                    TalkManager.Say("Путник, нажми на меня ПКМ, ты не пожалеешь.");
                    break;
            }

            yield return new WaitForSeconds(5);
        }
    }

    void GetNewHints() {
        hints = new Queue<string>();
        hints.Enqueue("Ходить туда-сюда и бесить меня можешь на клавиши WASD.");
        hints.Enqueue("Очевидно, осматриваться нужно мышкой. Это хотя бы тебе понятно?");
        hints.Enqueue("Взаимодействие с любым объектом происходит на клавишу ПКМ. Чтобы доставать меня вопросами она идеально подходит.");
        hints.Enqueue("Чтобы взять или выбросить предмет, нажми F. Смотри не потеряй мой топор.");
        hints.Enqueue("Чтобы предмет остался внутри ящика, он должен перестать двигаться.");
        hints.Enqueue("Ящик не резиновый, но мой кузен умудряется носить в нём целые брёвна. Достать ящик из-за спины всегда можно клавишей E");
        hints.Enqueue("Инструмент в руке можно удобно повесить на крючок ящика, нажав клавишу 1.");
        hints.Enqueue("Не забывай почаще проверять новые письма. Это твоя работа.");
        hints.Enqueue("Я тебе дал уже " + hintsGiven + " советов. Я начинаю повторяться.");
    }
}