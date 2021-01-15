using Photon.Pun;
using Photon.Realtime;
using Reaction;

namespace Multiverse.Pun2
{
    public class Pun2MvLibraryServer : MonoBehaviourPunCallbacks, IMvLibraryServer
    {
        RxnEvent IMvLibraryServer.OnDisconnected { get; } = new RxnEvent();

        public void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            ((IMvLibraryServer) this).OnDisconnected.AsOwner.Invoke();
        }
    }
}