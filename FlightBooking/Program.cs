using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Configuration;

namespace FlightBooking
{
	class MainClass
	{

		private const string DBCONFIG_FILENAME = "dbconn.properties";

		private Query q;

		public MainClass()
		{
			/* prepare the database connection stuff */
			q = new Query();
			q.OpenConnection();
		}

		public static void Usage ()
		{
			/* prints the choices for commands and parameters */
			Console.WriteLine ();
			Console.WriteLine (" *** Please enter one of the following commands *** ");
			Console.WriteLine ("> login <username> <password>");
			Console.WriteLine ("> search <origin_city> <destination_city> <direct> <date> <nb itineraries>");
			Console.WriteLine ("> book <itinerary_id>");
			Console.WriteLine ("> reservations");
			Console.WriteLine ("> cancel <reservation_id>");
			Console.WriteLine ("> quit");
		}

		public string[] Tokenize(string command)
		{
			string pat = "\"([^\"]*)\"|(\\S+)";
			Regex r = new Regex(pat);
			// Match the regular expression pattern against a text string.
			Match m = r.Match(command);
			List<string> tokens = new List<string> ();
			while (m.Success) 
			{
				for (int i = 1; i <= 2; i++) 
				{
					Group g = m.Groups[i];
					if (g.Length > 0)
					{
						tokens.Add (g.Value);
					}
				}
				m = m.NextMatch();
			}
			return tokens.ToArray();
		}

		public void Menu ()
		{
			/* prepare to read the user's command and parameter(s) */
			string command = null;

			while (true) {
				Usage ();
				Console.Write ("> ");
				command = Console.ReadLine ();
				string[] tokens = Tokenize (command.Trim ());
				if (tokens.Length == 0) {
					Console.WriteLine ("Please enter a command");
					continue; // back to top of loop
				}

				if (tokens [0].Equals ("login")) {
					if (tokens.Length == 3) {
						/* authenticate the user */
						string username = tokens [1];
						string password = tokens [2];
						//q.transaction_login(username, password);
					} else {
						Console.WriteLine ("Error: Please provide a username and password");
					}
				} else if (tokens [0].Equals ("search")) {
					/* search for flights */
					if (tokens.Length == 6) {
						string originCity = tokens [1];
						string destinationCity = tokens [2];
						bool direct = tokens [3].Equals ("1");
						int day;
						int count;
						try {
							day = Int32.Parse (tokens [4]);
							count = Int32.Parse (tokens [5]);
						} catch (FormatException e) {
							Console.WriteLine ("Failed to parse integer");
							continue;
						}
						Console.WriteLine ("Searching for flights");
						//q.Transaction_search_unsafe(originCity, destinationCity, direct, day, count);
						q.Transaction_search_safe(originCity, destinationCity, direct, day, count);
					} else {
						Console.WriteLine (
							"Error: Please provide all search parameters <origin_city> <destination_city> <direct> <date> <nb itineraries>");
					}
				} else if (tokens [0].Equals ("book")) {
					/* book a flight ticket */
					if (tokens.Length == 2) {
						int itinerary_id = Int32.Parse (tokens [1]);
						Console.WriteLine ("Booking itinerary.");
						//q.transaction_book(itinerary_id);
					} else {
						Console.WriteLine ("Error: Please provide an itinerary_id");
					}
				} else if (tokens [0].Equals ("reservations")) {
					/* list all reservations */
					//q.transaction_reservations();
				} else if (tokens [0].Equals ("cancel")) {
					/* cancel a reservation */
					if (tokens.Length == 2) {
						int reservation_id = Int32.Parse (tokens [1]);
						Console.WriteLine ("Canceling reservation.");
						//q.transaction_cancel(reservation_id);
					} else {
						Console.WriteLine ("Error: Please provide a reservation_id");
					}
				} else if (tokens [0].Equals ("quit")) {
					Environment.Exit (0);
				} else {
					Console.WriteLine ("Error: unrecognized command '" + tokens [0] + "'");
				}
			}
		}

		public static void Main (string[] args)
		{
			MainClass app = new MainClass ();
			app.Menu ();
			Console.ReadLine ();
		}

	}
}
