-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- 主机： 127.0.0.1
-- 生成日期： 2024-07-25 12:55:45
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
-- 表的结构 `bfi_responses`
--

CREATE TABLE `bfi_responses` (
  `id` int(11) NOT NULL,
  `player_id` int(11) NOT NULL,
  `responses` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `feedback` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- 转存表中的数据 `bfi_responses`
--

INSERT INTO `bfi_responses` (`id`, `player_id`, `responses`, `feedback`) VALUES
(38, 60, '[1,1,1,1,1,1,1,1,1,1,1,1,1,1,1]', ' OK');

-- --------------------------------------------------------

--
-- 表的结构 `outfits`
--

CREATE TABLE `outfits` (
  `Player_id` int(11) NOT NULL,
  `Player_colors` varchar(500) DEFAULT NULL,
  `Player_outfits` varchar(200) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- 转存表中的数据 `outfits`
--

INSERT INTO `outfits` (`Player_id`, `Player_colors`, `Player_outfits`) VALUES
(60, '[{\"r\":1,\"g\":0.9261354804039001,\"b\":0.8524802327156067,\"a\":1},{\"r\":0.7077661156654358,\"g\":0.3359142243862152,\"b\":0.8043939471244812,\"a\":1}]', '[\"Bob\",\"Eyes\",\"wOpen\",\"None\",\"Shirt_Long\",\"Rosette\",\"None\",\"Rosette\",\"None\",\"None\"]'),
(61, '[{\"r\":0.5472000241279602,\"g\":0.34889471530914307,\"b\":0.34889471530914307,\"a\":1},{\"r\":1,\"g\":0.9196118712425232,\"b\":0.5127999782562256,\"a\":1}]', '[\"Bob\",\"Eyes\",\"w\",\"None\",\"Suit_Short\",\"Leather\",\"None\",\"Leather\",\"None\",\"None\"]'),
(62, '[{\"r\":0.5472000241279602,\"g\":0.34889471530914307,\"b\":0.34889471530914307,\"a\":1},{\"r\":0.9719878435134888,\"g\":0.7246074676513672,\"b\":0.30399394035339355,\"a\":1}]', '[\"Bun\",\"Eyes\",\"wOpen\",\"None\",\"Suit_Short\",\"Rosette\",\"Suit_Long\",\"Rosette\",\"Suit_Long\",\"Suit_Short\"]');

-- --------------------------------------------------------

--
-- 表的结构 `output_data`
--

CREATE TABLE `output_data` (
  `id` int(11) NOT NULL,
  `playerId` int(11) NOT NULL,
  `killNo` int(11) NOT NULL,
  `deathNo` int(11) NOT NULL,
  `surviveTime` float NOT NULL,
  `collisionNo` int(11) NOT NULL,
  `bulletCollision` int(11) NOT NULL,
  `bulletCollisionOnLiving` int(11) NOT NULL,
  `remainHP` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`remainHP`)),
  `remainBullet` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`remainBullet`)),
  `totalVoiceDetectionDuration` float NOT NULL,
  `organizeNo` int(11) NOT NULL,
  `fullNo` int(11) NOT NULL,
  `placeholderNo` int(11) NOT NULL,
  `rankNo` int(11) NOT NULL,
  `giftNo` int(11) NOT NULL,
  `createTeamNo` int(11) NOT NULL,
  `joinTeamNo` int(11) NOT NULL,
  `quitTeamNo` int(11) NOT NULL,
  `repairQuantity` int(11) NOT NULL,
  `restartNo` int(11) NOT NULL,
  `usePlaceholderNo` int(11) NOT NULL,
  `petNo` int(11) NOT NULL,
  `sendMessageNo` int(11) NOT NULL,
  `durationOfRound` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- 转存表中的数据 `output_data`
--

INSERT INTO `output_data` (`id`, `playerId`, `killNo`, `deathNo`, `surviveTime`, `collisionNo`, `bulletCollision`, `bulletCollisionOnLiving`, `remainHP`, `remainBullet`, `totalVoiceDetectionDuration`, `organizeNo`, `fullNo`, `placeholderNo`, `rankNo`, `giftNo`, `createTeamNo`, `joinTeamNo`, `quitTeamNo`, `repairQuantity`, `restartNo`, `usePlaceholderNo`, `petNo`, `sendMessageNo`, `durationOfRound`) VALUES
(12, 60, 1, 0, 143.5, 0, 0, 0, '[0,60,80]', '[0]', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
(13, 60, 0, 0, 41.2188, 0, 0, 0, '[0]', '[0]', 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
(14, 61, 0, 0, 98.5, 0, 0, 0, '[0]', '[0]', 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0);

-- --------------------------------------------------------

--
-- 表的结构 `player`
--

CREATE TABLE `player` (
  `Player_id` int(11) NOT NULL,
  `Player_name` varchar(45) NOT NULL,
  `Player_password` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- 转存表中的数据 `player`
--

INSERT INTO `player` (`Player_id`, `Player_name`, `Player_password`) VALUES
(60, 'demo', '12345'),
(61, 'demo2', '1111'),
(64, 'demo3', '12345'),
(65, 'demo4', '12345'),
(68, 'demo5', '12345');

--
-- 转储表的索引
--

--
-- 表的索引 `bfi_responses`
--
ALTER TABLE `bfi_responses`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_player_id` (`player_id`);

--
-- 表的索引 `outfits`
--
ALTER TABLE `outfits`
  ADD PRIMARY KEY (`Player_id`);

--
-- 表的索引 `output_data`
--
ALTER TABLE `output_data`
  ADD PRIMARY KEY (`id`),
  ADD KEY `playerId` (`playerId`);

--
-- 表的索引 `player`
--
ALTER TABLE `player`
  ADD PRIMARY KEY (`Player_id`);

--
-- 在导出的表使用AUTO_INCREMENT
--

--
-- 使用表AUTO_INCREMENT `bfi_responses`
--
ALTER TABLE `bfi_responses`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=39;

--
-- 使用表AUTO_INCREMENT `outfits`
--
ALTER TABLE `outfits`
  MODIFY `Player_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=63;

--
-- 使用表AUTO_INCREMENT `output_data`
--
ALTER TABLE `output_data`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- 使用表AUTO_INCREMENT `player`
--
ALTER TABLE `player`
  MODIFY `Player_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=70;

--
-- 限制导出的表
--

--
-- 限制表 `bfi_responses`
--
ALTER TABLE `bfi_responses`
  ADD CONSTRAINT `fk_player_id` FOREIGN KEY (`player_id`) REFERENCES `player` (`Player_id`);

--
-- 限制表 `output_data`
--
ALTER TABLE `output_data`
  ADD CONSTRAINT `output_data_ibfk_1` FOREIGN KEY (`playerId`) REFERENCES `player` (`Player_id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
