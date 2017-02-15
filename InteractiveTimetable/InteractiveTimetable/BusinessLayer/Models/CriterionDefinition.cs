﻿using System.Collections.Generic;
using InteractiveTimetable.BusinessLayer.Contracts;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace InteractiveTimetable.BusinessLayer.Models
{
    public class CriterionDefinition : BusinessEntityBase
    {
        [NotNull]
        public int Number { get; set; }

        [MaxLength(255), NotNull]
        public string Definition { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<CriterionGrade> CriterionGrades { get; set; }

        [ForeignKey(typeof(CriterionGradeType))]
        public int CriterionTypeId { get; set; }
    }
}