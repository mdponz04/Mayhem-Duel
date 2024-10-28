using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheCastle
{
	public class GridCell
	{
		public Vector3 gridCenterPosition { get; set; }
		public bool isOccupied { get; set; }
		
		public GridCell(Vector3 gridCenterPosition)
		{
			this.gridCenterPosition = gridCenterPosition;
			this.isOccupied = false;
		}

		
	}
}
