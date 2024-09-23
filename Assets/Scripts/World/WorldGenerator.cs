using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
	[Header("Свойства острова")]
	[Range(1, 32)]
	public int chunkSize = 16;
	[Range(1, 15)]
	public int islandRadiusInChunks;
	[Range(1, 16)]
	public int polygonSize;

	public bool IsSpawningPlants;
	public bool IsSpawningVillage;
	public bool IsDeactivatingChunks;

	public Biome[] biomes;

	[Header("Свойства поверхности")]
	public LandNoiseGenerator WorldNoise;
	public WeatherNoiseGenerator WeaterNoise;


	[Header("Остальное")]
	public GameObject ChunkPrefab;
	public Transform Player;
	public string seed;
	public int viewDistanceInChunks;

	Vector3 playerCoord;
	Vector3 startCoord;
	Vector3 spawnPoint;
	Transform ChunkHolder;
	Queue<Vector3> ChunksToSpawn;

	ThingsGenerator ThingsGenerator;

	public static Dictionary<Vector3, Chunk> chunksDict;
	public static int SchunkSize;
	public static bool SIsSpawningPlants;

	/**********/

	private void Awake()
	{

		//StartCoroutine(Timer()); 
		SchunkSize = chunkSize;
		SIsSpawningPlants = IsSpawningPlants;

		StartCoroutine(SpawnChunkParallel());

		// Если в сцене отсутствует CHUNKHOLDER, то генерит новый
		if (GameObject.Find("ChunkHolder") == null)
			ChunkHolder = new GameObject("ChunkHolder").transform;
		else
			ChunkHolder = GameObject.Find("ChunkHolder").transform;

		if (seed.Length == 0)
			seed = DateTime.Now.GetHashCode().ToString();

		WorldNoise.SetSeed(seed);
		WeaterNoise.SetSeed(seed);




		ThingsGenerator = GetComponent<ThingsGenerator>();
		ThingsGenerator.GenerateAllMatters();
		ThingsGenerator.GenerateNatureStructures();

		chunksDict = new Dictionary<Vector3, Chunk>();

		CreateChunks(Vector3.zero, 0);
	}

	private void Start()
	{
		spawnPoint = new Vector3(0, WorldNoise.IslandTerrainNoise(Vector3.zero, chunkSize), 0);
		startCoord = Vector3.down;

		StartCoroutine(Teleportation(3f));
		InputManager.CanPlayerMove = true;

		if (IsSpawningVillage)
			StartCoroutine(SpawnVillage());
	}

	private void Update()
	{
		playerCoord = ClampCoord(Player.position, chunkSize);
		if (startCoord != playerCoord)
		{
			startCoord = playerCoord;
			CreateChunks(playerCoord, viewDistanceInChunks);
			UpdateChunks();
		}
	}

	/**********/

	public void TeleportPlayerToStart()
	{
		StartCoroutine(Teleportation(0.1f));
	}

	IEnumerator SpawnVillage()
	{
		yield return new WaitForSeconds(2);
		GetComponent<VillageGenerator>().GenerateVillage(spawnPoint);
	}
	   
	IEnumerator Teleportation(float seconds)
	{
		Player.GetComponent<CharacterController>().enabled = false;
		Player.transform.position = spawnPoint + Vector3.one * 5;
		yield return new WaitForSeconds(seconds);
		Player.GetComponent<CharacterController>().enabled = true;
	}	

	void SpawnItem(GameObject item, Vector3 pos)
	{
		GameObject gameObject = Instantiate(item, pos, Quaternion.identity);
		gameObject.SetActive(true);
		Rigidbody RB = gameObject.GetComponent<Rigidbody>();
		RB.isKinematic = false;
		RB.useGravity = true;
		RB.linearVelocity = Vector3.down;
		gameObject.transform.SetParent(null);
	}

	void UpdateChunks()
	{
		foreach (KeyValuePair<Vector3, Chunk> chunk in chunksDict)
		{
			if ((chunk.Key - playerCoord).magnitude > viewDistanceInChunks * 1.5f && IsDeactivatingChunks)
				chunk.Value.DeActivate();
			else
				chunk.Value.gameObject.SetActive(true);
		}
	}

	void CreateChunks(Vector3 centerChunkCoord, int viewDistance) //Радиус 0 для одной точки и т, д
	{
		int a, b, c;
		a = Mathf.FloorToInt(centerChunkCoord.x);
		b = Mathf.FloorToInt(centerChunkCoord.y);
		c = Mathf.FloorToInt(centerChunkCoord.z);
		PlaneSearch(a, b, c, a, b, c); // Обработка центра кубов

		// Дальше ходим по граням
		for (int r = 1; r <= viewDistance; r++)
		{
			PlaneSearch(a - r, b - r, c + r, a + r, b + r, c + r); // Полная верхняя грань
			PlaneSearch(a - r, b - r, c - r, a + r, b + r, c - r); // Полная нижняя грань грань
			PlaneSearch(a + r, b - r, c - r + 1, a + r, b + r, c + r - 1);// Передняя грань без верха и низа
			PlaneSearch(a - r, b - r, c - r + 1, a - r, b + r, c + r - 1); // Задняя грань без верха и низа
			PlaneSearch(a - r + 1, b + r, c - r + 1, a + r - 1, b + r, c + r - 1); // Правая грань без границ
			PlaneSearch(a - r + 1, b - r, c - r + 1, a + r - 1, b - r, c + r - 1); // Левая грань без границ
		}
	}

	void PlaneSearch(int f_a, int f_b, int f_c, int s_a, int s_b, int s_c)
	{
		for (int i = f_a; i <= s_a; i++)
			for (int j = f_b; j <= s_b; j++)
				for (int k = f_c; k <= s_c; k++)
				{
					Vector3 newChunkPos = new Vector3(i, j, k);

					if (!chunksDict.ContainsKey(newChunkPos) && !ChunksToSpawn.Contains(newChunkPos))
					{
						ChunksToSpawn.Enqueue(newChunkPos);
					}
				}
	}

	IEnumerator SpawnChunkParallel()
	{
		ChunksToSpawn = new Queue<Vector3>();
		for (; ; )
		{
			yield return new WaitForEndOfFrame();
			if (ChunksToSpawn.Count == 0)
				continue;
			else
			{
				Vector3 newChunkPos = ChunksToSpawn.Dequeue();

				SpawnChunk(newChunkPos);
			}
		}
	}

	void SpawnChunk(Vector3 coord)
	{
		GameObject chunkObject = Instantiate(ChunkPrefab, new Vector3(coord.x, coord.y, coord.z) * chunkSize + Vector3.up * chunkSize / 2, Quaternion.identity, ChunkHolder);
		chunkObject.name = "chunk " + coord.x + " " + coord.y + " " + coord.z;

		Chunk chunk = chunkObject.GetComponent<Chunk>();

		int[,,] nodes = new int[chunkSize + 1, chunkSize + 1, chunkSize + 1];
		int[,,] tempNodes = new int[chunkSize + 1, chunkSize + 1, chunkSize + 1];
		int[,,] prolNodes = new int[chunkSize + 1, chunkSize + 1, chunkSize + 1];
		for (int x = 0; x < chunkSize + 1; x++)
		{
			for (int y = 0; y < chunkSize + 1; y++)
			{
				for (int z = 0; z < chunkSize + 1; z++)
				{
					Vector3 clearCoord = coord * chunkSize + new Vector3(x, y, z);
					nodes[x, y, z] = WorldNoise.Noise(clearCoord, chunkSize);
					tempNodes[x, y, z] = WeaterNoise.GetTemperature(clearCoord);
					prolNodes[x, y, z] = WeaterNoise.GetProlificacy(clearCoord, WorldNoise.IslandTerrainNoise(clearCoord, chunkSize));
				}
			}
		}

		chunk.nodes = nodes;
		chunk.tempNodes = tempNodes;
		chunk.prolNodes = prolNodes;
		// перенести внутрь чанка
		chunk.height = Mathf.Clamp(coord.y * chunkSize + chunkSize / 2, 0, 1000);

		// Если чанк находится под поверхностью, то его биом - пещера, иначе подбирает по высоте
		if (chunk.height < WorldNoise.IslandTerrainNoise(coord * chunkSize, chunkSize) - chunkSize * 2)
			chunk.biome = new Biome(biomes[biomes.Length - 1]);
		else
			for (int i = 0; i < biomes.Length; i++)
			{
				if (chunk.height >= biomes[i].minHeight)
					chunk.biome = new Biome(biomes[i]);
				if (chunk.height < biomes[i].minHeight)
					break;
			}

		chunk.chunkSize = chunkSize;
		chunk.polygonSize = polygonSize;
		chunk.Init();
		chunksDict.Add(coord, chunk);
	}

	public static Vector3 ClampCoord(Vector3 vectr)
	{
		if (SchunkSize != 0)
			return new Vector3((int)vectr.x / SchunkSize, (int)vectr.y / SchunkSize, (int)vectr.z / SchunkSize);
		else
			return new Vector3((int)vectr.x / 16, (int)vectr.y / 16, (int)vectr.z / 16); // костыль
	}

	public static Vector3 ClampCoord(Vector3 vectr, int chunkSize)
	{
		return new Vector3((int)vectr.x / chunkSize, (int)vectr.y / chunkSize, (int)vectr.z / chunkSize);
	}
}

