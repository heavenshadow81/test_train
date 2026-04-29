using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class CustomInputGUI : MonoBehaviour {
	public class Info {
		public TouchInfo touch;
		public List<TouchInfo.Phase> list;

		public Info(TouchInfo t) {
			touch = t;
			list = new List<TouchInfo.Phase>();
			list.Add(t.phase);
		}
	}

	public Dictionary<int, Info> touchPhaseTraceDict = new Dictionary<int, Info>();

	void Update() {
		TouchInfo[] touches = CustomInput.touches;
		for(int i = 0; i < touches.Length; i++) {
			TouchInfo t = touches[i];
			if(touchPhaseTraceDict.ContainsKey(t.id)) {
				Info info = touchPhaseTraceDict[t.id];
				info.touch = t;

				//Debug.Log (info.touch.position);

				if(t.phase == TouchInfo.Phase.Begin) {
					info.list.Clear();
					info.list.Add(t.phase);
				}
				else if(info.list.Count == 0) {
					info.list.Add(t.phase);
				}
				else if(info.list[info.list.Count-1] != t.phase) {
					info.list.Add(t.phase);
				}
			}
			else {
				Info info = new Info(t);
				touchPhaseTraceDict[t.id] = info;
			}
		}
	}

	void OnGUI() {
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		sb.AppendFormat("touch count = {0}\n", CustomInput.touchCount);
		
		for(int i = 0, cnt = CustomInput.maxUserCount; i < cnt; i++) {
			sb.AppendFormat("ID {0} = {1}\n", i, CustomInput.GetTouchCountOfUser(i));
		}
		
		sb.AppendLine();

		var e = touchPhaseTraceDict.GetEnumerator();
		for(int i = 0, cnt = touchPhaseTraceDict.Count; i < cnt; i++) {
			e.MoveNext();
			int id = e.Current.Key;
			Info info = e.Current.Value;

			TouchInfo touch = info.touch;
			StringBuilder phaseStr = new StringBuilder();
			for(int j = 0; j < info.list.Count; j++) {
				TouchInfo.Phase phase = info.list[j];
				if(j != 0) phaseStr.Append(" -> ");
				phaseStr.Append(phase);
			}

			sb.AppendFormat("Touch({0}) UserID={1} Type={2} Phase={3} Position={4} Diameter={5}\n", 
			                touch.id,
			                touch.userId,
			                touch.type,
			                phaseStr.ToString(),
			                touch.position,
			                touch.diameter);
		}
		
		GUILayout.Label(sb.ToString());
	}
}
