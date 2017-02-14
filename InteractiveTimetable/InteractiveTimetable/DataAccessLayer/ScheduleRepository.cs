﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InteractiveTimetable.BusinessLayer.Models;
using SQLite;

namespace InteractiveTimetable.DataAccessLayer
{
    class ScheduleRepository : BaseRepository
    {
        public ScheduleRepository(SQLiteConnection connection) : base(connection)
        {
        }

        public Schedule GetSchedule(int id)
        {
            return _database.GetItem<Schedule>(id);
        }

        public IEnumerable<Schedule> GetSchedules()
        {
            return _database.GetItems<Schedule>();
        }

        public int SaveSchedule(Schedule schedule)
        {
            return _database.SaveItem(schedule);
        }

        public int DeleteSchedule(int id)
        {
            return _database.DeleteItem<Schedule>(id);
        }
    }
}
