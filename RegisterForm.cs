using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace videos
{
    public partial class RegisterForm: Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();
            string email = textBox3.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("用户名、密码和邮箱不能为空！");
                return;
            }

            string connStr = "Data Source=localhost;Initial Catalog=videoapp;Integrated Security=True;";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // 检查用户名是否已存在
                string checkSql = "SELECT COUNT(*) FROM [user] WHERE username = @username";
                using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
                {
                    checkCmd.Parameters.AddWithValue("@username", username);
                    int exists = (int)checkCmd.ExecuteScalar();
                    if (exists > 0)
                    {
                        MessageBox.Show("用户名已存在，请选择其他用户名！");
                        return;
                    }
                }

                string insertSql = @"
    INSERT INTO [User] (username, password, email, avatar_url)
    VALUES (@username, @password, @email, @avatar_url)";

                using (SqlCommand insertCmd = new SqlCommand(insertSql, conn))
                {
                    insertCmd.Parameters.AddWithValue("@username", username);
                    insertCmd.Parameters.AddWithValue("@password", password); 
                    insertCmd.Parameters.AddWithValue("@email", email);
                    insertCmd.Parameters.AddWithValue("@avatar_url", DBNull.Value); // 可替换为实际图片URL字符串

                    int result = insertCmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("注册成功！");
                        this.Close(); // 或跳转到登录界面
                    }
                    else
                    {
                        MessageBox.Show("注册失败，请重试！");
                    }
                }

            }
        }
    }
}
