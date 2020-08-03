using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine;
using System;

public class ObjectData 
{
	public Vector3 StartPoint;
	public GameObject Object;
	public MeshFilter meshfilter;
	public MeshRenderer meshRenderer;
	public MeshCollider meshCollider;
	public Dictionary<Vector3Int, VoxelInfo> voxels = new Dictionary<Vector3Int, VoxelInfo>();
	public static readonly Vector3[][] QUAD_VERTS = ChunkData.QUAD_VERTS;
	public static readonly Vector3Int[] Directions =
	{
		new Vector3Int(0,-1,0),new Vector3Int(0,1,0),new Vector3Int(0,0,-1),new Vector3Int(0,0,1),new Vector3Int(-1,0,0),new Vector3Int(1,0,0)
	};
	public void Move(Vector3 mouseVector)
	{
		
		
		int Dir = 0;
		float minDistance = 10;
		
		for (int i = 0; i < 6; i++)
		{
			if (Mathf.Abs(Vector3.Cross(mouseVector.normalized, Directions[i]).magnitude) < minDistance
				&& Vector3.Dot(mouseVector, Directions[i]) > 0)
			{
				Dir = i;
				minDistance = Mathf.Abs(Vector3.Cross(mouseVector.normalized, Directions[i]).magnitude);
			}
			
		}
		var newVoxels = new Dictionary<Vector3Int, VoxelInfo>();
		foreach (var voxel in voxels)
		{
			WorldDataManager.Instance.ActiveWorld.SetVoxelAt(voxel.Key, null);
			newVoxels.Add(voxel.Key + Directions[Dir] * (int)mouseVector.magnitude, voxel.Value);
			
		}
		foreach (var voxel in newVoxels)
		{
			WorldDataManager.Instance.ActiveWorld.SetVoxelAt(voxel.Key, voxel.Value);
		}
		voxels = newVoxels;
		Object.transform.position +=  Directions[Dir] * (int)mouseVector.magnitude;
		StartPoint += Directions[Dir] * (int)mouseVector.magnitude;
		GenerateObjectMesh();
		

	}
	public bool UpdateObject(Vector3Int newPoint,VoxelInfo newVoxel,bool isErase=false)
	{
		if (!isErase)
		{
			foreach (var voxel in voxels.Keys)
			{
				if ((newPoint - voxel).magnitude <= 1)
				{
					if (voxels.ContainsKey(newPoint))
						return false;
					voxels.Add(newPoint, newVoxel);
					GenerateObjectMesh();
					return true;
				}
			}
			return false;
		}
		else
		{
			if (voxels.ContainsKey(newPoint))
			{
				
				voxels.Remove(newPoint);
				GenerateObjectMesh();
				if (voxels.Count == 0)
				{
					Destroy();
					return true;
				}
				else
					return false;
			}
			else return false;
			
		}
	}
	public ObjectData(Vector3Int point,VoxelInfo v)
	{
		StartPoint = point;
		voxels.Add(point, v);
		Object = new GameObject();
		Object.transform.position = point;
		meshfilter= Object.AddComponent<MeshFilter>(); 
		meshRenderer= Object.AddComponent<MeshRenderer>();
		meshCollider= Object.AddComponent<MeshCollider>();
		GenerateObjectMesh();
	}
	public ObjectData(ObjectData obj, VoxelInfo v)
	{
		StartPoint = obj.StartPoint;
		foreach (var voxel in obj.voxels)
		{
			voxels.Add(voxel.Key, v);
		}
		Object = new GameObject();
		Object.transform.position = StartPoint;
		meshfilter = Object.AddComponent<MeshFilter>();
		meshRenderer = Object.AddComponent<MeshRenderer>();
		meshCollider = Object.AddComponent<MeshCollider>();
		GenerateObjectMesh();
	}
	public void CopyFrom(ObjectData obj)
	{
		Vector3Int moveVector = new Vector3Int( (int)(StartPoint - obj.StartPoint).x, (int)(StartPoint - obj.StartPoint).y, (int)(StartPoint - obj.StartPoint).z);
		foreach (var point in obj.voxels.Keys)
		{
			if (voxels.ContainsKey(point + moveVector))
				voxels[point + moveVector] = obj.voxels[point];
			else
				Debug.LogWarning("There is no voxel that correspond to original");
		}
		GenerateObjectMesh();
	}
	public void TemporaryChangeVoxelInfo(VoxelInfo v)
	{
		GenerateObjectMesh(v);
	}
	public void BackToOriginal()
	{
		GenerateObjectMesh();
	}
	public void Destroy()
	{
		GameObject.DestroyImmediate(Object);
	}
	private void DFS(Vector3Int StartPoint, Vector3Int[] findDir, List<Vector3Int> faceVoxels,Vector3Int Dir)
	{
		faceVoxels.Add(StartPoint);
		for (int i = 0; i < 4; i++)
		{
			var nowVoxel = StartPoint + findDir[i];
			if (voxels.ContainsKey(nowVoxel) && !voxels.ContainsKey(nowVoxel + Dir)&&faceVoxels.Find(delegate (Vector3Int v) { return v == nowVoxel; })==Vector3Int.zero)
			{
				DFS(nowVoxel, findDir, faceVoxels, Dir);
			}
		}
	}
	private List<Vector3Int> GetOneFace(Vector3Int selectedVoxel,Vector3Int Dir)
	{
		Vector3Int x=new Vector3Int(), y= new Vector3Int();
		if (Dir.x != 0) { x = new Vector3Int(0, 1, 0); y = new Vector3Int(0, 0, 1); }
		else if (Dir.y != 0) { x = new Vector3Int(1, 0, 0); y = new Vector3Int(0, 0, 1); }
		else if (Dir.z != 0) { x = new Vector3Int(0, 1, 0); y = new Vector3Int(1, 0, 0); }
		Vector3Int[] findDir = new Vector3Int[4] { -x,x,-y,y };
		List<Vector3Int> faceVoxels = new List<Vector3Int>();
		DFS(selectedVoxel, findDir, faceVoxels, Dir);
		return faceVoxels;
	}
	public Vector3 Stretch(Vector3Int Dir,Vector3 point,int Length,bool isAdd)
	{
		point -= new Vector3(Dir.x/2.0f, Dir.y / 2.0f, Dir.z / 2.0f);
		Vector3Int SelectedVoxel = new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), Mathf.FloorToInt(point.z));
		if (!voxels.ContainsKey(SelectedVoxel)) { Debug.Log("There is no voxel to stretch"); return SelectedVoxel; }
		List<KeyValuePair<Vector3Int, VoxelInfo>> addVoxels=new List<KeyValuePair<Vector3Int, VoxelInfo>>();
		List<KeyValuePair<Vector3Int, VoxelInfo>> subVoxels = new List<KeyValuePair<Vector3Int, VoxelInfo>>();
		List<Vector3Int> SrtetchVoxelList;
		Vector3 newPoint=point;
		if (isAdd) newPoint += Dir * (Length);
		else newPoint -= Dir * (Length);
		newPoint += new Vector3(Dir.x / 2.0f, Dir.y / 2.0f, Dir.z / 2.0f);
		SrtetchVoxelList = GetOneFace(SelectedVoxel, Dir);
		foreach (var voxel in SrtetchVoxelList)
		{
			if (isAdd)
			{
				for (int i = 1; i <= Length; i++)
				{
					if (WorldDataManager.Instance.ActiveWorld.GetVoxelAt(voxel + Dir * i) != null)
						return point += new Vector3(Dir.x / 2.0f, Dir.y / 2.0f, Dir.z / 2.0f);
					addVoxels.Add(new KeyValuePair<Vector3Int, VoxelInfo>(voxel + Dir * i, voxels[voxel]));
				}
			}
			else
			{
				for (int i = 0; i < Length; i++)
				{
					subVoxels.Add(new KeyValuePair<Vector3Int, VoxelInfo>(voxel - Dir * i, voxels[voxel]));
				}
			}

		}
		if (isAdd)
		{
			foreach (var temp in addVoxels)
			{
				
				voxels.Add(temp.Key, temp.Value);
			}
		}
		else
		{
			foreach (var temp in subVoxels)
			{
				WorldDataManager.Instance.ActiveWorld.SetVoxelAt(temp.Key, null);
				voxels.Remove(temp.Key);
			}
		}
		GenerateObjectMesh();
		return newPoint;
	}
	public static ObjectData operator +(ObjectData obj0,ObjectData obj1)
	{
		List<KeyValuePair<Vector3Int, VoxelInfo>> differentVoxels = new List<KeyValuePair<Vector3Int, VoxelInfo>>();
		foreach (var voxel in obj1.voxels)
		{
			if (!obj0.voxels.ContainsKey(voxel.Key))
				differentVoxels.Add(voxel);
		}
		foreach (var voxel in differentVoxels)
			obj0.voxels.Add(voxel.Key,voxel.Value);
		obj0.GenerateObjectMesh();
		return obj0;
	}
	public static ObjectData operator -(ObjectData obj0, ObjectData obj1)
	{
		List<KeyValuePair<Vector3Int, VoxelInfo>> SameVoxels = new List<KeyValuePair<Vector3Int, VoxelInfo>>();
		foreach (var voxel in obj1.voxels)
		{
			if (obj0.voxels.ContainsKey(voxel.Key))
				SameVoxels.Add(voxel);
		}
		foreach (var voxel in SameVoxels)
		{
			obj0.voxels.Remove(voxel.Key);
			WorldDataManager.Instance.ActiveWorld.SetVoxelAt(voxel.Key, null);
		}
		if (obj0.voxels.Count == 0) { obj0.Destroy(); return null; }
		obj0.GenerateObjectMesh();
		return obj0;
	}
	public bool SubVoxels(Dictionary<Vector3Int, VoxelInfo> Voxels)
	{
		foreach (var voxel in Voxels)
		{
			if (voxels.ContainsKey(voxel.Key))
			{
				voxels.Remove(voxel.Key);
				WorldDataManager.Instance.ActiveWorld.SetVoxelAt(voxel.Key, null);
			}
		}
		if (voxels.Count == 0) { Destroy();return false; }
		GenerateObjectMesh();
		return true;
	}
	public static Dictionary<Vector3Int,VoxelInfo> GetSameVoxels(List<ObjectData> objs)
	{
		Dictionary<Vector3Int, VoxelInfo> sameVoxels = new Dictionary<Vector3Int, VoxelInfo>();
		for (int i = 0; i < objs.Count; i++)
		{
			for (int j = i+1; j < objs.Count; j++)
			{
				foreach (var point in objs[i].voxels.Keys)
					if (objs[j].voxels.ContainsKey(point) && !sameVoxels.ContainsKey(point))
					{
						sameVoxels.Add(point, objs[i].voxels[point]);
					}

			}
		}
		return sameVoxels;
	}
	private void GenerateObjectMesh(VoxelInfo info=null)
	{
		Dictionary<VoxelInfo, List<Vector3>> voxelVertListDict = new Dictionary<VoxelInfo, List<Vector3>>();

		List<Material> voxelMatsList = new List<Material>();
		if (info != null)
		{
			voxelVertListDict.Add(info, new List<Vector3>());
			voxelMatsList.Add(info.material);
			foreach (var voxel in voxels.Keys)
			{
				List<Vector3> voxelVertsList = voxelVertListDict[info];
				for (int i = 0; i < Directions.Length; i++)
				{
					if (!voxels.ContainsKey(voxel + Directions[i]))
					{
						foreach (var vert in QUAD_VERTS[i])
						{
							voxelVertsList.Add(voxel - StartPoint + vert);
						}
					}
				}

			}
		}
		else
		{
			foreach (var voxel in voxels.Keys)
			{
				if (!voxelVertListDict.ContainsKey(voxels[voxel]))
				{
					voxelVertListDict.Add(voxels[voxel], new List<Vector3>());
					voxelMatsList.Add(voxels[voxel].material);
				}
				List<Vector3> voxelVertsList = voxelVertListDict[voxels[voxel]];
				for (int i = 0; i < Directions.Length; i++)
				{
					if (!voxels.ContainsKey(voxel + Directions[i]))
					{
						foreach (var vert in QUAD_VERTS[i])
						{
							voxelVertsList.Add(voxel - StartPoint + vert);
						}
					}
				}

			}
		}
		List<Vector3> totalVertices = new List<Vector3>();
		List<int> totalIndices = new List<int>();
		List<SubMeshDescriptor> subMeshDescList = new List<SubMeshDescriptor>();
		foreach (var verts in voxelVertListDict.Values)
		{
			//Add a descriptor for each vert list
			subMeshDescList.Add(new SubMeshDescriptor(totalIndices.Count, verts.Count, MeshTopology.Quads));
			//Append to total lists
			foreach (var vert in verts)
			{
				totalIndices.Add(totalIndices.Count);
				totalVertices.Add(vert);
			}
		}

		//Output voxelmesh
		var ObjectMesh = new Mesh();
		ObjectMesh.SetVertices(totalVertices);//Set vertex buffer
		ObjectMesh.SetIndexBufferParams(totalIndices.Count, IndexFormat.UInt32);//Set index buffer format and data,since the max count is 65565*4*6 so must be int32
		ObjectMesh.SetIndexBufferData(totalIndices, 0, 0, totalIndices.Count);

		//Set submesh description
		int j = 0;
		foreach (var desc in subMeshDescList)
		{
			ObjectMesh.SetSubMesh(j, desc);
			j++;
		}
		//optimization
		ObjectMesh.Optimize();
		ObjectMesh.RecalculateNormals();
		ObjectMesh.RecalculateTangents();

		meshRenderer.materials = voxelMatsList.ToArray();
		meshfilter.mesh = ObjectMesh;
		meshCollider.sharedMesh = ObjectMesh;

		

	}
}
