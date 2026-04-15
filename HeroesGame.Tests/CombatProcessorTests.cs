using System.Linq;
using HeroesGame;
using HeroesGame.Contract;
using HeroesGame.Implementation.Hero;
using Moq;
using NUnit.Framework;

namespace HeroesGame.Tests
{
    [TestFixture]
    public class CombatProcessorTests
    {
        private Mock<IWeapon> weaponMock;
        private Mock<IMonster> monsterMock;
        private TestHero hero;
        private CombatProcessor combatProcessor;

        [SetUp]
        public void SetUp()
        {
            weaponMock = new Mock<IWeapon>();
            weaponMock.Setup(w => w.Damage).Returns(20);
            weaponMock.Setup(w => w.ArmorPenetration()).Returns(0);

            monsterMock = new Mock<IMonster>();

            hero = new TestHero(weaponMock.Object);
            combatProcessor = new CombatProcessor(hero);
        }

        [Test]
        public void Constructor_ShouldSetInitialValuesCorrectly()
        {
            Assert.That(combatProcessor.Hero, Is.EqualTo(hero));
            Assert.That(combatProcessor.Logger, Is.Not.Null);
            Assert.That(combatProcessor.Logger, Is.Empty);
        }

        [Test]
        public void Fight_WhenMonsterDiesFromFirstHit_ShouldAddLogsAndGainExperience()
        {
            monsterMock.Setup(m => m.TakeHit(weaponMock.Object)).Returns(20);
            monsterMock.Setup(m => m.IsDead()).Returns(true);
            monsterMock.Setup(m => m.Experience()).Returns(50);

            double initialXp = hero.Experience;

            combatProcessor.Fight(monsterMock.Object);

            Assert.That(hero.Experience, Is.EqualTo(initialXp + 50));
            Assert.That(combatProcessor.Logger.Count, Is.EqualTo(2));
            Assert.That(combatProcessor.Logger[0], Does.Contain("hits"));
            Assert.That(combatProcessor.Logger[1], Does.Contain("dies"));
        }

        [Test]
        public void Fight_WhenMonsterSurvives_ShouldLogMonsterHit()
        {
            monsterMock.SetupSequence(m => m.IsDead())
                .Returns(false)
                .Returns(true);

            monsterMock.Setup(m => m.TakeHit(weaponMock.Object)).Returns(20);
            monsterMock.Setup(m => m.Hit(hero)).Returns(10);
            monsterMock.Setup(m => m.Experience()).Returns(30);

            combatProcessor.Fight(monsterMock.Object);

            Assert.That(combatProcessor.Logger.Any(x => x.Contains("dealing 10 damage")), Is.True);
        }

        [Test]
        public void Fight_WhenMonsterEventuallyDies_ShouldGainExperience()
        {
            monsterMock.SetupSequence(m => m.IsDead())
                .Returns(false)
                .Returns(true);

            monsterMock.Setup(m => m.TakeHit(weaponMock.Object)).Returns(20);
            monsterMock.Setup(m => m.Hit(hero)).Returns(5);
            monsterMock.Setup(m => m.Experience()).Returns(60);

            double initialXp = hero.Experience;

            combatProcessor.Fight(monsterMock.Object);

            Assert.That(hero.Experience, Is.EqualTo(initialXp + 60));
        }

        private class TestHero : BaseHero
        {
            public TestHero(IWeapon weapon) : base(weapon)
            {
            }

            protected override void LevelUp()
            {
                this.Level++;
            }
        }
    }
}