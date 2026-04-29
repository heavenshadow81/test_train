// Copyright (c) 2010 Bob Berkebile
// Modified in 2014 (Undo support.)
// 
// Please direct any bugs/comments/suggestions to http://www.pixelplacement.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(iTweenPath))]
public class iTweenPathEditor : Editor
{
	iTweenPath _target;
	GUIStyle style = new GUIStyle();
	public static int count = 0;
	
	void OnEnable(){
		//i like bold handle labels since I'm getting old:
		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = Color.white;
		_target = (iTweenPath)target;
		
		//lock in a default path name:
		if(!_target.initialized){
			_target.initialized = true;
			_target.pathName = "New Path " + ++count;
			_target.initialName = _target.pathName;
		}
	}
	
	public override void OnInspectorGUI(){
		//draw the path?
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Path Visible");
		_target.pathVisible = EditorGUILayout.Toggle(_target.pathVisible);
		EditorGUILayout.EndHorizontal();
		
		//path name:
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Path Name");

		EditorGUI.BeginChangeCheck();
		string newPathName = EditorGUILayout.TextField(_target.pathName);
		if(newPathName == ""){
			newPathName = _target.initialName;
		}
		if(EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(_target, "Change iTween Path Name");
			_target.pathName = newPathName;
			EditorUtility.SetDirty(_target);
		}

		EditorGUILayout.EndHorizontal();

		//path color:
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Path Color");

		EditorGUI.BeginChangeCheck();
		Color newPathColor = EditorGUILayout.ColorField(_target.pathColor);
		if(EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(_target, "Change iTween Path Color");
			_target.pathColor = newPathColor;
			EditorUtility.SetDirty(_target);
		}
		EditorGUILayout.EndHorizontal();
		
		//exploration segment count control:
		EditorGUILayout.BeginHorizontal();

		EditorGUI.BeginChangeCheck();
		int newNodeCount = Mathf.Max(2, EditorGUILayout.IntField("Node Count", _target.nodeCount));
		if(EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(_target, "Change iTween Path Node Count");

			_target.nodeCount = newNodeCount;

			//add node?
			if(_target.nodeCount > _target.nodes.Count){
                int addCount = _target.nodeCount - _target.nodes.Count;
                for (int i = 0; i < addCount; i++) {
					_target.nodes.Add(Vector3.zero);	
				}
			}
			
			//remove node?
			if(_target.nodeCount < _target.nodes.Count){
				int removeCount = _target.nodes.Count - _target.nodeCount;
				_target.nodes.RemoveRange(_target.nodes.Count-removeCount,removeCount);
			}

			EditorUtility.SetDirty(_target);
		}

		EditorGUILayout.EndHorizontal();

		//node display:
		EditorGUI.indentLevel = 4;
		for (int i = 0; i < _target.nodes.Count; i++) {
			string nodeName = string.Format("Node {0}", i+1);

			EditorGUI.BeginChangeCheck();

			Vector3 newNode = EditorGUILayout.Vector3Field(nodeName, _target.nodes[i]);

			if(EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(_target, string.Format("Change iTween Path {0}", nodeName)); 
				_target.nodes[i] = newNode;
				EditorUtility.SetDirty(_target);
			}
		}
	}
	
	void OnSceneGUI(){
		Tools.current = Tool.None;

		if(_target.pathVisible){			
			if(_target.nodes.Count > 0){

				//path begin and end labels:
				Handles.Label(_target.nodes[0], "'" + _target.pathName + "' Begin", style);
				Handles.Label(_target.nodes[_target.nodes.Count-1], "'" + _target.pathName + "' End", style);
				
				//node handle display:
				for (int i = 0; i < _target.nodes.Count; i++) {
					// begin check
					EditorGUI.BeginChangeCheck();

					Vector3 pos = Handles.PositionHandle(_target.nodes[i], Quaternion.identity);

					// end check
					if(EditorGUI.EndChangeCheck()) {
						Undo.RecordObject(target, "Adjust iTween Path");
						_target.nodes[i] = pos;
						EditorUtility.SetDirty(_target);
					}
				}
			}	
		}
	}
}