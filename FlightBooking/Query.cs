using System;
using System.Configuration;
using System.Data.SqlClient;
using Npgsql;

namespace FlightBooking
{
	public class Query
	{

		private string sqlDriver;
		private string sqlUrl;

		NpgsqlConnection conn;

		// Logged In User
		private String username;
		private int cid; // Unique customer ID

		// Canned queries

		// search (one hop) -- This query ignores the month and year entirely. You
		// can change it to fix the month and year
		// to July 2015 or you can add month and year as extra, optional, arguments
		private string SEARCH_ONE_HOP_SQL = "SELECT year, month_id, day_of_month, carrier_id, flight_num, origin_city, actual_time "
			+ "FROM Flights " + "WHERE origin_city = :originCity AND dest_city = :destCity AND day_of_month = :dayOfMonth "
			+ "ORDER BY actual_time ASC LIMIT :limit ";

		public Query ()
		{
			sqlDriver = ConfigurationManager.AppSettings ["flightservice.driver"];
			sqlUrl = ConfigurationManager.AppSettings ["flightservice.url"];
		}

		public void OpenConnection()
		{
			try {
				conn = new NpgsqlConnection(sqlUrl);
				conn.Open();
			}
			catch(Exception e) {
				Console.WriteLine (e.Message);
			}
		}

		public void CloseConnection()
		{
			conn.Close();
		}

		public void Transaction_login(string username, string password)
		{
			// Add code here
		}

		public void Transaction_search_unsafe (string originCity, string destinationCity, bool directFlight,
		                                       int dayOfMonth, int numberOfItineraries)
		{
			// one hop itineraries
			string unsafeSearchSQL = "SELECT year, month_id, day_of_month, carrier_id, flight_num, origin_city, actual_time "
			                         + "FROM Flights "
			                         + "WHERE origin_city = \'" + originCity + "\' AND dest_city = \'" + destinationCity
			                         + "\' AND day_of_month =  " + dayOfMonth + " " + "ORDER BY actual_time ASC LIMIT "
			                         + numberOfItineraries;

			Console.WriteLine ("Submitting query: " + unsafeSearchSQL);
			try {
				NpgsqlCommand stmt = new NpgsqlCommand (unsafeSearchSQL, conn);
				NpgsqlDataReader dr = stmt.ExecuteReader ();
				while (dr.Read ()) {
					int result_year = dr.GetInt32 (dr.GetOrdinal ("year"));
					int result_monthId = dr.GetInt32 (dr.GetOrdinal ("month_id"));
					int result_dayOfMonth = dr.GetInt32 (dr.GetOrdinal ("day_of_month"));
					string result_carrierId = dr.GetString (dr.GetOrdinal ("carrier_id"));
					string result_flightNum = dr.GetString (dr.GetOrdinal ("flight_num"));
					string result_originCity = dr.GetString (dr.GetOrdinal ("origin_city"));
					int result_time = dr.GetInt32 (dr.GetOrdinal ("actual_time"));
					Console.WriteLine ("Flight: " + result_year + "," + result_monthId + "," + result_dayOfMonth + ","
					+ result_carrierId + "," + result_flightNum + "," + result_originCity + "," + result_time);
				}
				dr.Close ();
			} catch (Exception e) {
				Console.WriteLine (e.Message);
			}
		}

	/**
	 * Searches for flights from the given origin city to the given destination
	 * city, on the given day of the month. If "directFlight" is true, it only
	 * searches for direct flights, otherwise is searches for direct flights and
	 * flights with two "hops". Only searches for up to the number of
	 * itineraries given. Prints the results found by the search.
	 */
		public void Transaction_search_safe (string originCity, string destCity, bool directFlight,
											 int dayOfMonth, int numberOfItineraries)
		{
			try {
				// one hop itineraries
				NpgsqlCommand stmt = new NpgsqlCommand (SEARCH_ONE_HOP_SQL, conn);
				stmt.Parameters.Add (new NpgsqlParameter ("originCity", NpgsqlTypes.NpgsqlDbType.Varchar));
				stmt.Parameters.Add (new NpgsqlParameter ("destCity", NpgsqlTypes.NpgsqlDbType.Varchar));
				stmt.Parameters.Add (new NpgsqlParameter ("dayOfMonth", NpgsqlTypes.NpgsqlDbType.Integer));
				stmt.Parameters.Add (new NpgsqlParameter ("limit", NpgsqlTypes.NpgsqlDbType.Integer));
				stmt.Prepare ();
				stmt.Parameters [0].Value = originCity;
				stmt.Parameters [1].Value = destCity;
				stmt.Parameters [2].Value = dayOfMonth;
				stmt.Parameters [3].Value = numberOfItineraries;
				NpgsqlDataReader dr = stmt.ExecuteReader ();
				while (dr.Read ()) {
					int result_year = dr.GetInt32 (dr.GetOrdinal ("year"));
					int result_monthId = dr.GetInt32 (dr.GetOrdinal ("month_id"));
					int result_dayOfMonth = dr.GetInt32 (dr.GetOrdinal ("day_of_month"));
					string result_carrierId = dr.GetString (dr.GetOrdinal ("carrier_id"));
					string result_flightNum = dr.GetString (dr.GetOrdinal ("flight_num"));
					string result_originCity = dr.GetString (dr.GetOrdinal ("origin_city"));
					int result_time = dr.GetInt32 (dr.GetOrdinal ("actual_time"));
					Console.WriteLine ("Flight: " + result_year + "," + result_monthId + "," + result_dayOfMonth + ","
					+ result_carrierId + "," + result_flightNum + "," + result_originCity + "," + result_time);
				}
				dr.Close ();
			} catch (Exception e) {
				Console.WriteLine (e.Message);
			}
			// Add code here

		}

		public void BeginTransaction(string isolationLevel) {
			try {
				NpgsqlCommand stmt = new NpgsqlCommand("BEGIN ISOLATION LEVEL SERIALIZABLE;", conn);
				stmt.ExecuteNonQuery();
			}
			catch(Exception e) {
				Console.WriteLine (e.Message);
			}
		}

		public void CommitTransaction() {
			try {
				NpgsqlCommand stmt = new NpgsqlCommand("COMMIT;", conn);
				stmt.ExecuteNonQuery();
			}
			catch(Exception e) {
				Console.WriteLine (e.Message);
			}
		}

		public void RollbackTransaction() {
			try {
				NpgsqlCommand stmt = new NpgsqlCommand("ROLLBACK;", conn);
				stmt.ExecuteNonQuery();
			}
			catch(Exception e) {
				Console.WriteLine (e.Message);
			}
		}


	}
}

