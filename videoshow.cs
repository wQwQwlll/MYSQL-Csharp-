using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace videos
{
    public partial class videoshow: Form
    {
        private string vpath;
        private string currentuserID;
        private int videoID;
        public videoshow(string path,string userid, int videoID)
        {
            InitializeComponent();
            vpath = path;
            currentuserID = userid;
            this.AutoScroll = true;
            this.videoID = videoID;
        }

        private void videoshow_Load(object sender, EventArgs e)
        {
            LoadComments();

            // 插入观看历史
            string connectionString = "Server=localhost;Database=videoapp;Trusted_Connection=True;";
            string insertQuery = @"
        INSERT INTO watch_history (user_id, video_id)
        SELECT @userId, @videoId
        WHERE NOT EXISTS (
            SELECT 1 FROM watch_history WHERE user_id = @userId AND video_id = @videoId
        );"; // 防止重复记录（可选）

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
            {
                cmd.Parameters.AddWithValue("@userId", currentuserID);
                cmd.Parameters.AddWithValue("@videoId", videoID);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("录入观看历史失败：" + ex.Message);
                }
            }

            this.BeginInvoke(new Action(() =>
            {

                axWindowsMediaPlayer1.URL = vpath;
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int userId = int.Parse(currentuserID); 
            int videoId = videoID;

            string connectionString = "Server=localhost;Database=videoapp;Trusted_Connection=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            IF NOT EXISTS (
                SELECT 1 FROM video_like WHERE user_id = @userId AND video_id = @videoId
            )
            BEGIN
                INSERT INTO video_like (user_id, video_id) VALUES (@userId, @videoId);
            END
            ELSE
            BEGIN
                RAISERROR('您已点赞过此视频。', 16, 1);
            END";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@videoId", videoId);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("点赞成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Message.Contains("您已点赞过此视频"))
                            MessageBox.Show("您已经点过赞了！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        else
                            MessageBox.Show("点赞失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string content = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(content))
            {
                MessageBox.Show("评论内容不能为空！");
                return;
            }

            string connectionString = "Server=localhost;Database=videoapp;Trusted_Connection=True;";
            string insertQuery = @"INSERT INTO comment (content, post_time, user_id, video_id, parent_id)
                          VALUES (@content, GETDATE(), @userId, @videoId, NULL)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
            {
                cmd.Parameters.AddWithValue("@content", content);
                cmd.Parameters.AddWithValue("@userId", currentuserID);
                cmd.Parameters.AddWithValue("@videoId", videoID);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("评论成功！");
                    textBox1.Clear();
                    LoadComments(); // 重新加载评论显示
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("评论失败: " + ex.Message);
                }
            }
        }

        private void LoadComments()
        {
            textBox2.Clear();
            string connectionString = "Server=localhost;Database=videoapp;Trusted_Connection=True;";
            string query = @"SELECT u.username, c.content, c.post_time
                    FROM comment c
                    INNER JOIN [user] u ON c.user_id = u.user_id
                    WHERE c.video_id = @videoId
                    ORDER BY c.post_time DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@videoId", videoID);

                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string username = reader["username"].ToString();
                            string content = reader["content"].ToString();
                            DateTime time = Convert.ToDateTime(reader["post_time"]);
                            textBox2.AppendText($"{username} ({time:yyyy-MM-dd HH:mm}):\r\n{content}\r\n\r\n");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("加载评论失败: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=localhost;Database=videoapp;Trusted_Connection=True;";
            int followerId = int.Parse(currentuserID); // 当前登录用户
            int videoId = this.videoID;                // 当前视频ID
            int followeeId = -1;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 1. 通过 video_id 查询上传者（被关注者）ID
                string getUploaderQuery = "SELECT user_id FROM video WHERE video_id = @videoId";
                using (SqlCommand cmd = new SqlCommand(getUploaderQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@videoId", videoId);
                    var result = cmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("找不到该视频对应的上传者！");
                        return;
                    }
                    followeeId = Convert.ToInt32(result);
                }

                // 2. 防止关注自己
                if (followerId == followeeId)
                {
                    MessageBox.Show("不能关注自己！");
                    return;
                }

                // 3. 插入关注记录
                string insertFollowQuery = @"
            IF NOT EXISTS (
                SELECT 1 FROM follow WHERE follower_id = @follower AND followee_id = @followee
            )
            BEGIN
                INSERT INTO follow (follower_id, followee_id) VALUES (@follower, @followee);
            END
            ELSE
            BEGIN
                RAISERROR('您已关注该用户。', 16, 1);
            END";

                using (SqlCommand followCmd = new SqlCommand(insertFollowQuery, conn))
                {
                    followCmd.Parameters.AddWithValue("@follower", followerId);
                    followCmd.Parameters.AddWithValue("@followee", followeeId);

                    try
                    {
                        followCmd.ExecuteNonQuery();
                        MessageBox.Show("关注成功！");
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Message.Contains("您已关注"))
                            MessageBox.Show("您已经关注过该用户了！");
                        else
                            MessageBox.Show("关注失败：" + ex.Message);
                    }
                }
            }
        }
    }
}
