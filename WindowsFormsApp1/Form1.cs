using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1

{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ComboBoxData();
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
                int selectedID;
                int.TryParse(comboBox1.SelectedValue.ToString(), out selectedID);
                label_data(selectedID);
                data_grid(selectedID);
                richTextData(selectedID);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ComboBoxData();

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBox1.SelectedValue != null)
            {
                int selectedID;

                int.TryParse(comboBox1.SelectedValue.ToString(), out selectedID);

                label_data(selectedID);
                data_grid(selectedID);
                richTextData(selectedID);

            }

            else
            {
                MessageBox.Show("Data tidak ditemukan");
            }
        }

        private void ComboBoxData()
        {
            string connectionString = "Data source=DESKTOP-H0DAI5O\\SQLEXPRESS;Initial catalog=EsemkaVote;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Name FROM VotingHeader";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            comboBox1.Items.Clear();


                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);


                            comboBox1.DataSource = dataTable;
                            comboBox1.DisplayMember = "Name";
                            comboBox1.ValueMember = "Id";

                            if (comboBox1.Items.Count > 0)
                            {
                                comboBox1.SelectedIndex = 0;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label_data(int Id)
        {
            string connectionString = "Data source=DESKTOP-H0DAI5O\\SQLEXPRESS;Initial catalog=EsemkaVote;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT TOP 1
                                    E.Name AS EmployeeName, E.Photo, VH.Name, VH.Description, COUNT(VD.VotedCandidateId) AS TotalVotes
                                    FROM VotingDetail VD
                                    JOIN VotingCandidate VC ON VD.VotedCandidateId = VC.Id
                                    JOIN Employee E ON VC.EmployeeId = E.Id
                                    JOIN VotingHeader VH ON VC.VotingHeaderId = VH.Id
                                    WHERE VC.VotingHeaderId = @Id
                                    GROUP BY E.Name, E.Photo, VH.Name, VH.Description
                                    ORDER BY TotalVotes DESC"; // Perbaikan di sini, menggunakan VH.Id

                string query2 = @"SELECT COUNT(VD.VotedCandidateId) AS FullVotes
                                    FROM VotingDetail VD
                                    JOIN VotingCandidate VC ON VD.VotedCandidateId = VC.Id
                                    JOIN VotingHeader VH ON VC.VotingHeaderId = VH.Id
                                    WHERE VH.Id = @Id";

                int totalVotes = 0;
                int fullVotes = 1;

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                label2.Text = reader["Name"].ToString();
                                label3.Text = reader["Description"].ToString();
                                totalVotes = Convert.ToInt32(reader["TotalVotes"]);
                                label4.Text = reader["EmployeeName"].ToString();
                                label6.Text = totalVotes.ToString();
                            }

                            // Mengambil path gambar dari kolom Photo
                            string assetPath = @"C:\\Users\\ACER\\Downloads\\PEMADATAN LKS\\PEMADATAN LKS\\ITSSB\\04022025\\assets\\";

                            // Mengecek apakah ada gambar yang terkait
                            if (reader["Photo"] != DBNull.Value)
                            {
                                string photoFileName = reader["Photo"].ToString();
                                string photoFilePath = Path.Combine(assetPath, photoFileName);

                                // Mengecek apakah file gambar ada di path yang ditentukan
                                if (File.Exists(photoFilePath))
                                {
                                    pictureBox1.Image = Image.FromFile(photoFilePath); // Menampilkan gambar
                                }
                                else
                                {
                                    // Jika file gambar tidak ada
                                    MessageBox.Show("Gambar tidak ditemukan: " + photoFilePath);

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }

                using (SqlCommand cmd2 = new SqlCommand(query2, conn))
                {
                    cmd2.Parameters.AddWithValue("@Id", Id);
                    var result = cmd2.ExecuteScalar();
                    if (result != null)
                    {
                        fullVotes = Convert.ToInt32(result);
                    }
                }

                decimal percentage = (decimal)totalVotes / fullVotes * 100;
                label7.Text = totalVotes.ToString() + "/" + fullVotes.ToString();
                label6.Text = string.Format("{0} %", percentage.ToString("0.00"));
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void data_grid(int Id)
        {
            string connectionString = "Data source=DESKTOP-H0DAI5O\\SQLEXPRESS;Initial catalog=EsemkaVote;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT D.Name, COUNT(VD.VotedCandidateId) AS VoteDivisi
                FROM VotingDetail VD
                JOIN VotingCandidate VC ON VD.VotedCandidateId = VC.Id
                JOIN VotingHeader VH ON VC.VotingHeaderId = VH.Id
                JOIN Employee E ON VC.EmployeeId = E.Id
                JOIN Division D ON E.DivisionId = D.Id
                WHERE VH.Id = @Id
                GROUP BY D.Name";

                string query2 = @"
                SELECT COUNT(VD.VotedCandidateId) AS TotalVotes
                FROM VotingDetail VD
                JOIN VotingCandidate VC ON VD.VotedCandidateId = VC.Id
                JOIN VotingHeader VH ON VC.VotingHeaderId = VH.Id
                WHERE VH.Id = @Id
                ";

                int totalVotes = 1;

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);


                            // Add a new column for Percentage
                            if (!dataTable.Columns.Contains("Percentage"))
                            {
                                dataTable.Columns.Add("Percentage", typeof(string));
                            }


                            dataGridView1.DataSource = dataTable;

                            // Now, format the Percentage column
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }

                using (SqlCommand cmd2 = new SqlCommand(query2, conn))
                {
                    cmd2.Parameters.AddWithValue("@Id", Id);
                    var result = cmd2.ExecuteScalar();
                    if (result != null)
                    {
                        totalVotes = Convert.ToInt32(result);
                    }
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        // Check if the VoteDivisi column has value
                        if (row.Cells["VoteDivisi"].Value != null)
                        {
                            int votesInDivision = Convert.ToInt32(row.Cells["VoteDivisi"].Value);

                            // Calculate the percentage
                            double percentage = ((double)votesInDivision / totalVotes) * 100;

                            row.Cells["Percentage"].Value = string.Format("{0} %", percentage.ToString("0.00"));
                        }
                    }
                }
            }
        }



        private void datagridview1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextData(int Id)
        {
            string connectionString = "Data source=DESKTOP-H0DAI5O\\SQLEXPRESS;Initial catalog=EsemkaVote;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT VD.Reason AS EmployeeName 
                FROM VotingDetail VD
                JOIN VotingCandidate VC ON VD.VotedCandidateId = VC.Id
                JOIN VotingHeader VH ON VC.VotingHeaderId = VH.Id
                JOIN Employee E ON VC.EmployeeId = E.Id
                JOIN Division D ON E.DivisionId = D.Id
                WHERE VC.VotingHeaderId = @Id
                ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", comboBox1.SelectedValue);
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                richTextBox1.Text = reader["EmployeeName"].ToString();
                                richTextBox2.Text = reader["EmployeeName"].ToString();
                                richTextBox3.Text = reader["EmployeeName"].ToString();
                                richTextBox4.Text = reader["EmployeeName"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

        }
    }
}
