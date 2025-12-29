using UnityEngine;

public class VillageGenerator : MonoBehaviour {
    [Range(16, 48)]
    public int VillageRadius = 32;

    public GameObject axe, crate, campfire, elder, elderHouse, signPost;
    public GameObject geolog, geologStation_0, geologStation_1, geologStation_2, tigel, pickaxe, fuelCube;
    public GameObject blacksmith, forge, workbench;

    public GameObject[] shlak;

    Matter villagerMatter;

    GameObject SpawnItem(GameObject item, Vector3 pos, Quaternion quat, Matter main = null, Matter side = null, int rarity = -1) {
        GameObject gameObject = Instantiate(ThingsGenerator.GenerateThingPrefab(item, main, side, rarity), pos, quat);
        gameObject.transform.SetParent(null);
        return gameObject;
    }

    public void DropAxe() {
        SpawnItem(axe, new Vector3(0, 100, 0), Quaternion.identity).SetActive(true);
    }

    public void GenerateVillage(Vector3 center) {
        Vector3 centerChunkCoord = WorldGenerator.ClampCoord(center);
        /*
        while (!TestIsland.chunksDict.ContainsKey(centerChunkCoord))
            yield return new WaitForEndOfFrame();
        */

        Chunk centerChunk = WorldGenerator.chunksDict[centerChunkCoord];

        // Т.к. координаты вспомогательных узлов не совпадают с координатами треугольников, то я напрямую беру первый узел и получаю его высоту
        float properCampfireHeight = centerChunk.gameObject.GetComponent<MeshFilter>().mesh.vertices[0].y;
        center.y = centerChunk.gameObject.transform.position.y + properCampfireHeight;

        GameObject tmp;
        GameObject signpost;

        villagerMatter = ThingsGenerator.GetNextMatter(-1, MoleculeType.Fiber);

        //Очищаем место под старейшиной через костыль
        tmp = SpawnItem(campfire, center + Vector3.right * 2, campfire.transform.rotation);
        tmp.GetComponent<Building>().CleanAround();
        Destroy(tmp);

        //Старейшина и еже с ним
        GameObject elderObj = SpawnItem(elder, center + Vector3.right * 2 + Vector3.up * 30,
            elder.transform.rotation * Quaternion.Euler(Vector3.down * 90), villagerMatter);
        elderObj.SetActive(true);

        tmp = SpawnItem(campfire, center, campfire.transform.rotation);
        tmp.SetActive(false);
        elderObj.GetComponent<Elder>().campfire = tmp;

        tmp = SpawnItem(axe, center + new Vector3(2, 5, -2), axe.transform.rotation, null, null, 0);
        tmp.SetActive(false);
        elderObj.GetComponent<Elder>().axe = tmp;

        tmp = SpawnItem(crate, center + new Vector3(2, 5, -2), crate.transform.rotation);
        tmp.SetActive(false);
        elderObj.GetComponent<Elder>().crate = tmp;

        tmp = SpawnItem(elderHouse, center + Vector3.forward * 10, elderHouse.transform.rotation * Quaternion.Euler(Vector3.down * 180));
        tmp.SetActive(false);
        elderObj.GetComponent<Elder>().elderHouse = tmp;

        signpost = SpawnItem(signPost, center + Vector3.forward * 5 + Vector3.left * 8,
            signPost.transform.rotation * Quaternion.Euler(Vector3.down * 210));
        signpost.SetActive(false);
        elderObj.GetComponent<Elder>().signPost = signpost;

        //Геолог и еже с ним
        GameObject geologObj = SpawnItem(geolog, center + Vector3.right * 2 + Vector3.up * 30,
            geolog.transform.rotation * Quaternion.Euler(Vector3.down * 90), villagerMatter);
        geologObj.SetActive(false);
        elderObj.GetComponent<Elder>().geolog = geologObj;
        geologObj.GetComponent<Geolog>().signPost = signpost;

        tmp = SpawnItem(pickaxe, center + new Vector3(2, 5, -2), pickaxe.transform.rotation * Quaternion.Euler(Vector3.down * 90), null, null,
            0);
        tmp.SetActive(false);
        geologObj.GetComponent<Geolog>().pickaxe = tmp;

        tmp = SpawnItem(geologStation_0, center + Vector3.right * 10, geologStation_0.transform.rotation * Quaternion.Euler(Vector3.down * 90));
        tmp.SetActive(false);
        geologObj.GetComponent<Geolog>().geologStation[0] = tmp;

        tmp = SpawnItem(geologStation_1, center + Vector3.right * 10, geologStation_1.transform.rotation * Quaternion.Euler(Vector3.down * 90));
        tmp.SetActive(false);
        geologObj.GetComponent<Geolog>().geologStation[1] = tmp;

        tmp = SpawnItem(geologStation_2, center + Vector3.right * 10, geologStation_2.transform.rotation * Quaternion.Euler(Vector3.down * 90));
        tmp.SetActive(false);
        geologObj.GetComponent<Geolog>().geologStation[2] = tmp;

        tmp = SpawnItem(tigel, center + new Vector3(9, 0, 3.5f), tigel.transform.rotation * Quaternion.Euler(Vector3.down * 60));
        tmp.SetActive(false);
        geologObj.GetComponent<Geolog>().tigel = tmp;

        tmp = SpawnItem(fuelCube, center, fuelCube.transform.rotation);
        tmp.SetActive(false);
        geologObj.GetComponent<Geolog>().fuelCube = tmp;

        //Кузнец и еже с ним
        GameObject smithObj = SpawnItem(blacksmith, center + Vector3.right * 2 + Vector3.up * 30, blacksmith.transform.rotation,
            villagerMatter);
        smithObj.SetActive(false);
        geologObj.GetComponent<Geolog>().blacksmith = smithObj;
        smithObj.GetComponent<Blacksmith>().signPost = signpost;

        tmp = SpawnItem(forge, center + Vector3.left * 10, forge.transform.rotation);
        tmp.SetActive(false);
        smithObj.GetComponent<Blacksmith>().forge = tmp;

        tmp = SpawnItem(workbench, center + Vector3.left * 9 + Vector3.back * 4,
            workbench.transform.rotation * Quaternion.Euler(Vector3.down * -90));
        tmp.SetActive(false);
        smithObj.GetComponent<Blacksmith>().workbench = tmp;

        for (int i = 0; i < shlak.Length; i++) {
            tmp = SpawnItem(shlak[i], center + Vector3.up * (1 + i * 0.5f), Quaternion.identity);
            tmp.SetActive(true);
        }
    }
}