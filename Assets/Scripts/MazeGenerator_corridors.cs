using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine; 


public class MazeGenerator_corridors : MonoBehaviour {

	[SerializeField] GameObject m_mazeBlock;
	[SerializeField] float m_distanceMazeBlocks;

	[SerializeField] int m_mazeWidth;
	[SerializeField] int m_mazeHeight;

	[SerializeField] float m_sameDirectionFactor;

	bool[,] m_mazeGrid;

	List<Vector2> m_directions;

	List<GameObject> m_mazeObjects;


	// Use this for initialization
	void Start () {
		
	}

	public void GenerateMaze()
	{
		if (m_mazeObjects == null || m_mazeObjects.Count == 0) 
		{
			m_mazeObjects = new List<GameObject> ();

			m_directions = new List<Vector2> { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
		} 
		else 
		{ 
			foreach (GameObject mazeBlock in m_mazeObjects) 
			{
				DestroyImmediate (mazeBlock);
			}

			m_mazeObjects.Clear ();
		}


		m_mazeGrid = new bool[m_mazeWidth, m_mazeHeight];


        List<Vector2> directions = new List<Vector2> { Vector2.up, Vector2.down, Vector2.left, Vector2.right }; 
        CarvePassageFrom (1, 1, directions);

		CreateBorder ();
		CreateMazeBlocks ();

	}

	void CreateBorder()
	{
		int counter_width = 0; 
		int counter_height = 0; 

		while(counter_width < m_mazeWidth) 
		{
			while (counter_height < m_mazeHeight) 
			{
				if (counter_width != 0 && counter_height != 0 && counter_width != m_mazeWidth - 1 && counter_height != m_mazeHeight - 1 &&
					(counter_width == 1 || counter_height == 1 || counter_width == m_mazeWidth - 2 || counter_height == m_mazeHeight - 2) )
				{
					m_mazeGrid [counter_width,counter_height] = true;	
				}

				counter_height++;
			}

			counter_height = 0;
			counter_width++;

		}
	}

	//worker function
	void CarvePassageFrom(int _xParameter, int _yParameter , List<Vector2> directions)
	{ 
        if (Random.value < m_sameDirectionFactor)
        {
            directions = Fisher_Yates_Shuffle (directions);
		}

		foreach (Vector2 direction in directions) 
		{
			int new_x = _xParameter + (int)direction.x * 2;
			int new_y = _yParameter + (int)direction.y * 2;

			int inbetween_x = _xParameter + (int)direction.x;
			int inbetween_y = _yParameter + (int)direction.y;

			if (new_x >= 0 && new_x < m_mazeWidth  && 
				new_y >= 0 && new_y < m_mazeHeight && 
				m_mazeGrid[new_x,new_y] != true)
			{
				m_mazeGrid [_xParameter,_yParameter] = true;
				m_mazeGrid [inbetween_x,inbetween_y] = true;
				m_mazeGrid [new_x,new_y] = true;

				List<Vector2> directionCopy = new List<Vector2> ();
				directionCopy.AddRange(directions); 

                CarvePassageFrom(new_x,new_y, directionCopy);
			}
		}

	}

	void CreateMazeBlocks()
	{
		Vector3 elementPosition = Vector3.zero  ;
		GameObject elementObject = null;
		int counter_width = 0; 
		int counter_height = 0; 

		while(counter_width < m_mazeWidth) 
		{
			while (counter_height < m_mazeHeight) 
			{
				if (m_mazeGrid [counter_width,counter_height] == false) 
				{
					elementPosition.x = (counter_width - (m_mazeWidth / 2)) * m_distanceMazeBlocks;
					elementPosition.z = (counter_height - (m_mazeHeight / 2)) * m_distanceMazeBlocks;

					elementObject = Instantiate (m_mazeBlock, elementPosition, m_mazeBlock.transform.rotation);

					m_mazeObjects.Add (elementObject);

					elementObject.name = "MazeBlock_";

					elementObject.name += counter_width + "_" + counter_height;
				}

				counter_height++;
			}

			counter_height = 0;
			counter_width++;

		}
	}

	public static List<Vector2> Fisher_Yates_Shuffle (List<Vector2>listToShuffle) 
	{	
		Vector2 vector;

		int count = listToShuffle.Count;
		for (int index = 0; index < count; index++)
		{
			int randomIndex = index + (int)(Random.value * (count - index));
			vector = listToShuffle[randomIndex];
			listToShuffle[randomIndex] = listToShuffle[index];
			listToShuffle[index] = vector;
		}

		return listToShuffle;
	}

	public void ClearMaze()
	{
		if(m_mazeObjects != null)
		{
			foreach (GameObject mazeBlock in m_mazeObjects) {
				DestroyImmediate (mazeBlock);
			}
			m_mazeObjects.Clear ();
		}
	}
}
