using System;
using HeroesGame.Constant;
using HeroesGame.Contract;
using HeroesGame.Implementation.Hero;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace HeroesGame.Tests
{
    [TestFixture]
    public class HeroTests
    {
        private Mock<IWeapon> weaponMock;
        private Mock<BaseHero> heroMock;
        private BaseHero hero;

        [SetUp]
        public void SetUp()
        {
            weaponMock = new Mock<IWeapon>();

            heroMock = new Mock<BaseHero>(weaponMock.Object);

            hero = heroMock.Object;
        }

        [Test]
        public void Constructor_ShouldSetInitialValuesCorrectly()
        {
            Assert.That(hero.Level, Is.EqualTo(HeroConstants.InitialLevel));
            Assert.That(hero.Experience, Is.EqualTo(HeroConstants.InitialExperience));
            Assert.That(hero.MaxHealth, Is.EqualTo(HeroConstants.InitialMaxHealth));
            Assert.That(hero.Health, Is.EqualTo(HeroConstants.InitialMaxHealth));
            Assert.That(hero.Armor, Is.EqualTo(HeroConstants.InitialArmor));
            Assert.That(hero.Weapon, Is.EqualTo(weaponMock.Object));
        }

        [TestCase(10)]
        [TestCase(20)]
        [TestCase(30)]
        public void TakeHit_ShouldDecreaseHealthCorrectly(double damage)
        {
            double initialHealth = hero.Health;
            double expectedFinalDamage = damage - hero.Armor;
            double expectedHealth = initialHealth - expectedFinalDamage;

            double result = hero.TakeHit(damage);

            Assert.That(result, Is.EqualTo(expectedFinalDamage));
            Assert.That(hero.Health, Is.EqualTo(expectedHealth));
        }

        [TestCase(-1)]
        [TestCase(-10)]
        [TestCase(-100)]
        public void TakeHit_NegativeDamage_ShouldThrowArgumentException(double damage)
        {
            Assert.Throws<ArgumentException>(() => hero.TakeHit(damage));
        }

        [Test]
        public void GainExperience_ShouldIncreaseExperienceCorrectly()
        {
            double xpToGain = 50;
            double expectedXp = hero.Experience + xpToGain;

            hero.GainExperience(xpToGain);

            Assert.That(hero.Experience, Is.EqualTo(expectedXp));
        }

        [Test]
        public void GainExperience_WhenMaximumExperienceIsReached_ShouldResetExperienceRemainder()
        {
            double xpToGain = HeroConstants.MaximumExperience + 25;

            hero.GainExperience(xpToGain);

            Assert.That(hero.Experience, Is.EqualTo(25));
        }

        [Test]
        public void GainExperience_WhenMaximumExperienceIsReached_ShouldCallLevelUp()
        {
            heroMock.Protected()
                .Setup("LevelUp");

            hero.GainExperience(HeroConstants.MaximumExperience);

            heroMock.Protected()
                .Verify("LevelUp", Times.Once());
        }

        [Test]
        public void GainExperience_WhenExperienceIsBelowMaximum_ShouldNotCallLevelUp()
        {
            heroMock.Protected()
                .Setup("LevelUp");

            hero.GainExperience(HeroConstants.MaximumExperience - 1);

            heroMock.Protected()
                .Verify("LevelUp", Times.Never());
        }

        [Test]
        public void Heal_ShouldIncreaseHealthCorrectly()
        {
            hero.TakeHit(20);

            double healthBeforeHeal = hero.Health;
            double expectedHealth = healthBeforeHeal + hero.Level * HeroConstants.HealPerLevel;

            if (expectedHealth > hero.MaxHealth)
            {
                expectedHealth = hero.MaxHealth;
            }

            hero.Heal();

            Assert.That(hero.Health, Is.EqualTo(expectedHealth));
        }

        [Test]
        public void Heal_ShouldNotIncreaseHealthAboveMaxHealth()
        {
            hero.TakeHit(1);

            hero.Heal();

            Assert.That(hero.Health, Is.LessThanOrEqualTo(hero.MaxHealth));
        }

        [Test]
        public void IsDead_ShouldReturnFalse_WhenHealthIsAboveZero()
        {
            Assert.That(hero.IsDead(), Is.False);
        }

        [Test]
        public void IsDead_ShouldReturnTrue_WhenHealthIsZeroOrLess()
        {
            hero.TakeHit(hero.Health + hero.Armor + 1);

            Assert.That(hero.IsDead(), Is.True);
        }
    }
}