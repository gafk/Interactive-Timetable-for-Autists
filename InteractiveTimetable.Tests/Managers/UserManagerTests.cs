﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InteractiveTimetable.BusinessLayer.Managers;
using InteractiveTimetable.BusinessLayer.Models;
using NUnit.Framework;
using SQLite;

namespace InteractiveTimetable.Tests.Managers
{
    [TestFixture]
    class UserManagerTests
    {
        private SQLiteConnection _connection;
        private UserManager _userManager;
        private HospitalTripManager _hospitalTripManager;
        private ScheduleManager _scheduleManager;


        public string GenerateRandomString(int stringLength)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            return new string(Enumerable.Repeat(chars, stringLength)
                                         .Select(s => s[random.Next(s.Length)]).
                                         ToArray());
        }

        [SetUp]
        public void InitializationBeforeTests()
        {
            /* Creating connection */
            string dbPath = "TestsDatabase.db3";
            _connection = new SQLiteConnection(dbPath);

            /* Initializing managers */
            _userManager = new UserManager(_connection);
            _hospitalTripManager = new HospitalTripManager(_connection);
            _scheduleManager = new ScheduleManager(_connection);

            /* Creating cards */
            var activityType = _scheduleManager.Cards.CardTypes.GetActivityCardType();
            var goalType = _scheduleManager.Cards.CardTypes.GetMotivationGoalCardType();

            Card card1 = new Card()
            {
                PhotoPath = "path/to/photo1.jpg",
                CardTypeId = activityType.Id
            };
            Card card2 = new Card()
            {
                PhotoPath = "path/to/photo2.jpg",
                CardTypeId = activityType.Id
            };
            Card card3 = new Card()
            {
                PhotoPath = "path/to/photo3.jpg",
                CardTypeId = activityType.Id
            };
            Card card4 = new Card()
            {
                PhotoPath = "path/to/photo4.jpg",
                CardTypeId = goalType.Id
            };
            _scheduleManager.Cards.SaveCard(card1);
            _scheduleManager.Cards.SaveCard(card2);
            _scheduleManager.Cards.SaveCard(card3);
            _scheduleManager.Cards.SaveCard(card4);
        }

        [TearDown]
        public void ShutDown()
        {
            _connection.Dispose();
            File.Delete("TestsDatabase.db3");
        }

        [Test]
        public void CreateSimpleUser()
        {
            // arrange
            User user = new User()
            {
                FirstName = "Alexander",
                LastName = "Petrenko",
                PatronymicName = "Andreevich",
                BirthDate = DateTime.Today,
                PhotoPath = "avatar1.jpg"
            };

            // act
            var userId = _userManager.SaveUser(user);
            User addedUser = _userManager.GetUser(userId);

            // assert
            Assert.AreEqual(user, addedUser);
        }

        [Test]
        public void CreateUserWithTrips()
        {
            // arrange
            HospitalTrip hospitalTrip1 = new HospitalTrip()
            {
                StartDate = DateTime.Now.AddDays(4),
                FinishDate = DateTime.Now.AddDays(6),
            };

            HospitalTrip hospitalTrip2 = new HospitalTrip()
            {
                StartDate = DateTime.Now.AddDays(9),
                FinishDate = DateTime.Now.AddDays(11),
            };

            User user = new User()
            {
                FirstName = "Alexander",
                LastName = "Petrenko",
                PatronymicName = "Andreevich",
                BirthDate = DateTime.Today,
                PhotoPath = "avatar1.jpg",
                HospitalTrips = new List<HospitalTrip>
                {
                    hospitalTrip1,
                    hospitalTrip2
                }
            };

            // act
            var userId = _userManager.SaveUser(user);
            User addedUser = _userManager.GetUser(userId);

            // assert
            Assert.AreEqual(user, addedUser);
            Assert.AreEqual(user.HospitalTrips, addedUser.HospitalTrips);
        }

        [Test]
        public void CreateAndGetManyUsers()
        {
            // arrange
            HospitalTrip hospitalTrip1 = new HospitalTrip()
            {
                StartDate = DateTime.Now.AddDays(4),
                FinishDate = DateTime.Now.AddDays(6),
            };

            HospitalTrip hospitalTrip2 = new HospitalTrip()
            {
                StartDate = DateTime.Now.AddDays(4),
                FinishDate = DateTime.Now.AddDays(6),
            };

            User user1 = new User()
            {
                FirstName = "Alexander",
                LastName = "Petrenko",
                PatronymicName = "Andreevich",
                BirthDate = DateTime.Today,
                PhotoPath = "avatar1.jpg",
                HospitalTrips = new List<HospitalTrip>
                {
                    hospitalTrip1
                }
            };

            User user2 = new User()
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                PatronymicName = "Ivanovich",
                BirthDate = DateTime.Today,
                PhotoPath = "avatar1.jpg",
                HospitalTrips = new List<HospitalTrip>
                {
                    hospitalTrip2
                }
            };

