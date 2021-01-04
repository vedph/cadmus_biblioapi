-- MySQL dump 10.13  Distrib 8.0.21, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: cadmus-biblio
-- ------------------------------------------------------
-- Server version	8.0.21

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `author`
--

DROP TABLE IF EXISTS `author`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `author` (
  `id` char(36) NOT NULL,
  `first` varchar(50) NOT NULL,
  `last` varchar(50) NOT NULL,
  `lastx` varchar(50) NOT NULL,
  `suffix` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_last` (`last`),
  KEY `ix_lastx` (`lastx`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `authorcontainer`
--

DROP TABLE IF EXISTS `authorcontainer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `authorcontainer` (
  `authorId` char(36) NOT NULL,
  `containerId` char(36) NOT NULL,
  `role` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`authorId`,`containerId`),
  KEY `authorcontainer_container_idx` (`containerId`),
  CONSTRAINT `authorcontainer_author` FOREIGN KEY (`authorId`) REFERENCES `author` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `authorcontainer_container` FOREIGN KEY (`containerId`) REFERENCES `container` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `authorwork`
--

DROP TABLE IF EXISTS `authorwork`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `authorwork` (
  `authorId` char(36) NOT NULL,
  `workId` char(36) NOT NULL,
  `role` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`authorId`,`workId`),
  KEY `authorwork_work_idx` (`workId`),
  CONSTRAINT `authorwork_author` FOREIGN KEY (`authorId`) REFERENCES `author` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `authorwork_work` FOREIGN KEY (`workId`) REFERENCES `work` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `container`
--

DROP TABLE IF EXISTS `container`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `container` (
  `id` char(36) NOT NULL,
  `key` varchar(300) NOT NULL,
  `typeId` varchar(20) DEFAULT NULL,
  `title` varchar(200) NOT NULL,
  `titlex` varchar(200) NOT NULL,
  `language` char(3) NOT NULL,
  `edition` smallint NOT NULL,
  `publisher` varchar(50) DEFAULT NULL,
  `yearPub` smallint NOT NULL,
  `placePub` varchar(100) DEFAULT NULL,
  `location` varchar(500) DEFAULT NULL,
  `accessDate` datetime DEFAULT NULL,
  `number` varchar(50) DEFAULT NULL,
  `note` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `container_worktype_idx` (`typeId`),
  KEY `ix_key` (`key`) /*!80000 INVISIBLE */,
  KEY `ix_titlex` (`titlex`),
  CONSTRAINT `container_worktype` FOREIGN KEY (`typeId`) REFERENCES `worktype` (`id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `keyword`
--

DROP TABLE IF EXISTS `keyword`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `keyword` (
  `id` int NOT NULL AUTO_INCREMENT,
  `language` char(3) NOT NULL,
  `value` varchar(50) NOT NULL,
  `valuex` varchar(50) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_language` (`language`) /*!80000 INVISIBLE */,
  KEY `ix_value` (`value`) /*!80000 INVISIBLE */,
  KEY `ix_valuex` (`valuex`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `keywordcontainer`
--

DROP TABLE IF EXISTS `keywordcontainer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `keywordcontainer` (
  `keywordId` int NOT NULL,
  `containerId` char(36) NOT NULL,
  PRIMARY KEY (`keywordId`,`containerId`),
  KEY `keywordcontainer_container_idx` (`containerId`),
  CONSTRAINT `keywordcontainer_container` FOREIGN KEY (`containerId`) REFERENCES `container` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `keywordcontainer_keyword` FOREIGN KEY (`keywordId`) REFERENCES `keyword` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `keywordwork`
--

DROP TABLE IF EXISTS `keywordwork`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `keywordwork` (
  `keywordId` int NOT NULL,
  `workId` char(36) NOT NULL,
  PRIMARY KEY (`keywordId`,`workId`),
  KEY `keywordwork_work_idx` (`workId`),
  CONSTRAINT `keywordwork_keyword` FOREIGN KEY (`keywordId`) REFERENCES `keyword` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `keywordwork_work` FOREIGN KEY (`workId`) REFERENCES `work` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `work`
--

DROP TABLE IF EXISTS `work`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `work` (
  `id` char(36) NOT NULL,
  `key` varchar(300) NOT NULL,
  `typeId` varchar(20) DEFAULT NULL,
  `containerId` char(36) DEFAULT NULL,
  `title` varchar(200) NOT NULL,
  `titlex` varchar(200) NOT NULL,
  `language` char(3) NOT NULL,
  `edition` smallint NOT NULL,
  `publisher` varchar(50) DEFAULT NULL,
  `yearPub` smallint NOT NULL,
  `placePub` varchar(100) DEFAULT NULL,
  `location` varchar(500) DEFAULT NULL,
  `accessDate` datetime DEFAULT NULL,
  `firstPage` smallint NOT NULL,
  `lastPage` smallint NOT NULL,
  `note` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `type_work_idx` (`typeId`),
  KEY `work_container_idx` (`containerId`),
  KEY `ix_key` (`key`) /*!80000 INVISIBLE */,
  KEY `ix_titlex` (`titlex`),
  CONSTRAINT `work_container` FOREIGN KEY (`containerId`) REFERENCES `container` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `work_worktype` FOREIGN KEY (`typeId`) REFERENCES `worktype` (`id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `worktype`
--

DROP TABLE IF EXISTS `worktype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `worktype` (
  `id` varchar(20) NOT NULL,
  `name` varchar(100) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2021-01-04 13:32:43
