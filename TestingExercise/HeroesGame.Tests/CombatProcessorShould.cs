using HeroesGame.Constant;
using HeroesGame.Contract;
using HeroesGame.Implementation.Hero;
using HeroesGame.Implementation.Monster;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroesGame.Tests
{
    [TestFixture]
    public class CombatProcessorShould
    {
        private CombatProcessor _combatProcessor;

        [SetUp]
        public void Setup()
        {
            this._combatProcessor = new CombatProcessor(new Hunter());
        }

        [Test]
        public void InitializeCorrectly()
        {
            //Assert
            Assert.That(_combatProcessor.Hero, Is.Not.Null);
            Assert.That(_combatProcessor.Logger, Is.Not.Null);
            Assert.That(_combatProcessor.Logger, Is.Empty);
        }
        
        [Test]
        public void FightCorrectly_WeakerEnemy()
        {
            //Arrange
            IMonster monster = new MedusaTheGorgon(1);
            this.LevelUp(50);

            //Act
            _combatProcessor.Fight(monster);
            var logger = _combatProcessor.Logger;

            //Assert
            Assert.That(logger.Count, Is.EqualTo(2));
            Assert.That(logger, Does.Contain("The monster dies. (4 XP gained.)").And.Contain("The Hunter hits the MedusaTheGorgon dealing 510 damage to it."));
        }

        [Test]
        public void FightCorrectly_StrongerEnemy()
        {
            //Arrange
            IMonster monster = new MedusaTheGorgon(50);

            //Act
            _combatProcessor.Fight(monster);
            var logger = _combatProcessor.Logger;

            //Assert
            Assert.That(logger, Has.Count.EqualTo(12));
            Assert.That(logger, Does.Contain("The hero dies on level 1 after 4 fights."));
            Assert.That(_combatProcessor.Hero.IsDead, Is.True);
        }

        public void LevelUp(int level)
        {
            for (int i = 0; i < level; i++)
            {
                this._combatProcessor.Hero.GainExperience(HeroConstants.MaximumExperience);
            }
        }
    }
}
