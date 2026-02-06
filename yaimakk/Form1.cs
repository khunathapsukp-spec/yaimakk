namespace yaimakk;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows.Forms;

public partial class Form1 : Form
    {
    private string connString;

    public Form1()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        {
            try
            {
                const string dbPath = "db/database.db";
                Directory.CreateDirectory(Path.GetDirectoryName(dbPath) ?? string.Empty);

                if (!File.Exists(dbPath))
                {
                    SQLiteConnection.CreateFile(dbPath);
                    using var conn = new SQLiteConnection(connString);
                    conn.Open();

                    const string createTableSQL = """
                        CREATE TABLE userData (
                            ProductID INTEGER PRIMARY KEY AUTOINCREMENT,
                            Productname TEXT NOT NULL,
                            Brand TEXT NOT NULL,
                            Price TEXT,
                        )
                        """;

                    const string sampleDataSQL = """
                        INSERT INTO userData (username, password, name, lastname, email) 
                        VALUES ('john', '123', 'John', 'Smith', 'john.smith@email.com')
                        """;

                    using var cmd = new SQLiteCommand(createTableSQL, conn);
                    cmd.ExecuteNonQuery();

                    using var cmd2 = new SQLiteCommand(sampleDataSQL, conn);
                    cmd2.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error สร้างฐานข้อมูล: {ex.Message}", "ข้อผิดพลาด",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
    private void Deleteproduct_Click(object sender, EventArgs e)
    {
        try
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("กรุณาเลือกแถวที่ต้องการลบ", "เลือกข้อมูล",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("คุณแน่ใจหรือไม่ที่จะลบข้อมูลผู้ใช้นี้?",
                "ยืนยันการลบ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            int userId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["userId"].Value);
            const string deleteSQL = "DELETE FROM userData WHERE userId = @userId";

            using var conn = new SQLiteConnection(connString);
            conn.Open();
            using var cmd = new SQLiteCommand(deleteSQL, conn);
            cmd.Parameters.AddWithValue("@userId", userId);

            if (cmd.ExecuteNonQuery() > 0)
            {
                MessageBox.Show("ลบข้อมูลผู้ใช้เรียบร้อย", "สำเร็จ",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearTextBoxes();
                LoadUserData();
            }
            else
            {
                MessageBox.Show("ไม่พบข้อมูลที่จะลบ", "ไม่พบข้อมูล",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error ลบข้อมูล: {ex.Message}", "ข้อผิดพลาด",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
