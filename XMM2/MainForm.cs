using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
namespace XMM2
{
    public partial class MainForm : Form
    {
        //  Constants to use when creating connections to the database
        private const string DbServerHost = "127.0.0.1";
        private const string DbUsername = "CurrenH";
        private const string DbUuserPassword = "dfcg22r";
        private const string DbName = "oop";

        //  Declare MySQL connection
        MySqlConnection dbConnection;

        //  Lists for movies genres and members
        List<Movie> Movies = new List<Movie>();
        List<Genre> Genres = new List<Genre>();
        List<Member> Members = new List<Member>();

        public MainForm()
        {
            InitializeComponent();


            //             == FORMAT ==
            // ( host_name, username, password, db_name )
            //  This method sets up a connection to a MySQL database
            SetDBConnection("127.0.0.1", "CurrenH", "dfcg22r", "oop");
            // =======================================================


            getMoviesFromDB();

            getGenresFromDB();

            getMembersFromDB();
        }

        private void SetDBConnection(string serverAddress, string username, string password, string dbName)
        {
            //  String to connect to database
            string connection = "Host=" + serverAddress + "; Username=" + username + "; Password=" + password + "; Database=" + dbName + ";";

            dbConnection = new MySqlConnection(connection);
        }

        private MySqlConnection CreateDBConnection(string serverAddress, string username, string passwd, string dbName)
        {
            //  String to connect to database
            string connection = "Host=" + serverAddress + "; Username=" + username + "; Password=" + passwd + "; Database=" + dbName + ";";

            dbConnection = new MySqlConnection(connection);

            return dbConnection;
        }
        private void getMoviesFromDB()
        {
            //  Clear ListView
            moviesListView.Items.Clear();

            //  Clear movie list
            Movies.Clear();

            Movie currentMovie;

            //  Connect to the database
            dbConnection.Open();

            //  String representing the SQL query to be executed           
            string sqlQuery = "SELECT * FROM movie;";
            Console.WriteLine("SQL Query: " + sqlQuery);

            //  SQL containing the query to be executed
            MySqlCommand dbCommand = new MySqlCommand(sqlQuery, dbConnection);

            //  Stores the result of the SQL query sent to the database
            MySqlDataReader dataReader = dbCommand.ExecuteReader();

            Console.WriteLine("\n=================");

            try
            {
                //  Read all lines
                while (dataReader.Read())
                {

                    //  Declare new movie
                    currentMovie = new Movie();

                    currentMovie.ID = dataReader.GetInt32(0);
                    currentMovie.Title = dataReader.GetString(1);
                    currentMovie.Year = dataReader.GetInt32(2);
                    currentMovie.Length = dataReader.GetInt32(3);
                    currentMovie.Rating = dataReader.GetDouble(4);

                    //  Load all genres associated with the movie
                    currentMovie.Genres = LoadMovieGenres(currentMovie.ID); 

                    //  If the image path is empty, declare string to make instance not null
                    if (dataReader.GetString(5)=="")
                    {
                        currentMovie.ImagePath = @"images\noimage.jpg";
                        movieImageList.Images.Add(Image.FromFile(currentMovie.ImagePath.ToString()));
                        Movies.Add(currentMovie);
                    }

                    else
                    {
                        currentMovie.ImagePath = dataReader.GetString(5);
                        movieImageList.Images.Add(Image.FromFile(currentMovie.ImagePath.ToString()));
                        Movies.Add(currentMovie);
                    }

                    Console.WriteLine("image = " + currentMovie.ImagePath);


                    Console.WriteLine("Title = " + currentMovie.Title);

                    //  Movies.Add(currentMovie);

                }
            }
            //  Error checking
            catch
            {
                MessageBox.Show("Error detected here.");
            }            

            //  After executing the queries close the connection
            dbConnection.Close();

            //  Show movies method
            DisplayMovies();
        }

