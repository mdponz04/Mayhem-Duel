using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.TheCastle
{
    public class PlacedObject_Done : MonoBehaviour
    {
        public static PlacedObject_Done Create(Vector3 worldPosition, PlacedObjectTypeSO placedObjectTypeSO, Vector3 objectScalling)
        {
            Transform placedObjectTransform = Instantiate(placedObjectTypeSO.prefab, worldPosition, Quaternion.identity);

            placedObjectTransform.localScale = objectScalling;

            PlacedObject_Done placedObject = placedObjectTransform.GetComponent<PlacedObject_Done>();
            placedObject.Setup(placedObjectTypeSO);

            return placedObject;
        }

        private PlacedObjectTypeSO placedObjectTypeSO;
        
        private void Setup(PlacedObjectTypeSO placedObjectTypeSO)
        {
            this.placedObjectTypeSO = placedObjectTypeSO;
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }

        public override string ToString()
        {
            return placedObjectTypeSO.nameString;
        }
    }
}
