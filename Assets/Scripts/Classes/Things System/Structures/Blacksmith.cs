using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blacksmith : Villager {
    [Header("Boiler")]
    public int mattersNeeded = 4;

    public Color hatColor;
    public GameObject hat;

    [HideInInspector]
    public GameObject forge, garden, workbench;

    int firstMet;

    /**********/

    private void Start() {
        firstMet = 0;
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

    public override bool Interact(GameObject handObj = null) {
        if (firstMet == 0) {
            TalkManager.Say("Дарова, богатырь. Я Ефим, кузнец. Конкретно меня накрыло после того самогону. ");
            firstMet++;
        } else if (firstMet == 1) {
            TalkManager.Say("Куда бередить чтобы дом найти я знать не знаю, так шо останусь здесь. А ты мне подсобишь построить новую кузню.");
            firstMet++;
        } else if (firstMet == 2) {
            TalkManager.Say("Принеси мне материалов, разных и побольше, а мы потом как-нибудь сочтёмся.");
            firstMet++;
        } else if (firstMet == 3) {
            BringMaterialsCheck();
        } else
            TalkManager.Say("Рад тебя видеть!");

        return false;
    }

    protected override Task GenerateTask() {
        int rnd = 0;
        switch (rnd) {
            case 0:
                return new Task {
                    taskType = TaskType.make,
                    CheckConditionsMet = BringHammerCheck,
                    villager = this,
                    text =
                        "Посмотрим, насколько хорошо ты управляешься с кузней. Сделай-ка мне новый молот. Принеси и брось рядом, тогда я признаю тебя как своего подмастерья. \n- Чтобы сделать молот нужно положить две детали (цилиндр и куб) на верстак. \n- Чтобы сделать деталь, нужно положить материал или другую деталь на наковальню.",
                    name = "Ефим"
                };
        }

        return null;
    }

    void BringMaterialsCheck() {
        Collider[] colls = Physics.OverlapSphere(transform.position, pickUpRadius, ~IgnoreMask);
        GameObject[] matters = new GameObject[mattersNeeded];
        List<int> matterIdList = new List<int>();

        int have = 0;
        bool IsAnyCopies = false;
        foreach (Collider coll in colls) {
            if (coll.gameObject.GetComponent<MatterCube>()) {
                if (!matterIdList.Contains(coll.gameObject.GetComponent<MatterCube>().mainM.id)) {
                    matters[have] = coll.gameObject;
                    matterIdList.Add(coll.gameObject.GetComponent<MatterCube>().mainM.id);
                    have++;
                } else
                    IsAnyCopies = true;
            }
        }

        if (have == mattersNeeded) {
            foreach (GameObject matter in matters) {
                Destroy(matter);
            }

            forge.SetActive(true);
            firstMet++;
            SpawnStation();
            TalkManager.Say("Потрудился ты на славу, богатырь. Теперь можешь ковать всё что тебе вздумается!");
        } else if (have == 0)
            TalkManager.Say("Положи кубики рядом со мной, я пойму что они для меня.");
        else if (IsAnyCopies)
            TalkManager.Say("Неее, все материалы должны быть разными. Без повторений.");
        else
            TalkManager.Say("Отлично, осталось ещё " + (mattersNeeded - have) + " кубика");
    }

    void BringHammerCheck(Task task) {
        Collider[] colls = Physics.OverlapSphere(transform.position, pickUpRadius, ~IgnoreMask);
        GameObject hammer = null;

        foreach (Collider coll in colls) {
            if (coll.gameObject.GetComponent<Hammer>()) {
                hammer = coll.gameObject;
                break;
            }
        }

        if (hammer) {
            Destroy(curLetter);
            VillagerAnswer(true, 4, "Отлично! С таким молотом я смогу делать более хитроумные детали.");
            forge.GetComponent<Forge>().Upgrade();
        } else
            TalkManager.Say("Сначала принеси молот, потом подпишу письмо. Не наоборот.");
    }

    void SpawnStation() {
        forge.GetComponent<Building>().CleanAround();
        workbench.GetComponent<Building>().CleanAround();
        workbench.SetActive(true);
        forge.SetActive(true);

        transform.position = forge.transform.position + Vector3.left * 1;
        transform.Rotate(new Vector3(0, 180, 0));

        signPost.GetComponent<SignPost>().EnqueTask(GenerateTask());
    }

    IEnumerator CallingPlayer() {
        while (firstMet == 0) {
            switch (Random.Range(0, 2)) {
                case 0:
                    TalkManager.Say("Ог ты ж, как голова то болит.");
                    break;
                case 1:
                    TalkManager.Say("Мутиииит, аааа. Есть тут минеральный источник?");
                    break;
            }

            yield return new WaitForSeconds(5);
        }
    }
}