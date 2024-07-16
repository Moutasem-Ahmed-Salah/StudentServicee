using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace StudentServicee
{
    /// <summary>
    /// Summary description for StudentService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class StudentService : System.Web.Services.WebService
    {

        [WebMethod]
        public Boolean StudentSign(string username, string password)
        {
            try
            {

                string connectionString = "Data Source=MO3TA-LAPTOP;Initial Catalog=E-learning;Integrated Security=True";

                string sql = "SELECT COUNT(*) FROM Students WHERE Username = @Username AND Password = @Password";

                int count = 0;


                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {

                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);


                    conn.Open();
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }


                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur and log them, or return false
                throw new Exception("An error occurred while signing in: " + ex.Message);
            }
        }


        [WebMethod]
        public void StudentRegister(string name, string address, string area, string mobileNumber, string username, string password,string mail)
        {
            try
            {
                // Connection string to your SQL Server
                string connectionString = "Data Source=MO3TA-LAPTOP;Initial Catalog=E-learning;Integrated Security=True";

                // SQL query to insert a new student record
                string sql = "INSERT INTO Students (Name, Address, Area, MobileNumber, Username, Password,Mail) " +
                             "VALUES (@Name, @Address, @Area, @MobileNumber, @Username, @Password,@mail)";

                // Create a SqlConnection and a SqlCommand object
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    // Set the parameter values
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@Area", area);
                    command.Parameters.AddWithValue("@MobileNumber", mobileNumber);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@mail", mail);

                    // Open the connection and execute the command
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur and log them, or return an error message
                throw new Exception("An error occurred while registering the student: " + ex.Message);
            }
        }

        [WebMethod]
        public void AddToCart(int StudentID, int CourseID, int Quantity)
        {
            string connectionString = "Data Source=MO3TA-LAPTOP;Initial Catalog=E-learning;Integrated Security=True";

            string insertQuery = "INSERT INTO ShoppingCart (StudentID, CourseID, Quantity) VALUES (@StudentID, @CourseID, @Quantity)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@StudentID", StudentID);
                command.Parameters.AddWithValue("@CourseID", CourseID);
                command.Parameters.AddWithValue("@Quantity", Quantity);

                try
                {
                    connection.Open();

                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Error: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        [WebMethod]
        public void UpdateCart(int shoppingCartID, int quantity)
        {
            string connectionString = "Data Source=MO3TA-LAPTOP;Initial Catalog=E-learning;Integrated Security=True";
            string query = "UPDATE ShoppingCart SET Quantity = @Quantity WHERE ShoppingCartID = @ShoppingCartID";

           
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Quantity", quantity);
                        command.Parameters.AddWithValue("@ShoppingCartID", shoppingCartID);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            
           
        }

        [WebMethod]
        public void DeleteCart(int shoppingCartID)
        {
            string connectionString = "Data Source=MO3TA-LAPTOP;Initial Catalog=E-learning;Integrated Security=True";
            string query = "DELETE FROM ShoppingCart WHERE ShoppingCartID = @ShoppingCartID";

            
            
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ShoppingCartID", shoppingCartID);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            
           
        }
        [WebMethod]
        public DataTable ViewCart(int studentID)
        {
            string connectionString = "Data Source=MO3TA-LAPTOP;Initial Catalog=E-learning;Integrated Security=True";
            string query = @"SELECT sc.ShoppingCartID, c.CourseID, c.CourseName, c.CourseDescription, c.CourseCategory, c.CoursePrice, sc.Quantity
                     FROM ShoppingCart sc
                     JOIN Courses c ON sc.CourseID = c.CourseID
                     WHERE sc.StudentID = @StudentID";

            DataTable cartTable = new DataTable("ShoppingCart"); 

            
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@StudentID", studentID);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            cartTable.Load(reader);
                        }
                    }
                }
            
           

            return cartTable;
        }

        [WebMethod]
        public string[] CourseDetail(string courseName)
        {
            string connectionString = "Data Source=MO3TA-LAPTOP;Initial Catalog=E-learning;Integrated Security=True";
            string[] courseDetails = new string[4];

            string sqlQuery = "SELECT CourseID, CourseCategory, CourseDescription, CoursePrice FROM Courses WHERE CourseName = @CourseName";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@CourseName", courseName);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                           
                            courseDetails[0] = reader.GetInt32(0).ToString();//CourseID 
                            courseDetails[1] = reader.GetString(1); // CourseCategory
                            courseDetails[2] = reader.GetString(2); // CourseDescription
                            courseDetails[3] = reader.GetInt32(3).ToString(); // CoursePrice
                        }
                    }
                }
            }
            return courseDetails;


        }



        [WebMethod]
        public DataTable ViewCourses()
        {
            DataTable coursesTable = new DataTable("Courses");            
            string connectionString = "Data Source=MO3TA-LAPTOP;Initial Catalog=E-learning;Integrated Security=True";            
            string sqlQuery = "SELECT CourseName, CoursePrice FROM Courses";            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter(sqlQuery, conn))
                {
                    adapter.Fill(coursesTable);
                }
            }        
            return coursesTable;
        }
        [WebMethod]
        public DataTable GetTotal(int studentID)
        {
            DataTable cartDetails = new DataTable("CheckoutDetails");


            string sqlQuery = @"
                SELECT 
                    sc.ShoppingCartID,
                    c.CourseName,
                    c.CoursePrice,
                    sc.Quantity,
                    (c.CoursePrice * sc.Quantity) AS SubTotal
                FROM 
                    ShoppingCart sc
                JOIN 
                    Courses c ON sc.CourseID = c.CourseID
                WHERE 
                    sc.StudentID = @StudentID";


            string connectionString = "Data Source=MO3TA-LAPTOP;Initial Catalog=E-learning;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {

                    command.Parameters.AddWithValue("@StudentID", studentID);


                    SqlDataAdapter adapter = new SqlDataAdapter(command);


                    adapter.Fill(cartDetails);
                }
            }

            return cartDetails;
        }
        [WebMethod]
        public void ConfirmOrder(int studentID, int total)
        {
            string connectionString = "Data Source=MO3TA-LAPTOP;Initial Catalog=E-learning;Integrated Security=True";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM ShoppingCart WHERE StudentID = @StudentID";
                using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", studentID);
                    command.ExecuteNonQuery();
                }
            }

            string mail = "";
            String StudentName = "";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                string selectMailQuery = "SELECT Mail,Name FROM Students WHERE ID = @StudentID";

                using (SqlCommand selectMailCommand = new SqlCommand(selectMailQuery, connection))
                {
                    selectMailCommand.Parameters.AddWithValue("@StudentID", studentID);

                   
                    using (SqlDataReader reader = selectMailCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            mail = reader["Mail"].ToString();
                            StudentName = reader["Name"].ToString();
                        }
                    }
                }
                MailMessage mailMessage = new MailMessage();
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                mailMessage.From = new MailAddress("mo3ta999@gmail.com");
                mailMessage.To.Add(mail);
                mailMessage.Subject = "E-Learning Course Order";
                mailMessage.Body = "Hello " + StudentName + ", Sending you this email to confirm The order You Just Purchased of Total " + total + "$,Happy Learning!";
                smtpClient.Port = 587;
                smtpClient.Credentials = new System.Net.NetworkCredential("mo3ta999@gmail.com", "wdec lsdp rtpp htfv\r\n");
                smtpClient.EnableSsl = true;
                smtpClient.Send(mailMessage);




            }

        }
    }
}