        private List<Genre> LoadMovieGenres(int movieID)
        {
            //  The following objects will be used to access the jt_genre_movie table
            MySqlConnection dbConnection2 = CreateDBConnection(DbServerHost, DbUsername, DbUuserPassword, DbName);
            MySqlCommand dbCommand2;
            MySqlDataReader dataReader2;

            //The following objects will be used to access the genre table
            MySqlConnection dbConnection3 = CreateDBConnection(DbServerHost, DbUsername, DbUuserPassword, DbName);
            MySqlCommand dbCommand3;
            MySqlDataReader dataReader3;


            string currentGenreCode;

            Genre currentGenre;

            //  Declare genre list
            List<Genre> GenreList = new List<Genre>();

            //  Open DB connection
            dbConnection2.Open();

            //  SQL query checking
            string sqlQuery = "SELECT genre_code FROM jt_genre_movie WHERE movie_id = " + movieID + ";";

            Console.WriteLine("sqlQuery = " + sqlQuery);

            dbCommand2 = new MySqlCommand(sqlQuery, dbConnection2);

            dataReader2 = dbCommand2.ExecuteReader();

            //  While there are genre_codes in dataReader2
            while (dataReader2.Read())
            {
                currentGenre = new Genre();

                currentGenreCode = dataReader2.GetString(0);

                //  Open a connection to access the genre table
                dbConnection3.Open();

                //  SQL query checking
                sqlQuery = "SELECT * FROM genre WHERE code = '" + currentGenreCode + "';";

                Console.WriteLine("sqlQuery = " + sqlQuery);

                dbCommand3 = new MySqlCommand(sqlQuery, dbConnection3);

                dataReader3 = dbCommand3.ExecuteReader();

                //  Read a line from the genre table
                dataReader3.Read();

                //  Associate read genre objects with the data reader
                currentGenre.Code = dataReader3.GetString(0);
                currentGenre.Name = dataReader3.GetString(1);
                currentGenre.Description = dataReader3.GetString(2);

                Console.WriteLine("currentGenre = " + currentGenre.Code + " - " + currentGenre.Name + " - " + currentGenre.Description);

                //  Add to the genre list
                GenreList.Add(currentGenre);

                //  Close DB connection
                dbConnection3.Close();
            }

            //  Close DB connection
            dbConnection2.Close();

            return GenreList;
        }

       
        private void DisplayMovies()
        {
            for (int i = 0; i < Movies.Count; i++)
            {
                moviesListView.Items.Add(Movies[i].Title);
                moviesListView.Items[i].SubItems.Add(Movies[i].Year.ToString());
                moviesListView.Items[i].SubItems.Add(Movies[i].Length.ToString());
            }
        }

        private void FormatListView()
        {
            //  Formatting for all ListView columns

            ColumnHeader columnHeader1 = new ColumnHeader();
            columnHeader1.Text = "Title";
            columnHeader1.TextAlign = HorizontalAlignment.Left;
            columnHeader1.Width = 160;
            moviesListView.Columns.Add(columnHeader1);
            moviesListView.View = View.Details;

            ColumnHeader columnHeader2 = new ColumnHeader();
            columnHeader2.Text = "Year";
            columnHeader2.TextAlign = HorizontalAlignment.Left;
            columnHeader2.Width = 80;
            moviesListView.Columns.Add(columnHeader2);

            ColumnHeader columnHeader3 = new ColumnHeader();
            columnHeader3.Text = "Length";
            columnHeader3.TextAlign = HorizontalAlignment.Left;
            columnHeader3.Width = 80;
            moviesListView.Columns.Add(columnHeader3);

            //  Disable TextBoxes to be made read only and not modifiable
            titleTextBox.Enabled = false;
            yearTextBox.Enabled = false;
            lengthTextBox.Enabled = false;
            genreTextBox.Enabled = false;
            imagePathTextBox.Enabled = false;
            ratingTextBox.Enabled = false;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //  Call method to format movieListView
            FormatListView();
        }
        private void getGenresFromDB()
        {
            Genre currentGenre;

            //  Connect to the database
            dbConnection.Open();

            //  SQL query to execute in the database          
            string sqlQuery = "SELECT * FROM genre;";
            Console.WriteLine("SQL Query: " + sqlQuery);

            //  SQL containing the query to be executed
            MySqlCommand dbCommand = new MySqlCommand(sqlQuery, dbConnection);

            //  Result of the SQL query sent to the database
            MySqlDataReader dataReader = dbCommand.ExecuteReader();

            Console.WriteLine("\n=================");

            //  Read each line
            while (dataReader.Read())
            {

                //  Declare new movie
                currentGenre = new Genre();

                currentGenre.Code = dataReader.GetString(0);
                currentGenre.Name = dataReader.GetString(1);

                Console.WriteLine("Code = " + currentGenre.Code+"\n"+ "Name = "+ currentGenre.Name);

                Genres.Add(currentGenre);

            }

            //  Close DB connection
            dbConnection.Close();

            //  Display genres method
            DisplayGenres();

        }

