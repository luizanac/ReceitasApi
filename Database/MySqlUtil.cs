using System;
using ReceitasApi.Constants;

namespace ReceitasApi.Database {
    public class MySqlUtil {
        public static string GetConnectionString () {
            return "Server={db_server};Database={db_name};Uid={db_user};Pwd={db_password};SslMode=Preferred;ConvertZeroDateTime=True;AllowZeroDateTime=True;"
                .Replace ("{db_user}", Environment.GetEnvironmentVariable (EnvVars.DbUserName))
                .Replace ("{db_password}", Environment.GetEnvironmentVariable (EnvVars.DbPassword))
                .Replace ("{db_server}", Environment.GetEnvironmentVariable (EnvVars.DbServer))
                .Replace ("{db_name}", Environment.GetEnvironmentVariable (EnvVars.DbName));
        }
    }
}