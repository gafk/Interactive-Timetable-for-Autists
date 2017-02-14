﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InteractiveTimetable.BusinessLayer.Models;
using SQLite;

namespace InteractiveTimetable.DataAccessLayer
{
    class CriterionGradeRepository : BaseRepository
    {
        public CriterionGradeRepository(SQLiteConnection connection) : base(connection)
        {
        }

        public CriterionGrade GetCriterionGrade(int id)
        {
            return _database.GetItem<CriterionGrade>(id);
        }

        public IEnumerable<CriterionGrade> GetCriterionGrades()
        {
            return _database.GetItems<CriterionGrade>();
        }

        public int SaveCriterionGrade(CriterionGrade criterionGrade)
        {
            return _database.SaveItem(criterionGrade);
        }

        public int DeleteCriterion(int id)
        {
            return _database.DeleteItem<CriterionGrade>(id);
        }
    }
}
