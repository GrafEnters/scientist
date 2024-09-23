using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
	public CubeGrid cubeGrid;
	public MeshFilter walls;
	List<Vector3> vertices;
	List<int> triangles;

	public Mesh GenerateMesh(int[,,] map, int squareSize)
	{

		cubeGrid = new CubeGrid(map, squareSize);
		vertices = new List<Vector3>();
		triangles = new List<int>();

		for (int x = 0; x < cubeGrid.cubes.GetLength(0); x++)
		{
			for (int y = 0; y < cubeGrid.cubes.GetLength(1); y++)
			{
				for (int z = 0; z < cubeGrid.cubes.GetLength(2); z++)
				{
					TriangulateCube(cubeGrid.cubes[x, y, z]);
				}
			}
		}


		// Так создаётся меш. Ему нужно передать вершины и стороны. + В конце мы его просим пересчитать, чтобы не было обратных направленных нормалей.
		Mesh mesh = new Mesh();
		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
		return mesh;
	}


	public void MeshFromNodes(Cube cube, params int[] nodes)
	{
		for (int i = 0; i < nodes.Length; i += 3)
		{
			CreateTriangle(cube.n[nodes[i]], cube.n[nodes[i + 1]], cube.n[nodes[i + 2]]);
		}
	}

	// Список треугольников - массив int, который пачками по три соответствует индексу вершин
	void CreateTriangle(Node a, Node b, Node c)
	{
		AssignVertices(a, b, c);
		triangles.Add(a.vertexIndex);
		triangles.Add(b.vertexIndex);
		triangles.Add(c.vertexIndex);
	}

	// Каждую вершину мы нумеруем и добавляем в общий список
	void AssignVertices(params Node[] nodes)
	{
		for (int i = 0; i < nodes.Length; i++)
		{
			if (nodes[i].vertexIndex == -1)
			{
				nodes[i].vertexIndex = vertices.Count;
				vertices.Add(nodes[i].position);
			}
		}
	}

	struct Triangle
	{
		public int vertexIndexA;
		public int vertexIndexB;
		public int vertexIndexC;

		int[] vertices;
		public Triangle(int a, int b, int c)
		{
			vertexIndexA = a;
			vertexIndexB = b;
			vertexIndexC = c;
			vertices = new int[3];
			vertices[0] = a;
			vertices[1] = b;
			vertices[2] = c;
		}

		public int this[int i]
		{
			get
			{
				return vertices[i];
			}
		}

		public bool Contains(int vertexIndex)
		{
			return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
		}
	}

	public class CubeGrid
	{
		public Cube[,,] cubes;

		public CubeGrid(int[,,] map, int squareSize)
		{
			int nodeCountX = map.GetLength(0);
			int nodeCountY = map.GetLength(1);
			int nodeCountZ = map.GetLength(2);

			int mapSize = nodeCountX;

			// Создаём сетку узлов
			ControlNode[,,] controlNodes = new ControlNode[nodeCountX, nodeCountY, nodeCountZ];
			for (int x = 0; x < nodeCountX; x++)
			{
				for (int y = 0; y < nodeCountY; y++)
				{
					for (int z = 0; z < nodeCountZ; z++)
					{
						Vector3 position = Vector3.one / 2f - Vector3.one * nodeCountX / 2 + new Vector3(x, y, z);
						controlNodes[x, y, z] = new ControlNode(position, map[x, y, z] == 1, squareSize);
					}
				}
			}


			// Создаём сетку кубов
			cubes = new Cube[(mapSize - 1) / squareSize, (mapSize - 1) / squareSize, (mapSize - 1) / squareSize];
			for (int x = 0; x < cubes.GetLength(0); x++)
			{
				for (int y = 0; y < cubes.GetLength(1); y++)
				{
					for (int z = 0; z < cubes.GetLength(2); z++)
					{

						cubes[x, y, z] = new Cube(
							controlNodes[x * squareSize, y * squareSize, z * squareSize], controlNodes[x * squareSize, y * squareSize, (z + 1) * squareSize],
							controlNodes[(x + 1) * squareSize, y * squareSize, (z + 1) * squareSize], controlNodes[(x + 1) * squareSize, y * squareSize, z * squareSize],
							controlNodes[x * squareSize, (y + 1) * squareSize, z * squareSize], controlNodes[x * squareSize, (y + 1) * squareSize, (z + 1) * squareSize],
							controlNodes[(x + 1) * squareSize, (y + 1) * squareSize, (z + 1) * squareSize], controlNodes[(x + 1) * squareSize, (y + 1) * squareSize, z * squareSize]
							);
					}
				}
			}
		}
	}

	public class Cube
	{
		public ControlNode[] c; // ControlNodes
		public Node[] n; // Nodes

		// Для каждого сочетания вершин куба есть свой набор треугольников, которые нужно отрисовать. Сочетаний всего 16.
		public int configuration;

		public Cube
			(
			ControlNode c0, ControlNode c1, ControlNode c2, ControlNode c3,
			ControlNode c4, ControlNode c5, ControlNode c6, ControlNode c7
			)
		{

			c = new ControlNode[8] { c0, c1, c2, c3, c4, c5, c6, c7 };
			n = new Node[12] {
				c0.depth,c1.right,c3.depth,c0.right,
				c4.depth,c5.right,c7.depth,c4.right,
				c0.above,c1.above,c2.above,c3.above,
			};

			// Считаем правильную конфигурацию исходя из активированных контрольных узлов
			for (int i = 0; i < 8; i++)
			{
				if (c[i].isActive)
					configuration += (int)Mathf.Pow(2, i);
			}
		}
	}

	public class Node
	{
		public Vector3 position;
		public int vertexIndex = -1;

		public Node(Vector3 _pos)
		{
			position = _pos;
		}
	}

	public class ControlNode : Node
	{
		public bool isActive;
		public Node above, right, depth;

		public ControlNode(Vector3 _pos, bool _isActive, float squareSize) : base(_pos)
		{
			isActive = _isActive;
			above = new Node(position + Vector3.up * squareSize / 2f);
			right = new Node(position + Vector3.right * squareSize / 2f);
			depth = new Node(position + Vector3.forward * squareSize / 2f);
		}
		public Node GetNode(ControlNode second)
		{
			Vector3 position = Vector3.one;
			return new Node(position);
		}
	}

	void TriangulateCube(Cube cube)
	{
		// Для каждого сочетания говорим правильный порядок точек в отрисовке треугольника
		switch (cube.configuration)
		{
			// 0 Active ControlNodes:
			case 0:
				break;

			// 1 Active ControlNodes:
			case 1:
				MeshFromNodes(cube, 0, 3, 8);
				break;
			case 2:
				MeshFromNodes(cube, 1, 0, 9);
				break;
			case 4:
				MeshFromNodes(cube, 2, 1, 10);
				break;
			case 8:
				MeshFromNodes(cube, 3, 2, 11);
				break;
			case 16:
				MeshFromNodes(cube, 4, 8, 7);
				break;
			case 32:
				MeshFromNodes(cube, 5, 9, 4);
				break;
			case 64:
				MeshFromNodes(cube, 6, 10, 5);
				break;
			case 128:
				MeshFromNodes(cube, 7, 11, 6);
				break;

			// 2 Active ControlNodes:
			case 3:
				MeshFromNodes(cube, 3, 8, 9, 3, 9, 1);
				break;
			case 5:
				MeshFromNodes(cube, 3, 8, 0, 1, 10, 2);
				break;
			case 6:
				MeshFromNodes(cube, 0, 9, 2, 9, 10, 2);
				break;
			case 9:
				MeshFromNodes(cube, 11, 8, 0, 11, 0, 2);
				break;
			case 10:
				MeshFromNodes(cube, 0, 9, 1, 2, 11, 3);
				break;
			case 12:
				MeshFromNodes(cube, 1, 10, 11, 1, 11, 3);
				break;
			case 17:
				MeshFromNodes(cube, 3, 7, 4, 3, 4, 0);
				break;
			case 18:
				MeshFromNodes(cube, 8, 7, 4, 0, 9, 1);
				break;
			case 20:
				MeshFromNodes(cube, 1, 10, 2, 8, 7, 4);
				break;
			case 24:
				MeshFromNodes(cube, 2, 11, 3, 8, 7, 4);
				break;
			case 33:
				MeshFromNodes(cube, 3, 8, 0, 9, 4, 5);
				break;
			case 34:
				MeshFromNodes(cube, 0, 4, 5, 0, 5, 1);
				break;
			case 36:
				MeshFromNodes(cube, 4, 5, 9, 1, 10, 2);
				break;
			case 40:
				MeshFromNodes(cube, 4, 5, 9, 2, 11, 3);
				break;
			case 48:
				MeshFromNodes(cube, 8, 7, 5, 8, 5, 9);
				break;
			case 65:
				MeshFromNodes(cube, 3, 8, 0, 5, 6, 10);
				break;
			case 66:
				MeshFromNodes(cube, 0, 9, 1, 5, 6, 10);
				break;
			case 68:
				MeshFromNodes(cube, 1, 5, 6, 1, 6, 2);
				break;
			case 72:
				MeshFromNodes(cube, 2, 11, 3, 5, 6, 10);
				break;
			case 80:
				MeshFromNodes(cube, 7, 4, 8, 5, 6, 10);
				break;
			case 96:
				MeshFromNodes(cube, 9, 4, 6, 9, 6, 10);
				break;
			case 129:
				MeshFromNodes(cube, 3, 8, 0, 6, 7, 11);
				break;
			case 130:
				MeshFromNodes(cube, 0, 9, 1, 6, 7, 11);
				break;
			case 132:
				MeshFromNodes(cube, 1, 10, 2, 6, 7, 11);
				break;
			case 136:
				MeshFromNodes(cube, 6, 7, 3, 6, 3, 2);
				break;
			case 144:
				MeshFromNodes(cube, 8, 11, 6, 8, 6, 4);
				break;
			case 160:
				MeshFromNodes(cube, 4, 5, 9, 6, 7, 11);
				break;
			case 192:
				MeshFromNodes(cube, 5, 7, 11, 5, 11, 10);
				break;

			// 3 Active ControlNodes:
			case 7:
				MeshFromNodes(cube, 8, 9, 10, 8, 10, 3, 3, 10, 2);
				break;
			case 11:
				MeshFromNodes(cube, 8, 9, 11, 11, 9, 2, 2, 9, 1);
				break;
			case 13:
				MeshFromNodes(cube, 8, 10, 11, 0, 1, 10, 0, 10, 8);
				break;
			case 14:
				MeshFromNodes(cube, 0, 9, 11, 0, 11, 3, 9, 10, 11);
				break;
			case 19:
				MeshFromNodes(cube, 3, 7, 1, 7, 4, 1, 1, 4, 9);
				break;
			case 21:
				MeshFromNodes(cube, 0, 3, 7, 0, 7, 4, 1, 10, 2);
				break;
			case 22:
				MeshFromNodes(cube, 0, 9, 10, 0, 10, 2, 8, 7, 4);
				break;
			case 25:
				MeshFromNodes(cube, 0, 2, 4, 7, 4, 11, 11, 4, 2);
				break;
			case 26:
				MeshFromNodes(cube, 8, 7, 4, 0, 9, 1, 2, 11, 3);
				break;
			case 28:
				MeshFromNodes(cube, 1, 10, 11, 1, 11, 3, 8, 7, 4);
				break;
			case 35:
				MeshFromNodes(cube, 8, 4, 3, 4, 5, 3, 3, 5, 1);
				break;
			case 37:
				MeshFromNodes(cube, 3, 8, 0, 4, 5, 9, 1, 10, 2);
				break;
			case 38:
				MeshFromNodes(cube, 0, 4, 2, 2, 4, 5, 2, 5, 10);
				break;
			case 41:
				MeshFromNodes(cube, 4, 5, 9, 0, 2, 11, 0, 11, 8);
				break;
			case 42:
				MeshFromNodes(cube, 0, 4, 1, 1, 4, 5, 2, 11, 3);
				break;
			case 44:
				MeshFromNodes(cube, 4, 5, 9, 11, 3, 1, 11, 1, 10);
				break;
			case 49:
				MeshFromNodes(cube, 3, 7, 5, 3, 5, 9, 3, 9, 0);
				break;
			case 50:
				MeshFromNodes(cube, 7, 5, 1, 7, 1, 0, 7, 0, 8);
				break;
			case 52:
				MeshFromNodes(cube, 7, 5, 9, 7, 9, 8, 1, 10, 2);
				break;
			case 56:
				MeshFromNodes(cube, 7, 5, 9, 7, 9, 8, 11, 3, 2);
				break;
			case 67:
				MeshFromNodes(cube, 3, 8, 9, 3, 9, 1, 5, 6, 10);
				break;
			case 69:
				MeshFromNodes(cube, 3, 8, 0, 1, 5, 6, 1, 6, 2);
				break;
			case 70:
				MeshFromNodes(cube, 0, 9, 5, 0, 5, 6, 0, 6, 2);
				break;
			case 73:
				MeshFromNodes(cube, 0, 2, 11, 0, 11, 8, 5, 6, 10);
				break;
			case 74:
				MeshFromNodes(cube, 0, 9, 1, 5, 6, 10, 3, 2, 11);
				break;
			case 76:
				MeshFromNodes(cube, 5, 6, 11, 5, 11, 3, 5, 3, 1);
				break;
			case 81:
				MeshFromNodes(cube, 0, 3, 7, 0, 7, 4, 5, 6, 10);
				break;
			case 82:
				MeshFromNodes(cube, 4, 8, 7, 0, 9, 1, 5, 6, 10);
				break;
			case 84:
				MeshFromNodes(cube, 1, 5, 6, 1, 6, 2, 4, 8, 7);
				break;
			case 88:
				MeshFromNodes(cube, 4, 8, 7, 5, 6, 10, 3, 2, 11);
				break;
			case 97:
				MeshFromNodes(cube, 8, 0, 3, 4, 6, 9, 9, 6, 10);
				break;
			case 98:
				MeshFromNodes(cube, 0, 4, 6, 0, 6, 10, 0, 10, 1);
				break;
			case 100:
				MeshFromNodes(cube, 4, 6, 2, 4, 2, 1, 4, 1, 9);
				break;
			case 104:
				MeshFromNodes(cube, 9, 4, 6, 9, 6, 10, 3, 2, 11);
				break;
			case 112:
				MeshFromNodes(cube, 8, 7, 6, 8, 6, 10, 8, 10, 9);
				break;
			case 131:
				MeshFromNodes(cube, 7, 11, 6, 8, 9, 1, 8, 1, 3);
				break;
			case 133:
				MeshFromNodes(cube, 8, 0, 3, 1, 10, 2, 7, 11, 6);
				break;
			case 134:
				MeshFromNodes(cube, 7, 11, 6, 0, 9, 10, 0, 10, 2);
				break;
			case 137:
				MeshFromNodes(cube, 0, 2, 6, 0, 6, 7, 0, 7, 8);
				break;
			case 138:
				MeshFromNodes(cube, 0, 9, 1, 3, 2, 6, 3, 6, 7);
				break;
			case 140:
				MeshFromNodes(cube, 1, 10, 6, 1, 6, 7, 1, 7, 3);
				break;
			case 145:
				MeshFromNodes(cube, 0, 3, 11, 0, 11, 6, 0, 6, 4);
				break;
			case 146:
				MeshFromNodes(cube, 0, 9, 1, 8, 11, 6, 8, 6, 4);
				break;
			case 148:
				MeshFromNodes(cube, 1, 10, 2, 8, 11, 6, 8, 6, 4);
				break;
			case 152:
				MeshFromNodes(cube, 4, 8, 3, 4, 3, 2, 4, 2, 6);
				break;
			case 161:
				MeshFromNodes(cube, 8, 0, 3, 4, 5, 9, 7, 11, 6);
				break;
			case 162:
				MeshFromNodes(cube, 11, 6, 7, 4, 5, 1, 4, 1, 0);
				break;
			case 164:
				MeshFromNodes(cube, 4, 5, 9, 7, 11, 6, 1, 10, 2);
				break;
			case 168:
				MeshFromNodes(cube, 4, 5, 9, 3, 2, 6, 3, 6, 7);
				break;
			case 176:
				MeshFromNodes(cube, 11, 6, 5, 11, 5, 9, 11, 9, 8);
				break;
			case 193:
				MeshFromNodes(cube, 8, 0, 3, 11, 10, 5, 11, 5, 7);
				break;
			case 194:
				MeshFromNodes(cube, 9, 1, 0, 11, 10, 5, 11, 5, 7);
				break;
			case 196:
				MeshFromNodes(cube, 1, 5, 7, 1, 7, 11, 1, 11, 2);
				break;
			case 200:
				MeshFromNodes(cube, 3, 2, 10, 3, 10, 5, 3, 5, 7);
				break;
			case 208:
				MeshFromNodes(cube, 8, 11, 10, 8, 10, 5, 8, 5, 4);
				break;
			case 224:
				MeshFromNodes(cube, 11, 10, 9, 11, 9, 4, 11, 4, 7);
				break;


			// 4 Active ControlNodes:
			case 15:
				MeshFromNodes(cube, 8, 9, 10, 8, 10, 11);
				break;
			case 23:
				MeshFromNodes(cube, 9, 7, 4, 9, 10, 2, 9, 2, 7, 2, 3, 7);
				break;
			case 27:
				MeshFromNodes(cube, 7, 4, 9, 7, 9, 1, 7, 1, 11, 11, 1, 2);
				break;
			case 29:
				MeshFromNodes(cube, 4, 1, 11, 4, 11, 7, 4, 0, 1, 11, 1, 10);
				break;
			case 30:
				MeshFromNodes(cube, 4, 8, 7, 9, 10, 11, 9, 11, 3, 9, 3, 0);
				break;
			case 39:
				MeshFromNodes(cube, 3, 8, 4, 3, 4, 5, 3, 5, 2, 2, 5, 10);
				break;
			case 43:
				MeshFromNodes(cube, 5, 8, 4, 5, 2, 8, 1, 2, 5, 8, 2, 11);
				break;
			case 45:
				MeshFromNodes(cube, 4, 5, 9, 0, 1, 10, 0, 10, 8, 8, 10, 11);
				break;
			case 46:
				MeshFromNodes(cube, 4, 5, 10, 4, 10, 3, 4, 3, 0, 3, 10, 11);
				break;
			case 51:
				MeshFromNodes(cube, 3, 7, 5, 3, 5, 1);
				break;
			case 53:
				MeshFromNodes(cube, 1, 10, 2, 7, 5, 3, 0, 3, 9, 3, 5, 9);
				break;
			case 54:
				MeshFromNodes(cube, 7, 5, 10, 7, 10, 2, 7, 2, 0, 7, 0, 8);
				break;
			case 57:
				MeshFromNodes(cube, 2, 7, 9, 2, 11, 7, 2, 9, 0, 7, 5, 9);
				break;
			case 58:
				MeshFromNodes(cube, 7, 5, 1, 7, 1, 0, 7, 0, 8, 11, 3, 2);
				break;
			case 60:
				MeshFromNodes(cube, 7, 5, 9, 7, 9, 8, 11, 3, 1, 11, 1, 10);
				break;
			case 71:
				MeshFromNodes(cube, 8, 5, 2, 8, 2, 3, 8, 9, 5, 2, 5, 6);
				break;
			case 75:
				MeshFromNodes(cube, 5, 6, 10, 9, 1, 2, 9, 2, 11, 9, 11, 8);
				break;
			case 77:
				MeshFromNodes(cube, 5, 11, 0, 11, 5, 6, 1, 5, 0, 0, 11, 8);
				break;
			case 78:
				MeshFromNodes(cube, 5, 0, 9, 5, 3, 0, 5, 6, 3, 6, 11, 3);
				break;
			case 83:
				MeshFromNodes(cube, 5, 6, 10, 7, 4, 9, 7, 9, 1, 7, 1, 3);
				break;
			case 85:
				MeshFromNodes(cube, 0, 3, 7, 0, 7, 4, 1, 5, 6, 1, 6, 2);
				break;
			case 86:
				MeshFromNodes(cube, 0, 9, 5, 0, 5, 6, 0, 6, 2, 4, 8, 7);
				break;
			case 89:
				MeshFromNodes(cube, 5, 6, 10, 2, 11, 7, 2, 7, 4, 2, 4, 0);
				break;
			case 90:
				MeshFromNodes(cube, 4, 8, 7, 5, 6, 10, 3, 2, 11, 0, 9, 1);
				break;
			case 92:
				MeshFromNodes(cube, 4, 8, 7, 3, 1, 5, 3, 5, 11, 11, 5, 6);
				break;
			case 99:
				MeshFromNodes(cube, 3, 4, 10, 3, 8, 4, 3, 10, 1, 4, 6, 10);
				break;
			case 101:
				MeshFromNodes(cube, 8, 0, 3, 4, 6, 2, 4, 2, 1, 4, 1, 9);
				break;
			case 102:
				MeshFromNodes(cube, 4, 6, 2, 4, 2, 0);
				break;
			case 105:
				MeshFromNodes(cube, 4, 6, 9, 6, 10, 9, 0, 2, 11, 0, 11, 8);
				break;
			case 106:
				MeshFromNodes(cube, 3, 2, 11, 0, 4, 6, 0, 6, 10, 0, 10, 1);
				break;
			case 108:
				MeshFromNodes(cube, 3, 9, 6, 3, 6, 11, 3, 1, 9, 6, 9, 4);
				break;
			case 113:
				MeshFromNodes(cube, 10, 0, 7, 10, 7, 6, 10, 9, 0, 7, 0, 3);
				break;
			case 114:
				MeshFromNodes(cube, 0, 8, 7, 0, 7, 6, 0, 6, 1, 1, 6, 10);
				break;
			case 116:
				MeshFromNodes(cube, 2, 9, 7, 2, 1, 9, 2, 7, 6, 9, 8, 7);
				break;
			case 120:
				MeshFromNodes(cube, 8, 7, 6, 8, 6, 10, 8, 10, 9, 3, 2, 11);
				break;
			case 135:
				MeshFromNodes(cube, 8, 9, 10, 8, 10, 2, 8, 2, 3, 7, 11, 6);
				break;
			case 139:
				MeshFromNodes(cube, 6, 8, 1, 6, 7, 8, 6, 1, 2, 8, 9, 1);
				break;
			case 141:
				MeshFromNodes(cube, 0, 7, 8, 0, 6, 7, 0, 1, 6, 1, 10, 6);
				break;
			case 142:
				MeshFromNodes(cube, 7, 0, 10, 7, 10, 6, 7, 3, 0, 10, 0, 9);
				break;
			case 147:
				MeshFromNodes(cube, 6, 9, 3, 6, 3, 11, 6, 4, 9, 3, 9, 1);
				break;
			case 149:
				MeshFromNodes(cube, 1, 10, 2, 0, 3, 11, 0, 11, 6, 0, 6, 4);
				break;
			case 150:
				MeshFromNodes(cube, 4, 8, 11, 4, 11, 6, 2, 0, 9, 2, 9, 10);
				break;
			case 153:
				MeshFromNodes(cube, 4, 0, 2, 4, 2, 6);
				break;
			case 154:
				MeshFromNodes(cube, 0, 9, 1, 4, 8, 3, 4, 3, 2, 4, 2, 6);
				break;
			case 156:
				MeshFromNodes(cube, 1, 6, 8, 1, 10, 6, 1, 8, 3, 6, 4, 8);
				break;
			case 163:
				MeshFromNodes(cube, 7, 11, 6, 3, 8, 4, 3, 4, 5, 3, 5, 1);
				break;
			case 165:
				MeshFromNodes(cube, 4, 5, 9, 7, 11, 6, 1, 10, 2, 8, 0, 3);
				break;
			case 166:
				MeshFromNodes(cube, 7, 11, 6, 4, 5, 10, 4, 10, 2, 4, 2, 0);
				break;
			case 169:
				MeshFromNodes(cube, 4, 5, 9, 0, 2, 6, 0, 6, 7, 0, 7, 8);
				break;
			case 170:
				MeshFromNodes(cube, 4, 5, 1, 4, 1, 0, 7, 3, 2, 7, 2, 6);
				break;
			case 172:
				MeshFromNodes(cube, 4, 5, 9, 1, 10, 6, 1, 6, 7, 1, 7, 3);
				break;
			case 177:
				MeshFromNodes(cube, 3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9);
				break;
			case 178:
				MeshFromNodes(cube, 0, 11, 5, 5, 11, 6, 8, 11, 0, 0, 5, 1);
				break;
			case 180:
				MeshFromNodes(cube, 1, 10, 2, 9, 8, 11, 9, 11, 6, 9, 6, 5);
				break;
			case 184:
				MeshFromNodes(cube, 2, 5, 8, 2, 8, 3, 2, 6, 5, 8, 5, 9);
				break;
			case 195:
				MeshFromNodes(cube, 8, 9, 1, 8, 1, 3, 7, 11, 5, 11, 10, 5);
				break;
			case 197:
				MeshFromNodes(cube, 7, 11, 2, 7, 2, 1, 7, 1, 5, 8, 0, 3);
				break;
			case 198:
				MeshFromNodes(cube, 11, 0, 5, 5, 0, 9, 2, 0, 11, 11, 5, 7);
				break;
			case 201:
				MeshFromNodes(cube, 0, 10, 7, 0, 7, 8, 0, 2, 10, 7, 10, 5);
				break;
			case 202:
				MeshFromNodes(cube, 0, 9, 1, 3, 2, 10, 3, 10, 5, 3, 5, 7);
				break;
			case 204:
				MeshFromNodes(cube, 7, 3, 1, 7, 1, 5);
				break;
			case 209:
				MeshFromNodes(cube, 10, 4, 3, 10, 5, 4, 10, 3, 11, 4, 0, 3);
				break;
			case 210:
				MeshFromNodes(cube, 0, 9, 1, 8, 11, 10, 8, 10, 5, 8, 5, 4);
				break;
			case 212:
				MeshFromNodes(cube, 1, 4, 11, 1, 11, 2, 1, 5, 4, 11, 4, 8);
				break;
			case 216:
				MeshFromNodes(cube, 4, 10, 5, 8, 3, 2, 8, 2, 10, 8, 10, 4);
				break;
			case 225:
				MeshFromNodes(cube, 8, 0, 3, 9, 4, 7, 9, 7, 11, 9, 11, 10);
				break;
			case 226:
				MeshFromNodes(cube, 0, 7, 10, 0, 10, 1, 0, 4, 7, 10, 7, 11);
				break;
			case 228:
				MeshFromNodes(cube, 7, 11, 2, 7, 2, 1, 7, 1, 4, 4, 1, 9);
				break;
			case 232:
				MeshFromNodes(cube, 9, 7, 2, 9, 4, 7, 9, 2, 10, 7, 3, 2);
				break;
			case 240:
				MeshFromNodes(cube, 9, 8, 11, 9, 11, 10);
				break;


			// 5 Active ControlNodes:
			case 31:
				MeshFromNodes(cube, 9, 7, 4, 9, 11, 7, 9, 10, 11);
				break;
			case 47:
				MeshFromNodes(cube, 8, 10, 11, 8, 5, 10, 8, 4, 5);
				break;
			case 55:
				MeshFromNodes(cube, 3, 7, 5, 3, 5, 10, 3, 10, 2);
				break;
			case 59:
				MeshFromNodes(cube, 7, 5, 1, 7, 1, 2, 7, 2, 11);
				break;
			case 61:
				MeshFromNodes(cube, 0, 5, 9, 0, 7, 5, 0, 1, 10, 0, 10, 11, 0, 11, 7);
				break;
			case 62:
				MeshFromNodes(cube, 0, 8, 7, 0, 7, 5, 0, 5, 10, 0, 10, 11, 0, 11, 3);
				break;
			case 79:
				MeshFromNodes(cube, 11, 8, 9, 11, 9, 5, 11, 5, 6);
				break;
			case 87:
				MeshFromNodes(cube, 9, 5, 6, 9, 6, 2, 9, 2, 3, 9, 3, 7, 9, 7, 4);
				break;
			case 91:
				MeshFromNodes(cube, 5, 6, 10, 4, 9, 1, 4, 1, 2, 4, 2, 7, 7, 2, 11);
				break;
			case 93:
				MeshFromNodes(cube, 11, 7, 4, 11, 4, 0, 11, 0, 1, 11, 1, 5, 11, 5, 6);
				break;
			case 94:
				MeshFromNodes(cube, 4, 8, 7, 0, 9, 5, 0, 5, 6, 0, 6, 3, 3, 6, 11);
				break;
			case 103:
				MeshFromNodes(cube, 3, 8, 4, 3, 4, 6, 3, 6, 2);
				break;
			case 107:
				MeshFromNodes(cube, 1, 8, 4, 1, 4, 6, 1, 6, 10, 1, 2, 11, 1, 11, 8);
				break;
			case 109:
				MeshFromNodes(cube, 1, 8, 0, 1, 11, 8, 1, 6, 11, 1, 4, 6, 1, 9, 4);
				break;
			case 110:
				MeshFromNodes(cube, 0, 4, 6, 0, 6, 11, 0, 11, 3);
				break;
			case 115:
				MeshFromNodes(cube, 1, 3, 7, 1, 7, 6, 1, 6, 10);
				break;
			case 117:
				MeshFromNodes(cube, 9, 0, 3, 9, 3, 7, 9, 7, 6, 9, 6, 2, 9, 2, 1);
				break;
			case 118:
				MeshFromNodes(cube, 0, 8, 7, 0, 7, 6, 0, 6, 2);
				break;
			case 121:
				MeshFromNodes(cube, 7, 6, 10, 7, 10, 9, 7, 9, 0, 7, 0, 2, 7, 2, 11);
				break;
			case 122:
				MeshFromNodes(cube, 0, 8, 7, 0, 7, 6, 0, 6, 1, 1, 6, 10, 11, 3, 2);
				break;
			case 124:
				MeshFromNodes(cube, 6, 11, 3, 6, 3, 1, 6, 1, 9, 6, 9, 8, 6, 8, 7);
				break;
			case 143:
				MeshFromNodes(cube, 8, 9, 10, 8, 10, 6, 8, 6, 7);
				break;
			case 151:
				MeshFromNodes(cube, 3, 11, 6, 3, 6, 4, 3, 4, 9, 3, 9, 10, 3, 10, 2);
				break;
			case 155:
				MeshFromNodes(cube, 2, 6, 4, 2, 4, 9, 2, 9, 1);
				break;
			case 157:
				MeshFromNodes(cube, 0, 1, 10, 0, 10, 6, 0, 6, 4);
				break;
			case 158:
				MeshFromNodes(cube, 3, 4, 8, 3, 6, 4, 3, 10, 6, 3, 9, 10, 3, 0, 9);
				break;
			case 167:
				MeshFromNodes(cube, 3, 11, 2, 4, 7, 8, 6, 5, 10);
				break;
			case 171:
				MeshFromNodes(cube, 8, 4, 5, 8, 5, 1, 8, 1, 2, 8, 2, 6, 8, 6, 7);
				break;
			case 173:
				MeshFromNodes(cube, 4, 5, 9, 0, 1, 6, 0, 6, 7, 0, 7, 8, 1, 10, 6);
				break;
			case 174:
				MeshFromNodes(cube, 10, 4, 5, 10, 0, 4, 10, 3, 0, 10, 7, 3, 10, 6, 7);
				break;
			case 179:
				MeshFromNodes(cube, 3, 11, 6, 3, 6, 5, 3, 5, 1);
				break;
			case 181:
				MeshFromNodes(cube, 1, 10, 2, 5, 9, 0, 5, 0, 3, 5, 3, 6, 6, 3, 11);
				break;
			case 182:
				MeshFromNodes(cube, 5, 10, 2, 5, 2, 0, 5, 0, 8, 5, 8, 11, 5, 11, 6);
				break;
			case 185:
				MeshFromNodes(cube, 0, 2, 6, 0, 6, 5, 0, 5, 9);
				break;
			case 186:
				MeshFromNodes(cube, 8, 3, 2, 8, 2, 6, 8, 6, 5, 8, 5, 1, 8, 1, 0);
				break;
			case 188:
				MeshFromNodes(cube, 6, 5, 9, 6, 9, 8, 6, 8, 3, 6, 3, 1, 6, 1, 10);
				break;
			case 199:
				MeshFromNodes(cube, 2, 3, 8, 2, 8, 9, 2, 9, 5, 2, 5, 7, 2, 7, 11);
				break;
			case 203:
				MeshFromNodes(cube, 2, 10, 5, 2, 5, 7, 2, 7, 8, 2, 8, 9, 2, 9, 1);
				break;
			case 205:
				MeshFromNodes(cube, 7, 8, 0, 7, 0, 1, 7, 1, 5);
				break;
			case 206:
				MeshFromNodes(cube, 0, 9, 5, 0, 5, 3, 3, 5, 7);
				break;
			case 211:
				MeshFromNodes(cube, 4, 9, 1, 4, 1, 3, 4, 3, 11, 4, 11, 10, 4, 10, 5);
				break;
			case 213:
				MeshFromNodes(cube, 11, 2, 1, 11, 1, 5, 11, 5, 4, 11, 4, 0, 11, 0, 3);
				break;
			case 214:
				MeshFromNodes(cube, 5, 4, 8, 5, 8, 11, 5, 11, 2, 5, 2, 0, 5, 0, 9);
				break;
			case 217:
				MeshFromNodes(cube, 2, 10, 5, 2, 5, 4, 2, 4, 0);
				break;
			case 218:
				MeshFromNodes(cube, 0, 9, 1, 2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8);
				break;
			case 220:
				MeshFromNodes(cube, 3, 1, 5, 3, 5, 4, 3, 4, 8);
				break;
			case 227:
				MeshFromNodes(cube, 4, 7, 11, 4, 11, 10, 4, 10, 1, 4, 1, 3, 4, 3, 8);
				break;
			case 229:
				MeshFromNodes(cube, 8, 0, 3, 4, 1, 9, 4, 2, 1, 4, 7, 2, 7, 11, 2);
				break;
			case 230:
				MeshFromNodes(cube, 2, 0, 4, 2, 4, 7, 2, 7, 11);
				break;
			case 233:
				MeshFromNodes(cube, 7, 8, 0, 7, 0, 2, 7, 2, 10, 7, 10, 9, 7, 9, 4);
				break;
			case 234:
				MeshFromNodes(cube, 10, 1, 0, 10, 0, 4, 10, 4, 7, 10, 7, 3, 10, 3, 2);
				break;
			case 236:
				MeshFromNodes(cube, 1, 9, 4, 1, 4, 7, 1, 7, 3);
				break;
			case 241:
				MeshFromNodes(cube, 9, 0, 3, 9, 3, 11, 9, 11, 10);
				break;
			case 242:
				MeshFromNodes(cube, 8, 11, 10, 8, 10, 1, 8, 1, 0);
				break;
			case 244:
				MeshFromNodes(cube, 11, 2, 1, 11, 1, 9, 11, 9, 8);
				break;
			case 248:
				MeshFromNodes(cube, 8, 3, 2, 8, 2, 10, 8, 10, 9);
				break;

			// 6 Active ControlNodes:
			case 63:
				MeshFromNodes(cube, 7, 5, 10, 7, 10, 11);
				break;
			case 95:
				MeshFromNodes(cube, 9, 5, 6, 9, 6, 11, 9, 11, 7, 9, 7, 4);
				break;
			case 111:
				MeshFromNodes(cube, 8, 4, 6, 8, 6, 11);
				break;
			case 119:
				MeshFromNodes(cube, 3, 7, 6, 3, 6, 2);
				break;
			case 123:
				MeshFromNodes(cube, 7, 6, 10, 7, 10, 1, 7, 1, 2, 7, 2, 11);
				break;
			case 125:
				MeshFromNodes(cube, 0, 1, 9, 7, 6, 11);
				break;
			case 126:
				MeshFromNodes(cube, 0, 8, 7, 0, 7, 6, 0, 6, 11, 0, 11, 3);
				break;
			case 159:
				MeshFromNodes(cube, 9, 10, 6, 9, 6, 4);
				break;
			case 175:
				MeshFromNodes(cube, 8, 4, 5, 8, 5, 10, 8, 10, 6, 8, 6, 7);
				break;
			case 183:
				MeshFromNodes(cube, 5, 10, 2, 5, 2, 3, 5, 3, 11, 5, 11, 6);
				break;
			case 187:
				MeshFromNodes(cube, 1, 2, 6, 1, 6, 5);
				break;
			case 189:
				MeshFromNodes(cube, 0, 1, 10, 0, 10, 6, 0, 6, 5, 0, 5, 9);
				break;
			case 190:
				MeshFromNodes(cube, 6, 5, 10, 8, 3, 0);
				break;
			case 207:
				MeshFromNodes(cube, 7, 8, 9, 7, 9, 5);
				break;
			case 215:
				MeshFromNodes(cube, 3, 11, 2, 4, 9, 5);
				break;
			case 219:
				MeshFromNodes(cube, 4, 10, 5, 4, 2, 10, 4, 1, 2, 4, 9, 1);
				break;
			case 221:
				MeshFromNodes(cube, 4, 0, 1, 4, 1, 5);
				break;
			case 222:
				MeshFromNodes(cube, 3, 4, 8, 3, 5, 4, 3, 9, 5, 3, 0, 9);
				break;
			case 231:
				MeshFromNodes(cube, 2, 3, 8, 2, 8, 4, 2, 4, 7, 2, 7, 11);
				break;
			case 235:
				MeshFromNodes(cube, 4, 7, 8, 1, 2, 10);
				break;
			case 237:
				MeshFromNodes(cube, 7, 8, 0, 7, 0, 1, 7, 1, 9, 7, 9, 4);
				break;
			case 238:
				MeshFromNodes(cube, 0, 4, 7, 0, 7, 3);
				break;
			case 243:
				MeshFromNodes(cube, 3, 11, 10, 3, 10, 1);
				break;
			case 245:
				MeshFromNodes(cube, 11, 0, 3, 11, 9, 0, 11, 1, 9, 11, 2, 1);
				break;
			case 246:
				MeshFromNodes(cube, 8, 11, 2, 8, 2, 0);
				break;
			case 249:
				MeshFromNodes(cube, 9, 0, 2, 9, 2, 10);
				break;
			case 250:
				MeshFromNodes(cube, 8, 3, 2, 8, 2, 10, 8, 10, 1, 8, 1, 0);
				break;
			case 252:
				MeshFromNodes(cube, 9, 8, 3, 9, 3, 1);
				break;


			// 7 Active ControlNodes:
			case 127:
				MeshFromNodes(cube, 7, 6, 11);
				break;
			case 191:
				MeshFromNodes(cube, 6, 5, 10);
				break;
			case 223:
				MeshFromNodes(cube, 4, 9, 5);
				break;
			case 239:
				MeshFromNodes(cube, 4, 7, 8);
				break;
			case 247:
				MeshFromNodes(cube, 3, 11, 2);
				break;
			case 251:
				MeshFromNodes(cube, 2, 10, 1);
				break;
			case 253:
				MeshFromNodes(cube, 1, 9, 0);
				break;
			case 254:
				MeshFromNodes(cube, 8, 3, 0);
				break;

			// 8 Active ControlNodes:
			case 255:
				break;
		}
	}
}