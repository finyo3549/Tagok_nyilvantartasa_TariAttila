using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Tagok_nyilvantartasa_HF
{
    internal class Program
    {
        static List<Tagok> tagokList = new List<Tagok>();
        static MySqlConnection connection = null;
        static MySqlCommand command = null;
        static void Main(string[] args)
        {
            beolvas();
            megjelenit();
            ujTagFelvetele();
            ujTagTorlese();
            Console.WriteLine("\nProgram vége");
            Console.ReadLine();
        }

        private static void ujTagFelvetele()
        {
            Console.Write("Add meg az új tag azonosítóját: ");
            int azon = Convert.ToInt32(Console.ReadLine());
            Console.Write("Add meg az új tag nevét: ");
            string nev = Console.ReadLine();
            Console.Write("Add meg az új tag születési évét: ");
            int szulev = Convert.ToInt32(Console.ReadLine());
            Console.Write("Add meg az új tag irányítószámát: ");
            int irszam = Convert.ToInt32(Console.ReadLine());
            Console.Write("Add meg az új tag országát: ");
            string orsz = Console.ReadLine();
            Tagok ujTag = new Tagok(azon, nev, szulev, irszam, orsz);
            command.CommandText = "INSERT INTO `ugyfel`(`azon`, `nev`, `szulev`, `irszam`, `orsz`) VALUES  (@azon,@nev,@szulev,@irszam,@orsz)";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@azon", ujTag.azon);
            command.Parameters.AddWithValue("@nev", ujTag.nev);
            command.Parameters.AddWithValue("@szulev", ujTag.szulev);
            command.Parameters.AddWithValue("@irszam", ujTag.irszam);
            command.Parameters.AddWithValue("@orsz", ujTag.orsz);

            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }
                command.ExecuteNonQuery();
                connection.Close();
                Console.WriteLine("A rekord sikeresen beillesztve");
                tagokList.Add(ujTag);
            }

            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

    

        private static void megjelenit()
        {
            foreach(Tagok tagok in tagokList)
            {
                Console.WriteLine(tagok);
            }
        }

        private static void ujTagTorlese()
        {
            Console.WriteLine("Új tag törlése");
            Console.WriteLine("Jelenlegi tagok:");
            megjelenit();
            Console.Write("Adja meg a törlendő azonosítót: ");
            int torlendoAzon = Convert.ToInt32(Console.ReadLine());

            command.CommandText = "DELETE FROM `ugyfel` WHERE `azon` = @azon";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@azon", torlendoAzon);

            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }
                int toroltSorok = command.ExecuteNonQuery();
                connection.Close();

                if (toroltSorok > 0)
                {
                    Console.WriteLine("Sikeresen törölve");
                    tagokList.Clear();
                    beolvas();
                    Console.WriteLine("A jelenlegi rekordok:");
                    megjelenit();
                }
                else
                {
                    Console.WriteLine("Nincs ilyen azonosítójú rekord a táblában");
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        private static void beolvas()
        {
            MySqlConnectionStringBuilder sb = new MySqlConnectionStringBuilder();
            sb.Clear();
            sb.Server = "localhost";
            sb.UserID = "root";
            sb.Password = "";
            sb.Database = "tagdij";
            sb.CharacterSet = "utf8";
            connection = new MySqlConnection(sb.ConnectionString);
            command = connection.CreateCommand();
            try
            {
                connection.Open();
                command.CommandText = "SELECT * FROM ugyfel";
                using (MySqlDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Tagok uj = new Tagok(dr.GetInt32("azon"), dr.GetString("nev"), dr.GetInt32("szulev"),dr.GetInt32("irszam"), dr.GetString("orsz"));
                        tagokList.Add(uj);
                    }
                }
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(0);
            }
        }
    }
    }

