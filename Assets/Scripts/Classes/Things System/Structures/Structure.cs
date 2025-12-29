using UnityEngine;

public abstract class Structure : Thing {
    [Header("Structure params")]
    public int hp;

    public override void Init(int id, Matter main, Matter side) {
        base.Init(id, main, side);
        thingName += id.ToString();
        hp = 3;
    }

    protected GameObject DropResourse(GameObject resourse, Vector3 pos) {
        GameObject res = Instantiate(resourse, pos, Quaternion.identity);
        res.GetComponent<Rigidbody>().isKinematic = false;
        res.GetComponent<Rigidbody>().useGravity = true;
        res.SetActive(true);
        res.transform.SetParent(null);
        return res;
    }

    protected GameObject DropResourse(GameObject resourse, Vector3 pos, Quaternion quaternion) {
        GameObject res = Instantiate(resourse, pos, quaternion);
        res.GetComponent<Rigidbody>().isKinematic = false;
        res.GetComponent<Rigidbody>().useGravity = true;
        res.SetActive(true);
        res.transform.SetParent(null);
        return res;
    }
}