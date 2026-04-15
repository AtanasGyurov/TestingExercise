using HeroesGame.Constant;
using HeroesGame.Contract;
using HeroesGame.Implementation.Monster;
using Moq;
using NUnit.Framework;

namespace HeroesGame.Tests
{
    [TestFixture]
    public class MonsterTests
    {
        private Mock<BaseMonster> monsterMock;
        private Mock<IWeapon> weaponMock;
        private Mock<IHero> heroMock;
        private BaseMonster monster;

        [SetUp]
        public void SetUp()
        {
            monsterMock = new Mock<BaseMonster>(1);

            weaponMock = new Mock<IWeapon>();
            heroMock = new Mock<IHero>();

            monster = monsterMock.Object;
        }

        [Test]
        public void Constructor_ShouldSetInitialValuesCorrectly()
        {
            Assert.That(monster.Level, Is.EqualTo(1));
            Assert.That(monster.Health, Is.EqualTo(MonsterConstants.MaxHealthPerLevel * 1));
        }

        [Test]
        public void Hit_ShouldCallHeroTakeHit()
        {
            monsterMock.Setup(m => m.Damage()).Returns(20);
            heroMock.Setup(h => h.TakeHit(20)).Returns(20);

            double result = monster.Hit(heroMock.Object);

            Assert.That(result, Is.EqualTo(20));
        }

        [Test]
        public void TakeHit_ShouldDecreaseHealthCorrectly()
        {
            weaponMock.Setup(w => w.Damage).Returns(30);
            weaponMock.Setup(w => w.ArmorPenetration()).Returns(5);
            monsterMock.Setup(m => m.Armor()).Returns(10);

            double initialHealth = monster.Health;
            int expectedDamage = 30 + 5 - 10;
            double expectedHealth = initialHealth - expectedDamage;

            double result = monster.TakeHit(weaponMock.Object);

            Assert.That(result, Is.EqualTo(expectedDamage));
            Assert.That(monster.Health, Is.EqualTo(expectedHealth));
        }

        [Test]
        public void TakeHit_ShouldWorkWithDifferentValues()
        {
            weaponMock.Setup(w => w.Damage).Returns(50);
            weaponMock.Setup(w => w.ArmorPenetration()).Returns(10);
            monsterMock.Setup(m => m.Armor()).Returns(20);

            double initialHealth = monster.Health;

            monster.TakeHit(weaponMock.Object);

            Assert.That(monster.Health, Is.EqualTo(initialHealth - (50 + 10 - 20)));
        }

        [Test]
        public void IsDead_ShouldReturnFalse_WhenHealthIsAboveZero()
        {
            Assert.That(monster.IsDead(), Is.False);
        }

        [Test]
        public void IsDead_ShouldReturnTrue_WhenHealthIsZeroOrLess()
        {
            monster.Health = 0;

            Assert.That(monster.IsDead(), Is.True);
        }
    }
}