            var user1Id = _userManager.SaveUser(user1);
            var user2Id = _userManager.SaveUser(user2);

            // act
            var addedUsers = _userManager.GetUsers().ToList();
            var addedUser1 = addedUsers.First(u => u.Id == user1Id);
            var addedUser2 = addedUsers.First(u => u.Id == user2Id);

            // assert
            Assert.AreEqual(user1, addedUser1);
            Assert.AreEqual(user2, addedUser2);
            Assert.AreEqual(user1.HospitalTrips, addedUser1.HospitalTrips);
            Assert.AreEqual(user2.HospitalTrips, addedUser2.HospitalTrips);
        }

        [Test]
        public void CreateUserWithSchedules()
        {
            // arrange
            /* Creating and saving user */
            User user = new User()
            {
                FirstName = "Alexander",
                LastName = "Petrenko",
                PatronymicName = "Andreevich",
                BirthDate = DateTime.Today,
                PhotoPath = "avatar1.jpg"
            };
            var userId = _userManager.SaveUser(user);

            /* Creating and saving schedule */
            var activityCards = _scheduleManager.Cards.GetActivityCards().
                                         Select(x => x.Id).
                                         ToList();

            var goalCards = _scheduleManager.Cards.GetMotivationGoalCards().
                                     Select(x => x.Id).
                                     ToList();

            var ids = activityCards.Concat(goalCards).ToList();
            var scheduleId1 = _scheduleManager.SaveSchedule(userId, ids);
            var scheduleId2 = _scheduleManager.SaveSchedule(userId, ids);

            // act
            var addedUser = _userManager.GetUser(userId);

            // assert
            Assert.AreEqual(2, addedUser.Schedules.Count);
            Assert.AreEqual(scheduleId1, addedUser.Schedules[0].Id);
            Assert.AreEqual(scheduleId2, addedUser.Schedules[1].Id);
        }

        [Test]
        public void CreateUserWithIncorrectFirstName()
        {
            // arrange
            User user = new User()
            {
                FirstName = GenerateRandomString(256),
                LastName = "Petrenko",
                PatronymicName = "Andreevich",
                BirthDate = DateTime.Today,
                PhotoPath = "avatar1.jpg"
            };
            string exceptionMessage = Resources.Validation.
                                                UserValidationStrings.
                                                FirstNameLength;

            // act/assert
            var exception = Assert.Throws<ArgumentException>(
                delegate
                {
                    _userManager.SaveUser(user);
                });

            Assert.AreEqual(exceptionMessage,exception.Message);
        }

        [Test]
        public void CreateUserWithIncorrectLastName()
        {
            // arrange
            User user = new User()
            {
                FirstName = "Alexander",
                LastName = GenerateRandomString(256),
                PatronymicName = "Andreevich",
                BirthDate = DateTime.Today,
                PhotoPath = "avatar1.jpg"
            };
            string exceptionMessage = Resources.Validation.
                                                UserValidationStrings.
                                                LastNameLength;

            // act/assert
            var exception = Assert.Throws<ArgumentException>(
                delegate
                {
                    _userManager.SaveUser(user);
                });

            Assert.AreEqual(exceptionMessage, exception.Message);

        }

        [Test]
        public void CreateUserWithIncorrectPatronymicName()
        {
            // arrange
            User user = new User()
            {
                FirstName = "Alexander",
                LastName = "Petrenko",
                PatronymicName = GenerateRandomString(256),
                BirthDate = DateTime.Today,
                PhotoPath = "avatar1.jpg"
            };
            string exceptionMessage = Resources.Validation.
                                                UserValidationStrings.
                                                PatronymicNameLength;

            // act/assert
            var exception = Assert.Throws<ArgumentException>(
                delegate
                {
                    _userManager.SaveUser(user);
                });

            Assert.AreEqual(exceptionMessage, exception.Message);
        }

        [Test]
        public void CreateUserWithIncorrectPhotoPath()
        {
            // arrange
            User user = new User()
            {
                FirstName = "Alexander",
                LastName = "Petrenko",
                PatronymicName = "Andreevich",
                BirthDate = DateTime.Today,
                PhotoPath = GenerateRandomString(1025)
            };
            string exceptionMessage = Resources.Validation.
                                                UserValidationStrings.
                                                PhotoPathLength;

            // act/assert
            var exception = Assert.Throws<ArgumentException>(
                delegate
                {
                    _userManager.SaveUser(user);
                });

            Assert.AreEqual(exceptionMessage, exception.Message);
        }

        [Test]
        public void CreateUserWithIncorrectBirthDate()
        {
            // arrange
            User user = new User()
            {
                FirstName = "Alexander",
                LastName = "Petrenko",
                PatronymicName = "Andreevich",
                BirthDate = DateTime.Parse("01.01.1880"),
                PhotoPath = "avatar1.jpg"
            };
            string exceptionMessage = Resources.Validation.
                                                UserValidationStrings.
                                                NotCorrectBirthDate;

            // act/assert
            var exception = Assert.Throws<ArgumentException>(
                delegate
                {
                    _userManager.SaveUser(user);
                });

            Assert.AreEqual(exceptionMessage, exception.Message);

            user.BirthDate = DateTime.Today.AddDays(1);
            exception = Assert.Throws<ArgumentException>(
                delegate
                {
                    _userManager.SaveUser(user);
                });

            Assert.AreEqual(exceptionMessage, exception.Message);

        }

        [Test]
        public void SimpleEditUser()
        {
            // arrange
            User user = new User()
            {
                FirstName = "Alexander",
                LastName = "Petrenko",
                PatronymicName = "Andreevich",
                BirthDate = DateTime.Parse("25.07.1995"),
                PhotoPath = "avatar1.jpg"
            };

            var userId = _userManager.SaveUser(user);
            var addedUser = _userManager.GetUser(userId);

            // act
            addedUser.FirstName = "Ivan";
            addedUser.LastName = "Ivanov";
            _userManager.SaveUser(addedUser);
            addedUser = _userManager.GetUser(userId);

            // assert
            Assert.AreEqual("Ivan", addedUser.FirstName);
            Assert.AreEqual("Ivanov", addedUser.LastName);
        }

        [Test]
        public void EditUserWithTripDeleting()
        {
            // arrange
            HospitalTrip hospitalTrip = new HospitalTrip()
            {
                StartDate = DateTime.Now.AddDays(9),
                FinishDate = DateTime.Now.AddDays(11),
            };

            User user = new User()
            {
                FirstName = "Alexander",
                LastName = "Petrenko",
                PatronymicName = "Andreevich",
                BirthDate = DateTime.Today,
                PhotoPath = "avatar1.jpg",
                HospitalTrips = new List<HospitalTrip>
                {
                    hospitalTrip
                }
            };

            var userId = _userManager.SaveUser(user);
            var addedUser = _userManager.GetUser(userId);

            // act
            var trip = addedUser.HospitalTrips.First();
            _hospitalTripManager.DeleteHospitalTrip(trip.Id);
            addedUser = _userManager.GetUser(userId);

            // assert
            Assert.AreEqual(0, addedUser.HospitalTrips.Count);
        }

        [Test]
        public void EditUserWithScheduleEditing()
        {
            // arrange
            /* Creating and saving user */
            User user = new User()
            {
                FirstName = "Alexander",
                LastName = "Petrenko",
                PatronymicName = "Andreevich",
                BirthDate = DateTime.Today,
                PhotoPath = "avatar1.jpg"
            };
            var userId = _userManager.SaveUser(user);

            /* Creating and saving schedule */
            var activityCards = _scheduleManager.Cards.GetActivityCards().
                                         Select(x => x.Id).
                                         ToList();

            var goalCards = _scheduleManager.Cards.GetMotivationGoalCards().
                                     Select(x => x.Id).
                                     ToList();

            var ids = activityCards.Concat(goalCards).ToList();
            var scheduleId1 = _scheduleManager.SaveSchedule(userId, ids);
            var scheduleId2 = _scheduleManager.SaveSchedule(userId, ids);

            // act
            _scheduleManager.DeleteSchedule(scheduleId1);
            var scheduleId3 = _scheduleManager.SaveSchedule(userId, ids);
            var scheduleId4 = _scheduleManager.SaveSchedule(userId, ids);
            user = _userManager.GetUser(userId);

            // assert
            Assert.AreEqual(3, user.Schedules.Count);
            Assert.AreEqual(scheduleId2, user.Schedules[0].Id);
            Assert.AreEqual(scheduleId3, user.Schedules[1].Id);
            Assert.AreEqual(scheduleId4, user.Schedules[2].Id);
        }

        [Test]
        public void EditUserWithTripAdding()
        {
            // arrange
            HospitalTrip hospitalTrip = new HospitalTrip()
            {
                StartDate = DateTime.Now.AddDays(9),
                FinishDate = DateTime.Now.AddDays(11),
            };

            User user = new User()
            {
                FirstName = "Alexander",
                LastName = "Petrenko",
                PatronymicName = "Andreevich",
                BirthDate = DateTime.Today,
                PhotoPath = "avatar1.jpg",
            };

            var userId = _userManager.SaveUser(user);
            var addedUser = _userManager.GetUser(userId);

            // act
            hospitalTrip.UserId = userId;
            var hospitalTripId = _hospitalTripManager.
                SaveHospitalTrip(hospitalTrip);
            var addedHospitalTrip = _hospitalTripManager.
                    GetHospitalTrip(hospitalTripId);
            addedUser = _userManager.GetUser(userId);

            // assert
            Assert.AreEqual(addedHospitalTrip, addedUser.HospitalTrips.First());
        }

        [Test]
        public void EditUserWithTripEditing()
        {
            // arrange
            HospitalTrip hospitalTrip = new HospitalTrip()
            {
                StartDate = DateTime.Now.AddDays(9),
                FinishDate = DateTime.Now.AddDays(11),
            };

            User user = new User()
            {
                FirstName = "Alexander",
                LastName = "Petrenko",
                PatronymicName = "Andreevich",
                BirthDate = DateTime.Today,
                PhotoPath = "avatar1.jpg",
                HospitalTrips = new List<HospitalTrip>
                {
                    hospitalTrip
                }
            };

            var userId = _userManager.SaveUser(user);
            var addedUser = _userManager.GetUser(userId);

            // act
            var trip = addedUser.HospitalTrips.First();
            trip.StartDate = DateTime.Today.AddDays(15);
            trip.FinishDate = DateTime.Today.AddDays(20);
            _hospitalTripManager.SaveHospitalTrip(trip);
            addedUser = _userManager.GetUser(userId);

            // assert
            Assert.AreEqual(DateTime.Today.AddDays(15), 
                addedUser.HospitalTrips.First().StartDate);

            Assert.AreEqual(DateTime.Today.AddDays(20), 
                addedUser.HospitalTrips.First().FinishDate);
        }

        [Test]
        public void EditUserWithSettingBirthDateAfterTripStartDate()
        {
            // arrange
            HospitalTrip hospitalTrip = new HospitalTrip()
            {
                StartDate = DateTime.Today.AddDays(-3),
                FinishDate = DateTime.Today.AddDays(11),
            };

            User user = new User()
            {
                FirstName = "Alexander",
                LastName = "Petrenko",
                PatronymicName = "Andreevich",
                BirthDate = DateTime.Today,
                PhotoPath = "avatar1.jpg",
                HospitalTrips = new List<HospitalTrip>
                {
                    hospitalTrip
                }
            };
            string exceptionMessage = Resources.Validation.
                                                UserValidationStrings.
                                                BirthDateLaterThanTrip;

            var userId = _userManager.SaveUser(user);
            var addedUser = _userManager.GetUser(userId);

            // act/assert
            addedUser.BirthDate = DateTime.Today.AddDays(-2);
        
            var exception = Assert.Throws<ArgumentException>(
                delegate
                {
                    _userManager.SaveUser(addedUser);
                });

            Assert.AreEqual(exceptionMessage, exception.Message);
        }

        [Test]
        public void DeleteSimpleUser()
        {
            // arrange
            User user = new User()
            {
                FirstName = "Alexander",
                LastName = "Petrenko",
                PatronymicName = "Andreevich",
                BirthDate = DateTime.Today,
                PhotoPath = "avatar1.jpg",
            };
            var userId = _userManager.SaveUser(user);

            // act
            _userManager.DeleteUser(userId);
            var deletedUser = _userManager.GetUser(userId);

            // assert
            Assert.AreEqual(null, deletedUser);
        }

        [Test]
        public void DeleteUserWithTrips()
        {
            // arrange
            HospitalTrip hospitalTrip1 = new HospitalTrip()
            {
                StartDate = DateTime.Now.AddDays(4),
                FinishDate = DateTime.Now.AddDays(6),
            };

            HospitalTrip hospitalTrip2 = new HospitalTrip()
            {
                StartDate = DateTime.Now.AddDays(9),
                FinishDate = DateTime.Now.AddDays(11),
            };

            User user = new User()
            {
                FirstName = "Alexander",
                LastName = "Petrenko",
                PatronymicName = "Andreevich",
                BirthDate = DateTime.Today,
                PhotoPath = "avatar1.jpg",
                HospitalTrips = new List<HospitalTrip>
                {
                    hospitalTrip1,
                    hospitalTrip2
                }
            };
            var userId = _userManager.SaveUser(user);
            var addedUser = _userManager.GetUser(userId);
            var firstTripId = addedUser.HospitalTrips.ToList()[0].Id;
            var secondTripId = addedUser.HospitalTrips.ToList()[1].Id;

            // act
            _userManager.DeleteUser(userId);
            var deletedUser = _userManager.GetUser(userId);
            var deletedTrip1 = _hospitalTripManager.GetHospitalTrip(firstTripId);
            var deletedTrip2 = _hospitalTripManager.GetHospitalTrip(secondTripId);

            // assert
            Assert.AreEqual(null, deletedUser);
            Assert.AreEqual(null, deletedTrip1);
            Assert.AreEqual(null, deletedTrip2);
        }

        [Test]
        public void DeleteUserWithSchedules()
        {
            // arrange
            /* Creating and saving user */
            User user = new User()
            {
                FirstName = "Alexander",
                LastName = "Petrenko",
                PatronymicName = "Andreevich",
                BirthDate = DateTime.Today,
                PhotoPath = "avatar1.jpg"
            };
            var userId = _userManager.SaveUser(user);

            /* Creating and saving schedule */
            var activityCards = _scheduleManager.Cards.GetActivityCards().
                                         Select(x => x.Id).
                                         ToList();

            var goalCards = _scheduleManager.Cards.GetMotivationGoalCards().
                                     Select(x => x.Id).
                                     ToList();

            var ids = activityCards.Concat(goalCards).ToList();
            var scheduleId1 = _scheduleManager.SaveSchedule(userId, ids);
            var scheduleId2 = _scheduleManager.SaveSchedule(userId, ids);
            
            // act
            _userManager.DeleteUser(userId);
            var deletedUser = _userManager.GetUser(userId);
            var deletedSchedule1 = _scheduleManager.GetSchedule(scheduleId1);
            var deletedSchedule2 = _scheduleManager.GetSchedule(scheduleId2);

            // assert
            Assert.AreEqual(null, deletedUser);
            Assert.AreEqual(null, deletedSchedule1);
            Assert.AreEqual(null, deletedSchedule2);
        }

        [Test]
        public void CurrentUserTrip()
        {
            // arrange
            HospitalTrip hospitalTrip1 = new HospitalTrip()
            {
                StartDate = DateTime.Now.AddDays(-5),
                FinishDate = DateTime.Now.AddDays(6),
            };

            User user = new User()
            {
                FirstName = "Alexander",
                LastName = "Petrenko",
                PatronymicName = "Andreevich",
                BirthDate = DateTime.Today,
                PhotoPath = "avatar1.jpg",
                HospitalTrips = new List<HospitalTrip>
                {
                    hospitalTrip1
                }
            };
            var userId = _userManager.SaveUser(user);

            // act
            var hasCurrentTrip = _userManager.IsUserInPresentTimetable(userId);

            // assert
            Assert.AreEqual(true, hasCurrentTrip);
        }

        [Test]
        public void NotCurrentUserTrip()
        {
            // arrange
            HospitalTrip hospitalTrip1 = new HospitalTrip()
            {
                StartDate = DateTime.Now.AddDays(3),
                FinishDate = DateTime.Now.AddDays(6),
            };

            User user = new User()
            {
                FirstName = "Alexander",
                LastName = "Petrenko",
                PatronymicName = "Andreevich",
                BirthDate = DateTime.Today,
                PhotoPath = "avatar1.jpg",
                HospitalTrips = new List<HospitalTrip>
                {
                    hospitalTrip1
                }
            };
            var userId = _userManager.SaveUser(user);

            // act
            var hasCurrentTrip = _userManager.IsUserInPresentTimetable(userId);

            // assert
            Assert.AreEqual(false, hasCurrentTrip);

        }
    }
}