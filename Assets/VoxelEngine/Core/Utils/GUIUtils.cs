using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIUtils {
	
	public static Rect[] Separate(Rect mainRect, int xCount, int yCount) {
		float itemWidth = mainRect.width / xCount;
		float itemHeight = mainRect.height / yCount;
		List<Rect> list = new List<Rect>();
		for(int y=0; y<yCount; y++) {
			for(int x=0; x<xCount; x++) {
				Rect rect = new Rect(mainRect.x+itemWidth*x, mainRect.y+itemHeight*y, itemWidth, itemHeight);
				list.Add(rect);
			}
		}
		return list.ToArray();
	}
	
}
