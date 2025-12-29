using System.Collections.Generic;
using UnityEngine;

public class ThingsGenerator : MonoBehaviour {
    [Header("Цвета")]
    public List<Color> materialColors;

    [Header("Вещи")]
    public GameObject[] NaturePlants;

    public GameObject[] NatureRocks;
    public GameObject[] Decoratives;

    public Material baseMaterial, transparentMaterial;

    public static GameObject world;
    public static Queue<Matter> mattersQueue, CrystalMQ, FiberMQ;
    public static Queue<Color> colorsQueue;
    public static GameObject[] SStructures, SDecoratives;

    static int curId;

    /***********/
    public void GenerateAllMatters() {
        curId = 0;
        mattersQueue = GenerateMattersFromMaterials();
        world = gameObject;
    }

    // Генерирует вещества на основе их признаков и редкости
    Queue<Matter> GenerateMattersFromMaterials() {
        colorsQueue = new Queue<Color>(materialColors);

        int rnd = Random.Range(0, 15);
        for (int i = 0; i < rnd; i++) {
            colorsQueue.Enqueue(colorsQueue.Dequeue());
        }

        mattersQueue = new Queue<Matter>();
        CrystalMQ = new Queue<Matter>();
        FiberMQ = new Queue<Matter>();

        int count = 0;
        for (int i = 0; i < 4; i++) {
            Material material = new Material(baseMaterial);

            Matter matter = new Matter {
                material = material,
                id = count,
                Rarity = 0,
                moleculeType = count % 2 == 0 ? MoleculeType.Crystal : MoleculeType.Fiber,
                Density = Random.Range(1, 20),
                Durability = Random.Range(1, 20),
                BurningTime = 0,
                BouncingScale = 0,
                isGlass = false,
                isMagnet = false,
                isGlow = false,
                isFastGrowing = false,
                isTeleporting = false
            };

            if (matter.isGlass) {
                material = new Material(transparentMaterial);
                material.SetFloat("alpha", Random.Range(0.3f, 0.8f));
                matter.material = material;
            }

            Color color = colorsQueue.Dequeue();
            colorsQueue.Enqueue(color);
            material.SetColor("baseColor", color);

            count++;

            if (matter.moleculeType == MoleculeType.Crystal)
                material.SetFloat("metallic", 1);

            if (matter.moleculeType == MoleculeType.Crystal)
                material.SetFloat("smoothness", Random.Range(0.6f, 1));

            mattersQueue.Enqueue(matter);
        } // Common

        for (int i = 0; i < 8; i++) {
            Material material = new Material(baseMaterial);

            Matter matter = new Matter {
                material = material,
                id = count,
                Rarity = 1,
                moleculeType = count % 2 == 0 ? MoleculeType.Crystal : MoleculeType.Fiber,
                Density = Random.Range(1, 40),
                Durability = Random.Range(1, 40),
                BurningTime = Random.Range(0, 2) == 0 ? 0 : Random.Range(3, 10),
                BouncingScale = Random.Range(0, 3) == 0 ? 0 : Random.Range(3, 10),
                isGlass = false,
                isMagnet = false,
                isGlow = false,
                isFastGrowing = false,
                isTeleporting = false
            };

            if (matter.isGlass) {
                material = new Material(transparentMaterial);
                material.SetFloat("alpha", Random.Range(0.1f, 0.5f));
                matter.material = material;
            }

            Color color = colorsQueue.Dequeue();
            colorsQueue.Enqueue(color);
            material.SetColor("baseColor", color);

            count++;
            if (matter.moleculeType == MoleculeType.Crystal)
                material.SetFloat("metallic", 1);

            if (matter.moleculeType == MoleculeType.Crystal)
                material.SetFloat("smoothness", Random.Range(0.6f, 1));

            mattersQueue.Enqueue(matter);
        } // Uncommon

        for (int i = 0; i < 12; i++) {
            Material material = new Material(baseMaterial);

            Matter matter = new Matter {
                material = material,
                id = count,
                Rarity = 2,
                moleculeType = count % 2 == 0 ? MoleculeType.Crystal : MoleculeType.Fiber,
                Density = Random.Range(1, 60),
                Durability = Random.Range(1, 60),
                BurningTime = Random.Range(0, 2) == 0 ? Random.Range(3, 10) : 0,
                BouncingScale = Random.Range(0, 3) == 0 ? Random.Range(3, 10) : 0,
                isGlass = Random.Range(0, 10) == 0 ? true : false,
                isMagnet = Random.Range(0, 4) == 0 ? true : false,
                isGlow = false,
                isFastGrowing = false,
                isTeleporting = false
            };

            if (matter.isGlass) {
                material = new Material(transparentMaterial);
                material.SetFloat("alpha", Random.Range(0.1f, 0.5f));
                matter.material = material;
            }

            Color color = colorsQueue.Dequeue();
            colorsQueue.Enqueue(color);
            material.SetColor("baseColor", color);

            count++;
            if (matter.moleculeType == MoleculeType.Crystal)
                material.SetFloat("metallic", 1);

            if (matter.moleculeType == MoleculeType.Crystal)
                material.SetFloat("smoothness", Random.Range(0.6f, 1));

            mattersQueue.Enqueue(matter);
        } // Rare

        for (int i = 0; i < 16; i++) {
            Material material = new Material(baseMaterial);

            Matter matter = new Matter {
                material = material,
                id = count,
                Rarity = 3,
                moleculeType = count % 2 == 0 ? MoleculeType.Crystal : MoleculeType.Fiber,
                Density = Random.Range(1, 80),
                Durability = Random.Range(1, 80),
                BurningTime = Random.Range(0, 2) == 0 ? Random.Range(3, 10) : 0,
                BouncingScale = Random.Range(0, 3) == 0 ? Random.Range(3, 10) : 0,
                isGlass = Random.Range(0, 15) == 0 ? true : false,
                isMagnet = Random.Range(0, 4) == 0 ? true : false,
                isGlow = Random.Range(0, 5) == 0 ? true : false,
                isFastGrowing = false,
                isTeleporting = false
            };

            if (matter.isGlass) {
                material = new Material(transparentMaterial);
                material.SetFloat("alpha", Random.Range(0.1f, 0.5f));
                matter.material = material;
            }

            Color color = colorsQueue.Dequeue();
            colorsQueue.Enqueue(color);
            material.SetColor("baseColor", color);

            count++;
            if (matter.moleculeType == MoleculeType.Crystal)
                material.SetFloat("metallic", 1);

            if (matter.moleculeType == MoleculeType.Crystal)
                material.SetFloat("smoothness", Random.Range(0.6f, 1));

            mattersQueue.Enqueue(matter);
        } // Mythical

        for (int i = 0; i < 20; i++) {
            Material material = new Material(baseMaterial);

            Matter matter = new Matter {
                material = material,
                id = count,
                Rarity = 4,
                moleculeType = count % 2 == 0 ? MoleculeType.Crystal : MoleculeType.Fiber,
                Density = Random.Range(1, 80),
                Durability = Random.Range(1, 80),
                BurningTime = Random.Range(0, 2) == 0 ? Random.Range(3, 10) : 0,
                BouncingScale = Random.Range(0, 3) == 0 ? Random.Range(3, 10) : 0,
                isGlass = Random.Range(0, 20) == 0 ? true : false,
                isMagnet = Random.Range(0, 4) == 0 ? true : false,
                isGlow = Random.Range(0, 5) == 0 ? true : false,
                isFastGrowing = Random.Range(0, 6) == 0 ? true : false,
                isTeleporting = Random.Range(0, 7) == 0 ? true : false
            };

            if (matter.isGlass) {
                material = new Material(transparentMaterial);
                material.SetFloat("alpha", Random.Range(0.1f, 0.5f));
                matter.material = material;
            }

            Color color = colorsQueue.Dequeue();
            colorsQueue.Enqueue(color);
            material.SetColor("baseColor", color);

            count++;
            if (matter.moleculeType == MoleculeType.Crystal)
                material.SetFloat("metallic", 1);

            if (matter.moleculeType == MoleculeType.Crystal)
                material.SetFloat("smoothness", Random.Range(0.6f, 1));

            mattersQueue.Enqueue(matter);
        } // Legendary

        return mattersQueue;
    }

