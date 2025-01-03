﻿using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.TheCastle
{
    public class PlacedObject_Done : MonoBehaviour
    {
        public static PlacedObject_Done Create(Vector3 worldPosition, PlacedObjectTypeSO placedObjectTypeSO, Vector3 objectScalling)
        {
            Transform placedObjectTransform = Instantiate(placedObjectTypeSO.prefab, worldPosition, Quaternion.identity);
            placedObjectTransform.localScale = objectScalling;
            placedObjectTransform.GetComponent<NetworkObject>().Spawn();


            PlacedObject_Done placedObject = placedObjectTransform.GetComponent<PlacedObject_Done>();
            placedObject.Setup(placedObjectTypeSO);

            return placedObject;
        }
        //[Rpc(SendTo.Server)]
        //public static PlacedObject_Done CreateServerRpc(Vector3 worldPosition, PlacedObjectTypeSO placedObjectTypeSO, Vector3 objectScalling)
        //{
        //    Transform placedObjectTransform = Instantiate(placedObjectTypeSO.prefab, worldPosition, Quaternion.identity);
        //    placedObjectTransform.GetComponent<NetworkObject>().Spawn();

        //    placedObjectTransform.localScale = objectScalling;

        //    PlacedObject_Done placedObject = placedObjectTransform.GetComponent<PlacedObject_Done>();
        //    placedObject.Setup(placedObjectTypeSO);

        //    return placedObject;
        //}
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

        //public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        //{
        //    serializer.SerializeValue(ref placedObjectTypeSO);
        //}
    }
}
