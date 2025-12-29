using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class Thing : MonoBehaviour {
    public string thingName;
    public int volume = 1;
    public MeshRenderer[] mainRenderers;
    public MeshRenderer[] sideRenderers;

    [HideInInspector]
    public int id;

    [HideInInspector]
    public float durability, density;

    public Matter mainM, sideM;

    public Thing() { }

    public Thing(int _id, string _name, Matter _matter, Matter _Smatter) {
        id = _id;
        thingName = _name;
        mainM = _matter;
        sideM = _Smatter;
    }

    public virtual void Init(int id, Matter main, Matter side) {
        this.id = id;
        mainM = main;
        if (side != null)
            sideM = side;

        if (mainRenderers.Length > 0) {
            foreach (MeshRenderer mainR in mainRenderers)
                mainR.material = main.material;
        }

        if (sideRenderers.Length > 0) {
            foreach (MeshRenderer sideR in sideRenderers)
                sideR.material = side.material;

            durability = (mainM.Durability + sideM.Durability) / 2;
            density = (mainM.Density + sideM.Density) / 2;
        } else {
            durability = mainM.Durability;
            density = mainM.Density;
        }

        GetComponent<Rigidbody>().mass = volume * density;
    }

    public void Burn() {
        int burningTime = mainM.BurningTime;
        if (burningTime > 0)
            StartCoroutine(Burning(burningTime));
    }

    IEnumerator Burning(float burningTime) {
        Vector3 scale = gameObject.transform.localScale;
        float curTime = burningTime;
        // Отключить интерактивность??
        while (curTime >= 0) {
            curTime -= Time.fixedDeltaTime;
            gameObject.transform.localScale = scale * curTime / burningTime;
            yield return new WaitForFixedUpdate();
        }

        Destroy(gameObject);
        yield return null;
    }
}