    public static Matter GetNextMatter(int needRarity, MoleculeType molecule) {
        Matter matter = mattersQueue.Dequeue();
        mattersQueue.Enqueue(matter);

        if (needRarity == -1)
            return matter;

        int foolprof = 0;
        while (!(matter.Rarity == needRarity && matter.moleculeType == molecule) && foolprof < 61) {
            matter = mattersQueue.Dequeue();
            mattersQueue.Enqueue(matter);
            foolprof++;
        }

        if (foolprof == 61)
            Debug.Log("Редкость " + needRarity + " молекула " + molecule + " не найден из " + mattersQueue.Count);

        return matter;
    }

    /***********/

    public static GameObject GenerateThingPrefab(GameObject model, Matter main, Matter side, int needRarity = -1,
        MoleculeType molecule = MoleculeType.Fiber) {
        if ((main == null))
            main = GetNextMatter(needRarity, molecule);

        if ((side == null))
            side = GetNextMatter(needRarity, molecule);

        //Генерирует объект в реальном мире из префаба, меняет ему параметры
        GameObject tmp = Instantiate(model, world.transform);
        tmp.GetComponent<Thing>().Init(curId++, main, side);
        tmp.SetActive(false);

        return tmp;
    }

    public static GameObject GenerateThing(GameObject model, Matter main, Matter side, int needRarity = -1,
        MoleculeType molecule = MoleculeType.Fiber) {
        if ((main == null))
            main = GetNextMatter(needRarity, molecule);

        if ((side == null))
            side = GetNextMatter(needRarity, molecule);

        //Генерирует объект в реальном мире из префаба, меняет ему параметры
        GameObject tmp = Instantiate(model);
        tmp.GetComponent<Thing>().Init(curId++, main, side);

        return tmp;
    }