[Serializable]
public class LandNoiseGenerator
{
	public float FlatspawnRadius = 22;


	[Tooltip("Количество наложенных шумов. С каждой октавой частота удваивается")]
	[Range(1, 10)]
	public int octaves = 1;
	[Tooltip("Насколько большими будут биомы")]
	public int multiplier = 35;
	[Tooltip("Насколько сильно норма смещена в сторону суши")]
	[Range(0, 1f)]
	public float dump = 0.3f;
	[Tooltip("Насколько резкими будут переходы")]
	[Range(1, 10)]
	public float sharpness = 2;
	[Tooltip("Уровень влияния более высокочастотных шумов")]
	[Range(0f, 1f)]
	public float persistance = 0.9f;
	[Tooltip("Уровень плотности чанков")]
	[Range(0f, 100f)]
	public float densityEdge = 40f;
	[Tooltip("Насколько сплющен будет остров")]
	[Range(0, 100f)]
	public int negativedump = 0;
	[Tooltip("Горы выше этой высоты резко обрубаются")]
	[Range(0, 100f)]
	public int mountainsHeight = 100;


	SimplexNoiseGenerator SNG;


	public void SetSeed(string seed)
	{
		SNG = new SimplexNoiseGenerator(seed);
	}

	public int Noise(Vector3 point, int chunkSize)
	{
		float height;
		height = IslandTerrainNoise(point, chunkSize);


		if (0 > point.y || point.y > height)
			return 0;

		if (Mathf.Abs(point.x) < chunkSize * 1.5f && Mathf.Abs(point.z) < chunkSize * 1.5f)
			return 1;
		else if (point.y > height - 3)
			return SNG.coherentNoise(new Vector3(point.x, height, point.z), octaves, multiplier, dump, sharpness, persistance) <= (densityEdge) ? 1 : 0;
		else if (height > 0 && point.y < 2)
			return 1;
		else
			return SNG.coherentNoise(point, octaves, multiplier, dump, sharpness, persistance) <= (densityEdge) ? 1 : 0;
	}

