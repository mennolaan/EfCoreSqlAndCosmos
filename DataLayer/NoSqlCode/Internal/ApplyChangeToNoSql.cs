﻿// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DataLayer.EfClassesNoSql;
using DataLayer.EfClassesSql;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("Test")]
namespace DataLayer.NoSqlCode.Internal
{
    internal class ApplyChangeToNoSql
    {
        private readonly DbContext _sqlContext;
        private readonly NoSqlDbContext _noSqlContext;

        public ApplyChangeToNoSql(DbContext sqlContext, NoSqlDbContext noSqlContext)
        {
            _sqlContext = sqlContext ?? throw new ArgumentNullException(nameof(sqlContext));
            _noSqlContext = noSqlContext ?? throw new ArgumentNullException(nameof(noSqlContext)); ;
        }

        public bool UpdateNoSql(IImmutableList<BookChangeInfo> booksToUpdate)
        {
            if (_noSqlContext == null || !booksToUpdate.Any()) return false;

            foreach (var bookToUpdate in booksToUpdate)
            {
                switch (bookToUpdate.State)
                {
                    case EntityState.Deleted:
                    {
                        var noSqlBook = _noSqlContext.Find<BookListNoSql>(bookToUpdate.BookId);
                        _noSqlContext.Remove(noSqlBook);
                    }
                        break;
                    case EntityState.Modified:
                    {
                        var noSqlBook = _noSqlContext.Find<BookListNoSql>(bookToUpdate.BookId);
                        noSqlBook = _sqlContext.Set<Book>().ProjectBook(bookToUpdate.BookId);
                    }
                        break;
                    case EntityState.Added:
                        var newBook = _sqlContext.Set<Book>().ProjectBook(bookToUpdate.BookId);
                        _noSqlContext.Add(newBook);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return true;
        }

        public async Task<bool> UpdateNoSqlAsync(IImmutableList<BookChangeInfo> booksToUpdate)
        {
            if (_noSqlContext == null || !booksToUpdate.Any()) return false;

            foreach (var bookToUpdate in booksToUpdate)
            {
                switch (bookToUpdate.State)
                {
                    case EntityState.Deleted:
                    {
                        var noSqlBook = await _noSqlContext.FindAsync<BookListNoSql>(bookToUpdate.BookId);
                        _noSqlContext.Remove(noSqlBook);
                    }
                        break;
                    case EntityState.Modified:
                    {
                        var noSqlBook = await _noSqlContext.FindAsync<BookListNoSql>(bookToUpdate.BookId);
                        noSqlBook = _sqlContext.Set<Book>().ProjectBook(bookToUpdate.BookId);
                    }
                        break;
                    case EntityState.Added:
                        var newBook = _sqlContext.Set<Book>().ProjectBook(bookToUpdate.BookId);
                        _noSqlContext.Add(newBook);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return true;
        }
    }
}