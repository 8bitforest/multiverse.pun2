using System.Threading.Tasks;
using Photon.Pun;
using Reaction;

namespace Multiverse.Pun2
{
    public class Pun2MvLibraryServer : MonoBehaviourPunCallbacks, IMvLibraryServer
    {
        RxnEvent IMvLibraryServer.OnDisconnected { get; } = new RxnEvent();
        
        public override void OnLeftRoom()
        {
            ((IMvLibraryServer) this).OnDisconnected.AsOwner.Invoke();
        }

        public async Task Disconnect()
        {
            if (!PhotonNetwork.InRoom)
                return;
            
            PhotonNetwork.LeaveRoom();
            await ((IMvLibraryServer) this).OnDisconnected.Wait(Multiverse.Timeout);
        }
    }
}