-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- 主機： 127.0.0.1
-- 產生時間： 2024-07-15 11:20:31
-- 伺服器版本： 10.4.32-MariaDB
-- PHP 版本： 8.0.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- 資料庫： `demo`
--

-- --------------------------------------------------------

--
-- 資料表結構 `outfits`
--

CREATE TABLE `outfits` (
  `Player_id` int(11) NOT NULL,
  `Player_colors` varchar(500) DEFAULT NULL,
  `Player_outfits` varchar(200) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- 傾印資料表的資料 `outfits`
--

INSERT INTO `outfits` (`Player_id`, `Player_colors`, `Player_outfits`) VALUES
(60, '[{\"r\":1,\"g\":0.9261354804039001,\"b\":0.8524802327156067,\"a\":1},{\"r\":0.7077661156654358,\"g\":0.3359142243862152,\"b\":0.8043939471244812,\"a\":1}]', '[\"Bob\",\"Eyes\",\"wOpen\",\"None\",\"Shirt_Long\",\"Rosette\",\"None\",\"Rosette\",\"None\",\"None\"]'),
(61, '[{\"r\":0.5472000241279602,\"g\":0.34889471530914307,\"b\":0.34889471530914307,\"a\":1},{\"r\":1,\"g\":0.9196118712425232,\"b\":0.5127999782562256,\"a\":1}]', '[\"Bob\",\"Eyes\",\"w\",\"None\",\"Suit_Short\",\"Leather\",\"None\",\"Leather\",\"None\",\"None\"]');

--
-- 已傾印資料表的索引
--

--
-- 資料表索引 `outfits`
--
ALTER TABLE `outfits`
  ADD PRIMARY KEY (`Player_id`);

--
-- 在傾印的資料表使用自動遞增(AUTO_INCREMENT)
--

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `outfits`
--
ALTER TABLE `outfits`
  MODIFY `Player_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=62;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
