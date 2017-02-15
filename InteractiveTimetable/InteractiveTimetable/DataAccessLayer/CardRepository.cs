﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InteractiveTimetable.BusinessLayer.Models;
using SQLite;

namespace InteractiveTimetable.DataAccessLayer
{
    class CardRepository : BaseRepository
    {
        public CardRepository(SQLiteConnection connection) : base(connection)
        {
        }

        public Card GetCard(int id)
        {
            return _database.GetItem<Card>(id);
        }

        public IEnumerable<Card> GetCards()
        {
            return _database.GetItems<Card>();
        }

        public int SaveCard(Card card)
        {
            return _database.SaveItem(card);
        }

        public int DeleteCard(int id)
        {
            return _database.DeleteItem<Card>(id);
        }
    }
}
