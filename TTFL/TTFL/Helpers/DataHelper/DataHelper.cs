using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

using TTFL.Models;

namespace TTFL.Helpers.DataHelper
{
    public class DataHelper
    {
        /// <summary>
        /// Insert data to database
        /// </summary>
        /// <param name="dbCnx"></param>
        /// <param name="team"></param>
        /// <returns></returns>
        public static async Task<KeyValuePair<int?, bool>> InsertTeamDataAsync(TeamResult team)
        {
            string query = "[DBO].[INSERT_TEAM_DATA]";
            using SqlConnection con = new(Program.DbCnx);
            using DbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = query;

            cmd.Parameters.Add(new SqlParameter("@TEAM_NAME", team.Name));
            cmd.Parameters.Add(new SqlParameter("@TEAM_RANK", team.Rank));
            cmd.Parameters.Add(new SqlParameter("@PLAYERS", JsonConvert.SerializeObject(team.Players)));
            await con.OpenAsync();
            using DbDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                return new KeyValuePair<int?, bool>(Convert.ToInt32(reader["P_NUMBER"]), Convert.ToBoolean(reader["P_IS_SCRAPPED"]));
            }
            throw new Exception("Error query");
        }

        /// <summary>
        /// Check if daily scrap is done
        /// </summary>
        /// <param name="dbCnx"></param>
        /// <returns></returns>
        public static async Task<KeyValuePair<int?, bool>> CheckDailyScrapAsync()
        {
            string query = "[dbo].[CHECK_DAILY_SCRAP]";
            using SqlConnection con = new(Program.DbCnx);
            using DbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = query;

            await con.OpenAsync();
            using DbDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                return new KeyValuePair<int?, bool>(Convert.ToInt32(reader["P_NUMBER"]), Convert.ToBoolean(reader["P_IS_SCRAPPED"]));
            }
            return new KeyValuePair<int?, bool>();
        }

        /// <summary>
        /// Update daily scrap status
        /// </summary>
        /// <param name="dbCnx"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateDailyScrap(int? pickId)
        {
            string query = "[DBO].[UPDATE_DAILY_SCRAP]";
            using SqlConnection con = new(Program.DbCnx);
            using DbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = query;
            cmd.Parameters.Add(new SqlParameter("@P_NUMBER", pickId));

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return true;
        }
    }
}
