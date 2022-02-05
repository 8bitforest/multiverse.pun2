using Multiverse.LibraryInterfaces;
using Photon.Pun;

namespace Multiverse.Pun2
{
    public class Pun2MvLibraryHost : MonoBehaviourPunCallbacks, IMvLibraryHost
    {
        public int HostLibId => PhotonNetwork.LocalPlayer.ActorNumber;
    }
}