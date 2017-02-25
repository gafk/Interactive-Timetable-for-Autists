﻿using System;
using System.IO;
using System.Linq;
using InteractiveTimetable.DataAccessLayer;
using NUnit.Framework;
using SQLite;

namespace InteractiveTimetable.Tests.Repositories
{
    [TestFixture]
    class CriterionDefinitionRepositoryTests
    {
        private SQLiteConnection _connection;
        private CriterionDefinitionRepository _repository;

        [SetUp]
        public void InitializationBeforeTests()
        {
            /* Creating connection */
            string dbPath = "TestsDatabase.db3";
            _connection = new SQLiteConnection(dbPath);

            /* Initialize repository */
            _repository = new CriterionDefinitionRepository(_connection);
        }

        [TearDown]
        public void ShutDown()
        {
            _connection.Dispose();
            File.Delete("TestsDatabase.db3");
        }

        [Test]
        public void GetCriterionDefinitions()
        {
            // arrange

            // act
            var criterions = _repository.GerCriterionDefinitions();

            // assert
            Assert.AreEqual(_repository.GetNumberOfCriterions(),
                            criterions.Count());
        }

        [Test]
        public void DeleteCriterionDefinition()
        {
            // arrange
            var criterionId =
                    _repository.GerCriterionDefinitions().ToList()[0].Id;

            // act
            _repository.DeleteCriterionDefinition(criterionId);
            var criterion = _repository.GetCriterionDefinition(criterionId);

            // assert
            Assert.AreEqual(null, criterion);
        }

        [Test]
        public void DeleteCriterionDefinitionWithGrades()
        {
            // TODO: Implement when grades repositroy is done
        }

        [Test]
        public void GetCriterionDefinitionByNumber()
        {
            // arrange
            int number = 5;

            // act
            var criterion = _repository.GetCriterionDefinitionByNumber(number);

            // assert
            Assert.AreEqual(number, criterion.Number);
        }

        [Test]
        public void GetCriterionDefinitionByNotValidNumber()
        {
            // arrange
            int number = -2;
            var exceptionMessage = "Not a valid number.";

            // act
            var exception1 = Assert.Throws<ArgumentException>(delegate
            {
                _repository.GetCriterionDefinitionByNumber(number);
            });

            number = 19;

            var exception2 = Assert.Throws<ArgumentException>(delegate
            {
                _repository.GetCriterionDefinitionByNumber(number);
            });

            // assert
            Assert.AreEqual(exceptionMessage, exception1.Message);
            Assert.AreEqual(exceptionMessage, exception2.Message);
        }

        [Test]
        public void IsPointGradeTypeCriterion()
        {
            // arrange
            var pointCriterion = _repository.GetCriterionDefinitionByNumber(3);
            var tickCriterion = _repository.GetCriterionDefinitionByNumber(18);

            // act
            bool isPoint
                    = _repository.IsPointGradeTypeCriterion(pointCriterion);
            bool isNotPoint
                    = _repository.IsPointGradeTypeCriterion(tickCriterion);

            // assert
            Assert.AreEqual(true, isPoint);
            Assert.AreEqual(false, isNotPoint);
        }

        [Test]
        public void isTickGradeTypeCriterion()
        {
            // arrange
            var pointCriterion = _repository.GetCriterionDefinitionByNumber(3);
            var tickCriterion = _repository.GetCriterionDefinitionByNumber(18);

            // act
            bool isTick
                    = _repository.IsTickGradeTypeCriterion(tickCriterion);
            bool isNotTick
                    = _repository.IsTickGradeTypeCriterion(pointCriterion);

            // assert
            Assert.AreEqual(true, isTick);
            Assert.AreEqual(false, isNotTick);
        }

        [Test]
        public void GetCriterionGradeType()
        {
            // arrange
            var pointCriterion = _repository.GetCriterionDefinitionByNumber(3);
            var tickCriterion = _repository.GetCriterionDefinitionByNumber(18);

            // act
            var pointGradeType
                    = _repository.GetCriterionGradeType(pointCriterion);
            var tickGradeType
                    = _repository.GetCriterionGradeType(tickCriterion);

            // assert
            Assert.AreEqual("POINT_GRADE", pointGradeType.TypeName);
            Assert.AreEqual("TICK_GRADE", tickGradeType.TypeName);
        }
    }
}