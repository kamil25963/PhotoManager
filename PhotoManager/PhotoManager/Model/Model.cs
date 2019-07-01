﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.IO;
namespace PhotoManager.Model
{
    class Model
    {
		private List<Album> albums;
		public List<Album> Albums
		{
			get { return albums; }
			set { albums = value; }
		}

		public Model()
        {

        }


        #region Login
        public bool checkPassword(string formLogin, string formPassword)
        {
            var dbCon = Database.Instance();
            string passwdFromDatabase = null;
            dbCon.DatabaseName = "photomanager";
            if (dbCon.IsConnect())
            {
                string passQuery = "select password from users where login = \"" + formLogin + "\"";

                if (dbCon.Connection.State != ConnectionState.Open)
                {
                    dbCon.Connection.Open();
                }
                var cmd = new MySqlCommand(passQuery, dbCon.Connection);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    passwdFromDatabase = reader.GetString(0);
                }
                dbCon.Close();
            }
            if (passwdFromDatabase != null && SHA1Hash(formPassword) == passwdFromDatabase)
            {
                LoggingWindow.hideLoggingWindow();
                Form1.InstanceForm1.ShowDialog();

                return true;
            }
            else
                return false;
        }
        #endregion Login

        #region Register
        public bool EmailExists(User user)
        {
            string query = "select count(*) from users where email = \"" + user.Email + "\";";
            if (Database.RecordExist(query))
                return true;
            return false;
        }
        public bool UserExists(User user)
        {
            string query = "select count(*) from users where login = \"" + user.Login + "\";";
            if (Database.RecordExist(query))
                return true;
            return false;
        }

        public bool CreateAccount(User user)
        {
            var dbCon = Database.Instance();
            dbCon.DatabaseName = "photomanager";
            if (dbCon.IsConnect())
            {
                if (dbCon.Connection.State != ConnectionState.Open)
                {
                    using (MySqlCommand command = dbCon.Connection.CreateCommand())
                    {
                        command.CommandText = "insert into users values(@id,@name,@surname,@login,@password,@email);";
                        command.Parameters.AddWithValue("@id", null);
                        command.Parameters.AddWithValue("@name", user.Name);
                        command.Parameters.AddWithValue("@surname", user.Surname);
                        command.Parameters.AddWithValue("@login", user.Login);
                        command.Parameters.AddWithValue("@password", SHA1Hash(user.Password));
                        command.Parameters.AddWithValue("@email", user.Email);
                        dbCon.Connection.Open();
                        try
                        {
                            int result = command.ExecuteNonQuery();
                            if (result < 0)
                                return false;
                            else
                                return true;
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show(exc.ToString());
                        }
                    }
                }
                dbCon.Close();
            }
            return true;
        }
        #endregion Register

        #region Album

		// załadowanie albumów do comboBoxa z bazy
		public List<Album> GetAlbums()
		{
			albums = new List<Album>();

			var dbCon = Database.Instance();
			//string passwdFromDatabase = null;
			dbCon.DatabaseName = "photomanager";
			if (dbCon.IsConnect())
			{
				string passQuery = "select name from albums";

				if (dbCon.Connection.State != ConnectionState.Open)
				{
					dbCon.Connection.Open();
				}
				var cmd = new MySqlCommand(passQuery, dbCon.Connection);

				var reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					albums.Add(new Album(reader.GetString(0)));
					//passwdFromDatabase = reader.GetString(0);
				}
				dbCon.Close();
			}

			return albums;
		}

        public bool AddAlbum(Album album)
        {
            Console.WriteLine(album.Name + " | " + album.Description + " | " + album.SelectedType+ " | " + album.CreationDate.ToString());
            try
            {
                var dbCon = Database.Instance();
                dbCon.DatabaseName = "photomanager";
                if (dbCon.IsConnect())
                {
                    if (dbCon.Connection.State != ConnectionState.Open)
                    {
                        using (MySqlCommand command = dbCon.Connection.CreateCommand())
                        {
                            command.CommandText = "insert into albums values(@id,@name,@creationdate,@description,@type);";
                            command.Parameters.AddWithValue("@id", null);
                            command.Parameters.AddWithValue("@name", album.Name);
                            command.Parameters.AddWithValue("@creationdate", album.CreationDate.ToString("yyyy-MM-dd HH:mm:ss"));
                            command.Parameters.AddWithValue("@description", album.Description);
                            command.Parameters.AddWithValue("@type", album.SelectedType);

                            dbCon.Connection.Open();
                            try
                            {
                                int result = command.ExecuteNonQuery();
                                if (result < 0)
                                    return false;
                                else
                                    return true;
                            }
                            catch (Exception exc)
                            {
                                MessageBox.Show(exc.ToString(), "Problem podczas dodawania albumu.");
                            }
                        }
                    }
                    dbCon.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("DODAWANIE ZDJĘCIA BŁĄD", "Error", MessageBoxButtons.OK); //tego tu nie bedzie
            }
            return true;
        }
    
        #endregion Album

        #region Pictures

        public bool AddPhoto(string path, Photo photo)
        {
            try
            {
                var dbCon = Database.Instance();
                dbCon.DatabaseName = "photomanager";
                if (dbCon.IsConnect())
                {
                    if (dbCon.Connection.State != ConnectionState.Open)
                    {
                        using (MySqlCommand command = dbCon.Connection.CreateCommand())
                        {
                            command.CommandText = "insert into photos values(@id,@name,@creationdate,@description,@format,@size,@pictureB);";
                            command.Parameters.AddWithValue("@id", null);
                            command.Parameters.AddWithValue("@name", photo.Name);
                            command.Parameters.AddWithValue("@creationdate", photo.CreationDate);
                            command.Parameters.AddWithValue("@description", photo.Description);
                            command.Parameters.AddWithValue("@format", photo.Format);
                            command.Parameters.AddWithValue("@size", photo.PhotoSize);
                            command.Parameters.AddWithValue("@pictureB", photo.EncodePhoto(path));
                            dbCon.Connection.Open();
                            try
                            {
                                int result = command.ExecuteNonQuery();
                                if (result < 0)
                                    return false;
                                else
                                    return true;
                            }
                            catch (Exception exc)
                            {
                                MessageBox.Show(exc.ToString(), "Problem podczas dodawania zdjęcia.");
                            }
                        }
                    }
                    dbCon.Close();
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show("DODAWANIE ZDJĘCIA BŁĄD", "Error", MessageBoxButtons.OK); //tego tu nie bedzie
            }
            return true;
        }

        public void ReadPhoto(int AlbumID)
        {
            //To dopiero jak bede meić liste obiektów obiekt Album dla zalogowanego Usera, bo jest tam lista zdjęć. 
        }

        #endregion Pictures

        #region Other
        public string SHA1Hash(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(bytes);
            return HexStringFromBytes(hashBytes);
        }

        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }
        #endregion Other

    }

}
