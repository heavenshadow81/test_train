using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using ML.PlaywallKids.DragonPark;

public class PlayWallDatabase {
	#region Constants
	public const string kIP = "1.215.249.42";
	public const string kDatabaseName = "sbb";
	//public const string kDatabaseName = "iMarketing_v1.0"; //전시회
	public const string kDatabaseID = "sbb";
	public const string kDatabasePW = "a%#*0058";
	#endregion

    #region Private variables
    #endregion

    public static void Query(string sql, System.Action<SqlDataReader> handler) {
		SqlConnection connection = null;
		
		Debug.Log ("PlayWallDatabase.Query() : Start DB Connection...");
		using(connection = new SqlConnection("Data Source="+kIP+"; Initial Catalog="+kDatabaseName+"; User Id="+kDatabaseID+"; Password="+kDatabasePW+";")) {
			try {
				connection.Open();
				Debug.Log ("PlayWallDatabase.Query() : DB Connected!");
				
				SqlCommand cmd = new SqlCommand(sql, connection);
				SqlDataReader sdr = cmd.ExecuteReader();
				
				if(handler != null)
					handler(sdr);
			}
			catch(System.Exception e) {	
				Debug.Log ("PlayWallDatabase.Query() : Failed to connect...");
				Debug.Log (e);
			}
			finally {
				connection.Close();
			}
		}
	}

	public static void GetWeatherInfo(System.Action<BackgroundManager.Weather> handler) {
		string sql = "select TOP 1 SKY_NAME from SBB_WEATHER ORDER BY TIME_OBSERVATION DESC";
		Query (sql, (reader)=>{
			var weather = BackgroundManager.Weather.Sunny;

			if(reader.HasRows && reader.Read()) {
				string weatherStr = reader.GetString(0);
				if(!string.IsNullOrEmpty(weatherStr)) {
					if(weatherStr.Contains("맑음")) {
						weather = BackgroundManager.Weather.Sunny;
					}
					else if(weatherStr.Contains("구름")) {
						weather = BackgroundManager.Weather.Cloudy;
					}
					else if(weatherStr.Contains("비")) {
						weather = BackgroundManager.Weather.Rainy;
					}
					else if(weatherStr.Contains("눈")) {
						weather = BackgroundManager.Weather.Snowy;
					}
				}
			}

			if(handler != null) {
				handler(weather);
			}
		});
	}
}
