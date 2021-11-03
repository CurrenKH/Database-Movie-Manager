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
        private const string dbHost = "127.0.0.1";
        private const string dbUsername = "CurrenH";
        private const string dbPassword = "dfcg22r";
        private const string dbName = "oop";

        //  Declare MySQL connection
        MySqlConnection dbConnection;

        //  Lists for movies genres and members
        List<Movie> movieList = new List<Movie>();
        List<Genre> genreList = new List<Genre>();
        List<Member> memberList = new List<Member>();

        public MainForm()
        {
            InitializeComponent();


            //             == FORMAT ==
            //  ( host_name, username, password, db_name )
            //  This method sets up a connection to a MySQL database
            SetDBConnection("127.0.0.1", "CurrenH", "dfcg22r", "oop");
            // =======================================================

            //  Read movies from database
            ReadMoviesDB();

            //  Read genres from database
            ReadGenresDB();

            //  Read members from database
            ReadMembersDB();
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
        private void ReadMoviesDB()
        {
            //  Clear ListView
            moviesListView.Items.Clear();

            //  Clear movie list
            movieList.Clear();

            //  Declare movie variable
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
                    currentMovie.Genres = ReadDBGenres(currentMovie.ID);

                    //  Load all members associated with the movie
                    currentMovie.Members = ReadDBMembers(currentMovie.ID);

                    //  If the image path is empty, declare string to make instance not null
                    if (dataReader.GetString(5) == "")
                    {
                        currentMovie.ImagePath = @"images\noimage.jpg";
                        movieImageList.Images.Add(Image.FromFile(currentMovie.ImagePath.ToString()));
                        movieList.Add(currentMovie);
                    }

                    else
                    {
                        currentMovie.ImagePath = dataReader.GetString(5);
                        movieImageList.Images.Add(Image.FromFile(currentMovie.ImagePath.ToString()));
                        movieList.Add(currentMovie);
                    }

                    Console.WriteLine("image = " + currentMovie.ImagePath);


                    Console.WriteLine("Title = " + currentMovie.Title);
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

        private List<Genre> ReadDBGenres(int movieID)
        {
            //  The following objects will be used to access the jt_genre_movie table
            MySqlConnection dbConnection2 = CreateDBConnection(dbHost, dbUsername, dbPassword, dbName);
            MySqlCommand dbCommand2;
            MySqlDataReader dataReader2;

            //  The following objects will be used to access the genre table
            MySqlConnection dbConnection3 = CreateDBConnection(dbHost, dbUsername, dbPassword, dbName);
            MySqlCommand dbCommand3;
            MySqlDataReader dataReader3;

            //  String for genre code
            string existingGenreCode;

            //  Declare genre variable
            Genre existingGenre;

            //  Declare genre list
            List<Genre> GenreList = new List<Genre>();

            //  Open DB connection
            dbConnection2.Open();

            //  SQL query checking
            string sqlQuery = "SELECT genre_code FROM jt_genre_movie WHERE movie_id = " + movieID + ";";

            Console.WriteLine("sqlQuery = " + sqlQuery);

            //  Pair query with db connection
            dbCommand2 = new MySqlCommand(sqlQuery, dbConnection2);

            //  Execute SQL query
            dataReader2 = dbCommand2.ExecuteReader();

            //  While there are genre codes
            while (dataReader2.Read())
            {
                //  Declare genre variable
                existingGenre = new Genre();

                //  Read genre code value
                existingGenreCode = dataReader2.GetString(0);

                //  Open a connection to access the genre table
                dbConnection3.Open();

                //  SQL query checking
                sqlQuery = "SELECT * FROM genre WHERE code = '" + existingGenreCode + "';";

                Console.WriteLine("sqlQuery = " + sqlQuery);

                //  Pair query with DB connection
                dbCommand3 = new MySqlCommand(sqlQuery, dbConnection3);

                //  Execute SQL query
                dataReader3 = dbCommand3.ExecuteReader();

                //  Read a line from genre table
                dataReader3.Read();

                //  Associate read genre objects with the data reader
                existingGenre.Code = dataReader3.GetString(0);
                existingGenre.Name = dataReader3.GetString(1);
                existingGenre.Description = dataReader3.GetString(2);

                Console.WriteLine("currentGenre = " + existingGenre.Code + " - " + existingGenre.Name + " - " + existingGenre.Description);

                //  Add to the genre list
                GenreList.Add(existingGenre);

                //  Close DB connection
                dbConnection3.Close();
            }

            //  Close DB connection
            dbConnection2.Close();

            return GenreList;
        }

        private List<Member> ReadDBMembers(int movieID)
        {
            //  The following objects will be used to access the jt_genre_movie table
            MySqlConnection dbConnection8 = CreateDBConnection(dbHost, dbUsername, dbPassword, dbName);
            MySqlCommand dbCommand8;
            MySqlDataReader dataReader8;

            //  The following objects will be used to access the genre table
            MySqlConnection dbConnection9 = CreateDBConnection(dbHost, dbUsername, dbPassword, dbName);
            MySqlCommand dbCommand9;
            MySqlDataReader dataReader9;

            //  Declare int variable for the current member ID
            int loadMemberID;

            //  Declare member variable
            Member loadMember;

            //  Declare new list for members
            List<Member> loadMemberList = new List<Member>();

            //  Open DB connection
            dbConnection8.Open();

            //  SQL query checking
            string sqlQuery = "SELECT member_id FROM jt_movie_member WHERE movie_id = " + movieID + ";";

            //  Pair query with DB connection
            dbCommand8 = new MySqlCommand(sqlQuery, dbConnection8);

            //  Execute SQL query
            dataReader8 = dbCommand8.ExecuteReader();

            //  While there are member codes
            while (dataReader8.Read())
            {
                //  Declare movie variable
                loadMember = new Member();

                //  Read member ID value
                loadMemberID = dataReader8.GetInt32(0);

                //  Open DB connection
                dbConnection9.Open();

                //  SQL query checking
                sqlQuery = "SELECT * FROM member WHERE id = '" + loadMemberID + "';";

                //  Pair query with DB connection
                dbCommand9 = new MySqlCommand(sqlQuery, dbConnection9);

                //  Execute SQL query
                dataReader9 = dbCommand9.ExecuteReader();

                //  Read a line from member table
                dataReader9.Read();

                //  Associate read member objects with the data reader
                loadMember.ID = dataReader9.GetInt32(0);
                loadMember.Name = dataReader9.GetString(1);
                loadMember.DOB = dataReader9.GetDateTime(2);
                loadMember.Type = dataReader9.GetInt32(3);

                //  Add to the member list
                loadMemberList.Add(loadMember);

                //  Close DB connection
                dbConnection9.Close();
            }
            //  Close DB connection
            dbConnection9.Close();

            return loadMemberList;
        }
        private void DisplayMovies()
        {
            for (int i = 0; i < movieList.Count; i++)
            {
                //  Create LVI and populate ListView
                ListViewItem lvi = new ListViewItem();
                lvi.Text = movieList[i].Title;
                lvi.SubItems.Add(movieList[i].Year.ToString());
                lvi.SubItems.Add(movieList[i].ID.ToString());

                //  Add object to ListView
                moviesListView.Items.Add(lvi);
            }
        }

        private void FormatListView()
        {
            //  Formatting for all ListView columns

            ColumnHeader titleColumn = new ColumnHeader();
            titleColumn.Text = "Title";
            titleColumn.Width = 160;
            moviesListView.Columns.Add(titleColumn);
            moviesListView.View = View.Details;

            ColumnHeader yearColumn = new ColumnHeader();
            yearColumn.Text = "Year";
            yearColumn.Width = 80;
            moviesListView.Columns.Add(yearColumn);

            ColumnHeader idColumn = new ColumnHeader();
            idColumn.Text = "ID";
            idColumn.Width = 80;
            moviesListView.Columns.Add(idColumn);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //  Call method to format movieListView
            FormatListView();
        }
        private void ReadGenresDB()
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

                //  Associate genre items from db
                currentGenre.Code = dataReader.GetString(0);
                currentGenre.Name = dataReader.GetString(1);

                Console.WriteLine("Code = " + currentGenre.Code + "\n" + "Name = " + currentGenre.Name);

                //  Add to genre list
                genreList.Add(currentGenre);

                //  Add to ComboBoxes
                addMovieGenreComboBox.Items.Add(currentGenre.Name);
                genreComboBox.Items.Add(currentGenre.Name);
            }

            //  Close DB connection
            dbConnection.Close();

            //  Display genres method
            DisplayGenres();

        }

        private void DisplayGenres()
        {
            //  For each genre in the list, display it in the ListBox
            for (int i = 0; i < genreList.Count; i++)
            {
                //  Add genre names
                genreListBox.Items.Add(genreList[i].Name);
            }
        }

        private void ReadMembersDB()
        {
            //  Clear members list
            memberList.Clear();

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
                memberList.Add(currentMember);

            }
            //  close DB connection
            dbConnection.Close();

            //  Display members method
            DisplayMembers();
        }


        private void DisplayMembers()
        {
            //  For each member in the list, display it in the ListBox
            for (int i = 0; i < memberList.Count; i++)
            {
                //  Add member names
                membersListBox.Items.Add(memberList[i].Name);
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            //  Closes the form
            this.Close();
        }

        private int MovieData(string movieTitle)
        {
            //  Declare counter for loop
            int counter = 0;

            //  Loop to read each movie found in the list
            foreach (Movie movies in movieList)
            {
                if (movieTitle != movieList[counter].Title)
                {
                    counter++;
                }
            }
            return counter;
        }

        private void RefreshAddMovieGenres()
        {
            //  Loop to repopulate addMovieGenreComboBox after a clear data method is used
            for (int i = 0; i < genreList.Count; i++)
            {
                addMovieGenreComboBox.Items.Add(genreList[i].Name);
            }
        }

        private void MoviesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //  If an item is selected
                if (moviesListView.SelectedItems.Count > 0)
                {

                    //  Set int variable to selected ListView item in array (#0)
                    int intselectedindex = moviesListView.SelectedIndices[0];

                    //  String selected ListView item (movie title) as text
                    string text = moviesListView.Items[intselectedindex].Text;

                    //  Associate genre field from selected movie
                    foreach (Genre genre in movieList[MovieData(text)].Genres)
                    {
                        genreComboBox.Text = genre.Name.ToString();
                    }

                    //  Fields to display information by the movie list item via loop method -> title
                    idTextBox.Text = movieList[MovieData(text)].ID.ToString();
                    titleTextBox.Text = movieList[MovieData(text)].Title;
                    yearTextBox.Text = movieList[MovieData(text)].Year.ToString();
                    lengthTextBox.Text = movieList[MovieData(text)].Length.ToString();
                    ratingTextBox.Text = movieList[MovieData(text)].Rating.ToString("N2");
                    imagePathTextBox.Text = movieList[MovieData(text)].ImagePath;

                    //  Find image index for movieList to affiliate the correct image with the selected movie
                    int imageIndex = movieList.FindIndex(a => a.Title == text);
                    moviePictureBox.Image = movieImageList.Images[imageIndex];
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
                    checkMember = (Member)memberList[selected];

                    //  TextBoxes to display information requested
                    memberIDTextBox.Text = checkMember.ID.ToString();
                    memberNameTextBox.Text = checkMember.Name;

                    //  DateTime conversion to remove the Hours//Minutes//Seconds displayed
                    memberDOBTextBox.Text = Convert.ToDateTime(checkMember.DOB).ToString("yyyy-MM-dd");
                    memberImagePathTextBox.Text = checkMember.ImagePath;
                    memberTypeComboBox.Text = checkMember.Type.ToString();

                    //  Clear ListView
                    moviesListView.Clear();

                    //  Method to format the ListView
                    FormatListView();

                    //  When a member is selected, it shows the movies they are in with the movie ListView
                    foreach (Movie currentMovie in movieList)
                    {
                        foreach (Member currentMember in currentMovie.Members)
                        {
                            if (membersListBox.SelectedItem.ToString() == currentMember.Name)
                            {
                                //  Create LVI and populate ListView under the correct genre chosen from genreListBox
                                ListViewItem lvi = new ListViewItem();
                                lvi.Text = currentMovie.Title;
                                lvi.SubItems.Add(currentMovie.Year.ToString());
                                lvi.SubItems.Add(currentMovie.ID.ToString());

                                //  Add object to ListView
                                moviesListView.Items.Add(lvi);
                            }
                        }
                    }
                    
                    //  Check assigned integer for each member type and display correct string
                    if (checkMember.Type == 1)
                    {
                        memberTypeComboBox.Text = "Actor/Actresse";
                    }
                    else if (checkMember.Type == 2)
                    {
                        memberTypeComboBox.Text = "Director";
                    }
                    else if (checkMember.Type == 3)
                    {
                        memberTypeComboBox.Text = "Producer";
                    }
                    else if (checkMember.Type == 4)
                    {
                        memberTypeComboBox.Text = "Director of photography";
                    }
                    
                }

                //  Declare int variable for the selected indice (#0) in the ListBox
                int selectedindex = membersListBox.SelectedIndices[0];

                //  If there is a selected item (greater than 0 which is 1)
                if (selectedindex >= 0)
                {
                    //  String selected ListBox item (member name) as text
                    String text = membersListBox.Items[selectedindex].ToString();

                    //  Find image index for member to affiliate the correct image with the selected member
                    int index = memberList.FindIndex(a => a.Name == text);

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
            catch
            {
                //  Display an error message.
                //  MessageBox.Show("Error Detected.");
            }
        }

        private int InsertDBMovie(Movie newMovie)
        {
            try
            {
                //  The following objects will be used to create a movie item in the movie table
                MySqlConnection dbConnection4 = CreateDBConnection(dbHost, dbUsername, dbPassword, dbName);
                MySqlCommand dbCommand4;

                //  Declare int variable for rows affected upon changes
                int queryResult;

                //  Open database connection
                dbConnection4.Open();

                //  SQL query to execute in the db
                string sqlQuery = "INSERT INTO movie VALUES('" + newMovie.ID + "', '" + newMovie.Title + "', '"
                    + newMovie.Year + "', '" + newMovie.Length + "', '" + newMovie.Rating + "', '" + newMovie.ImagePath + "');";

                //  SQL containing the query to be executed
                dbCommand4 = new MySqlCommand(sqlQuery, dbConnection4);

                //  Result of rows affected
                queryResult = dbCommand4.ExecuteNonQuery();

                //  Close DB connection
                dbConnection4.Close();

                return queryResult;

            }
            catch
            {
                //  Error Message
                MessageBox.Show("Error upon movie insertion detected.");

                //  Open and close connection upon an error
                MySqlConnection dbConnection4 = CreateDBConnection(dbHost, dbUsername, dbPassword, dbName);

                //  Close DB connection
                dbConnection4.Close();

                return 0;
            }
        }

        private void AddMovieButton_Click(object sender, EventArgs e)
        {
            //  Replace inputted backslashes inserted by OpenFileDialog to forward slashes
            //  Due to MySQL deleting backslashes in its syntax when read
            //  Source: https://stackoverflow.com/questions/41935210/replace-all-blackslashes-with-forward-slash/41935242
            addMovieImagePathTextBox.Text = addMovieImagePathTextBox.Text.Replace("\\", "/");

            //  Declare movie variable
            Movie newMovie = new Movie();

            //  New movie data values pointed to the add movie fields
            newMovie.ID = int.Parse(addMovieIDTextBox.Text);
            newMovie.Title = addMovieTitleTextBox.Text;
            newMovie.Year = int.Parse(addMovieYearTextBox.Text);
            newMovie.Length = int.Parse(addMovieLengthTextBox.Text);
            newMovie.Rating = double.Parse(addMovieRatingTextBox.Text);
            newMovie.ImagePath = addMovieImagePathTextBox.Text;

            //  Empty Movies list
            movieList = new List<Movie>();

            //  Call method to insert add movie fields from form to a movie object in the list
            InsertDBMovie(newMovie);

            //  Clear imageList for adding movie item
            movieImageList.Images.Clear();

            //  Read movies from the database
            ReadMoviesDB();

            //  Read movie list and display updated data
            UpdateListView();

            //  Method to clear add movie TextBox/ComboBox data
            ClearAddMovieInputs();

            //  Method to repopulate addMovieGenreComboBox after a clear data method is used
            RefreshAddMovieGenres();
        }
        private int InsertDBMember(Member newMember)
        {
            try
            {
                //  The following objects will be used to create a member item in the member table
                MySqlConnection dbConnection5 = CreateDBConnection(dbHost, dbUsername, dbPassword, dbName);
                MySqlCommand dbCommand5;

                //  Declare int variable for rows affected upon changes
                int queryResult;

                //  Open database connection
                dbConnection5.Open();

                //  SQL query to execute in the db
                string sqlQuery = "INSERT INTO member VALUES('" + newMember.ID + "', '" + newMember.Name + "', '" + Convert.ToDateTime(newMember.DOB).ToString("yyyy-MM-dd") +
                "', '" + newMember.Type + "', '" + newMember.ImagePath + "');";

                //  SQL containing the query to be executed
                dbCommand5 = new MySqlCommand(sqlQuery, dbConnection5);

                //  Result of rows affected
                queryResult = dbCommand5.ExecuteNonQuery();

                //  Close DB connection
                dbConnection5.Close();

                return queryResult;
            }
            catch
            {
                //  Error Message
                MessageBox.Show("Error upon member insertion detected.");

                //  Open and close connection upon an error
                MySqlConnection dbConnection5 = CreateDBConnection(dbHost, dbUsername, dbPassword, dbName);

                //  Close DB connection
                dbConnection5.Close();

                return 0;
            }    
        }


        private void GenreListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //  Clear ListView
            moviesListView.Clear();

            //  Method to format the ListView
            FormatListView();

            //  Counting all genres that exist in the list
            foreach (Genre currentGenre in genreList)
            {
                //  Show information corresponding to the selected ListBox item
                if (genreListBox.SelectedItem.ToString() == currentGenre.Name)
                {
                    genreCodeTextBox.Text = currentGenre.Code;
                    genreNameTextBox.Text = currentGenre.Name;

                    //  (Displaying error for description, reason unknown at the moment)
                    genreDescriptionTextBox.Text = currentGenre.Description;
                }
            }

            //  Each movie which has a genre
            foreach (Movie currentMovie in movieList)
            {
                foreach (Genre currentGenre in currentMovie.Genres)
                {
                    //  If the ListBox selected item is equivalent to a genre name selected
                    if (genreListBox.SelectedItem.ToString() == currentGenre.Name)
                    {
                        //  Create LVI and populate ListView under the correct genre chosen from genreListBox
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = currentMovie.Title;
                        lvi.SubItems.Add(currentMovie.Year.ToString());
                        lvi.SubItems.Add(currentMovie.ID.ToString());

                        //  Add object to ListView
                        moviesListView.Items.Add(lvi);
                    }
                }
            }
        }
        private void UpdateListView()
        {
            //  Clear ListView
            moviesListView.Items.Clear();

            //  For each item name add it to the ListView
            for (int i = 0; i < movieList.Count; i++)
            {
                //  Create ListViewItem to hold the title and year for each movie
                ListViewItem lvi = new ListViewItem();
                lvi.Text = movieList[i].Title;
                lvi.SubItems.Add(movieList[i].Year.ToString());
                lvi.SubItems.Add(movieList[i].ID.ToString());

                //  Populate ListView with created LVI item
                moviesListView.Items.Add(lvi);

                //  If there is a ListView item selected
                if (moviesListView.SelectedItems.Count > 0)
                {
                    //  Set int variable to selected ListView item in array (#0)
                    int intselectedindex = moviesListView.SelectedIndices[0];

                    //  String selected ListView item (movie title) as text
                    String text = moviesListView.Items[intselectedindex].Text;

                    //  Find image index for Movies list to affiliate the correct image with the selected movie
                    int imageIndex = movieList.FindIndex(a => a.Title == text);
                    moviePictureBox.Image = movieImageList.Images[imageIndex];
                }
            }
        }

        private void UpdateMemberListBox()
        {
            //  Clear ListBox
            membersListBox.Items.Clear();
            //  For each item name add it to the ListView
            for (int i = 0; i < memberList.Count; i++)
            {
                membersListBox.Items.Add(memberList[i].Name);
            }
        }
        private void ClearMovieInputs()
        {
            // Clear TextBoxes
            idTextBox.Text = "";
            titleTextBox.Text = "";
            genreComboBox.Text = "";
            yearTextBox.Text = "";
            lengthTextBox.Text = "";
            ratingTextBox.Text = "";
            imagePathTextBox.Text = "";

            //  Clear ComboBox
            genreComboBox.Items.Clear();

            //  Clear pictureBox
            moviePictureBox.Image = null;
        }

        private void ClearAddMovieInputs()
        {
            //  Clear TextBoxes
            addMovieIDTextBox.Text = "";
            addMovieTitleTextBox.Text = "";
            addMovieGenreComboBox.Text = "";
            addMovieYearTextBox.Text = "";
            addMovieLengthTextBox.Text = "";
            addMovieRatingTextBox.Text = "";
            addMovieImagePathTextBox.Text = "";

            //  Clear ComboBox
            addMovieGenreComboBox.Items.Clear();
        }
        private void ClearAddMemberInputs()
        {
            //  Clear TextBoxes
            addMemberIDTextBox.Text = "";
            addMemberNameTextBox.Text = "";
            addMemberDOBTextBox.Text = "";
            addMemberTypeComboBox.Text = "";
            addMemberImagePathTextBox.Text = "";

            //  Clear ComboBox
            addMemberTypeComboBox.Items.Clear();
        }

        private void ClearAddGenreInputs()
        {
            //  Clear TextBoxes
            addGenreCodeTextBox.Text = "";
            addGenreNameTextBox.Text = "";
            addGenreDescriptionTextBox.Text = "";
        }

        private void ClearMemberInputs()
        {
            //  Clear TextBoxes
            memberIDTextBox.Text = "";
            memberNameTextBox.Text = "";
            memberDOBTextBox.Text = "";
            memberTypeComboBox.Text = "";
            memberImagePathTextBox.Text = "";

            //  Clear ComboBox
            memberTypeComboBox.Items.Clear();

            //  Clear pictureBox
            memberPictureBox.Image = null;
        }

        private void ClearGenreInputs()
        {
            //  Clear TextBoxes
            genreCodeTextBox.Text = "";
            genreNameTextBox.Text = "";
            genreDescriptionTextBox.Text = "";

            //  Clear ComboBoxes
            genreComboBox.Items.Clear();
            addMovieGenreComboBox.Items.Clear();
        }

        private void ReadGenresComboBox()
        {
            //  Loop to repopulate genreComboBox after a clear data method is used
            for (int i = 0; i < genreList.Count; i++)
            {
                genreComboBox.Items.Add(genreList[i].Name);
            }
        }

        private void ReadMemberTypesComboBox()
        {
            //  Repopulate ComboBox
            memberTypeComboBox.Items.Add("Actor/Actresse");
            memberTypeComboBox.Items.Add("Director");
            memberTypeComboBox.Items.Add("Producer");
            memberTypeComboBox.Items.Add("Director of photography");
        }

        private void ResetFilterButton_Click_1(object sender, EventArgs e)
        {
            //  Remove movie data method
            ClearMovieInputs();

            //  Remove genre data method
            ClearGenreInputs();

            //  Method to refresh the ListView data
            UpdateListView();

            //  Read genres method
            ReadGenresComboBox();

            //  Read genres method (for second ComboBox)
            RefreshAddMovieGenres();
        }

        private void AddMovieImagePathButton_Click(object sender, EventArgs e)
        {
            //  Use FileDialog to search for an image to select
            OpenFileDialog addMovieImage = new OpenFileDialog();

            //  Set filter to only show images to select from
            addMovieImage.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;...";

            if (addMovieImage.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //  String variable for the file path and name taken from OpenFileDialog
                string selectedImagePath = addMovieImage.FileName;

                //  Set image path TextBox by the selected file
                addMovieImagePathTextBox.Text = selectedImagePath;
            }
        }

        private void AddMemberButton_Click(object sender, EventArgs e)
        {
            //  Replace inputted backslashes inserted by OpenFileDialog to forward slashes
            //  Due to MySQL deleting backslashes in its syntax when read
            addMemberImagePathTextBox.Text = addMemberImagePathTextBox.Text.Replace("\\", "/");

            //  Declare member variable
            Member newMember = new Member();

            //  New member data values pointed to the add member fields
            newMember.ID = int.Parse(addMemberIDTextBox.Text);
            newMember.Name = addMemberNameTextBox.Text;
            newMember.DOB = DateTime.Parse(addMemberDOBTextBox.Text);
            //  newMember.Type = int.Parse(addMemberTypeComboBox.Text);
            newMember.ImagePath = addMemberImagePathTextBox.Text;

            //  Check inputted string and associate the correct ID to it from the table
            if (addMemberTypeComboBox.Text == "Actor/Actresse")
            {
                newMember.Type = int.Parse("1");
            }
            if (addMemberTypeComboBox.Text == "Director")
            {
                newMember.Type = int.Parse("2");
            }
            if (addMemberTypeComboBox.Text == "Producer")
            {
                newMember.Type = int.Parse("3");
            }
            if (addMemberTypeComboBox.Text == "Director of photography")
            {
                newMember.Type = int.Parse("4");
            }


            //  Empty Members list
            memberList = new List<Member>();

            //  Call method to insert add member fields from form to a member object in the list
            InsertDBMember(newMember);

            //  Clear imageList for adding member item
            membersImageList.Images.Clear();

            //  Read members from the database
            ReadMembersDB();

            //  Read member list and display updated data
            UpdateMemberListBox();

            //  Method to clear add member TextBox/ComboBox data
            ClearAddMemberInputs();

            //  Repopulate ComboBox
            addMemberTypeComboBox.Items.Add("Actor/Actresse");
            addMemberTypeComboBox.Items.Add("Director");
            addMemberTypeComboBox.Items.Add("Producer");
            addMemberTypeComboBox.Items.Add("Director of Photography");
        }

        private int ModifyDBMember(Member modifyMember)
        {
            try
            {
                //  The following objects will be used to modify a selected member item in the member table
                MySqlConnection dbConnection7 = CreateDBConnection(dbHost, dbUsername, dbPassword, dbName);

                //  Declare int variable for rows affected upon changes
                int queryResult;

                //  Open database connection
                dbConnection7.Open();

                //  SQL query to execute in the db         
                string sqlQuery = "UPDATE member SET name = '" + modifyMember.Name + "', date_of_birth = '" + Convert.ToDateTime(modifyMember.DOB).ToString("yyyy-MM-dd") +
                    "', member_type_id = '" + modifyMember.Type + "', image_file_path = '" + modifyMember.ImagePath + "' WHERE id = " + modifyMember.ID + ";";

                //  SQL containing the query to be executed
                MySqlCommand dbCommand7 = new MySqlCommand(sqlQuery, dbConnection7);

                //  Result of rows affected
                queryResult = dbCommand7.ExecuteNonQuery();

                //  Close DB connection
                dbConnection7.Close();

                return queryResult;
            }
            catch
            {
                //  Error Message
                MessageBox.Show("Error upon member modification detected.");

                //  Open and close connection upon an error
                MySqlConnection dbConnection7 = CreateDBConnection(dbHost, dbUsername, dbPassword, dbName);

                //  Close DB connection
                dbConnection7.Close();

                return 0;
            }
        }


        private void AddMemberImagePathButton_Click(object sender, EventArgs e)
        {
            //  Use FileDialog to search for an image to select
            OpenFileDialog addMemberImage = new OpenFileDialog();

            //  Set filter to only show images to select from
            addMemberImage.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;...";

            if (addMemberImage.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //  String variable for the file path and name taken from OpenFileDialog
                string selectedImagePath = addMemberImage.FileName;

                //  Set image path TextBox by the selected file
                addMemberImagePathTextBox.Text = selectedImagePath;
            }
        }
        private int DeleteDBMember(Member deleteMember)
        {
            try
            {
                //  The following objects will be used to create a member item in the member table
                MySqlConnection dbConnection6 = CreateDBConnection(dbHost, dbUsername, dbPassword, dbName);
                MySqlCommand dbCommand6;

                //  Declare int variable for rows affected upon changes
                int queryResult;

                //  Open database connection
                dbConnection6.Open();

                //  SQL query to execute in the db
                string sqlQuery = "DELETE FROM member WHERE id = '" + deleteMember.ID + "';";

                //  SQL containing the query to be executed
                dbCommand6 = new MySqlCommand(sqlQuery, dbConnection6);

                //  Result of rows affected
                queryResult = dbCommand6.ExecuteNonQuery();

                //  Close DB connection
                dbConnection6.Close();

                return queryResult;
            }
            catch
            {
                //  Error Message
                MessageBox.Show("Error upon member deletion detected.");

                //  Open and close connection upon an error
                MySqlConnection dbConnection6 = CreateDBConnection(dbHost, dbUsername, dbPassword, dbName);

                //  Close DB connection
                dbConnection6.Close();

                return 0;
            }

        }
        private void DeleteMemberButton_Click(object sender, EventArgs e)
        {
            //  Check if a member was selected
            if (membersListBox.SelectedIndex == -1)
            {
                //  Error message
                MessageBox.Show("You must select a member to delete.");
            }
            else
            {
                //  Declare member variable
                Member deleteMember = new Member();

                //  Selected member data values pointed to the read only member ID field
                deleteMember.ID = int.Parse(memberIDTextBox.Text);

                //  Empty Members list
                memberList = new List<Member>();

                //  Call method to delete member from the list according to the member fields read
                DeleteDBMember(deleteMember);

                //  Clear imageList for adding member item
                membersImageList.Images.Clear();

                //  Read members from the database
                ReadMembersDB();

                //  Read member list and display updated data
                UpdateMemberListBox();

                //  Method to clear member TextBox/ComboBox/pictureBox data
                ClearMemberInputs();

                //  Read member types method
                ReadMemberTypesComboBox();
            }
        }

        private void ModifyMemberButton_Click(object sender, EventArgs e)
        {
            //  Check if a ListBox selection for members exists
            if (membersListBox.SelectedIndex < 0)
            {
                //  Show error message
                MessageBox.Show("Select a member from the ListBox.");
            }
            //  Otherwise continue actions
            else
            {
                //  Enable TextBoxes and Button to allow access for changes made by the user
                memberNameTextBox.Enabled = true;
                memberDOBTextBox.Enabled = true;
                memberTypeComboBox.Enabled = true;
                memberImagePathButton.Enabled = true;
                saveMemberButton.Enabled = true;
            }
        }

        private void MemberImagePathButton_Click(object sender, EventArgs e)
        {
            //  Use FileDialog to search for an image to select
            OpenFileDialog modifyMemberImage = new OpenFileDialog();

            //  Set filter to only show images to select from
            modifyMemberImage.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;...";

            if (modifyMemberImage.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //  String variable for the file path and name taken from OpenFileDialog
                string selectedImagePath = modifyMemberImage.FileName;

                //  Set image path TextBox by the selected file
                memberImagePathTextBox.Text = selectedImagePath;
            }
        }

        private void SaveMemberButton_Click(object sender, EventArgs e)
        {
            //  Check if a ListBox selection for members exists
            if (membersListBox.SelectedIndex < 0)
            {
                //  Show error message
                MessageBox.Show("Select a member from the ListBox first if you wish to make any changes.");
            }
            else
            {
                //  Replace inputted backslashes inserted by OpenFileDialog to forward slashes
                //  Due to MySQL deleting backslashes in its syntax when read
                memberImagePathTextBox.Text = memberImagePathTextBox.Text.Replace("\\", "/");

                //  Declare member variable
                Member modifyMember = new Member();

                //  Modified member data values pointed to the modify member fields
                //  ID is not able to be changed due to it being the primary key
                modifyMember.ID = int.Parse(memberIDTextBox.Text);
                modifyMember.Name = memberNameTextBox.Text;
                modifyMember.DOB = DateTime.Parse(memberDOBTextBox.Text);
                //  modifyMember.Type = int.Parse(memberTypeComboBox.Text);
                modifyMember.ImagePath = memberImagePathTextBox.Text;

                //  Check inputted string and associate the correct ID to it from the table
                if (memberTypeComboBox.Text == "Actor/Actresse")
                {
                    modifyMember.Type = int.Parse("1");
                }
                if (memberTypeComboBox.Text == "Director")
                {
                    modifyMember.Type = int.Parse("2");
                }
                if (memberTypeComboBox.Text == "Producer")
                {
                    modifyMember.Type = int.Parse("3");
                }
                if (memberTypeComboBox.Text == "Director of photography")
                {
                    modifyMember.Type = int.Parse("4");
                }

                //  Empty Members list
                memberList = new List<Member>();

                //  Call method to modify and update member from the list
                ModifyDBMember(modifyMember);

                //  Clear imageList for adding member item
                membersImageList.Images.Clear();

                //  Read members from the database
                ReadMembersDB();

                //  Read member list and display updated data
                UpdateMemberListBox();

                //  Method to clear member TextBox/ComboBox/pictureBox data
                ClearMemberInputs();

                //  Method to refresh the ListView data
                UpdateListView();

                //  Read member types method
                ReadMemberTypesComboBox();

                //  Disable TextBoxes and Buttons to deny access for anymore changes made by the user
                memberNameTextBox.Enabled = false;
                memberDOBTextBox.Enabled = false;
                memberTypeComboBox.Enabled = false;
                memberImagePathButton.Enabled = false;
                saveMemberButton.Enabled = false;
            }
        }

        private void ResetMemberMovieButton_Click(object sender, EventArgs e)
        {
            //  Remove movie data method
            ClearMovieInputs();

            //  Remove member data method
            ClearMemberInputs();

            //  Method to refresh the ListView data
            UpdateListView();

            //  Read genres method
            ReadGenresComboBox();

            //  Read member types method
            ReadMemberTypesComboBox();

            //  Remove selected item to prevent program crashing when selecting member, resetting filter then trying to delete member
            membersListBox.ClearSelected();
        }

        private int DeleteDBMovie(Movie deleteMovie)
        {
            try
            {
                //  The following objects will be used to delete a movie item in the movie table and its reference from the join table (for the genre)
                MySqlConnection dbConnection10 = CreateDBConnection(dbHost, dbUsername, dbPassword, dbName);
                MySqlCommand dbCommand10;

                //  Declare int variable for rows affected upon changes
                int queryResult;

                //  Open database connection
                dbConnection10.Open();

                //  SQL query to execute in the db
                string sqlQuery = "DELETE FROM jt_genre_movie WHERE movie_id = '" + deleteMovie.ID + "';" + "DELETE FROM movie WHERE id = '" + deleteMovie.ID + "';";

                //  SQL containing the query to be executed
                dbCommand10 = new MySqlCommand(sqlQuery, dbConnection10);

                //  Result of rows affected
                queryResult = dbCommand10.ExecuteNonQuery();

                // Close DB connection
                dbConnection10.Close();

                return queryResult;

            }
            catch
            {
                MessageBox.Show("Error upon movie deletion detected.");

                //  Open and close connection upon an error
                MySqlConnection dbConnection10 = CreateDBConnection(dbHost, dbUsername, dbPassword, dbName);

                //  Close DB connection
                dbConnection10.Close();

                return 0;
            }
        }
        private void DeleteMovieButton_Click(object sender, EventArgs e)
        {
            //  If a ListView item is selected
            if (moviesListView.SelectedItems.Count > 0)
            {
                //  Declare movie variable
                Movie deleteMovie = new Movie();

                //  Selected movie data values pointed to the read only movie ID field
                deleteMovie.ID = int.Parse(idTextBox.Text);

                //  Empty Movies list
                movieList = new List<Movie>();

                //  Call method to insert add movie fields from form to a movie object in the list
                DeleteDBMovie(deleteMovie);

                //  Clear imageList for adding movie item
                movieImageList.Images.Clear();

                //  Read movies from the database
                ReadMoviesDB();

                //  Read movie list and display updated data
                UpdateListView();

                //  Remove movie data method
                ClearMovieInputs();

                //  Read genres method
                ReadGenresComboBox();
            }
            else
            {
                //  Show prompt message
                MessageBox.Show("Select a movie first.");
            }
        }

        private void ModifyMovieButton_Click(object sender, EventArgs e)
        {
            //  If a ListView item is selected
            if (moviesListView.SelectedItems.Count > 0)
            {
                //  Enable TextBoxes and Button to allow access for changes made by the user
                titleTextBox.Enabled = true;
                yearTextBox.Enabled = true;
                genreComboBox.Enabled = true;
                movieImagePathButton.Enabled = true;
                lengthTextBox.Enabled = true;
                ratingTextBox.Enabled = true;
                saveMovieButton.Enabled = true;
            }
            else
            {
                //  Show prompt message
                MessageBox.Show("Select a movie first.");
            }
        }
        private int ModifyDBMovie(Movie modifyMovie)
        {
            try
            {
                //  The following objects will be used to modify a movie item in the movie table
                MySqlConnection dbConnection11 = CreateDBConnection(dbHost, dbUsername, dbPassword, dbName);

                //  Declare int variable for rows affected upon changes
                int queryResult;

                //  Open database connection
                dbConnection11.Open();

                //  SQL query to execute in the db           
                string sqlQuery = "UPDATE movie SET image_file_path = '" + modifyMovie.ImagePath + "', title = '" + modifyMovie.Title + "', year = '"
                        + modifyMovie.Year + "', length = '" + modifyMovie.Length + "', audience_rating = '" + modifyMovie.Rating + "' WHERE id = " + modifyMovie.ID + ";";

                //  SQL containing the query to be executed
                MySqlCommand dbCommand11 = new MySqlCommand(sqlQuery, dbConnection11);

                //  Result of rows affected
                queryResult = dbCommand11.ExecuteNonQuery();

                //  Close DB connection
                dbConnection11.Close();

                return queryResult;
            }
            catch
            {
                MessageBox.Show("Error upon movie modification detected.");

                //  Open and close connection upon an error
                MySqlConnection dbConnection11 = CreateDBConnection(dbHost, dbUsername, dbPassword, dbName);

                dbConnection11.Close();

                return 0;
            }
        }
        private void SaveMovieButton_Click(object sender, EventArgs e)
        {
            //  Declare movie variable
            Movie modifyMovie = new Movie();

            //  Modified movie data values pointed to the modify movie fields
            //  ID is not able to be changed due to it being the primary key
            modifyMovie.ID = int.Parse(idTextBox.Text);
            modifyMovie.Title = titleTextBox.Text;
            modifyMovie.Year = int.Parse(yearTextBox.Text);
            modifyMovie.Length = int.Parse(lengthTextBox.Text);
            modifyMovie.Rating = double.Parse(ratingTextBox.Text);
            modifyMovie.ImagePath = imagePathTextBox.Text;

            //  Empty Movies list
            movieList = new List<Movie>();

            //  Call method to insert add movie fields from form to a movie object in the list
            ModifyDBMovie(modifyMovie);

            //  Clear imageList for adding movie item
            movieImageList.Images.Clear();

            //  Read movies from the database
            ReadMoviesDB();

            //  Read movie list and display updated data
            UpdateListView();

            //  Remove movie data method
            ClearMovieInputs();

            //  Read genres method
            ReadGenresComboBox();

            //  Disable TextBoxes and Buttons to deny access for anymore changes made by the user
            titleTextBox.Enabled = false;
            yearTextBox.Enabled = false;
            genreComboBox.Enabled = false;
            movieImagePathButton.Enabled = false;
            lengthTextBox.Enabled = false;
            ratingTextBox.Enabled = false;
            saveMovieButton.Enabled = false;
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            //  Search function for movie titles
            //  Source: https://stackoverflow.com/questions/20341113/search-listview-items-using-textbox
            if (movieSearchTextBox.Text != "")
            {
                for (int i = moviesListView.Items.Count - 1; i >= 0; i--)
                {
                    var item = moviesListView.Items[i];
                    if (item.Text.ToLower().Contains(movieSearchTextBox.Text.ToLower()))
                    {
                        //  item.BackColor = SystemColors.Highlight;
                        //  item.ForeColor = SystemColors.HighlightText;
                    }
                    else
                    {
                        moviesListView.Items.Remove(item);
                    }
                }
                if (moviesListView.SelectedItems.Count == 1)
                {
                    moviesListView.Focus();
                }

            }
            else
            {
                //  If the TextBox is empty revert ListView to default
                UpdateListView();

                //  Remove movie data method
                ClearMovieInputs();
            }
        }

        private void ResetMovieSearchButton_Click(object sender, EventArgs e)
        {
            //  Empty search TextBox
            movieSearchTextBox.Text = "";

            //  If the TextBox is empty revert ListView to default
            UpdateListView();

            //  Remove movie data method
            ClearMovieInputs();

            //  Read genres method
            ReadGenresComboBox();

            //  Read member types method
            ReadMemberTypesComboBox();
        }
        private int InsertDBGenre(Genre newGenre)
        {
            try
            {
                //  The following objects will be used to add a genre item in the genre table
                MySqlConnection dbConnection12 = CreateDBConnection(dbHost, dbUsername, dbPassword, dbName);
                MySqlCommand dbCommand12;

                //  Declare int variable for rows affected upon changes
                int queryResult;

                //  Open database connection
                dbConnection12.Open();

                //  SQL query to execute in the db 
                string sqlQuery = "INSERT INTO genre VALUES('" + newGenre.Code.ToUpper() + "', '" + newGenre.Name + "', '" + newGenre.Description + "');";

                //  SQL containing the query to be executed
                dbCommand12 = new MySqlCommand(sqlQuery, dbConnection12);

                //  Result of rows affected
                queryResult = dbCommand12.ExecuteNonQuery();

                //  Close DB connection
                dbConnection12.Close();

                return queryResult;
            }
            catch
            {
                MessageBox.Show("Error upon adding new genre detected.");

                //  Open and close connection upon an error
                MySqlConnection dbConnection12 = CreateDBConnection(dbHost, dbUsername, dbPassword, dbName);

                //  Close DB connection
                dbConnection12.Close();

                return 0;
            }
        }

        private void AddGenreButton_Click(object sender, EventArgs e)
        {
            //  Declare genre variable
            Genre newGenre = new Genre();

            //  New genre data values pointed to the add genre fields
            newGenre.Code = addGenreCodeTextBox.Text;
            newGenre.Name = addGenreNameTextBox.Text;
            newGenre.Description = addGenreDescriptionTextBox.Text;

            //  Empty Genres list
            genreList = new List<Genre>();

            //  Call method to insert add genre fields from form to a genre object in the list
            InsertDBGenre(newGenre);

            //  Clear ListBox
            genreListBox.Items.Clear();

            //  Read genres from the database
            ReadGenresDB();

            //  Clear inputs method
            ClearAddGenreInputs();

            //  Clear ComboBox
            addMovieGenreComboBox.Items.Clear();

            //  Method to repopulate addMovieGenreComboBox after a clear data method is used
            RefreshAddMovieGenres();
        }

        private void ModifyGenre_Click(object sender, EventArgs e)
        {
            //  Check if a ListBox selection for members exists
            if (genreListBox.SelectedIndex < 0)
            {
                //  Show error message
                MessageBox.Show("Select a genre from the ListBox.");
            }
            //  Otherwise continue actions
            else
            {
                //  Enable TextBoxes and Button to allow access for changes made by the user
                genreNameTextBox.Enabled = true;
                genreDescriptionTextBox.Enabled = true;
                saveGenreButton.Enabled = true;
            }
        }
        private int ModifyDBGenre(Genre genre)
        {
            //  The following objects will be used to modify a genre item in the genre table
            MySqlConnection dbConnection13 = CreateDBConnection(dbHost, dbUsername, dbPassword, dbName);

            //  Declare int variable for rows affected upon changes
            int queryResult;

            //  Open database connection
            dbConnection13.Open();

            //  SQL query to execute in the db    
            string sqlQuery = "UPDATE genre SET name = '" + genre.Name + "', description = '" + genre.Description + "' WHERE code = '" + genre.Code.ToUpper() + "';";

            //  SQL containing the query to be executed
            MySqlCommand dbCommand13 = new MySqlCommand(sqlQuery, dbConnection13);

            //  Result of rows affected
            queryResult = dbCommand13.ExecuteNonQuery();

            //  Close DB connection
            dbConnection13.Close();

            return queryResult;
        }

        private void SaveGenreButton_Click(object sender, EventArgs e)
        {
            //  Check if a ListBox selection for genres exists
            if (genreListBox.SelectedIndex < 0)
            {
                //  Show error message
                MessageBox.Show("Select a genre from the ListBox first if you wish to make any changes.");
            }
            else
            {

                //  Declare genre variable
                Genre modifyGenre = new Genre();

                //  Modified genre data values pointed to the modify genre fields
                //  Code is not able to be changed due to it being the primary key
                modifyGenre.Code = genreCodeTextBox.Text;
                modifyGenre.Name = genreNameTextBox.Text;
                modifyGenre.Description = genreDescriptionTextBox.Text;

                //  Empty Genres list
                genreList = new List<Genre>();

                //  Call method to insert add genre fields from form to a genre object in the list
                ModifyDBGenre(modifyGenre);

                //  Clear ListBox
                genreListBox.Items.Clear();

                //  Read genres from the database
                ReadGenresDB();

                //  Clear inputs method
                ClearGenreInputs();

                //  Disable TextBoxes and Buttons to deny access for anymore changes made by the user
                genreNameTextBox.Enabled = false;
                genreDescriptionTextBox.Enabled = false;
                saveGenreButton.Enabled = false;

                //// -- Call these 4 instructions below to refresh the genre data -- ////
                //// ---  in each ComboBox to prevent creating duplicate entries --- ////

                //  Clear ComboBox
                genreComboBox.Items.Clear();

                //  Method to repopulate genreComboBox after a clear data method is used
                ReadGenresComboBox();

                //  Clear ComboBox
                addMovieGenreComboBox.Items.Clear();

                //  Method to repopulate addMovieGenreComboBox after a clear data method is used
                RefreshAddMovieGenres();


                //// -- Call these 3 instructions below to refresh the genre data -- ////
                //// ---  associated with each movie after a genre modification  --- ////

                //  Empty Movies list
                movieList = new List<Movie>();

                //  Read movies from the database
                ReadMoviesDB();

                //  Read movie list and display updated data
                UpdateListView();
            }
        }
    }
}
