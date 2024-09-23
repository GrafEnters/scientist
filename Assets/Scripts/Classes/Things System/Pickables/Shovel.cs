using UnityEngine;

public class Shovel : Tool
{
	public override bool Use()
	{
		Dig();
		return true;
	}

	void Dig()
	{

		int x = Screen.width / 2;
		int y = Screen.height / 2;
		Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(new Vector3(x, y));
		if (Physics.Raycast(ray, out RaycastHit hit, Hold.maxOperateDistance))
		{
			GameObject ground = hit.collider.gameObject;

			// Если структура - разрушаем
			if (ground.GetComponent<Chunk>())
			{

				Mesh oldMesh = ground.GetComponent<MeshFilter>().mesh;
				Mesh mesh = new Mesh
				{
					vertices = oldMesh.vertices,
					triangles = oldMesh.triangles
				};



				Vector3[] mVertices = mesh.vertices;
				for (int i = 0; i < mVertices.Length; i++)
				{
					float distance = Vector3.Distance(mVertices[i] + ground.transform.position, hit.point);
					if (distance < 2)
					{
						mVertices[i].y -= 2 - distance;
					}
				}
				mesh.vertices = mVertices;
				mesh.RecalculateNormals();
				ground.GetComponent<MeshFilter>().mesh = mesh;
				ground.GetComponent<MeshCollider>().sharedMesh = mesh;
				return;
			}
		}
	}
}