        private void DisplayGenres()
        {
            //  For each genre in the list, display it in the ListBox
            for (int i = 0; i < Genres.Count; i++)
            {
                //  Add genre names
                genreListBox.Items.Add(Genres[i].Name);
            }
        }

        private void getMembersFromDB()
        {
            //  Clear members list
            Members.Clear();

            Member currentMember;

            //  Connect to the database
            dbConnection.Open();

            //  This is a string representing the SQL query to execute in the db            
            string sqlQuery = "SELECT * FROM member;";
            Console.WriteLine("SQL Query: " + sqlQuery);

            //  This is the actual SQL containing the query to be executed
            MySqlCommand dbCommand = new MySqlCommand(sqlQuery, dbConnection);

            //  This variable stores the result of the SQL query sent to the db
            MySqlDataReader dataReader = dbCommand.ExecuteReader();

            Console.WriteLine("\n=================");

            //  Read each line
            while (dataReader.Read())
            {

                //  Declare new member object
                currentMember = new Member();

                //  Associate read member objects with the data reader
                currentMember.ID = dataReader.GetInt32(0);
                currentMember.Name = dataReader.GetString(1);
                currentMember.DOB = dataReader.GetDateTime(2);
                currentMember.Type = dataReader.GetInt32(3);
                currentMember.ImagePath = dataReader.GetString(4);

                //  Add image to imageList
                membersImageList.Images.Add(Image.FromFile(currentMember.ImagePath.ToString()));

                Console.WriteLine("Code = " + currentMember.ID + "\n" + "Name = " + currentMember.Name);

                //  Add to members list
                Members.Add(currentMember);

            }
            //  close DB connection
            dbConnection.Close();

            //  Display members method
            DisplayMembers();
        }


