using Multiverse.LibraryInterfaces;
using Multiverse.Tests.Backend;
using UnityEngine;

namespace Multiverse.Pun2.Tests
{
    public class Pun2MvLibraryTestSuite : IMvLibraryTestSuite
    {
        public string Name => "Pun2";

        public IMvLibrary AddLibrary(GameObject gameObject)
        {
            return gameObject.AddComponent<Pun2MvLibrary>();
        }
    }
}