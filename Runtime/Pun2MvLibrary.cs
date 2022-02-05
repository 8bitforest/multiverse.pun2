using Multiverse.LibraryInterfaces;
using Photon.Pun;
using UnityEngine;

namespace Multiverse.Pun2
{
    [RequireComponent(typeof(Pun2MvLibraryMatchmaker))]
    public class Pun2MvLibrary : MonoBehaviour, IMvLibrary
    {
        private Pun2MvLibraryHost _host;
        private Pun2MvLibraryServer _server;
        private Pun2MvLibraryClient _client;
        private void Awake()
        {
            PhotonNetwork.Disconnect();
        }

        public IMvLibraryHost GetHost()
        {
            return _host = gameObject.AddComponent<Pun2MvLibraryHost>();
        }

        public IMvLibraryServer GetServer()
        {
            return _server =gameObject.AddComponent<Pun2MvLibraryServer>();
        }

        public IMvLibraryClient GetClient()
        {
            return _client = gameObject.AddComponent<Pun2MvLibraryClient>();
        }

        public IMvLibraryMatchmaker GetMatchmaker()
        {
            return GetComponent<Pun2MvLibraryMatchmaker>();
        }

        public void CleanupAfterDisconnect()
        {
            Destroy(_host);
            Destroy(_server);
            Destroy(_client);
            _host = null;
            _server = null;
            _client = null;
        }

        public void SetTimeout(float seconds)
        {
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout = (int) (seconds * 1000);
        }
    }
}