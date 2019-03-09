using NewMao.Common.Enum;
using NewMao.Common.Mapper;
using NewMao.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NewMao.Core.Extensions
{
    public class TableMapper
    {
        public string OrgName { get; set; }
        public string AliasName { get; set; }
    }

    /// <summary>
    /// sql指令相關延伸方法
    /// </summary>
    public static class SqlCommandExtension
    {
        /// <summary>
        /// 取得真實table名稱
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string GetTableName<T>(this T table) where T : Type
        {
            var tableAttr = table.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault() as TableAttribute;
            return tableAttr == null ? table.Name : tableAttr.Name;
        }

        /// <summary>
        /// 取得Table Select Command Mode
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static SelectType GetTableSelectType<T>(this T table) where T : Type
        {
            var tableAttr = table.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault() as TableAttribute;
            return tableAttr == null ? SelectType.Common : tableAttr.TableSelectType;
        }

        /// <summary>
        /// 取得Table設定之是否需要作distinct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static bool GetNeedDistinct<T>(this T table) where T : Type
        {
            var tableAttr = table.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault() as TableAttribute;
            return tableAttr == null ? false : tableAttr.NeedDistinct;
        }

        public static string GetValueBypropertyInfo<T>(this T TEntity, string attrName)
        {
            var attrValue = "";
            var vmodePropertyInfos = TEntity.GetType().GetProperties();
            foreach (var prop in vmodePropertyInfos)
            {
                if (prop.Name == attrName)
                {
                    attrValue = prop.GetValue(TEntity).ToString();
                    break;
                }
            }
            return attrValue;
        }

        public static string GetSelectCommand<T, TConditions>(this T viewmodel, TConditions conditions) where T : Type
            where TConditions : IBasePager
        {
            var selectCmd = new StringBuilder();
            var mainTable = viewmodel.GetTableName();
            var vmodelPropertyInfos = viewmodel.GetProperties();
            var columnAttrList = new List<ColumnAttribute>();
            var tableList = new Dictionary<string, TableMapper>();
            tableList.Add(mainTable, new TableMapper
            {
                OrgName = mainTable,
                AliasName = $"table{(tableList.Count + 1).ToString().PadLeft(2, '0')}"
            });

            // Step1: Get Table List and All Column Attributes
            foreach (var propertyInfo in vmodelPropertyInfos)
            {
                var columnAttr = propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault() as ColumnAttribute;
                if (columnAttr != null)
                {
                    if (!string.IsNullOrEmpty(columnAttr.BelongTable))
                    {
                        var tableName = string.IsNullOrEmpty(columnAttr.BelongTableAlias) ? columnAttr.BelongTable : columnAttr.BelongTableAlias;
                        if (!tableList.ContainsKey(tableName))
                        {
                            tableList.Add(tableName, new TableMapper
                            {
                                OrgName = columnAttr.BelongTable,
                                AliasName = $"table{(tableList.Count + 1).ToString().PadLeft(2, '0')}"
                            });
                        }
                    }
                    columnAttrList.Add(columnAttr);
                }
            }

            var needDistinct = viewmodel.GetNeedDistinct() ? " DISTINCT " : " ";
            switch (viewmodel.GetTableSelectType())
            {
                case SelectType.Complicated:
                    var tableColumnAttrList = new Dictionary<string, List<ColumnAttribute>>();
                    // Step2: Set Select String(before from)
                    for (int i = 0; i < columnAttrList.Count; i++)
                    {
                        var item = columnAttrList[i];
                        var selectwordOrNot = i == 0 ? $"SELECT SQL_CALC_FOUND_ROWS{needDistinct}" : "";
                        var commaKeywordOrNot = i == columnAttrList.Count - 1 ? " " : ", ";
                        // use Main Table Alias if there is not setting [BelongTable]
                        var tableName = string.IsNullOrEmpty(item.BelongTableAlias) ? item.BelongTable : item.BelongTableAlias;
                        var tableAlias = string.IsNullOrEmpty(tableName) ? tableList[mainTable].AliasName : tableList[tableName].AliasName;
                        var columnAlias = string.IsNullOrEmpty(columnAttrList[i].Alias) ? "" : $" AS {columnAttrList[i].Alias}";
                        selectCmd.Append($"{selectwordOrNot}{tableAlias}.{item.Name}{columnAlias}{commaKeywordOrNot}");

                        // Prepare Join Statememt
                        if (!string.IsNullOrEmpty(item.ForiegnTable) || item.JoinByValue == true)
                        {
                            if (!tableColumnAttrList.ContainsKey(tableName))
                            {
                                tableColumnAttrList.Add(tableName, new List<ColumnAttribute>() { item });
                            }
                            else
                            {
                                tableColumnAttrList[tableName].Add(item);
                            }
                        }
                    }

                    // Step3: Set Select string(after from)
                    selectCmd.Append($" FROM {mainTable} AS {tableList[mainTable].AliasName} ");
                    foreach (var item in tableColumnAttrList)
                    {
                        var table = item.Key == "$Main$" ? tableList[mainTable].OrgName : tableList[item.Key].OrgName;
                        var tableAlias = table == "$Main$" ? tableList[mainTable].AliasName : tableList[item.Key].AliasName;
                        for (int i = 0; i < item.Value.Count; i++)
                        {
                            var currentColAttr = item.Value[i];
                            var joinType = "";
                            switch (currentColAttr.ForiegnJoinType)
                            {
                                case JoinType.InnerJoin:
                                    joinType = " JOIN ";
                                    break;
                                case JoinType.LeftOutterJoin:
                                    joinType = " LEFT JOIN ";
                                    break;
                                case JoinType.RightOutterJoin:
                                    joinType = " RIGHT JOIN ";
                                    break;
                                default:
                                    joinType = " JOIN ";
                                    break;
                            }
                            var joinKeywordOrNot = i == 0 ? $"{joinType} {table} AS {tableAlias} ON " : " ";
                            var andKeywordOrNot = i == item.Value.Count - 1 ? " " : "AND ";
                            if (currentColAttr.JoinByValue == false)
                            {
                                var foreignTable = currentColAttr.ForiegnTable == "$Main$" ? mainTable : currentColAttr.ForiegnTable;
                                var foreignTableAlias = tableList[foreignTable].AliasName;
                                selectCmd.Append($"{joinKeywordOrNot} {tableAlias}.{currentColAttr.Name} = {foreignTableAlias}.{currentColAttr.ForiegnColumn} {andKeywordOrNot}");
                            }
                            else
                            {
                                selectCmd.Append($"{joinKeywordOrNot} {tableAlias}.{currentColAttr.Name} = '{currentColAttr.JoinValue}' {andKeywordOrNot}");
                            }
                        }
                    }
                    selectCmd.Append(conditions.GetSelectWhereCommand(tableList));
                    break;
                default: //SelectType.Common:
                    selectCmd.Append($"SELECT SQL_CALC_FOUND_ROWS {needDistinct}");
                    for (int i = 0; i < columnAttrList.Count; i++)
                    {
                        var columnAttr = columnAttrList[i];
                        var commaKeywordOrdNot = i == columnAttrList.Count - 1 ? " " : ", ";
                        if (columnAttr.BelongTable == "emp_data")
                        {
                            continue;
                        }
                        selectCmd.Append($"`{columnAttr.Name}`{commaKeywordOrdNot}");
                    }
                    selectCmd.Append($"FROM {mainTable}");
                    selectCmd.Append(conditions.GetSelectWhereCommand());
                    break;
            }

            var orderbyColumnList = columnAttrList.Where(x => x.OrderBy).OrderBy(x => x.OrderByIndex).ToList();
            for (int i = 0; i < orderbyColumnList.Count; i++)
            {
                var orderbyKeywordOrNot = i == 0 ? " ORDER BY " : " ";
                var CommaKeywordOrNot = i == orderbyColumnList.Count - 1 ? "" : ", ";
                var orderbyTypeOrNot = i == orderbyColumnList.Count - 1 ? orderbyColumnList[i].OrderByType == OrderByType.Asc ? " ASC " : " DESC " : "";
                var orderbyColumn = "";
                switch (viewmodel.GetTableSelectType())
                {
                    case SelectType.Common:
                        orderbyColumn = $"{orderbyKeywordOrNot}`{orderbyColumnList[i].Name}`{CommaKeywordOrNot}{orderbyTypeOrNot}";
                        break;
                    case SelectType.Complicated:
                        var tableName = string.IsNullOrEmpty(orderbyColumnList[i].BelongTableAlias) ? orderbyColumnList[i].BelongTable : orderbyColumnList[i].BelongTableAlias;
                        var tableAlias = string.IsNullOrEmpty(tableName) ? tableList[mainTable].AliasName : tableList[tableName].AliasName;
                        orderbyColumn = $"{orderbyKeywordOrNot}{tableAlias}.`{orderbyColumnList[i].Name}`{CommaKeywordOrNot}{orderbyTypeOrNot}";
                        break;
                }
                selectCmd.Append(orderbyColumn);
            }
            return selectCmd.ToString();
        }

        /// <summary>
        /// 動態產生Where條件字串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditions"></param>
        /// <param name="tableList"></param>
        /// <returns></returns>
        public static string GetSelectWhereCommand<T>(this T conditions, Dictionary<string, TableMapper> tableList = null) where T : IBasePager
        {
            //TODO: Between處理尚未包含Or Statement
            var whereClauseCmd = new StringBuilder();
            var whereClauseArr = new Dictionary<AndOrClause, List<string>>();
            whereClauseArr.Add(AndOrClause.AndClause, new List<string>());
            whereClauseArr.Add(AndOrClause.OrClause, new List<string>());
            var betweenPropertyInfo = new List<PropertyInfo>();
            var betweenWithTwoColumnPropertyInfo = new List<PropertyInfo>();
            // Step1: Get Property Info
            PropertyInfo[] myPropertyInfo = conditions.GetType().GetProperties();

            // Step2: Property Info Foreach
            for (int i = 0; i < myPropertyInfo.Length; i++)
            {
                var colAttr = myPropertyInfo[i].GetCustomAttributes(false).OfType<ColumnAttribute>().FirstOrDefault();
                if (colAttr != null)
                {
                    string tablekeyword = GetTablekeyword(tableList, colAttr);
                    var colName = $"{tablekeyword}{colAttr.Name}";
                    var paraName = $"@{myPropertyInfo[i].Name}";
                    var whereClause = colAttr.WhereTypeDefine;

                    var propValue = myPropertyInfo[i].GetValue(conditions);
                    if (propValue != null)
                    {
                        var andOrClauseType = colAttr.OrClause ? AndOrClause.OrClause : AndOrClause.AndClause;
                        switch (whereClause)
                        {
                            case WhereClause.Like:
                                whereClauseArr[andOrClauseType].Add($"{colName} LIKE CONCAT(\'%\', {paraName}, \'%\')");
                                break;
                            case WhereClause.LikeStartWith:
                                whereClauseArr[andOrClauseType].Add($"{colName} LIKE CONCAT({paraName}, \'%\')");
                                break;
                            case WhereClause.LikeEndWith:
                                whereClauseArr[andOrClauseType].Add($"{colName} LIKE CONCAT(\'%\', {paraName})");
                                break;
                            case WhereClause.Between:
                                betweenPropertyInfo.Add(myPropertyInfo[i]);
                                break;
                            case WhereClause.BetweenWithTwoColumns:
                                betweenWithTwoColumnPropertyInfo.Add(myPropertyInfo[i]);
                                break;
                            case WhereClause.GreaterThan:
                                whereClauseArr[andOrClauseType].Add($"{colName} > {paraName}");
                                break;
                            case WhereClause.LessThan:
                                whereClauseArr[andOrClauseType].Add($"{colName} < {paraName}");
                                break;
                            case WhereClause.GreaterThanOrEqualTo:
                                whereClauseArr[andOrClauseType].Add($"{colName} >= {paraName}");
                                break;
                            case WhereClause.LessThanOrEqualTo:
                                whereClauseArr[andOrClauseType].Add($"{colName} <= {paraName}");
                                break;
                            case WhereClause.In:
                                whereClauseArr[andOrClauseType].Add($"{colName} in {paraName}");
                                break;
                            case WhereClause.EqualTo:
                                whereClauseArr[andOrClauseType].Add($"{colName} = {paraName}");
                                break;
                            default: //WhereClause.NotEqualTo:
                                whereClauseArr[andOrClauseType].Add($"{colName} <> {paraName}");
                                break;
                        }
                    }
                }
            }

            // Step3: Concat Between Clause
            foreach (var item in betweenWithTwoColumnPropertyInfo)
            {
                var colAttr = item.GetCustomAttributes(false).OfType<ColumnAttribute>().FirstOrDefault();
                string tableKeyword = GetTablekeyword(tableList, colAttr);
                var propValue = item.GetValue(conditions);
                var whereCmd = $"@{item.Name} BETWEEN {tableKeyword}{colAttr.BetweenStart} AND {tableKeyword}{colAttr.BetweenEnd}";
                whereClauseArr[AndOrClause.AndClause].Add(whereCmd);
            }
            if (betweenPropertyInfo.Count > 0 && betweenPropertyInfo.Count % 2 == 0)
            {
                var belongColumnKeys = new Dictionary<string, List<string>>();
                var propColAttrList = new Dictionary<string, ColumnAttribute>();
                foreach (var item in betweenPropertyInfo)
                {
                    var colAttr = item.GetCustomAttributes(false).OfType<ColumnAttribute>().FirstOrDefault();
                    if (colAttr.BelongColumn == null) continue;
                    if (belongColumnKeys.ContainsKey(colAttr.BelongColumn))
                    {
                        belongColumnKeys[colAttr.BelongColumn].Add(item.Name);
                    }
                    else
                    {
                        belongColumnKeys.Add(colAttr.BelongColumn, new List<string>() { item.Name });
                    }
                    propColAttrList.Add(item.Name, colAttr);
                }

                foreach (var item in belongColumnKeys)
                {
                    var propItems = propColAttrList.Where(x => item.Value.Contains(x.Key));
                    var tableKeyWordOrNot = GetTablekeyword(tableList, propItems.FirstOrDefault().Value);
                    var betweenWhereClause = $" {tableKeyWordOrNot}{item.Key} BETWEEN ";
                    var betweenStart = propItems.FirstOrDefault(x => x.Value.BetweenDefine == BetweenIndentify.Start);
                    betweenWhereClause += $" @{betweenStart.Key} AND ";
                    var betweenEnd = propItems.FirstOrDefault(x => x.Value.BetweenDefine == BetweenIndentify.End);
                    betweenWhereClause += $" @{betweenEnd.Key}";

                    whereClauseArr[AndOrClause.AndClause].Add(betweenWhereClause);
                }
            }

            // Stop4: Concat All WhereClause string
            if (whereClauseArr[AndOrClause.AndClause].Count > 0
                || whereClauseArr[AndOrClause.OrClause].Count > 0)
            {
                whereClauseCmd.Append(" WHERE");
            }
            // Stop4-1: OrClause
            if (whereClauseArr[AndOrClause.OrClause].Count > 1)
            {
                var tempOrClauseString = new StringBuilder();
                for (int i = 0; i < whereClauseArr[AndOrClause.OrClause].Count; i++)
                {
                    var startKeyWordOrNot = i == 0 ? " ( " : "";
                    var endKeyWordOrNot = i == whereClauseArr[AndOrClause.OrClause].Count - 1 ? " ) " : " OR ";
                    tempOrClauseString.Append($" {startKeyWordOrNot} {whereClauseArr[AndOrClause.OrClause][i]}{endKeyWordOrNot}");
                }
                whereClauseArr[AndOrClause.AndClause].Add(tempOrClauseString.ToString());
            }
            // Stop4-2: AndClause
            for (int i = 0; i < whereClauseArr[AndOrClause.AndClause].Count; i++)
            {
                //var whereKeyWordOrNot = i == 0 ? " WHERE " : "";
                var andKeyWordOrNot = i == whereClauseArr[AndOrClause.AndClause].Count - 1 ? "" : " AND ";
                whereClauseCmd.Append($" {whereClauseArr[AndOrClause.AndClause][i]}{andKeyWordOrNot} ");
            }
            return whereClauseCmd.ToString();
        }

        /// <summary>
        /// 動態產生Insert語法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetInsertCommand<T>(this T model)
        {
            var insertCmd = new StringBuilder();

            // Step1: Get Property Info
            PropertyInfo[] myPropertyInfo = model.GetType().GetProperties(BindingFlags.Public
                | BindingFlags.Instance
                | BindingFlags.FlattenHierarchy);
            if (myPropertyInfo.Length == 0) return insertCmd.ToString();

            // Step2: Initial Insert Command
            insertCmd.Append($"INSERT INTO {model.GetType().GetTableName().Replace("ViewModel", "")}");
            insertCmd.Append("(");

            // Step3: Concat Column/Value string to insert command
            var colList = new List<string>();
            var paraList = new List<string>();
            myPropertyInfo.GetColParaList(model, out colList, out paraList, ColumnMode.All, false);
            var colStr = "";
            var paraStr = "";
            for (int i = 0; i < colList.Count; i++)
            {
                var commaOrNot = i == colList.Count - 1 ? "" : ",";
                colStr += $"`{colList[i]}`{commaOrNot} ";
                paraStr += $"`{paraList[i]}`{commaOrNot}";
            }
            insertCmd.Append($" {colStr} ) VALUES ( {paraStr} );");

            return insertCmd.ToString();
        }

        /// <summary>
        /// 動態產生 Batch Insert 語法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetBatchInsertCommand<T>(this T model)
        {
            var insertCmd = new StringBuilder();

            // Step1: Get Property Info
            PropertyInfo[] myPropertyInfo = model.GetType().GetProperties(BindingFlags.Public
                | BindingFlags.Instance
                | BindingFlags.FlattenHierarchy);
            if (myPropertyInfo.Length == 0) return insertCmd.ToString();

            // Step2: Initial Insert Command
            insertCmd.Append($"INSERT INTO {model.GetType().GetTableName().Replace("ViewModel", "")}");
            insertCmd.Append("(");

            // Step3: Concat Column/Value string to insert command
            var colList = new List<string>();
            var paraList = new List<string>();
            myPropertyInfo.GetColParaList(model, out colList, out paraList, ColumnMode.All, false);
            var colStr = "";
            var paraStr = "";
            for (int i = 0; i < colList.Count; i++)
            {
                var commaOrNot = i == colList.Count - 1 ? "" : ",";
                colStr += $"`{colList[i]}`{commaOrNot} ";
                paraStr += $"`{paraList[i]}`{commaOrNot} ";
            }
            insertCmd.Append($" {colStr} ) VALUES ( {paraStr} );");

            return insertCmd.ToString();
        }

        /// <summary>
        /// 動態產生Update語法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetUpdateCommand<T>(this T model)
        {
            var updateCmd = new StringBuilder();

            // Step1: Get Property Info
            PropertyInfo[] myPropertyInfo = model.GetType().GetProperties();
            if (myPropertyInfo.Length == 0) return updateCmd.ToString();

            // Step2: Initial Update Command
            updateCmd.Append($"UPDATE {model.GetType().GetTableName().Replace("ViewModel", "")}");

            // Step3: Concat Column/Value string to update command
            var colList = new List<string>();
            var paraList = new List<string>();
            myPropertyInfo.GetColParaList(model, out colList, out paraList, ColumnMode.NonKey);
            for (int i = 0; i < colList.Count; i++)
            {
                var setKeywordOrNot = i == 0 ? " SET " : "";
                var commaOrNot = i == colList.Count - 1 ? " " : ",";
                updateCmd.Append($"{setKeywordOrNot} `{colList}` = {paraList} {commaOrNot}");
            }

            // Step4: Concat Where string to update command
            var keyColList = new List<string>();
            var keyParaList = new List<string>();
            myPropertyInfo.GetColParaList(model, out keyColList, out keyParaList, ColumnMode.Key);
            for (int i = 0; i < keyColList.Count; i++)
            {
                var whereKeyWordOrNot = i == 0 ? " WHERE " : " ";
                var andKeyWordOrNot = i == keyColList.Count - 1 ? " " : " AND";
                updateCmd.Append($"{whereKeyWordOrNot}{keyColList[i]} = {keyParaList} {andKeyWordOrNot}");
            }

            return updateCmd.ToString();
        }

        /// <summary>
        /// 取得單筆Model資料語法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetSelectCommand<T>(this T model)
        {
            var selectCmd = new StringBuilder();

            // Step1: Get Property Info
            PropertyInfo[] myPropertyInfo = model.GetType().GetProperties();
            if (myPropertyInfo.Length == 0) return selectCmd.ToString();

            // Step2: Initial Select Command
            selectCmd.Append($"SELCT ");
            // Step3: Concat Column/Value string to select command
            // TODO: 需要過濾外來鍵Column
            selectCmd.Append("* ");
            selectCmd.Append($" FROM {model.GetType().GetTableName()} ");

            // Step4: Concat Where string to select command
            var keyColList = new List<string>();
            var keyParaList = new List<string>();
            myPropertyInfo.GetColParaList(model, out keyColList, out keyParaList, ColumnMode.Key, true);
            for (int i = 0; i < keyColList.Count; i++)
            {
                var whereKeyWordOrNot = i == 0 ? " WHERE " : " ";
                var andKeyWordOrNot = i == keyColList.Count - 1 ? " " : " AND ";
                selectCmd.Append($"{whereKeyWordOrNot}{keyColList[i]} = {keyParaList[i]} {andKeyWordOrNot}");
            }

            return selectCmd.ToString();
        }

        /// <summary>
        /// 依照key值取得刪除語法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetDeleteCommand<T>(this T model)
        {
            var deleteCmd = new StringBuilder();

            // Step1: Get Property Info
            PropertyInfo[] myPropertyInfo = model.GetType().GetProperties();
            if (myPropertyInfo.Length == 0) return deleteCmd.ToString();

            deleteCmd.Append($"DELETE FROM {model.GetType().GetTableName().Replace("ViewModel", "")}");

            // Step2: Delete Where Command
            var keyColList = new List<string>();
            var keyParaList = new List<string>();
            myPropertyInfo.GetColParaList(model, out keyColList, out keyParaList, ColumnMode.Key);
            for (int i = 0; i < keyColList.Count; i++)
            {
                var whereKeyWordOrNot = i == 0 ? " WHERE " : " ";
                var andKeyWordOrNot = i == keyColList.Count - 1 ? " " : " AND ";
                deleteCmd.Append($"{whereKeyWordOrNot}{keyColList[i]} = {keyParaList[i]} {andKeyWordOrNot}");
            }

            return deleteCmd.ToString();
        }

        #region --- Private Methods---

        private static string GetTablekeyword(Dictionary<string, TableMapper> tableList, ColumnAttribute colAttr)
        {
            var tableName = string.IsNullOrEmpty(colAttr.BelongTableAlias) ? colAttr.BelongTable : colAttr.BelongTableAlias;
            return tableList == null ?
                "" : string.IsNullOrEmpty(tableName) ?
                $"{tableList.FirstOrDefault().Value.AliasName}." : $"{tableList[tableName].AliasName}.";
        }

        /// <summary>
        /// 取得Column清單級Parameter格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myPropertyInfo"></param>
        /// <param name="model"></param>
        /// <param name="colList"></param>
        /// <param name="paraList"></param>
        /// <param name="columnMode"></param>
        /// <param name="needToCheckValue"></param>
        private static void GetColParaList<T>(this PropertyInfo[] myPropertyInfo
            , T model
            , out List<string> colList
            , out List<string> paraList
            , ColumnMode columnMode = ColumnMode.All
            , bool needToCheckValue = true)
        {
            colList = new List<string>();
            paraList = new List<string>();
            for (int i = 0; i < myPropertyInfo.Length; i++)
            {
                var colAttr = myPropertyInfo[i].GetCustomAttributes(true).OfType<ColumnAttribute>().FirstOrDefault();
                if (colAttr == null)
                {
                    continue;
                }
                var propValue = myPropertyInfo[i].GetValue(model);
                if (needToCheckValue)
                {
                    if (propValue == null)
                    {
                        if (colAttr.AllowNull == false)
                        {
                            continue;
                        }
                    }
                }
                if (colAttr != null)
                {
                    switch (columnMode)
                    {
                        case ColumnMode.Key:
                            if (colAttr.IsKey)
                            {
                                colList.Add(colAttr.Name);
                                paraList.Add($"@{myPropertyInfo[i].Name}");
                            }
                            break;
                        case ColumnMode.NonKey:
                            if (colAttr.IsKey == false)
                            {
                                colList.Add(colAttr.Name);
                                paraList.Add($"@{myPropertyInfo[i].Name}");
                            }
                            break;
                        default:
                            colList.Add(colAttr.Name);
                            paraList.Add($"@{myPropertyInfo[i].Name}");
                            break;
                    }
                }
            }
        }

        #endregion
    }
}
