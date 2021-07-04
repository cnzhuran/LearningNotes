using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridsGenerator : MonoBehaviour
{
	/// <summary>
	/// 网格长度
	/// </summary>
	public int width;
	/// <summary>
	/// 网格宽度
	/// </summary>
	public int height;

	/// <summary>
	/// 网格类型数据
	/// </summary>
	public List<EmGridType> gridTypeList;

	private Mesh _mesh;
	private Vector3[] _vertices;
	private int[] _triangles;
	private Vector2[] _uvs;

	private void Awake ()
	{
		DoGenerate(width, height);
	}

	private void MeshInstance()
	{
		if (_mesh != null)
			return;
		
		_mesh = new Mesh();
		_mesh.name = "GridsGenerator";
		GetComponent<MeshFilter>().mesh = _mesh;
	}

	public void ChangeGridType(int index, EmGridType gridType)
	{
		
	}

	public void DoGenerate(int xSize, int ySize)
	{
		if (xSize * ySize <= 0)
			return;
		
		MeshInstance();
			
		_vertices = new Vector3[xSize * ySize * 4];
		_uvs = new Vector2[_vertices.Length];
		_triangles = new int[xSize * ySize * 3 * 2];
		gridTypeList = new List<EmGridType>();

		for (int h = 0; h < ySize; h++)
		{
			for (int w = 0; w < xSize; w++)
			{
				Vector3 p = new Vector3(w, h);
				int index = h * xSize + w;

				Vector2 uv00 = new Vector2(0, 0);
				Vector2 uv11 = new Vector2((float)1 / 14, (float)1 / 14);

				if (index % 3 == 0)
				{
					uv00 = new Vector2(0, 0);
					uv11 = new Vector2(1, 1);
				}

				AddSquareItem(_vertices, _triangles, _uvs, p, index, uv00, uv11);
			}
		}
		
		_mesh.vertices = _vertices;
		_mesh.triangles = _triangles;
		_mesh.uv = _uvs;
		
		_mesh.RecalculateNormals();
	}

	public void AddSquareItem(Vector3[] vertices, int[] triangles, Vector2[] uvs, Vector3 pos, int index, Vector2 uv00, Vector2 uv11)
	{
		//创建顶点索引
		int vIndex = index * 4;
		int vIndex0 = vIndex;
		int vIndex1 = vIndex + 1;
		int vIndex2 = vIndex + 2;
		int vIndex3 = vIndex + 3;
        
		//存入顶点容器中 左下，左上，右上，右下
		vertices[vIndex0] = pos;
		vertices[vIndex1] = pos + new Vector3(0, 1 );
		vertices[vIndex2] = pos + new Vector3(1, 1 );
		vertices[vIndex3] = pos + new Vector3(1, 0);
		
		//创建三角形 按序绘制两个
		int tIndex = index * 6;
        
		triangles[tIndex] = vIndex0;
		triangles[tIndex + 1] = vIndex1;
		triangles[tIndex + 2] = vIndex2;

		triangles[tIndex + 3] = vIndex2;
		triangles[tIndex + 4] = vIndex3;
		triangles[tIndex + 5] = vIndex0;
        
		//设置uvs
		uvs[vIndex0] = new Vector2(uv00.x, uv00.y);
		uvs[vIndex1] = new Vector2(uv00.x, uv11.y);
		uvs[vIndex2] = new Vector2(uv11.x, uv11.y);
		uvs[vIndex3] = new Vector2(uv11.x, uv00.y);
	}
	
	private void OnDrawGizmos ()
	{
		if (_vertices == null)
		{
			return;
		}
		
		Gizmos.color = Color.black;
		for (int i = 0; i < _vertices.Length; i++)
		{
			Gizmos.DrawSphere(_vertices[i], 0.1f);
		}
	}
}

public enum EmGridType
{
	/// <summary>
	/// 通用
	/// </summary>
	Gt0 = 0,	
	Gt1 = 1,
	Gt2 = 2,
	Gt3 = 3,
	Gt4 = 4,
	Gt5 = 5,
}

#if UNITY_EDITOR
[CustomEditor(typeof(GridsGenerator))]
public class GridsGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		GridsGenerator builder = (GridsGenerator)target;
 
		EditorGUI.BeginChangeCheck();
 
		base.OnInspectorGUI();
 
		if (EditorGUI.EndChangeCheck())
		{
			// builder.DoGenerate(builder.width, builder.height);
		}
 
		if (GUILayout.Button("更新网格"))
		{
			builder.DoGenerate(builder.width, builder.height);
		}
	}
}
#endif
