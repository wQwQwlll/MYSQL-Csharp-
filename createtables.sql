CREATE DATABASE 你的数据库
GO
USE 你的数据库
GO

-- 1. 用户表
CREATE TABLE [user] (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    [password] VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    avatar_url VARCHAR(200) NULL
);

-- 2. 视频表
CREATE TABLE video (
    video_id INT IDENTITY(1,1) PRIMARY KEY,
    title VARCHAR(100) NOT NULL,
    [description] TEXT NULL,
    upload_time DATETIME NOT NULL DEFAULT GETDATE(),
    duration INT NOT NULL,
    video_url VARCHAR(200) NOT NULL,
    cover_url VARCHAR(200) NULL,
    views INT NOT NULL DEFAULT 0,
    user_id INT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES [user](user_id)
);

-- 3. 评论表
CREATE TABLE comment (
    comment_id INT IDENTITY(1,1) PRIMARY KEY,
    content TEXT NOT NULL,
    post_time DATETIME NOT NULL DEFAULT GETDATE(),
    user_id INT NOT NULL,
    video_id INT NOT NULL,
    parent_id INT NULL,
    FOREIGN KEY (user_id) REFERENCES [user](user_id),
    FOREIGN KEY (video_id) REFERENCES video(video_id),
    FOREIGN KEY (parent_id) REFERENCES comment(comment_id)
);

-- 4. 点赞表
CREATE TABLE video_like (
    like_num INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    video_id INT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES [user](user_id),
    FOREIGN KEY (video_id) REFERENCES video(video_id),
    CONSTRAINT UQ_like UNIQUE (user_id, video_id)  -- 防止重复点赞
);

-- 5. 观看记录表
CREATE TABLE watch_history (
    history_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    video_id INT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES [user](user_id),
    FOREIGN KEY (video_id) REFERENCES video(video_id)
);

-- 6. 关注表
CREATE TABLE follow (
    follower_num INT IDENTITY(1,1) PRIMARY KEY,
    follower_id INT NOT NULL,
    followee_id INT NOT NULL,
    follow_time DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (follower_id) REFERENCES [user](user_id),
    FOREIGN KEY (followee_id) REFERENCES [user](user_id),
    CONSTRAINT UQ_follow UNIQUE (follower_id, followee_id) 
);
