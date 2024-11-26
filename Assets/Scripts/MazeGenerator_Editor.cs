using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MazeGenerator_corridors))]
public class MazeGenerator_Editor : Editor {
	 
	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();
		MazeGenerator_corridors mazeGeneratorScript = (MazeGenerator_corridors)target;

		if (GUILayout.Button ("Build Maze")) {
			mazeGeneratorScript.GenerateMaze();
		}
		if (GUILayout.Button ("Clear Maze")) {
			mazeGeneratorScript.ClearMaze();
		}

	}
}		

