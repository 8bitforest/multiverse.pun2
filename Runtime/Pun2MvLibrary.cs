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

        public IMvLibraryMatchmaker GetMatchmaker()
        {
            return GetComponent<Pun2MvLibraryMatchmaker>();
        }
    }
}