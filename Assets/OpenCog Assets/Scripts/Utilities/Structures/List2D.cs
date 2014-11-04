using UnityEngine;
using System.Collections;

public class List2D<T> {
	
	private T[,] list;
	private Vector2i min, max;
	
	public List2D() {
		list = new T[0, 0];
	}

	
	public void Set(T obj, Vector2i pos) {
		Set(obj, pos.x, pos.y);
	}
	public void Set(T obj, int x, int y) {
		list[y-min.y, x-min.x] = obj;
	}
	
	public T GetInstance(Vector2i pos) {
		return GetInstance(pos.x, pos.y);
	}
	public T GetInstance(int x, int y) {
		T obj = SafeGet(x, y);
		if( object.Equals(obj, default(T)) ) {
			obj = System.Activator.CreateInstance<T>();
			AddOrReplace(obj, x, y);
		}
		return obj;
	}
	
	public T Get(Vector2i pos) {
		return Get(pos.x, pos.y);
	}
	public T Get(int x, int y) {
		return list[y-min.y, x-min.x];
	}
	
	public T SafeGet(Vector2i pos) {
		return SafeGet(pos.x, pos.y);
	}
	public T SafeGet(int x, int y) {
		if(!IsCorrectIndex(x, y)) return default(T);
		return list[y-min.y, x-min.x];
	}
	
	public void AddOrReplace(T obj, Vector2i pos) {
		Vector2i newMin = Vector2i.Min(min, pos);
		Vector2i newMax = Vector2i.Max(max, pos+Vector2i.one);
		if(newMin != min || newMax != max) {
			Resize(newMin, newMax);
		}
		Set(obj, pos);
	}
	public void AddOrReplace(T obj, int x, int y) {
		AddOrReplace(obj, new Vector2i(x, y));
	}
	private void Resize(Vector2i newMin, Vector2i newMax) {
		Vector2i oldMin = min;
		Vector2i oldMax = max;
		T[,] oldList = list;
		
		min = newMin;
		max = newMax;
		Vector2i size = newMax - newMin;
		list = new T[size.y, size.x];
		for(int x=oldMin.x; x<oldMax.x; x++) {
			for(int y=oldMin.y; y<oldMax.y; y++) {
				T val = oldList[y-oldMin.y, x-oldMin.x];
				Set(val, x, y);
			}
		}
	}
	
	public bool IsCorrectIndex(Vector2i pos) {
		return IsCorrectIndex(pos.x, pos.y);
	}
	private bool IsCorrectIndex(int x, int y) {
		if(x<min.x  || y<min.y) return false;
		if(x>=max.x || y>=max.y) return false;
		return true;
	}
	
	
}
