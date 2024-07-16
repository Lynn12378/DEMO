-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- 主机： 127.0.0.1
-- 生成日期： 2024-07-16 11:40:45
-- 服务器版本： 10.4.32-MariaDB
-- PHP 版本： 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- 数据库： `demo`
--

-- --------------------------------------------------------

--
-- 表的结构 `output_data`
--

CREATE TABLE `output_data` (
  `id` int(11) NOT NULL,
  `playerId` int(11) DEFAULT NULL,
  `killNo` int(11) DEFAULT NULL,
  `deathNo` int(11) DEFAULT NULL,
  `surviveTime` float DEFAULT NULL,
  `collisionNo` int(11) DEFAULT NULL,
  `bulletCollision` int(11) DEFAULT NULL,
  `bulletCollisionOnLiving` int(11) DEFAULT NULL,
  `remainHP` int(11) DEFAULT NULL,
  `remainBullet` int(11) DEFAULT NULL,
  `totalVoiceDetectionDuration` float DEFAULT NULL,
  `organizeNo` int(11) DEFAULT NULL,
  `fullNo` int(11) DEFAULT NULL,
  `placeholderNo` int(11) DEFAULT NULL,
  `rankNo` int(11) DEFAULT NULL,
  `giftNo` int(11) DEFAULT NULL,
  `createTeamNo` int(11) DEFAULT NULL,
  `joinTeamNo` int(11) DEFAULT NULL,
  `quitTeamNo` int(11) DEFAULT NULL,
  `repairQuantity` int(11) DEFAULT NULL,
  `restartNo` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- 转存表中的数据 `output_data`
--

INSERT INTO `output_data` (`id`, `playerId`, `killNo`, `deathNo`, `surviveTime`, `collisionNo`, `bulletCollision`, `bulletCollisionOnLiving`, `remainHP`, `remainBullet`, `totalVoiceDetectionDuration`, `organizeNo`, `fullNo`, `placeholderNo`, `rankNo`, `giftNo`, `createTeamNo`, `joinTeamNo`, `quitTeamNo`, `repairQuantity`, `restartNo`) VALUES
(11, 60, 1, 0, 15.1875, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0);

--
-- 转储表的索引
--

--
-- 表的索引 `output_data`
--
ALTER TABLE `output_data`
  ADD PRIMARY KEY (`id`),
  ADD KEY `playerId` (`playerId`);

--
-- 在导出的表使用AUTO_INCREMENT
--

--
-- 使用表AUTO_INCREMENT `output_data`
--
ALTER TABLE `output_data`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- 限制导出的表
--

--
-- 限制表 `output_data`
--
ALTER TABLE `output_data`
  ADD CONSTRAINT `output_data_ibfk_1` FOREIGN KEY (`playerId`) REFERENCES `player` (`Player_id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
