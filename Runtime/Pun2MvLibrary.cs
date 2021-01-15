using Photon.Pun;
using UnityEngine;

namespace Multiverse.Pun2
{
    [RequireComponent(typeof(Pun2MvLibraryMatchmaker))]
    public class Pun2MvLibrary : MonoBehaviour, IMvLibrary
    {
        private void Awake()
        {
            PhotonNetwork.Disconnect();
        }

        public IMvLibraryServer GetServer()
        {
            return gameObject.AddComponent<Pun2MvLibraryServer>();
        }

        public IMvLibraryClient GetClient()
        {
            return gameObject.AddComponent<Pun2MvLibraryClient>();
        }

        public IMvLibraryMatchmaker GetMatchmaker()
        {
            return GetComponent<Pun2MvLibraryMatchmaker>();
        }

        public void CleanupAfterDisconnect() { }
    }
}