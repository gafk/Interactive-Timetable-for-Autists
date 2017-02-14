﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InteractiveTimetable.BusinessLayer.Models;
using SQLite;

namespace InteractiveTimetable.DataAccessLayer
{
    class CardTypeResitory : BaseRepository
    {
        public CardTypeResitory(SQLiteConnection connection) : base(connection)
        {
        }

        public CardType GetCardType(int id)
        {
            return _database.GetItem<CardType>(id); _
        }

        public IEnumerable<CardType> GetCardTypes()
        {
            return _database.GetItems<CardType>();
        }

        public int SaveCardType(CardType cardType)
        {
            return _database.SaveItem(cardType);
        }

        public int DeleteCardType(int id)
        {
            return _database.DeleteItem<CardType>(id);
        }
    }

}
