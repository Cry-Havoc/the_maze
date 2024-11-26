using System.Collections;
using System.Collections.Generic;
using UnityEngine; 


public class MazeGenerator_3d : MonoBehaviour {

	[SerializeField] GameObject m_mazeBlock;
	[SerializeField] float m_distanceMazeBlocks;

	[SerializeField] int m_mazeWidth;
	[SerializeField] int m_mazeHeight;
	[SerializeField] int m_mazeLength;

	bool[,,] m_mazeGrid;
  
	void Start () {
 
		m_mazeGrid = new bool[m_mazeWidth,m_mazeHeight,m_mazeLength];

		GenerateMaze ();
	}


	void GenerateMaze()
	{
		CarvePassageFrom (0,0,0);
		CreateMazeBlocks ();
	}

	//worker function
	void CarvePassageFrom(int _xParameter, int _yParameter , int _zParameter)
	{
		List<Vector3> directions;
		directions = new List<Vector3> { Vector3.up, Vector3.down, Vector3.left, Vector3.right , Vector3.forward , Vector3.back};
		directions = Fisher_Yates_Shuffle (directions);

		foreach (Vector3 direction in directions) {

			int new_x = _xParameter + (int)direction.x * 2;
			int new_y = _yParameter + (int)direction.y * 2;
			int new_z = _zParameter + (int)direction.z * 2;

			int inbetween_x = _xParameter + (int)direction.x;
			int inbetween_y = _yParameter + (int)direction.y;
			int inbetween_z = _zParameter + (int)direction.z;

			if (new_x >= 0 && new_x < m_mazeWidth  && 
				new_y >= 0 && new_y < m_mazeHeight && 
				new_z >= 0 && new_z < m_mazeHeight && 
				m_mazeGrid[new_x,new_y,new_z] != true)
			{
				m_mazeGrid [_xParameter,_yParameter,_zParameter] = true;
				m_mazeGrid [inbetween_x,inbetween_y,inbetween_z] = true;
				m_mazeGrid [new_x,new_y,new_z] = true;

				CarvePassageFrom(new_x,new_y,new_z);
			}
		}
	}

	void CreateMazeBlocks()
	{
		Vector3 elementPosition = Vector3.zero  ;
		GameObject elementObject = null;
		int counter_width = 0; 
		int counter_height = 0; 
		int counter_length = 0; 

		while(counter_width < m_mazeWidth) 
		{
			while (counter_height < m_mazeHeight) 
			{
				while (counter_length < m_mazeLength) {
					
					if (m_mazeGrid [counter_width, counter_height, counter_length] == false) 
					{	
						elementPosition.x = (counter_width - (m_mazeWidth / 2)) * m_distanceMazeBlocks;
						elementPosition.y = (counter_height - (m_mazeHeight / 2)) * m_distanceMazeBlocks;
						elementPosition.z = (counter_length - (m_mazeLength / 2)) * m_distanceMazeBlocks;

						elementObject = Instantiate (m_mazeBlock, elementPosition, m_mazeBlock.transform.rotation);
						elementObject.name = "MazeBlock_";

						elementObject.name += counter_width + "_" + counter_height + "_" + counter_length;
					}

					counter_length++;
				}

				counter_length = 0;
				counter_height++;
			}

			counter_height = 0;
			counter_width++;

		}
	}

	public static List<Vector3> Fisher_Yates_Shuffle (List<Vector3>listToShuffle) 
	{	
		Vector3 vector;

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
}
