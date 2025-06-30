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
using System.IO;

namespace videos
{
    public partial class MainForm : Form
    {
        private string currentUserId;
        public MainForm(string userId)
        {
            InitializeComponent();
            currentUserId = userId;
        }



        private void button1_Click_1(object sender, EventArgs e)
        {
            string connStr = "Data Source=localhost;Initial Catalog=videoapp;Integrated Security=True;";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
            SELECT 
                u.username,
                u.avatar_url,
                (SELECT COUNT(*) FROM follow WHERE follower_id = u.user_id) AS follow_count
            FROM [user] u
            WHERE u.user_id = @user_id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@user_id", currentUserId);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string username = reader["username"].ToString();
                                string avatarUrl = reader["avatar_url"]?.ToString();
                                int followCount = Convert.ToInt32(reader["follow_count"]);


                                // 显示到界面
                                label2.Text = username;
                                label5.Text = followCount.ToString(); // 关注数


                                if (!string.IsNullOrEmpty(avatarUrl))
                                {
                                    try
                                    {
                                        pictureBox1.Load(avatarUrl);
                                    }
                                    catch
                                    {
                                        // 加载失败可用默认图
                                        pictureBox1.Image = null;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("未找到用户信息！");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("数据库连接失败：" + ex.Message);
                    }
                }
            }
            groupBox1.Show();
            groupBox2.Hide();
            groupBox3.Hide();
            groupBox4.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            groupBox1.Hide();
            groupBox2.Hide();
            groupBox3.Show();
            groupBox4.Hide();
            LoadWatchHistory();
        }
        private void LoadWatchHistory()
        {
            string connectionString = "Server=localhost;Database=videoapp;Trusted_Connection=True;";
            string query = @"
        SELECT TOP 4 v.video_url, v.title, v.cover_url
        FROM watch_history w
        JOIN video v ON w.video_id = v.video_id
        WHERE w.user_id = @userId
        ORDER BY w.history_id DESC";  // 最新记录优先

            PictureBox[] pictureBoxes = { pictureBox10, pictureBox11, pictureBox12, pictureBox13 };
            Label[] labels = { label13, label14, label15, label16 };

            foreach (var pb in pictureBoxes)
            {
                pb.Image = null;
                pb.Tag = null;
            }
            foreach (var label in labels)
            {
                label.Text = "";
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@userId", currentUserId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                int index = 0;
                while (reader.Read() && index < 4)
                {
                    string videoUrl = reader["video_url"].ToString();
                    string title = reader["title"].ToString();
                    string coverUrl = reader["cover_url"].ToString();

                    pictureBoxes[index].SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBoxes[index].Tag = videoUrl;

                    if (File.Exists(coverUrl))
                        pictureBoxes[index].Image = Image.FromFile(coverUrl);
                    else
                        pictureBoxes[index].Image = null;

                    labels[index].Text = title;

                    index++;
                }
            }
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadVideoCovers();
            groupBox1.Show();
            groupBox2.Hide();
            groupBox3.Hide();
            groupBox4.Hide();
            string connStr = "Data Source=localhost;Initial Catalog=videoapp;Integrated Security=True;";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
            SELECT 
                u.username,
                u.avatar_url,
                (SELECT COUNT(*) FROM follow WHERE follower_id = u.user_id) AS follow_count
            FROM [user] u
            WHERE u.user_id = @user_id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@user_id", currentUserId);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string username = reader["username"].ToString();
                                string avatarUrl = reader["avatar_url"]?.ToString();
                                int followCount = Convert.ToInt32(reader["follow_count"]);


                                // 显示到界面
                                label2.Text = username;
                                label5.Text = followCount.ToString(); // 关注数


                                if (!string.IsNullOrEmpty(avatarUrl))
                                {
                                    try
                                    {
                                        pictureBox1.Load(avatarUrl);
                                    }
                                    catch
                                    {
                                        // 加载失败可用默认图
                                        pictureBox1.Image = null;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("未找到用户信息！");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("数据库连接失败：" + ex.Message);
                    }
                }
            }



        }
        private void LoadVideoCovers()
        {

            string connectionString = "Server=localhost;Database=videoapp;Trusted_Connection=True;";
            string query = "SELECT cover_url FROM video ORDER BY video_id\r\nOFFSET 0 ROWS FETCH NEXT 8 ROWS ONLY;";
            string query1 = "SELECT title FROM video ORDER BY video_id\r\nOFFSET 0 ROWS FETCH NEXT 8 ROW ONLY";

            Label[] labels = { label4, label6, label7, label8, label9, label10, label11, label12 };
            // 存储封面路径
            var coverPaths = new string[8];

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    int index = 0;
                    while (reader.Read() && index < 8)
                    {
                        coverPaths[index] = reader["cover_url"] as string;
                        index++;
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("读取视频封面失败: " + ex.Message);
                    return;
                }
            }

            string qu = "SELECT video_id,video_url FROM video ORDER BY video_id";
            var videopath = new string[8];
            var videoids = new string[8];

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(qu, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                int index = 0;
                while (reader.Read() && index < 8)
                {
                    videopath[index] = reader["video_url"] as string;
                    videoids[index] = reader["video_id"].ToString();
                    index++;
                }
                reader.Close();
            }

            var tit = new string[8];
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query1, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                int index = 0;
                while (reader.Read() && index < 8)
                {
                    tit[index] = reader["title"] as string;
                    index++;
                }
                reader.Close();
            }

            // 将封面图片加载到pictureBox2到pictureBox9
            PictureBox[] pictureBoxes = { pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8, pictureBox9 };

            for (int i = 0; i < 8; i++)
            {
                pictureBoxes[i].Tag = videopath[i] + "," + videoids[i];
                labels[i].Text = tit[i];

            }


            for (int i = 0; i < coverPaths.Length; i++)
            {
                if (!string.IsNullOrEmpty(coverPaths[i]) && System.IO.File.Exists(coverPaths[i]))
                {
                    try
                    {
                        // 先释放已有Image资源，避免文件锁定
                        if (pictureBoxes[i].Image != null)
                        {
                            pictureBoxes[i].Image.Dispose();
                            pictureBoxes[i].Image = null;
                        }
                        pictureBoxes[i].Image = Image.FromFile(coverPaths[i]);
                        pictureBoxes[i].SizeMode = PictureBoxSizeMode.Zoom; // 适应大小
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"加载图片失败: {coverPaths[i]}，错误: {ex.Message}");
                    }
                }
                else
                {
                    pictureBoxes[i].Image = null; // 文件不存在或路径空，清空控件
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            groupBox1.Hide();
            groupBox3.Hide();
            groupBox2.Show();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            int videoId = int.Parse(pictureBox2.Tag.ToString().Split(',')[1]);
            videoshow videoshow = new videoshow(pictureBox2.Tag.ToString().Split(',')[0], currentUserId, videoId);
            videoshow.Show();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            videoshow videoshow = new videoshow(pictureBox5.Tag.ToString().Split(',')[0], currentUserId, int.Parse(pictureBox5.Tag.ToString().Split(',')[1]));
            videoshow.Show();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            videoshow videoshow = new videoshow(pictureBox3.Tag.ToString().Split(',')[0], currentUserId, int.Parse(pictureBox3.Tag.ToString().Split(',')[1]));
            videoshow.Show();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            videoshow videoshow = new videoshow(pictureBox4.Tag.ToString().Split(',')[0], currentUserId, int.Parse(pictureBox4.Tag.ToString().Split(',')[1]));
            videoshow.Show();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            videoshow videoshow = new videoshow(pictureBox6.Tag.ToString().Split(',')[0], currentUserId, int.Parse(pictureBox6.Tag.ToString().Split(',')[1]));
            videoshow.Show();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            videoshow videoshow = new videoshow(pictureBox7.Tag.ToString().Split(',')[0], currentUserId, int.Parse(pictureBox7.Tag.ToString().Split(',')[1]));
            videoshow.Show();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            videoshow videoshow = new videoshow(pictureBox8.Tag.ToString().Split(',')[0], currentUserId, int.Parse(pictureBox8.Tag.ToString().Split(',')[1]));
            videoshow.Show();
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            videoshow videoshow = new videoshow(pictureBox9.Tag.ToString().Split(',')[0], currentUserId, int.Parse(pictureBox9.Tag.ToString().Split(',')[1]));
            videoshow.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "选择头像图片";
            ofd.Filter = "图片文件 (*.jpg;*.png;*.jpeg;*.bmp)|*.jpg;*.png;*.jpeg;*.bmp";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string avatarPath = ofd.FileName;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Image = Image.FromFile(avatarPath);


                string connectionString = "Server=localhost;Database=videoapp;Trusted_Connection=True;";
                string updateQuery = "UPDATE [user] SET avatar_url = @avatarPath WHERE user_id = @userId;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@avatarPath", avatarPath);
                    cmd.Parameters.AddWithValue("@userId", currentUserId);

                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                            MessageBox.Show("头像上传成功！");
                        else
                            MessageBox.Show("用户不存在，头像上传失败。");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("上传失败: " + ex.Message);
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            groupBox4.Show();
            groupBox3.Hide();
            groupBox2.Hide();
            groupBox1.Hide();

            string connectionString = "Server=localhost;Database=videoapp;Trusted_Connection=True;";
            string query = @"
        SELECT TOP 4 v.video_url, v.title, v.cover_url
        FROM video_like l
        JOIN video v ON l.video_id = v.video_id
        WHERE l.user_id = @userId;";

            PictureBox[] pictureBoxes = { pictureBox14, pictureBox15, pictureBox16, pictureBox17 };
            Label[] labels = { label17, label18, label19, label20 };


            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@userId", currentUserId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                int index = 0;
                while (reader.Read() && index < pictureBoxes.Length)
                {
                    string videoUrl = reader["video_url"].ToString();
                    string title = reader["title"].ToString();
                    string coverUrl = reader["cover_url"].ToString();

                    pictureBoxes[index].SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBoxes[index].Tag = videoUrl;

                    if (File.Exists(coverUrl))
                    {
                        try
                        {
                            pictureBoxes[index].Image = Image.FromFile(coverUrl);
                        }
                        catch
                        {
                            pictureBoxes[index].Image = null;
                        }
                    }

                    labels[index].Text = title;
                    index++;
                }

                if (index == 0)
                {
                    MessageBox.Show("当前用户没有点赞过任何视频！");
                }
            }
        }
    }
}
