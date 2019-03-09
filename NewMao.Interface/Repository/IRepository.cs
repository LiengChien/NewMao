using NewMao.Common.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Interface.Repository
{
    public interface IRepository<TEntity, TConditions, TId> : IDisposable 
        where TConditions : IBasePager
    {
        //依照查詢條件取得資料清單
        IEnumerable<TEntity> GetAll(TConditions conditions, out int totalSize);
        //新增資料
        TId Create(TEntity model);
        //批次新增資料
        void BatchCreate(List<TEntity> models);
        //更新資料
        void Update(TEntity model);
        //批次更新資料
        void BatchUpdate(List<TEntity> models);
        //刪除資料
        void Delete(TEntity model);
        //取得一筆資料
        TEntity Get(TEntity model);
    }
}
