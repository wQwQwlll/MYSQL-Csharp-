using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace videos
{
    public partial class Form1: Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            string sql = "SELECT user_id FROM [user] WHERE username = @username AND password = @password";

            using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=videoapp;Integrated Security=True;"))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            int userId = Convert.ToInt32(result); // 获取user_id
                            MessageBox.Show("登录成功！");

                            this.Hide();
                            MainForm mainForm = new MainForm(userId.ToString()); // 传递user_id
                            mainForm.ShowDialog();
                            this.Show();
                        }
                        else
                        {
                            MessageBox.Show("用户名或密码错误！");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("连接失败：" + ex.Message);
                    }
                }
            }
        }
    }
}
