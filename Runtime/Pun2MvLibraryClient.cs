using Photon.Pun;
using Photon.Realtime;
using Reaction;

namespace Multiverse.Pun2
{
    public class Pun2MvLibraryClient : MonoBehaviourPunCallbacks, IMvLibraryClient
    {
        RxnEvent IMvLibraryClient.OnDisconnected { get; } = new RxnEvent();

        public void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            ((IMvLibraryClient) this).OnDisconnected.AsOwner.Invoke();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            MvNetworkManager.I.Client.Disconnect();
        }
    }
}