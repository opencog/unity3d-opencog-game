using UnityEngine;
using System.Collections;

public class List3D<T> {
	
	private T[,,] list;
	private Vector3i min, max;
	
	public List3D() {
		list = new T[0, 0, 0];
	}
	
	public List3D(Vector3i min, Vector3i max) {
		this.min = min;
		this.max = max;
		Vector3i size = GetSize();
		list = new T[size.z, size.y, size.x];
	}
	
	
	
	public void Set(T obj, Vector3i pos) {
		Set(obj, pos.x, pos.y, pos.z);
	}
	public void Set(T obj, int x, int y, int z) {
		list[z-min.z, y-min.y, x-min.x] = obj;
	}
	
	public T GetInstance(Vector3i pos) {
		return GetInstance(pos.x, pos.y, pos.z);
	}
	public T GetInstance(int x, int y, int z) {
		T obj = SafeGet(x, y, z);
		if( object.Equals(obj, default(T)) ) {
			obj = System.Activator.CreateInstance<T>();
			AddOrReplace(obj, x, y, z);
		}
		return obj;
	}
	public T Get(Vector3i pos) {
		return Get(pos.x, pos.y, pos.z);
	}
	public T Get(int x, int y, int z) {
		return list[z-min.z, y-min.y, x-min.x];
	}
	
	public T SafeGet(Vector3i pos) {
		return SafeGet(pos.x, pos.y, pos.z);
	}
	public T SafeGet(int x, int y, int z) {
		if(!IsCorrectIndex(x, y, z)) return default(T);
		return Get(x, y, z);
	}
	
	public void AddOrReplace(T obj, Vector3i pos) {
		Vector3i newMin = Vector3i.Min(min, pos);
		Vector3i newMax = Vector3i.Max(max, pos+Vector3i.one);
		if(newMin != min || newMax != max) {
			Resize(newMin, newMax);
		}
		Set(obj, pos);
	}
	public void AddOrReplace(T obj, int x, int y, int z) {
		AddOrReplace(obj, new Vector3i(x, y, z));
	}
	
	private void Resize(Vector3i newMin, Vector3i newMax) {
		Vector3i oldMin = min;
		Vector3i oldMax = max;
		T[,,] oldList = list;
		
		min = newMin;
		max = newMax;
		Vector3i size = newMax - newMin;
		list = new T[size.z, size.y, size.x];
		for(int x=oldMin.x; x<oldMax.x; x++) {
			for(int y=oldMin.y; y<oldMax.y; y++) {
				for(int z=oldMin.z; z<oldMax.z; z++) {
					T val = oldList[z-oldMin.z, y-oldMin.y, x-oldMin.x];
					Set(val, x, y, z);
				}
			}
		}
	}
	
	
	public bool IsCorrectIndex(Vector3i pos) {
		return IsCorrectIndex(pos.x, pos.y, pos.z);
	}
	public bool IsCorrectIndex(int x, int y, int z) {
		if(x<min.x  || y<min.y  || z<min.z) return false;
		if(x>=max.x || y>=max.y || z>=max.z) return false;
		return true;
	}
	
	public Vector3i GetMin() {
		return min;
	}
	public Vector3i GetMax() {
		return max;
	}
	public Vector3i GetSize() {
		return max-min;
	}
	
	public int GetMinX() {
		return min.x;
	}
	public int GetMinY() {
		return min.y;
	}
	public int GetMinZ() {
		return min.z;
	}
	
	public int GetMaxX() {
		return max.x;
	}
	public int GetMaxY() {
		return max.y;
	}
	public int GetMaxZ() {
		return max.z;
	}
	
}