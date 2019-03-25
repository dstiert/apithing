using apithing;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [Test]
        public void DoublerTest()
        {
            Assert.That(Doubler.Double(2), Is.EqualTo(4));
        }
    }
}