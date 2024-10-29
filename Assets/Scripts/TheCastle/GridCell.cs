using Assets.Scripts.TheCastle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheCastle
{
	public class GridCell
	{

		private GridSystem grid;	//which grid to call EventHandler
		private int x;				//position x in grid array
		private int y;              //position y in grid array
		public PlacedObject_Done placedObject;	//handle the scriptable object

        public Vector3 gridCenterPosition { get; set; }
		public bool isOccupied { get; set; }
		
		public GridCell(Vector3 gridCenterPosition, GridSystem grid, int x, int y)
		{
			this.gridCenterPosition = gridCenterPosition;
			this.isOccupied = false;

			this.grid = grid;
			this.x = x;
			this.y = y;
		}

		public void SetPlacedObject(PlacedObject_Done placedObject)
		{
			this.placedObject = placedObject;
			grid.TriggerGridObjectChanged(x, y);
		}

		public PlacedObject_Done GetPlacedObject()
		{
			return placedObject;
		}

		public void ClearPlacedObject()
		{
			placedObject = null;
			grid.TriggerGridObjectChanged(x, y);
		}

		public bool CanBuild()
		{
			return placedObject == null;
		}

        public override string ToString()
        {
            return x + ", " + y + "\n" + placedObject?.ToString();
        }
    }
}
