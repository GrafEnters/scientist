using System.Collections.Generic;
using UnityEngine;

public class Bush : Plant, IInteractable {
    public int maxBerriessAmount = 2;
    public float radius = 1;
    public GameObject BerryModel;

    [HideInInspector]
    public GameObject BerryPrefab;

    protected int berriesAmount, curdoubled;

    public List<Transform> BerryPlace;
    List<Vector3> BerryPosList;
    List<GameObject> BerriesObjList;

    public override void Init(int id, Matter main, Matter side) {
        base.Init(id, main, side);
        liveStepMax = Random.Range(15, 30);
        liveTimeMax = Random.Range(150, 300);
        maxBerriessAmount = Random.Range(2, 4);

        BerryPrefab = ThingsGenerator.GenerateThingPrefab(BerryModel, side, side);
        BerryPrefab.GetComponent<Fruit>().GetDadandSon(gameObject);
    }

    private void Start() {
        curdoubled = 0;
        berriesAmount = Random.Range(0, maxBerriessAmount);

        BerriesObjList = new List<GameObject>();
        BerryPosList = new List<Vector3>();
        foreach (Transform transform in BerryPlace) {
            BerryPosList.Add(transform.position);
            Destroy(transform.gameObject);
        }

        BerryPlace.Clear();
        Reproduce();
        Reproduce();
        Reproduce();
    }

    protected override void Grow() {
        base.Grow();
        curScale = 0.4f + (liveTime / 100f) * 0.6f;
        if (curScale <= 1) {
            transform.localScale = Vector3.one * curScale;
        } else if (Random.Range(1f, 2f) < curScale)
            OverGrown();
        else
            Reproduce();
    }

    void Reproduce() {
        if (Random.Range(0, 10) == 0) {
            if (curdoubled < 1) {
                // Настройка луча из камеры игрока
                Ray ray = new Ray(transform.position + new Vector3(Random.Range(-1.5f, 1.5f), 3, Random.Range(-1.5f, 1.5f)), Vector3.down);

                if (Physics.Raycast(ray, out RaycastHit hit, LayerMask.GetMask("Ground"), 6)) {
                    curdoubled++;
                    Instantiate(gameObject, hit.transform.position,
                        Quaternion.Euler(Random.Range(-15, 15), Random.Range(-90, 90), Random.Range(-15, 15)));
                }
            }
        } else if (BerriesObjList.Count < maxBerriessAmount && BerryPosList.Count > 0)
            GrowBerry();
    }

    public override void Break(Vector3 hitPoint, int durability) {
        base.Break(hitPoint, durability);
    }

    public override void Broken(Vector3 hitpoint) {
        Destroy(gameObject);
    }

    void DropBerry(GameObject berry) {
        BerryPosList.Add(berry.transform.position);
        berry.transform.position = transform.position + Vector3.up * 0.5f;

        berry.GetComponent<Rigidbody>().useGravity = true;
        berry.GetComponent<Rigidbody>().isKinematic = false;
        berry.GetComponent<Rigidbody>().AddForce(Vector3.up * 3, ForceMode.VelocityChange);
        berry.GetComponent<Rigidbody>().detectCollisions = true;
        berry.transform.SetParent(null);
    }

    void GrowBerry() {
        int rnd = Random.Range(0, BerryPosList.Count);
        GameObject berry = DropResourse(BerryPrefab, BerryPosList[rnd]);
        BerryPosList.Remove(BerryPosList[rnd]);
        berry.transform.SetParent(gameObject.transform);
        berry.SetActive(true);
        berry.GetComponent<Rigidbody>().isKinematic = true;
        berry.GetComponent<Rigidbody>().detectCollisions = false;
        BerriesObjList.Add(berry);
    }

    public bool Interact(GameObject handObject = null) {
        if (BerriesObjList.Count > 0) {
            int rnd = Random.Range(0, BerriesObjList.Count);
            GameObject berry = BerriesObjList[rnd];
            BerriesObjList.Remove(berry);
            DropBerry(berry);
        }

        return false;
    }

    private void OnDrawGizmos() {
        foreach (Transform transform in BerryPlace) {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.2f);
        }
    }
}