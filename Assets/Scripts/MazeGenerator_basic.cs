using System.Collections;
using System.Collections.Generic;
using UnityEngine; 


public class MazeGenerator_basic : MonoBehaviour {

	[SerializeField] GameObject m_mazeBlock;
	[SerializeField] float m_distanceMazeBlocks;

	[SerializeField] int m_mazeWidth;
	[SerializeField] int m_mazeHeight;

	/// <summary>
	/// this is the internal data for our maze, TRUE = open field , False = wall
	/// </summary>
	bool[,] m_mazeGrid; 

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () {

		///create a new two dimensional array 
		m_mazeGrid = new bool[m_mazeWidth,m_mazeHeight];
		 
		GenerateMaze ();
	}

	/// <summary>
	/// Generates the maze data and then place the blocks afterwards
	/// </summary>
	void GenerateMaze()
	{
        ///create a list with four direction vectors
        List<Vector2> directions = new List<Vector2> { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        ///randomize the directions
        directions = Fisher_Yates_Shuffle(directions);

        ///call our worker function from one corner of the maze
        CarvePassageFrom(1, 1, directions);
		///create 3d objects everywhere where a wall should be
		CreateMazeBlocks ();
	}

	/// <summary>
	/// This is our worker function, which is called over and over again
	/// This function checks if there is already a free space in each direction
	/// If not it creates a passage to the next space and calls itself again (recursion)
	/// </summary>
	void CarvePassageFrom(int _xParameter, int _yParameter, List<Vector2> directions)
	{
        directions = Fisher_Yates_Shuffle(directions);

        ///for each direction check if the field after the next field is still a wall,
        ///if so remove the wall on the next field and the wall after the next field
        ///then start to carve passages from the new field
        foreach (Vector2 direction in directions) {
		
			int new_x = _xParameter + (int)direction.x * 2;
			int new_y = _yParameter + (int)direction.y * 2;

			int inbetween_x = _xParameter + (int)direction.x;
			int inbetween_y = _yParameter + (int)direction.y;

			///check if the new position is outside of the grid
			///and was not already visited
			if (new_x >= 0 && new_x < m_mazeWidth  && 
				new_y >= 0 && new_y < m_mazeHeight && 
				m_mazeGrid[new_x,new_y] != true)
			{
				m_mazeGrid [_xParameter,_yParameter] = true;
				m_mazeGrid [inbetween_x,inbetween_y] = true;
				m_mazeGrid [new_x,new_y] = true;

                List<Vector2> directionCopy = new List<Vector2>();
                directionCopy.AddRange(directions);

                CarvePassageFrom(new_x,new_y, directionCopy);
			}
		}
	}

	/// <summary>
	/// Itterate through the maze grid and place walls  
	/// </summary>
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
					elementObject.name = "MazeBlock_";

					elementObject.name += counter_width + "_" + counter_height;
				}

				counter_height++;
			}

			counter_height = 0;
			counter_width++;

		}
	}

	/// <summary>
	/// Shuffle a List of Vectors randomly
	/// </summary>
	public static List<Vector2> Fisher_Yates_Shuffle (List<Vector2>listToShuffle) 
	{	
		Vector2 vector;

		int count = listToShuffle.Count;
		for (int index = 0; index < count; index++)
		{
			///pick a random index and replace the vectors from the current index with the random index
			int randomIndex = index + (int)(Random.value * (count - index));
			vector = listToShuffle[randomIndex];
			listToShuffle[randomIndex] = listToShuffle[index];
			listToShuffle[index] = vector;
		}

		///return the shuffled list
		return listToShuffle;
	}


}
