using Multiverse.Tests;
using Multiverse.Tests.Utils;
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
    public class LibraryHostNotCreatedTests : HostNotCreatedTests { }

    [TestFixture]
    public class LibraryHostCreatedTests : HostCreatedTests { }

    [TestFixture]
    public class LibraryClientMessageTests : ClientMessageTests { }

    [TestFixture]
    public class LibraryHostMessageTests : HostMessageTests { }

    [TestFixture]
    public class LibraryClientUniverseTests : ClientUniverseTests { }

    [TestFixture]
    public class LibraryHostUniverseTests : HostUniverseTests { }

    public class Pun2LibraryAdder : IMvTestLibraryAdder
    {
        public void AddLibrary(GameObject gameObject)
        {
            gameObject.AddComponent<Pun2MvLibrary>();
        }
    }
}