    public void GenerateNatureStructures() {
        SStructures = new GameObject[mattersQueue.Count * 2];
        SDecoratives = Decoratives;
        Queue<Matter> helpQueue = new Queue<Matter>();

        int index = 0;
        while (index < mattersQueue.Count * 2) {
            Matter mainMatter = mattersQueue.Dequeue();
            mattersQueue.Enqueue(mainMatter);

            if (mainMatter.moleculeType == MoleculeType.Crystal)
                SStructures[index] = GenerateThingPrefab(NatureRocks[Random.Range(0, NatureRocks.Length)], mainMatter, null, mainMatter.Rarity,
                    MoleculeType.Crystal);
            else
                SStructures[index] = GenerateThingPrefab(NaturePlants[Random.Range(0, NatureRocks.Length)], mainMatter, null, mainMatter.Rarity,
                    MoleculeType.Fiber);
            index++;
        }
    }

    public static GameObject GetStructure(float distance, int temp, int prol, FloraType type) {
        List<GameObject> possibleStr = new List<GameObject>();

        switch (type) {
            case FloraType.SourceType:
                foreach (GameObject structure in SStructures) {
                    if (structure.GetComponent<NatureStructure>().CheckWeatherComfort(distance, temp, prol))
                        possibleStr.Add(structure);
                }

                break;

            case FloraType.DecorativeType:
                foreach (GameObject plant in SDecoratives) {
                    if (plant.GetComponent<NatureStructure>().CheckWeatherComfort(distance, temp, prol))
                        possibleStr.Add(plant);
                }

                break;
        }

        if (possibleStr.Count > 0)
            return possibleStr[Random.Range(0, possibleStr.Count)];
        else
            return null;
    }
}

public enum FloraType {
    SourceType,
    DecorativeType
}