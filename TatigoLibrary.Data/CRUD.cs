﻿using PetaPoco;
using System.Collections.Generic;

namespace TatigoLibrary.Data
{
    public class Record<T> where T : new()
    {
        public static BaseDataAccess Context { get { return BaseDataContext.GetDataContext(); } }
        public static Database repo { get { return Context.PetaPocoContext; } }

        public bool IsNew()
        {
            return repo.IsNew(this);
        }

        public void Save()
        {
            repo.Save(this);
        }

        public int Update()
        {
            return repo.Update(this);
        }

        public int Update(IEnumerable<string> columns)
        {
            return repo.Update(this, columns);
        }

        public static int Update(string sql, params object[] args)
        {
            return repo.Update<T>(sql, args);
        }

        public static int Update(Sql sql)
        {
            return repo.Update<T>(sql);
        }

        public int Delete()
        {
            return repo.Delete(this);
        }

        public static int Delete(string sql, params object[] args)
        {
            return repo.Delete<T>(sql, args);
        }

        public static int Delete(Sql sql)
        {
            return repo.Delete<T>(sql);
        }

        public static int Delete(object primaryKey)
        {
            return repo.Delete<T>(primaryKey);
        }

        public static bool Exists(object primaryKey)
        {
            return repo.Exists<T>(primaryKey);
        }

        public static T SingleOrDefault(object primaryKey)
        {
            return repo.SingleOrDefault<T>(primaryKey);
        }

        public static T SingleOrDefault(string sql, params object[] args)
        {
            return repo.SingleOrDefault<T>(sql, args);
        }

        public static T SingleOrDefault(Sql sql)
        {
            return repo.SingleOrDefault<T>(sql);
        }

        public static T FirstOrDefault(string sql, params object[] args)
        {
            return repo.FirstOrDefault<T>(sql, args);
        }

        public static T FirstOrDefault(Sql sql)
        {
            return repo.FirstOrDefault<T>(sql);
        }

        public static T Single(object primaryKey)
        {
            return repo.Single<T>(primaryKey);
        }

        public static T Single(string sql, params object[] args)
        {
            return repo.Single<T>(sql, args);
        }

        public static T Single(Sql sql)
        {
            return repo.Single<T>(sql);
        }

        public static T First(string sql, params object[] args)
        {
            return repo.First<T>(sql, args);
        }

        public static T First(Sql sql)
        {
            return repo.First<T>(sql);
        }

        public static List<T> Fetch(string sql, params object[] args)
        {
            return repo.Fetch<T>(sql, args);
        }

        public static List<T> Fetch(Sql sql)
        {
            return repo.Fetch<T>(sql);
        }

        public static List<T> Fetch(long page, long itemsPerPage, string sql, params object[] args)
        {
            return repo.Fetch<T>(page, itemsPerPage, sql, args);
        }

        public static List<T> Fetch(long page, long itemsPerPage, Sql sql)
        {
            return repo.Fetch<T>(page, itemsPerPage, sql);
        }

        public static Page<T> Page(long page, long itemsPerPage, string sql, params object[] args)
        {
            return repo.Page<T>(page, itemsPerPage, sql, args);
        }

        public static Page<T> Page(long page, long itemsPerPage, Sql sql)
        {
            return repo.Page<T>(page, itemsPerPage, sql);
        }

        public static IEnumerable<T> Query(string sql, params object[] args)
        {
            return repo.Query<T>(sql, args);
        }

        public static IEnumerable<T> Query(Sql sql)
        {
            return repo.Query<T>(sql);
        }
    }
}