	public float IslandTerrainNoise(Vector3 point, int chunkSize)
	{
		float height;
		Vector3 noY = new Vector3(point.x - chunkSize / 2, 0, point.z - chunkSize / 2);

		if (noY.magnitude < FlatspawnRadius)
			height = SNG.coherentNoise(new Vector3(0, 10000, 0), octaves, multiplier * 10, dump, sharpness, persistance) - negativedump;
		else
			height = SNG.coherentNoise(new Vector3(point.x, 10000, point.z), octaves, multiplier * 10, dump, sharpness, persistance) - negativedump; // -            Mathf.Pow(noY.magnitude - Radius * chunkSize, 1f / 1.8f) * 5 ;

		/*
		if (height > mountainsHeight)
			return mountainsHeight;*/
		return height;
	}
}

[Serializable]
public class WeatherNoiseGenerator
{

	public int minTemp = 0, maxTemp = 100;

	[Tooltip("Высота, на которой температура теоретически максимальна")]
	public int maxTemperatureHeight;

	[Range(0, 10f)]
	public float changeTempDist, changeTempNorth, changeProlDist, changeProlHeight = 0.5f;

	public int maxProlHeight = 30, maxProlRadius = 20;
	public int AdditionalProlificacy = 5;
	[Space(20)]

	[Tooltip("Количество наложенных шумов. С каждой октавой частота удваивается")]
	[Range(1, 10)]
	public int octaves = 1;
	[Tooltip("Насколько большими будут биомы")]
	public int multiplier = 35;
	[Tooltip("Насколько сильно норма смещена в сторону суши")]
	[Range(0, 1f)]
	public float dump = 0f;
	[Tooltip("Насколько резкими будут переходы")]
	[Range(1, 10)]
	public float sharpness = 2;
	[Tooltip("Уровень влияния более высокочастотных шумов")]
	[Range(0f, 1f)]
	public float persistance = 0.9f;

	SimplexNoiseGenerator SNGtemperature;
	SimplexNoiseGenerator SNGprolificacy;


	public void SetSeed(string seed)
	{
		SNGtemperature = new SimplexNoiseGenerator(seed);
		SNGprolificacy = new SimplexNoiseGenerator(seed + "a");
	}

	public int GetTemperature(Vector3 point)
	{
		float degrees; // от 0 до 1  

		degrees = SNGtemperature.coherentNoise(point, octaves, multiplier, dump, sharpness, persistance, true);

		degrees -= Mathf.Abs(maxTemperatureHeight - point.y) * changeTempDist;
		degrees -= point.x * changeTempNorth;
		if (degrees < 0)
			degrees = 0;
		degrees /= 100;

		return minTemp + Mathf.FloorToInt(degrees * (maxTemp - minTemp));
	}

	//Плодородность: 50 - пустырь. < 40 Камни ; > 60 Деревья
	public int GetProlificacy(Vector3 point, float height)
	{
		float percent; // от 0 до 100  

		percent = SNGprolificacy.coherentNoise(point, octaves, multiplier, dump, sharpness, persistance, true);

		if (Mathf.Abs(maxProlHeight - point.y) < maxProlRadius)
			percent += (maxProlRadius - Mathf.Abs(maxProlHeight - point.y)) * changeProlDist;
		float undergroundeight = height - point.y;

		percent += AdditionalProlificacy; 
		percent -= undergroundeight * changeProlHeight;

		if (percent < 0)
			percent = 0;
		if (percent > 100)
			percent = 100;

		//percent -= Mathf.Abs(maxHumidityHeight2 - point.y) * changeHumDist * 2;

		return Mathf.Clamp(Mathf.FloorToInt(percent), 0, 100);
	}
}