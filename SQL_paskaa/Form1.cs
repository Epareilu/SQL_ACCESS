using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SQLServerDatabaseGUI
{
    public partial class Form1 : Form
    {
        private string ConnectionString = "Data Source=SERVER;Initial Catalog=DATABASE;Integrated Security=True";
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable;
        private SqlConnection connection;

        public Form1()
        {
            InitializeComponent();
            LoadTableNames();
        }

        private void LoadTableNames()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                DataTable schema = connection.GetSchema("Tables");
                tableComboBox.DisplayMember = "TABLE_NAME";
                tableComboBox.DataSource = schema;
            }
        }

        private void LoadData()
        {
            string tableName = tableComboBox.Text;

            connection = new SqlConnection(ConnectionString);
            dataAdapter = new SqlDataAdapter("SELECT * FROM " + tableName, connection);

            SqlCommandBuilder builder = new SqlCommandBuilder(dataAdapter);
            dataAdapter.UpdateCommand = builder.GetUpdateCommand();

            dataTable = new DataTable();
            dataAdapter.Fill(dataTable);

            dataGridView.DefaultCellStyle.Padding = new Padding(0);
            dataGridView.DataSource = dataTable;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView.EndEdit();

                // Trim whitespace from the values
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value != null)
                        {
                            cell.Value = cell.Value.ToString().Trim();
                        }
                    }
                }

                connection.Open();
                dataAdapter.Update(dataTable);
                connection.Close();

                MessageBox.Show("Data saved successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void tableComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
