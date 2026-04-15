using HeroesGame.Constant;
using HeroesGame.Implementation.Weapon;
using Moq;
using NUnit.Framework;

namespace HeroesGame.Tests
{
    [TestFixture]
    public class WeaponTests
    {
        private Mock<BaseWeapon> weaponMock;
        private BaseWeapon weapon;

        [SetUp]
        public void SetUp()
        {
            weaponMock = new Mock<BaseWeapon>();
            weapon = weaponMock.Object;
        }

        [Test]
        public void Constructor_ShouldSetInitialValuesCorrectly()
        {
            Assert.That(weapon.Damage, Is.EqualTo(WeaponConstants.InitialDamage));
            Assert.That(weapon.Level, Is.EqualTo(WeaponConstants.InitialLevel));
        }

        [Test]
        public void LevelUp_ShouldIncreaseLevelCorrectly()
        {
            int initialLevel = weapon.Level;

            weapon.LevelUp();

            Assert.That(weapon.Level, Is.EqualTo(initialLevel + 1));
        }

        [Test]
        public void LevelUp_ShouldIncreaseDamageCorrectly()
        {
            int initialDamage = weapon.Damage;

            weapon.LevelUp();

            Assert.That(weapon.Damage, Is.EqualTo(initialDamage + WeaponConstants.DamagePerLevel));
        }

        [Test]
        public void LevelUp_CalledMultipleTimes_ShouldIncreaseCorrectly()
        {
            int initialLevel = weapon.Level;
            int initialDamage = weapon.Damage;

            weapon.LevelUp();
            weapon.LevelUp();

            Assert.That(weapon.Level, Is.EqualTo(initialLevel + 2));
            Assert.That(weapon.Damage, Is.EqualTo(initialDamage + 2 * WeaponConstants.DamagePerLevel));
        }

        [Test]
        public void ArmorPenetration_ShouldBeCallable()
        {
            weaponMock.Setup(w => w.ArmorPenetration()).Returns(5);

            int result = weapon.ArmorPenetration();

            Assert.That(result, Is.EqualTo(5));
        }
    }
}