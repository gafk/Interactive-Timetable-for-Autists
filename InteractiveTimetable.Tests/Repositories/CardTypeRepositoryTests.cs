﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using InteractiveTimetable.BusinessLayer.Models;
using InteractiveTimetable.DataAccessLayer;
using NUnit.Framework;
using SQLite;

namespace InteractiveTimetable.Tests.Repositories
{
    [TestFixture]
    class CardTypeRepositoryTests
    {
        private SQLiteConnection _connection;
        private CardTypeRepository _repository;

        [SetUp]
        public void InitializationBeforeTests()
        {
            /* Creating connection */
            string dbPath = "TestsDatabase.db3";
            _connection = new SQLiteConnection(dbPath);

            /* Initialize repository */
            _repository= new CardTypeRepository(_connection);
        }

        [TearDown]
        public void ShutDown()
        {
            _connection.Dispose();
            File.Delete("TestsDatabase.db3");
        }

        [Test]
        public void CreateSimpleCardType()
        {
            // arrange
            CardType cardType = new CardType()
            {
                TypeName = "RANDOM_CARD"
            };

            // act
            int cardTypeId = _repository.SaveCardType(cardType);
            var addedCardType = _repository.GetCardType(cardTypeId);

            // assert
            Assert.AreEqual(cardType, addedCardType);
        }

        [Test]
        public void CreateCardTypeWithCards()
        {
            // arrange
            var _cardRepository = new CardRepository(_connection);
            Card card1 = new Card()
            {
                PhotoPath = "path/to/photo.jpg"
            };

            Card card2 = new Card()
            {
                PhotoPath = "path/to/photo2.jpg"
            };

            CardType cardType = new CardType()
            {
                TypeName = "RANDOM_CARD",
                Cards = new List<Card>
                {
                    card1,
                    card2
                }
            };

            // act
            int cardTypeId = _repository.SaveCardType(cardType);
            var addedCardType = _repository.GetCardType(cardTypeId);

            // assert
            Assert.AreEqual(cardType, addedCardType);
            Assert.AreEqual(2, addedCardType.Cards.Count);
        }

        [Test]
        public void GetCardTypes()
        {
            // arrange

            // act
            var cardTypes = _repository.GetCardTypes().ToList();
            var activityType = cardTypes.
                    FirstOrDefault(x => x.TypeName == "ACTIVITY_CARD");
            var motivationGoalType = cardTypes.
                    FirstOrDefault(x => x.TypeName == "MOTIVATION_GOAL_CARD");

            // assert
            Assert.AreEqual(2, cardTypes.Count);
            Assert.NotNull(activityType);
            Assert.NotNull(motivationGoalType);
        }

        [Test]
        public void GetActivityCardType()
        {
            // arrange
            var cardTypes = _repository.GetCardTypes().ToList();
            var expectedType = cardTypes.
                    FirstOrDefault(x => x.TypeName == "ACTIVITY_CARD");

            // act
            var actualType = _repository.GetActivityCardType();

            // assert
            Assert.AreEqual(expectedType, actualType);
        }

        [Test]
        public void GetMotivationGoalCardType()
        {
            // arrange
            var cardTypes = _repository.GetCardTypes().ToList();
            var expectedType = cardTypes.
                    FirstOrDefault(x => x.TypeName == "MOTIVATION_GOAL_CARD");

            // act
            var actualType = _repository.GetMotivationGoalCardType();

            // assert
            Assert.AreEqual(expectedType, actualType);
        }

        [Test]
        public void IsActivityCardType()
        {
            // arrange
            var activityType = _repository.GetActivityCardType();
            var motivationGoalType = _repository.GetMotivationGoalCardType();

            // act
            var isActivity
                    = _repository.IsActivityCardType(activityType.Id);
            var isNotActivity
                    = _repository.IsActivityCardType(motivationGoalType.Id);

            // assert
            Assert.AreEqual(true, isActivity);
            Assert.AreEqual(false, isNotActivity);
        }

        [Test]
        public void IsMotivationGoalCardType()
        {
            // arrange
            var activityType = _repository.GetActivityCardType();
            var motivationGoalType = _repository.GetMotivationGoalCardType();

            // act
            var isMotivationGoal
                    = _repository.IsMotivationGoalCardType(motivationGoalType.Id);
            var isNotMotivationGoal
                    = _repository.IsMotivationGoalCardType(activityType.Id);

            // assert
            Assert.AreEqual(true, isMotivationGoal);
            Assert.AreEqual(false, isNotMotivationGoal);
        }

        [Test]
        public void DeleteActivityType()
        {
            // arrange
            var type = _repository.GetActivityCardType();

            // act
            _repository.DeleteCardType(type.Id);
            var deletedType = _repository.GetActivityCardType();

            // assert
            Assert.IsNull(deletedType);
        }

        [Test]
        public void DeleteMotivationGoalType()
        {
            // arrange
            var type = _repository.GetMotivationGoalCardType();

            // act
            _repository.DeleteCardType(type.Id);
            var deletedType = _repository.GetMotivationGoalCardType();

            // assert
            Assert.IsNull(deletedType);
        }

        [Test]
        public void DeleteWithCards()
        {
            // arrange
            var _cardRepository = new CardRepository(_connection);
            Card card1 = new Card()
            {
                PhotoPath = "path/to/photo.jpg"
            };

            Card card2 = new Card()
            {
                PhotoPath = "path/to/photo2.jpg"
            };

            CardType cardType = new CardType()
            {
                TypeName = "RANDOM_CARD",
                Cards = new List<Card>
                {
                    card1,
                    card2
                }
            };

            var cardTypeId = _repository.SaveCardType(cardType);
            cardType = _repository.GetCardType(cardTypeId);
            var card1Id = cardType.Cards[0].Id;
            var card2Id = cardType.Cards[1].Id;

            // act
            _repository.DeleteCardTypeCascade(cardType);
            cardType = _repository.GetCardType(cardTypeId);
            card1 = _cardRepository.GetCard(card1Id);
            card2 = _cardRepository.GetCard(card2Id);

            // assert
            Assert.AreEqual(null, cardType);
            Assert.AreEqual(null, card1);
            Assert.AreEqual(null, card2);
        }
    }
}
