﻿using System;
using System.Collections.Generic;
using System.Linq;
using InteractiveTimetable.BusinessLayer.Models;
using SQLite;

namespace InteractiveTimetable.DataAccessLayer
{
    public class CardRepository : BaseRepository
    {
        public readonly CardTypeRepository CardTypes;

        public CardRepository(SQLiteConnection connection) : base(connection)
        {
            CardTypes = new CardTypeRepository(connection);
        }

        public Card GetCard(int cardId)
        {
            return _database.GetItemCascade<Card>(cardId);
        }

        public IEnumerable<Card> GetCards()
        {
            return _database.GetItemsCascade<Card>();
        }

        public int SaveCard(Card card)
        {
            /* Data validation */
            Validate(card);

            return _database.SaveItemCascade(card);
        }

        public int DeleteCard(int cardId)
        {
            return _database.DeleteItem<Card>(cardId);
        }

        public void DeleteCardCascade(Card card)
        {
            _database.DeleteItemCascade(card);
        }

        public IEnumerable<Card> GetActivityCards()
        {
            return GetCards().Where(
                x => CardTypes.IsActivityCardType(x.CardTypeId));
        }

        public IEnumerable<Card> GetMotivationGoalCards()
        {
            return GetCards().Where(
                x => CardTypes.IsMotivationGoalCardType(x.CardTypeId));
        }

        public bool IsActivityCard(int cardId)
        {
            var card = GetCard(cardId);
            return CardTypes.IsActivityCardType(card.CardTypeId);
        }

        public bool IsMotivationGoalCard(int cardId)
        {
            var card = GetCard(cardId);
            return CardTypes.IsMotivationGoalCardType(card.CardTypeId);
        }

        public bool IsCardExist(int cardId)
        {
            return GetCard(cardId) != null;
        }

        public bool IsCardInPresentTimetable(int cardId)
        {
            var card = GetCard(cardId);
            return card.ScheduleItems.Count != 0;
        }

        private void Validate(Card card)
        {
            /* Checking that ... */

            /* ... card is set */
            if (card == null)
            {
                throw new ArgumentException(Resources.Validation.
                                                      CardValidationStrings.
                                                      ArgumentIsNull);
            }

            /* ... path to photo is not longer than 1024 symbols */
            if (card.PhotoPath.Length > 1024)
            {
                throw new ArgumentException(Resources.Validation.
                                                      CardValidationStrings.
                                                      PhotoPathLength);
            }
        }

        public void InitializeForDebugging()
        {
            var activityCardType = CardTypes.GetActivityCardType();
            var goalCardType = CardTypes.GetMotivationGoalCardType();

            /* Create activity cards */
            int cardsCount = 24;
            string fileName = "/cards/activity_cards/activity_card_";
            string extension = ".png";

            for (int i = 0; i < cardsCount; ++i)
            {
                var card = new Card()
                {
                    CardTypeId = activityCardType.Id,
                    PhotoPath = fileName + (i + 1) + extension,
                };

                SaveCard(card);
            }

            /* Create motivation goal cards */
            cardsCount = 5;
            fileName = "/cards/motivation_goal_cards/motivation_goal_card_";

            for (int i = 0; i < cardsCount; ++i)
            {
                var card = new Card()
                {
                    CardTypeId = goalCardType.Id,
                    PhotoPath = fileName + (i + 1) + extension,
                };

                SaveCard(card);
            }

        }
    }
}
