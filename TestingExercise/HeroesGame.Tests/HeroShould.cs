using HeroesGame.Constant;
using HeroesGame.Contract;
using HeroesGame.Implementation.Hero;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroesGame.Tests
{
    [TestFixture]
    public class HeroShould
    {
        private Mock<Mage> _hero;

        [SetUp]
        public void Setup()
        {
            //Arrange
            //this._hero = new Mage();
            this._hero = new Mock<Mage>();
            this._hero.Protected()
                .Setup("LevelUp")
                .CallBase();
        }

        [Test]
        public void HaveCorrectInitialValues()
        {

            //Assert
            Assert.That(_hero.Object.Level, Is.EqualTo(HeroConstants.InitialLevel), "The initial level is not the same as expected.");
            Assert.That(_hero.Object.Experience, Is.EqualTo(HeroConstants.InitialExperience));
            Assert.That(_hero.Object.MaxHealth, Is.EqualTo(HeroConstants.InitialMaxHealth));
            Assert.That(_hero.Object.Health, Is.EqualTo(HeroConstants.InitialMaxHealth));
            Assert.That(_hero.Object.Armor, Is.EqualTo(HeroConstants.InitialArmor));
            Assert.That(_hero.Object.Weapon, Is.Not.Null);

        }

        [Test]
        public void TakeHitCorrectly([Values(20, 40, 70, 2, 5, 16)] int damage)
        {

            //Act
            _hero.Object.TakeHit(damage);

            //Assert
            Assert.That(_hero.Object.Health, Is.EqualTo(HeroConstants.InitialMaxHealth - damage + HeroConstants.InitialArmor));
        }

        [Test]
        public void ThrowExceptionForNegativeTakeHitValue([Range(-50, -10, 5)] int damage)
        {
            //Assert
            Assert.Throws<ArgumentException>(() => _hero.Object.TakeHit(damage));
        }

        [Test]
        public void GainExperienceCorrectly([Range(0, 500, 25)] double xp)
        {
            //Act
            _hero.Object.GainExperience(xp);

            //Assert

            if (xp >= HeroConstants.MaximumExperience)
            {
                var expectedXp = (HeroConstants.InitialExperience + xp) % HeroConstants.MaximumExperience;
                Assert.That(_hero.Object.Experience, Is.EqualTo(expectedXp));
                Assert.That(_hero.Object.Level, Is.EqualTo(HeroConstants.InitialLevel + 1));
            }
            else
            {
                Assert.That(_hero.Object.Experience, Is.EqualTo(HeroConstants.InitialExperience + xp));
            }
        }

        [Test]
        public void HealCorrectly([Range(5,25,1)]int level, [Range(25,500,25)]int damage)
        {
            //Act
            this.LevelUp(level);

            double totalDamage = HeroConstants.InitialMaxHealth + damage;
            totalDamage = _hero.Object.TakeHit(totalDamage);
            this._hero.Object.Heal();

            //Assert
            var healValue = this._hero.Object.Level * HeroConstants.HealPerLevel;
            var expectedHealth = (this._hero.Object.MaxHealth - totalDamage) + healValue;

            if (expectedHealth > _hero.Object.MaxHealth)
            {
                expectedHealth = _hero.Object.MaxHealth;
            }

            Assert.That(_hero.Object.Health, Is.EqualTo(expectedHealth));
        }

        public void LevelUp(int level)
        {
            for(int i = 0; i < level; i++)
            {
                this._hero.Object.GainExperience(HeroConstants.MaximumExperience);
            }
        }
        
        [Test]
        public void NotBeBornDead()
        {
            //Act
            var isDead = this._hero.Object.IsDead();
            //Assert
            Assert.That(isDead, Is.False);
        }

        [Test]
        public void BeDeadWhenCriticallyHit([Range(50,150,25)]double damage)
        {
            //Act
            var totalDamage = _hero.Object.TakeHit(damage);
            //Assert
            if(totalDamage >= _hero.Object.MaxHealth)
            {
                Assert.That(_hero.Object.IsDead, Is.True);
            }
            else
            {
                Assert.That(this._hero.Object.IsDead, Is.False);
            }
        }


    }
}
