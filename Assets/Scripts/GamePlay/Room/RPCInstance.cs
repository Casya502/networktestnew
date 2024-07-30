using System;
using FishNet.Object;

namespace GamePlay.Room
{
    public class RPCInstance : NetworkBehaviour
    {
        public static RPCInstance Instance { get; private set; }

        private void Awake()
        {
            if (Instance !=null)
            {
                Destroy(gameObject);
            }
            Instance = this;
        }
    }
}