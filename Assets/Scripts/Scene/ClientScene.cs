using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Steamworks
{
    public class ClientScene : NetworkBehaviour
    {
        public static List<GameObject> networkObjects = new List<GameObject>();
        public static void RegisterPrefab(GameObject objects)
        {
            Debug.Log(objects.name);
            networkObjects.Add(objects);
        }
    }
}
