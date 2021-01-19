using Multiverse.Tests;
using NUnit.Framework;
using UnityEngine;

namespace Multiverse.Pun2.Tests
{
    [SetUpFixture]
    public class LibraryTestSetUp : MultiverseTestSetUp<Pun2LibraryAdder> { }

    [TestFixture]
    public class LibraryMatchmakerTests : MatchmakerTests { }

    [TestFixture]
    public class LibraryClientNotJoinedTests : ClientNotJoinedTests { }

    [TestFixture]
    public class LibraryClientJoinedTests : ClientJoinedTests { }

    [TestFixture]
    public class LibraryServerNotCreatedTests : ServerNotCreatedTests { }

    [TestFixture]
    public class LibraryServerCreatedTests : ServerCreatedTests { }

    public class Pun2LibraryAdder : IMvTestLibraryAdder
    {
        public void AddLibrary(GameObject gameObject)
        {
            gameObject.AddComponent<Pun2MvLibrary>();
        }
    }
}