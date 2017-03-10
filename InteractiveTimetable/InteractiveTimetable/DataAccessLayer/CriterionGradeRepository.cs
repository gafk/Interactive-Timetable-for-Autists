﻿using System;
using System.Collections.Generic;
using System.Linq;
using InteractiveTimetable.BusinessLayer.Models;
using SQLite;

namespace InteractiveTimetable.DataAccessLayer
{
    internal class CriterionGradeRepository : BaseRepository
    {
        public CriterionDefinitionRepository CriterionDefinitions { get; }

        private const int MinimumPointGrade = 1;
        private const int MaximumPointGrade = 4;
        private const int LengthOfTickGradeString = 4;

        public CriterionGradeRepository(SQLiteConnection connection)
            : base(connection)
        {
            CriterionDefinitions = new CriterionDefinitionRepository(connection);
        }

        public CriterionGrade GetCriterionGrade(int gradeId)
        {
            return _database.GetItem<CriterionGrade>(gradeId);
        }

        public IEnumerable<CriterionGrade> GetCriterionGrades()
        {
            return _database.GetItems<CriterionGrade>();
        }

        public IEnumerable<CriterionGrade> GetDiagnosticCriterionGrades(
            int diagnosticId)
        {
            var allGrades = GetCriterionGrades();

            /*
             * Getting grades belonging to diagnostic 
             * and ordered by criterion definition id 
             */
            var diagnosticGrades = allGrades.
                    Where(d => d.DiagnosticId == diagnosticId).
                    OrderBy(d => d.CriterionDefinitionId);

            return diagnosticGrades;
        }

        public int SaveCriterionGrade(CriterionGrade grade)
        {
            /* Data validation */
            Validate(grade);

            return _database.SaveItem(grade);
        }

        public int DeleteCriterionGrade(int gradeId)
        {
            return _database.DeleteItem<CriterionGrade>(gradeId);
        }

        public CriterionGradeType GetCriterionGradeType(CriterionGrade grade)
        {
            var criterionId = grade.CriterionDefinitionId;
            return CriterionDefinitions.GetCriterionGradeType(criterionId);
        }

        internal void Validate(CriterionGrade grade)
        {
            /* Checking that ... */

            /* ... grade is set */
            if (grade == null)
            {
                throw new ArgumentException("Grade is not set.");
            }

            /* ... valid criterion definition id is set */
            var criterionId = grade.CriterionDefinitionId;
            var criterion = CriterionDefinitions.
                    GetCriterionDefinition(criterionId);

            if (criterion == null)
            {
                throw new ArgumentException("Not valid CriterionDefinitionId.");
            }

            /* ... valid grade format is set */
            bool isPointGradeType = CriterionDefinitions.
                    IsPointGradeTypeCriterion(criterion.Id);

            bool isTickGradeType = CriterionDefinitions.
                    IsTickGradeTypeCriterion(criterion.Id);

            if (isPointGradeType)
            {
                int numberGrade;

                if (int.TryParse(grade.Grade, out numberGrade))
                {
                    if (numberGrade < MinimumPointGrade ||
                        numberGrade > MaximumPointGrade)
                    {
                        throw new ArgumentException("Not valid grade.");
                    }
                }
                else
                {
                    throw new ArgumentException("Not valid grade.");
                }
            }
            else if (isTickGradeType)
            {
                if (grade.Grade.Length != LengthOfTickGradeString)
                {
                    throw new ArgumentException("Not valid grade.");
                }
            }
        }
    }
}