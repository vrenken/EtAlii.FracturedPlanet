using UnityEngine;
using UnityEditor;

// ReSharper disable once CheckNamespace
public class SlimUIWindow : EditorWindow {

	//string myString = "Hello";

	[MenuItem("Window/SlimUI Online Documentation")]
	public static void ShowWindow(){
		Application.OpenURL("https://www.slimui.com/documentation");
	}
}