        private void DisplayMembers()
        {
            //  For each member in the list, display it in the ListBox
            for (int i = 0; i < Members.Count; i++)
            {
                //  Add member names
                membersListBox.Items.Add(Members[i].Name);
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            //  Closes the form
            this.Close();
        }

        private void MoviesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                int index = moviesListView.FocusedItem.Index;

                //  If there is no item selected
                if (moviesListView.SelectedIndices.Count <= 0)
                {
                    return;
                }

                //  Declare index value as integer variable
                int intselectedindex = moviesListView.SelectedIndices[0];

                //  If there is an item selected
                if (intselectedindex >= 0)
                {
                    //  String selected ListView item (movie title) as text
                    String text = moviesListView.Items[intselectedindex].Text;

                    //  String variable for Regex argument (input, pattern, replacement string data)
                    string replacement = Regex.Replace(text, @"\t|\n|\r", "");

                    //  Find image index for movieList to affiliate the correct image with the selected movie
                    int imageIndex = Movies.FindIndex(a => a.Title == replacement);
                    moviePictureBox.Image = movieImageList.Images[imageIndex];
                }

                for (int i = 0; i < moviesListView.Items.Count; i++)
                {
                    //  Display movie info in TextBoxes
                    if (moviesListView.Items[i].Selected)
                    {
                        //  TextBoxes to display information requested
                        titleTextBox.Text = moviesListView.Items[i].SubItems[0].Text;
                        yearTextBox.Text = moviesListView.Items[i].SubItems[1].Text;
                        lengthTextBox.Text = moviesListView.Items[i].SubItems[2].Text;
                    }
                }

                //  Declare selected item in ListView to be the focused element
                int selected = moviesListView.FocusedItem.Index;

                //  Declare variables for movie and genre
                Movie getMovie;
                Genre getGenre;

                for (int i = 0; i < moviesListView.Items.Count; i++)
                    if (moviesListView.Items[i].Selected)
                    {
                        //  Selected items for the lists
                        getMovie = (Movie)Movies[selected];
                        getGenre = (Genre)Genres[selected];

                        //  TextBoxes to display information requested
                        ratingTextBox.Text = getMovie.Rating.ToString();
                        genreTextBox.Text = getGenre.Name;
                        imagePathTextBox.Text = getMovie.ImagePath;
                    }
                
            }

            catch
            {
                //  Display an error message.
                //  MessageBox.Show("Error Detected.");
            }
        }

        private void actorsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //  Declare selected item in ListBox to be the focused element
                int selected = membersListBox.SelectedIndex;

                //  Declare member variable
                Member checkMember;

                if (membersListBox.SelectedIndex != -1)
                {
                    //  Selected items for the lists
                    checkMember = (Member)Members[selected];

                    //  TextBoxes to display information requested
                    memberNameTextBox.Text = checkMember.Name;
                    memberDOBTextBox.Text = checkMember.DOB.ToString();
                    memberImagePathTextBox.Text = checkMember.ImagePath;

                    //  Check assigned integer for each member type and display correct string
                    if (checkMember.Type == 1)
                    {
                        memberTypeTextBox.Text = "Actor/Actresse";
                    }
                    else if (checkMember.Type == 2)
                    {
                        memberTypeTextBox.Text = "Director";
                    }
                    else if (checkMember.Type == 3)
                    {
                        memberTypeTextBox.Text = "Producer";
                    }
                    else if (checkMember.Type == 4)
                    {
                        memberTypeTextBox.Text = "Director of photography";
                    }
                }

                //-------------------------------

                //  If there is no selected item
                if (membersListBox.SelectedIndices.Count <= 0)
                {
                    return;
                }

                int selectedindex = membersListBox.SelectedIndices[0];


                if (selectedindex >= 0)
                {
                    //  String selected ListBox item (member name) as text
                    String text = membersListBox.Items[selectedindex].ToString();

                    //  String variable for Regex argument (input, pattern, replacement string data)
                    string replacement = Regex.Replace(text, @"\t|\n|\r", "");

                    //  Find image index for member to affiliate the correct image with the selected member
                    int index = Members.FindIndex(a => a.Name == replacement);

                    try
                    {
                        //  Display correct member image
                        memberPictureBox.Image = membersImageList.Images[index];
                    }
                    //  If there is an incorrect/null path
                    catch
                    {
                        MessageBox.Show("No image path found.");
                    }
                    


                }
            }
            catch { }
            
        }

        private void GenreListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //  Clear ListBox
            moviesListView.Items.Clear();

            //  String item selected
            string selected = genreListBox.SelectedItem.ToString();

            foreach (Movie entry in Movies)
            {
                foreach(Genre genre in entry.Genres)
                {
                    int i = 0;

                    //  Add items to ListView with the corresponding data
                    moviesListView.Items.Add(entry.Title);
                    moviesListView.Items[i].SubItems.Add(entry.Year.ToString());
                    moviesListView.Items[i].SubItems.Add(entry.Length.ToString());

                    i++;

                }
            }
            
        }
    }
}
