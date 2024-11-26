using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MazeGenerator_advanced : MonoBehaviour {

	[SerializeField] GameObject m_mazeBlock;

	/// <summary>
	/// this is a list with weights
	/// if we randomly pick an element of this list the heigher weights have a higher probability
	/// </summary>
    [SerializeField] WeightedList<GameObject> m_mazeBlocksWeighted;  
    
    [SerializeField] float m_distanceMazeBlocks;

	[SerializeField] int m_mazeWidth;
	[SerializeField] int m_mazeHeight;

	/// <summary>
	/// this is the internal data for our maze, TRUE = open field , False = wall
	/// </summary>
	bool[,] m_mazeGrid;

    /// <summary>
    /// this is a delegate for all functions that use the same paramters and return values
	/// we can save in this delegate the functions
	/// Fisher_Yates_Shuffle - DoNotChangeList - LeftTurn - RightTurn
    /// </summary>
    public delegate List<Vector2> ChooseDirection(Vector2 lastDirection, List<Vector2> directions);

    /// <summary>
    /// example how to use delegates - the function Fisher_Yates_Shuffle was assigned to ChooseDirection
    /// </summary>
    public ChooseDirection dRandomDirecion = Fisher_Yates_Shuffle;

    /// <summary>
    /// this is a list with weights but with the delegate function choose direction as list elements
    /// with this setup we can choose a random weighted function
    /// </summary>
    WeightedList<ChooseDirection> m_weightedDirectionFunctions = new WeightedList<ChooseDirection>(); 
	 
	public float m_weightRandomDirection = 1.0f;
    public float m_weightSameDirection = 1.0f;
    public float m_weightLeftTurnDirection = 1.0f;
    public float m_weightRightTurnDirection = 1.0f;


    void Start () {

		///create a new two dimensional array 
		m_mazeGrid = new bool[m_mazeWidth,m_mazeHeight];

		///weighted list of block gameobjects must be initalized
        m_mazeBlocksWeighted.InitializeWeights();

		//weighted list of delegate functions has to be filled with the correct weights and initialized
		WeightedList<ChooseDirection>.Entry randomDirEntry = new WeightedList<ChooseDirection>.Entry(m_weightRandomDirection, Fisher_Yates_Shuffle);
        WeightedList<ChooseDirection>.Entry sameDirEntry = new WeightedList<ChooseDirection>.Entry(m_weightSameDirection, DoNotChangeList);
        WeightedList<ChooseDirection>.Entry spiralLeftDirEntry = new WeightedList<ChooseDirection>.Entry(m_weightLeftTurnDirection, LeftTurn);
        WeightedList<ChooseDirection>.Entry spiralRightDirEntry = new WeightedList<ChooseDirection>.Entry(m_weightRightTurnDirection, RightTurn); 

        m_weightedDirectionFunctions.entries.Add(randomDirEntry);
        m_weightedDirectionFunctions.entries.Add(sameDirEntry);
        m_weightedDirectionFunctions.entries.Add(spiralLeftDirEntry);
        m_weightedDirectionFunctions.entries.Add(spiralRightDirEntry);  
        m_weightedDirectionFunctions.InitializeWeights(); 

        GenerateMaze();
	}

	/// <summary>
	/// Generates the maze data and then place the blocks afterwards
	/// </summary>
	void GenerateMaze()
	{
        ///create a list with four direction vectors
        List<Vector2> directions = new List<Vector2> { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        ///randomize the directions
        directions = dRandomDirecion(directions[0], directions);

        ///call our worker function from one corner of the maze
        CarvePassageFrom(1, 1, directions[0], directions);
		///create 3d objects everywhere where a wall should be
		CreateMazeBlocks ();
	}

	/// <summary>
	/// This is our worker function, which is called over and over again
	/// This function checks if there is already a free space in each direction
	/// If not it creates a passage to the next space and calls itself again (recursion)
	/// </summary>
	void CarvePassageFrom(int _xParameter, int _yParameter, Vector2 lastDirection, List<Vector2> directions)
	{
        directions = dRandomDirecion(lastDirection, directions); 
        //directions = GetDirectionWeighted(lastDirection, directions);

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

                List<Vector2> directionListCopy = new List<Vector2>();
                directionListCopy.AddRange(directions);

                CarvePassageFrom(new_x,new_y, direction, directionListCopy);
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

					elementObject = Instantiate (GetMazeBlockWeighted(), elementPosition, m_mazeBlock.transform.rotation);
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
	public static List<Vector2> Fisher_Yates_Shuffle(Vector2 lastDirection, List<Vector2>listToShuffle) 
	{	
		Vector2 vector;

		int count = listToShuffle.Count;
		for (int index = 0; index < count; index++)
		{
			///pick a random index and replace the vectors from the current index with the random index
			int randomIndex = index + (int)(UnityEngine.Random.value * (count - index));
			vector = listToShuffle[randomIndex];
			listToShuffle[randomIndex] = listToShuffle[index];
			listToShuffle[index] = vector;
		}

		///return the shuffled list
		return listToShuffle;
	}

    public static List<Vector2> DoNotChangeList(Vector2 lastDirection, List<Vector2> vectorList)
    {  
        ///return the unshuffled list
        return vectorList;
    }

    public static List<Vector2> LeftTurn(Vector2 lastDirection, List<Vector2> vectorList)
    {
        if(lastDirection == Vector2.up)
		{
            vectorList = new List<Vector2> { Vector2.left, Vector2.up, Vector2.right, Vector2.down }; 
        }
		else if(lastDirection == Vector2.down) 
        {
            vectorList = new List<Vector2> { Vector2.right, Vector2.down, Vector2.left, Vector2.up };
        }
        else if (lastDirection == Vector2.left)
        {
            vectorList = new List<Vector2> { Vector2.down, Vector2.left, Vector2.up, Vector2.right };
        }
        else if (lastDirection == Vector2.right)
        {
            vectorList = new List<Vector2> { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
        } 

        return vectorList;
    }

    public static List<Vector2> RightTurn(Vector2 lastDirection, List<Vector2> vectorList)
    {
        if (lastDirection == Vector2.down)
        {
            vectorList = new List<Vector2> { Vector2.left, Vector2.up, Vector2.right, Vector2.down };
        }
        else if (lastDirection == Vector2.up)
        {
            vectorList = new List<Vector2> { Vector2.right, Vector2.down, Vector2.left, Vector2.up };
        }
        else if (lastDirection == Vector2.right)
        {
            vectorList = new List<Vector2> { Vector2.down, Vector2.left, Vector2.up, Vector2.right };
        }
        else if (lastDirection == Vector2.left)
        {
            vectorList = new List<Vector2> { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
        }

        return vectorList;
    }

    public GameObject GetMazeBlock()
	{
		return m_mazeBlock;
	}

    public GameObject GetMazeBlockWeighted()
    {
        return m_mazeBlocksWeighted.GetRandom();
    }

    public List<Vector2> GetDirectionWeighted(Vector2 lastDirection, List<Vector2> vectorList)
    {
		ChooseDirection weightedDirFunction = m_weightedDirectionFunctions.GetRandom(); 

        return weightedDirFunction(lastDirection,vectorList);
    }



}
