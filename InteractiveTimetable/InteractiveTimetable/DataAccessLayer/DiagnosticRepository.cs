﻿using System.Collections.Generic;
using InteractiveTimetable.BusinessLayer.Models;
using SQLite;

namespace InteractiveTimetable.DataAccessLayer
{
    internal class DiagnosticRepository : BaseRepository
    {
        public DiagnosticRepository(SQLiteConnection connection) : base(connection)
        {
        }

        public Diagnostic GetDiagnostic(int id)
        {
            return _database.GetItem<Diagnostic>(id);
        }

        public IEnumerable<Diagnostic> GetDiagnostics()
        {
            return _database.GetItems<Diagnostic>();
        }

        public int SaveDiagnostic(Diagnostic diagnostic)
        {
            return _database.SaveItem(diagnostic);
        }

        public int DeleteDiagnostic(int id)
        {
            return _database.DeleteItem<Diagnostic>(id);
        }
    }
}
