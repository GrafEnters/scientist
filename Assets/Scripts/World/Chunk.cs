using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
	public int chunkSize;
	public int polygonSize;
	public Vector3 coord;
	public Biome biome;
	public float height;
	public LayerMask IgnoreLayers;

	MeshRenderer meshRenderer;
	MeshFilter meshFilter;
	MeshCollider meshCollider;
	Vector3[] vertices;
	int[] triangles;


	[HideInInspector]
	public int[,,] nodes, tempNodes, prolNodes;
	/**********/

	public void Init()
	{
		meshFilter = GetComponent<MeshFilter>();
		meshCollider = GetComponent<MeshCollider>();
		meshRenderer = GetComponent<MeshRenderer>();
		var block = new MaterialPropertyBlock();
		block.SetColor("_BaseColor", biome.color);
		meshRenderer.SetPropertyBlock(block);

		CreateMesh();
		if(WorldGenerator.SIsSpawningPlants)
			GrowPlants();

		meshRenderer.enabled = true;
	}

	void CreateMesh()
	{
		MeshGenerator meshGenerator = new MeshGenerator();
		Mesh mesh = meshGenerator.GenerateMesh(nodes, polygonSize);
		

		RemakeMeshToDiscrete(mesh.vertices, mesh.triangles);
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		meshFilter.mesh = mesh;
		meshCollider.sharedMesh = mesh;
	}

	void RemakeMeshToDiscrete(Vector3[] vert, int[] trig)
	{
		Vector3[] vertDiscrete = new Vector3[trig.Length];
		int[] trigDiscrete = new int[trig.Length];
		for (int i = 0; i < trig.Length; i++)
		{
			vertDiscrete[i] = vert[trig[i]];
			trigDiscrete[i] = i;
		}
		vertices = vertDiscrete;
		triangles = trigDiscrete;
	}

	void GrowPlants()
	{
		List<Vector3> centerList = new List<Vector3>();
		List<Vector3> normalsList = new List<Vector3>();
		for (int i = 0; i < triangles.Length;)
		{
			Vector3 p1, p2, p3;
			p1 = vertices[triangles[i++]];
			p2 = vertices[triangles[i++]];
			p3 = vertices[triangles[i++]];
			centerList.Add(((p1 + p2 + p3) / 3));
			normalsList.Add(CalcNormals(p1, p2, p3));

		}
		int floraValue = centerList.Count / 2;

		while (centerList.Count > 0)
		{
			int rnd = Random.Range(0, centerList.Count);
			Vector3 normal = normalsList[rnd];
			Vector3 center = centerList[rnd];

			int temperature, prolificacy;
			
			temperature = tempNodes[Mathf.FloorToInt(center.x) + chunkSize / 2, Mathf.FloorToInt(center.y) + chunkSize / 2, Mathf.FloorToInt(center.z) + chunkSize / 2];
			prolificacy = prolNodes[Mathf.FloorToInt(center.x) + chunkSize / 2, Mathf.FloorToInt(center.y) + chunkSize / 2, Mathf.FloorToInt(center.z) + chunkSize / 2];

			//Debug.Log("temp " +  temperature);
			//Debug.Log("prol " +  prolificacy);
			Vector3 worldPos = transform.position + center;
			Vector3 noY = worldPos; noY.y = 0;

			GameObject structure = ThingsGenerator.GetStructure(noY.magnitude, temperature, prolificacy, FloraType.SourceType);

			// Если очков флоры хватает и угол для структуры подходит, то ставит структуру
			if (structure && Vector3.Angle(normal, Vector3.up) < structure.GetComponent<NatureStructure>().maxAngle && floraValue >= 0)
			{
				GameObject plant = Instantiate(structure, worldPos, Quaternion.FromToRotation(Vector3.up, (normal + Vector3.up / 1.5f)).normalized, transform);

				plant.transform.Rotate(0, Random.Range(0, 180), 0);
				plant.SetActive(true);
				plant.transform.localScale *= Random.Range(0.3f, 2);

				floraValue -= plant.GetComponent<NatureStructure>().cost;

			} 
			//if (Random.Range(0, 4) > 0)
			else if (Random.Range(0, 3) > 0)
			{
				GameObject decorPref = ThingsGenerator.GetStructure(noY.magnitude, temperature, prolificacy, FloraType.DecorativeType);
				if (decorPref)
				{
					GameObject decor = Instantiate(decorPref, worldPos, Quaternion.FromToRotation(Vector3.up, normal), transform);
					decor.transform.Rotate(0, Random.Range(0, 180), 0);
					decor.SetActive(true);
					decor.transform.localScale *= Random.Range(1, 1.5f);
				}
			}

			// Убирает нормаль, чтобы два растения не росли из одной точки
			normalsList.Remove(normal);
			centerList.Remove(center);
		}

	}

	Vector3 CalcNormals(Vector3 a, Vector3 b, Vector3 c)
	{
		float wrki;
		Vector3 v1, v2, res;

		v1.x = a.x - b.x;
		v1.y = a.y - b.y;
		v1.z = a.z - b.z;

		v2.x = b.x - c.x;
		v2.y = b.y - c.y;
		v2.z = b.z - c.z;

		wrki = Mathf.Sqrt(Mathf.Pow(v1.y * v2.z - v1.z * v2.y, 2) + Mathf.Pow(v1.z * v2.x - v1.x * v2.z, 2) + Mathf.Pow(v1.x * v2.y - v1.y * v2.x, 2));
		res.x = (v1.y * v2.z - v1.z * v2.y) / wrki;
		res.y = (v1.z * v2.x - v1.x * v2.z) / wrki;
		res.z = (v1.x * v2.y - v1.y * v2.x) / wrki;
		return res;
	}

	public void DeActivate()
	{
		if (gameObject.activeSelf)
		{
			Collider[] colls = Physics.OverlapBox(transform.position, Vector3.one * chunkSize / 2, Quaternion.identity, ~IgnoreLayers);
			foreach (Collider coll in colls)
			{
				if (coll.transform.parent == null) 
					coll.transform.SetParent(transform);
			}
			gameObject.SetActive(false);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireCube(transform.position, Vector3.one * chunkSize);
	}
}