CREATE DATABASE ������ݿ�
GO
USE ������ݿ�
GO

-- 1. �û���
CREATE TABLE [user] (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    [password] VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    avatar_url VARCHAR(200) NULL
);

-- 2. ��Ƶ��
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

-- 3. ���۱�
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

-- 4. ���ޱ�
CREATE TABLE video_like (
    like_num INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    video_id INT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES [user](user_id),
    FOREIGN KEY (video_id) REFERENCES video(video_id),
    CONSTRAINT UQ_like UNIQUE (user_id, video_id)  -- ��ֹ�ظ�����
);

-- 5. �ۿ���¼��
CREATE TABLE watch_history (
    history_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    video_id INT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES [user](user_id),
    FOREIGN KEY (video_id) REFERENCES video(video_id)
);

-- 6. ��ע��
CREATE TABLE follow (
    follower_num INT IDENTITY(1,1) PRIMARY KEY,
    follower_id INT NOT NULL,
    followee_id INT NOT NULL,
    follow_time DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (follower_id) REFERENCES [user](user_id),
    FOREIGN KEY (followee_id) REFERENCES [user](user_id),
    CONSTRAINT UQ_follow UNIQUE (follower_id, followee_id) 
);
