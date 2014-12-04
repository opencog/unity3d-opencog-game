using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using System.Collections;
using OpenCog.BlockSet;
using OpenCog.BlockSet.BaseBlockSet;

namespace OpenCog
{

	public class BlockSetViewer 
	{
	
		#if UNITY_EDITOR
		private static string DRAG_AND_DROP = "drag block";
		#endif

		public static int SelectionGrid(OCBlockSet blockSet, int index, params GUILayoutOption[] options) 
		{

			Container<Vector2> scroll = BlockEditorUtils.GetStateObject<Container<Vector2>>(blockSet.GetHashCode());
			
			scroll.value = GUILayout.BeginScrollView(scroll, options);
			index = SelectionGrid(blockSet.Blocks, index);
			GUILayout.EndScrollView();

			return index;
			
		}
		
		private static int SelectionGrid(IList<OCBlock> items, int index) {
			Rect rect;
			int xCount, yCount;
			index = SelectionGrid(items, index, out rect, out xCount, out yCount);
			float itemWidth = rect.width/xCount;
			float itemHeight = rect.height/yCount;
			
			GUI.BeginGroup(rect);
			Vector2 mouse = Event.current.mousePosition;
			int posX = Mathf.FloorToInt(mouse.x/itemWidth);
			int posY = Mathf.FloorToInt(mouse.y/itemHeight);
			int realIndex = -1; // номер элемента под курсором
			if(posX >= 0 && posX < xCount && posY >= 0 && posY < yCount) realIndex = posY*xCount + posX;
			
			int dropX = Mathf.Clamp(posX, 0, xCount-1);
			int dropY = Mathf.Clamp(posY, 0, yCount-1);
			if(dropY == yCount-1 && items.Count%xCount != 0) dropX = Mathf.Clamp(dropX, 0, items.Count%xCount);
			int dropIndex = dropY*xCount + dropX; // ближайший элемент к курсору
			
			if(Event.current.type == EventType.MouseDrag && Event.current.button == 0 && realIndex == index) {
				#if UNITY_EDITOR
				DragAndDrop.PrepareStartDrag();
				DragAndDrop.objectReferences = new Object[0];
				DragAndDrop.paths = new string[0];
				DragAndDrop.SetGenericData(DRAG_AND_DROP, new Container<int>(index));
				DragAndDrop.StartDrag("DragAndDrop");
				#endif
				Event.current.Use();
			}
			
			if(Event.current.type == EventType.DragUpdated) {
				#if UNITY_EDITOR
				Container<int> data = (Container<int>)DragAndDrop.GetGenericData(DRAG_AND_DROP);
				if(data != null) {
					DragAndDrop.visualMode = DragAndDropVisualMode.Link;
					Event.current.Use();
				}
				#endif
			}
			
			if(Event.current.type == EventType.DragPerform) {
				#if UNITY_EDITOR
				Container<int> oldIndex = (Container<int>)DragAndDrop.GetGenericData(DRAG_AND_DROP);


				if(dropIndex > oldIndex.value) dropIndex--;
				dropIndex = Mathf.Clamp(dropIndex, 0, items.Count-1);
				Insert(items, dropIndex, oldIndex);
				
				index = dropIndex;

				DragAndDrop.AcceptDrag();
				DragAndDrop.PrepareStartDrag();
				#endif
				Event.current.Use();
			}

			#if UNITY_EDITOR
			if(Event.current.type == EventType.Repaint && DragAndDrop.visualMode == DragAndDropVisualMode.Link) {
				Vector2 pos = new Vector2(2+dropX*itemWidth, 2+dropY*itemHeight);
				Rect lineRect = new Rect(pos.x-2, pos.y, 2, itemWidth-2);
				BlockEditorUtils.FillRect(lineRect, Color.red);
			}
			#endif
			GUI.EndGroup();
			
			return index;
		}
		
		private static int SelectionGrid(IList<OCBlock> items, int index, out Rect rect, out int xCount, out int yCount) {
			xCount = Mathf.FloorToInt( Screen.width/66f );
			yCount = Mathf.CeilToInt( (float) items.Count/xCount );
			
			rect = GUILayoutUtility.GetAspectRect((float)xCount/yCount);
			float labelHeight = GUI.skin.label.CalcHeight(GUIContent.none, 0); // высота текста
			GUILayout.Space(labelHeight*yCount);
			rect.height += labelHeight*yCount;
			
			Rect[] rects = GUIUtils.Separate(rect, xCount, yCount);
			for(int i=0; i<items.Count; i++) {
				Rect position = rects[i];
				position.xMin += 2;
				position.yMin += 2;
					
				bool selected = DrawItem(position, items[i], i == index, i);
				if(selected) index = i;
			}
			
			return index;
		}
		
		private static bool DrawItem(Rect position, OCBlock block, bool selected, int index) {
			Rect texturePosition = position;
			texturePosition.height = texturePosition.width;
			Rect labelPosition = position;
			labelPosition.yMin += texturePosition.height;
			
			if(selected) BlockEditorUtils.FillRect(labelPosition, new Color( 61/255f, 128/255f, 223/255f ));
			if(block != null) {
				block.DrawPreview(texturePosition);
				GUI.Label(labelPosition, block.GetName());
			} else {
				BlockEditorUtils.FillRect(texturePosition, Color.grey);
				GUI.Label(labelPosition, "Null");
			}
			
			if(Event.current.type == EventType.MouseDown && Event.current.button == 0 && position.Contains(Event.current.mousePosition)) {
				Event.current.Use();
				return true;
			}
			return false;
		}
		
		private static void Insert(IList<OCBlock> items, int newIndex, int oldIndex) {
			List<OCBlock> list = new List<OCBlock>(items);
			OCBlock block = list[oldIndex];
			list.RemoveAt(oldIndex);
			list.Insert(newIndex, block);
			
			for(int i=0; i<items.Count; i++) {
				items[i] = list[i];
			}
		}
		
	}
	
}
