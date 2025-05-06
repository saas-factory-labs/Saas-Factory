USE `test`; -- database navn/schema

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
--
-- Database: `gf_gg`
--

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `agelimit`
--

CREATE TABLE `agelimit` (
  `id` bigint UNSIGNED NOT NULL,
  `age` int DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `agelimit`
--

INSERT INTO `agelimit` (`id`, `age`, `created_at`, `updated_at`, `deleted_at`) VALUES
(1, 13, NULL, NULL, NULL),
(2, 15, NULL, NULL, NULL),
(3, 18, NULL, NULL, NULL),
(4, 25, NULL, NULL, NULL),
(5, 30, NULL, NULL, NULL);

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `ambitions`
--

CREATE TABLE `ambitions` (
  `id` int UNSIGNED NOT NULL,
  `ambitionlevel` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `ambitions`
--

INSERT INTO `ambitions` (`id`, `ambitionlevel`) VALUES
(1, 'Play for fun'),
(2, 'Casual'),
(3, 'Pro Esport');

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `apikeys`
--

CREATE TABLE `apikeys` (
  `id` bigint UNSIGNED NOT NULL,
  `api_service_id` int DEFAULT NULL,
  `servicename` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `apivalue1` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `apivalue2` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `api_user_data`
--

CREATE TABLE `api_user_data` (
  `id` bigint UNSIGNED NOT NULL,
  `user_id` int NOT NULL,
  `data_key` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `data_value` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `article`
--

CREATE TABLE `article` (
  `id` int UNSIGNED NOT NULL,
  `title` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `user_id` int DEFAULT NULL,
  `appetizer` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `media` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `picturepath` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `air` int DEFAULT NULL,
  `article` text COLLATE utf8mb4_unicode_ci,
  `articleurl` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `battlenetserverlist`
--

CREATE TABLE `battlenetserverlist` (
  `id` int UNSIGNED NOT NULL,
  `server_alias` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `server_name` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `changelogs`
--

CREATE TABLE `changelogs` (
  `id` bigint UNSIGNED NOT NULL,
  `title` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `category_id` bigint UNSIGNED DEFAULT NULL,
  `created_by` bigint UNSIGNED NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `changelogs`
--

INSERT INTO `changelogs` (`id`, `title`, `description`, `category_id`, `created_by`, `created_at`, `updated_at`) VALUES
(1, 'test', 'test system update', 2, 1, '2023-05-13 23:04:56', '2023-05-13 23:04:56'),
(2, 'Test of entry', 'feature created', 1, 1, '2023-05-15 13:28:30', '2023-05-15 13:28:30'),
(3, 'Fixed the backlog creation tool', 'I added design to the backlog but I realised that I need to re do the design on the index too since forms aint the right choice for the list', 3, 1, '2023-05-15 13:30:07', '2023-05-15 13:30:07'),
(4, 'Created a blue greyish background for the backlog theme', 'Now I just need to add a redirect to the changelogs list so it auto redirects into the index.blade.php', 1, 1, '2023-05-15 14:07:28', '2023-05-15 14:07:28'),
(5, 'test of redirect', 'Redirect bug fixed', 3, 1, '2023-05-15 14:10:04', '2023-05-15 14:10:04'),
(6, 'the commitments is in server time', 'test of commitments', 1, 1, '2023-05-15 14:23:49', '2023-05-15 14:23:49'),
(7, 'Bug that needs to be fixed but not urgent', 'Timezone in laravel project needs to be updated', 4, 1, '2023-05-15 14:26:49', '2023-05-15 14:26:49'),
(8, 'This is urgent', 'Fix me now', 1, 1, '2023-05-15 14:31:35', '2023-05-15 14:31:35'),
(9, 'Working on the message controller and conversation', 'I made a model and a controller for conversations which is gonna be the new way of connecting messages between users. \r\nSo a conversation is a container of the message between sender and receiver. Its to make real time chat methods like discord or facebook messenger so this is gonna be the early version of this. \r\nBecause I want the message system to be up running flawless before the tournament system to make it easier to keep people on the website so they dont need to use discord or other third party apps unless highly necessary', 5, 1, '2023-05-17 03:13:27', '2023-05-17 03:13:27'),
(10, 'accordion', 'Accordion for left menu needs to be tested', 2, 1, '2023-05-19 08:26:14', '2023-05-19 08:26:14'),
(11, 'Changed sender and receiver id', 'All user_id that aint the auth user is sender id and auth user will always be receiver id.\r\nNeeds to be updated in the database structure aswell and all places in the system where the sender and receiver id is used', 3, 1, '2023-05-19 09:03:20', '2023-05-19 09:03:20'),
(12, 'Note', 'Light budget.\r\n\r\nBedst case scenario milestone og RoI alt efter økonomisk indsprøjtning.\r\n\r\nPersona 2&3.\r\n\r\nEFU uddybet.\r\n\r\nNote', 1, 1, '2023-05-19 12:23:13', '2023-05-19 12:23:13'),
(13, 'conversation bug', 'When a user tries to access a conversation they are not part of they need to get a error message or a note that they cant access this page', 4, 1, '2023-05-25 10:28:12', '2023-05-25 10:28:12'),
(14, 'Created a middleware for chat system to be able to see the count of messages in the topnavigation', 'I have added the middleware in the kernel file. \r\nAnd created the middleware with  the fetch data where auth userid and messages opened = 0', 4, 4, '2023-05-30 20:41:12', '2023-05-30 20:41:12'),
(15, 'CV', 'Kort baggrunds forklaring med lignende projekter', 2, 4, '2023-06-01 09:44:34', '2023-06-01 09:44:34'),
(16, 'Fixed count message in topnav with middleware', 'Fixed count messages in topnav with a middleware:\r\nclass ShareUnreadMessageCount', 3, 1, '2023-06-02 07:51:51', '2023-06-02 07:51:51'),
(17, 'Make knowledge sharing tool', 'Make knowledge sharing tool for organisations', 5, 1, '2023-06-07 11:00:39', '2023-06-07 11:00:39'),
(18, 'Friend Request', 'Decided to move the friend request into its own table for friend requests to make it easier to implement in other systems', 3, 1, '2023-06-19 10:48:29', '2023-06-19 10:48:29'),
(19, 'create a nice button', 'clip-path: polygon(85% 0, 100% 30%, 100% 100%, 15% 100%, 0 70%, 0 0);', 5, 1, '2023-06-22 16:33:15', '2023-06-22 16:33:15'),
(20, 'updated the guide section', 'Updated the guide section so now it works for multi search and is having the newest guides presented first', 2, 1, '2023-06-26 14:58:05', '2023-06-26 14:58:05'),
(21, 'Show Guide updates', 'Added other guides by author on a guide', 2, 1, '2023-06-26 15:48:54', '2023-06-26 15:48:54');

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `chats`
--

CREATE TABLE `chats` (
  `id` bigint UNSIGNED NOT NULL,
  `sender_id` bigint UNSIGNED NOT NULL,
  `receiver_id` bigint UNSIGNED NOT NULL,
  `message` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `comment`
--

CREATE TABLE `comment` (
  `id` bigint UNSIGNED NOT NULL,
  `user_id` int NOT NULL,
  `page_id` int DEFAULT NULL,
  `item_id` int DEFAULT NULL,
  `message` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `status` tinyint DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `like` int DEFAULT NULL,
  `report` int DEFAULT NULL,
  `postparentid` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `community`
--

CREATE TABLE `community` (
  `id` bigint UNSIGNED NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `game_id` int DEFAULT NULL,
  `password` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `twitch` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `youtube` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `twitter` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `picturepath` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `conversations`
--

CREATE TABLE `conversations` (
  `id` bigint UNSIGNED NOT NULL,
  `title` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `owner_id` bigint UNSIGNED DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `conversation_participants`
--

CREATE TABLE `conversation_participants` (
  `id` bigint UNSIGNED NOT NULL,
  `conversation_id` int NOT NULL,
  `user_id` int NOT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `country`
--

CREATE TABLE `country` (
  `id` int UNSIGNED NOT NULL,
  `code` varchar(5) COLLATE utf8mb4_unicode_ci NOT NULL,
  `countryname` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `country`
--

INSERT INTO `country` (`id`, `code`, `countryname`) VALUES
(1, 'US', 'United States'),
(2, 'CA', 'Canada'),
(3, 'AF', 'Afghanistan'),
(4, 'AL', 'Albania'),
(5, 'DZ', 'Algeria'),
(6, 'AS', 'American Samoa'),
(7, 'AD', 'Andorra'),
(8, 'AO', 'Angola'),
(9, 'AI', 'Anguilla'),
(10, 'AQ', 'Antarctica'),
(11, 'AG', 'Antigua and/or Barbuda'),
(12, 'AR', 'Argentina'),
(13, 'AM', 'Armenia'),
(14, 'AW', 'Aruba'),
(15, 'AU', 'Australia'),
(16, 'AT', 'Austria'),
(17, 'AZ', 'Azerbaijan'),
(18, 'BS', 'Bahamas'),
(19, 'BH', 'Bahrain'),
(20, 'BD', 'Bangladesh'),
(21, 'BB', 'Barbados'),
(22, 'BY', 'Belarus'),
(23, 'BE', 'Belgium'),
(24, 'BZ', 'Belize'),
(25, 'BJ', 'Benin'),
(26, 'BM', 'Bermuda'),
(27, 'BT', 'Bhutan'),
(28, 'BO', 'Bolivia'),
(29, 'BA', 'Bosnia and Herzegovina'),
(30, 'BW', 'Botswana'),
(31, 'BV', 'Bouvet Island'),
(32, 'BR', 'Brazil'),
(33, 'IO', 'British lndian Ocean Territory'),
(34, 'BN', 'Brunei Darussalam'),
(35, 'BG', 'Bulgaria'),
(36, 'BF', 'Burkina Faso'),
(37, 'BI', 'Burundi'),
(38, 'KH', 'Cambodia'),
(39, 'CM', 'Cameroon'),
(40, 'CV', 'Cape Verde'),
(41, 'KY', 'Cayman Islands'),
(42, 'CF', 'Central African Republic'),
(43, 'TD', 'Chad'),
(44, 'CL', 'Chile'),
(45, 'CN', 'China'),
(46, 'CX', 'Christmas Island'),
(47, 'CC', 'Cocos (Keeling) Islands'),
(48, 'CO', 'Colombia'),
(49, 'KM', 'Comoros'),
(50, 'CG', 'Congo'),
(51, 'CK', 'Cook Islands'),
(52, 'CR', 'Costa Rica'),
(53, 'HR', 'Croatia (Hrvatska)'),
(54, 'CU', 'Cuba'),
(55, 'CY', 'Cyprus'),
(56, 'CZ', 'Czech Republic'),
(57, 'CD', 'Democratic Republic of Congo'),
(58, 'DK', 'Denmark'),
(59, 'DJ', 'Djibouti'),
(60, 'DM', 'Dominica'),
(61, 'DO', 'Dominican Republic'),
(62, 'TP', 'East Timor'),
(63, 'EC', 'Ecudaor'),
(64, 'EG', 'Egypt'),
(65, 'SV', 'El Salvador'),
(66, 'GQ', 'Equatorial Guinea'),
(67, 'ER', 'Eritrea'),
(68, 'EE', 'Estonia'),
(69, 'ET', 'Ethiopia'),
(70, 'FK', 'Falkland Islands (Malvinas)'),
(71, 'FO', 'Faroe Islands'),
(72, 'FJ', 'Fiji'),
(73, 'FI', 'Finland'),
(74, 'FR', 'France'),
(75, 'FX', 'France, Metropolitan'),
(76, 'GF', 'French Guiana'),
(77, 'PF', 'French Polynesia'),
(78, 'TF', 'French Southern Territories'),
(79, 'GA', 'Gabon'),
(80, 'GM', 'Gambia'),
(81, 'GE', 'Georgia'),
(82, 'DE', 'Germany'),
(83, 'GH', 'Ghana'),
(84, 'GI', 'Gibraltar'),
(85, 'GR', 'Greece'),
(86, 'GL', 'Greenland'),
(87, 'GD', 'Grenada'),
(88, 'GP', 'Guadeloupe'),
(89, 'GU', 'Guam'),
(90, 'GT', 'Guatemala'),
(91, 'GN', 'Guinea'),
(92, 'GW', 'Guinea-Bissau'),
(93, 'GY', 'Guyana'),
(94, 'HT', 'Haiti'),
(95, 'HM', 'Heard and Mc Donald Islands'),
(96, 'HN', 'Honduras'),
(97, 'HK', 'Hong Kong'),
(98, 'HU', 'Hungary'),
(99, 'IS', 'Iceland'),
(100, 'IN', 'India'),
(101, 'ID', 'Indonesia'),
(102, 'IR', 'Iran (Islamic Republic of)'),
(103, 'IQ', 'Iraq'),
(104, 'IE', 'Ireland'),
(105, 'IL', 'Israel'),
(106, 'IT', 'Italy'),
(107, 'CI', 'Ivory Coast'),
(108, 'JM', 'Jamaica'),
(109, 'JP', 'Japan'),
(110, 'JO', 'Jordan'),
(111, 'KZ', 'Kazakhstan'),
(112, 'KE', 'Kenya'),
(113, 'KI', 'Kiribati'),
(114, 'KP', 'Korea, Democratic People\'s Republic of'),
(115, 'KR', 'Korea, Republic of'),
(116, 'KW', 'Kuwait'),
(117, 'KG', 'Kyrgyzstan'),
(118, 'LA', 'Lao People\'s Democratic Republic'),
(119, 'LV', 'Latvia'),
(120, 'LB', 'Lebanon'),
(121, 'LS', 'Lesotho'),
(122, 'LR', 'Liberia'),
(123, 'LY', 'Libyan Arab Jamahiriya'),
(124, 'LI', 'Liechtenstein'),
(125, 'LT', 'Lithuania'),
(126, 'LU', 'Luxembourg'),
(127, 'MO', 'Macau'),
(128, 'MK', 'Macedonia'),
(129, 'MG', 'Madagascar'),
(130, 'MW', 'Malawi'),
(131, 'MY', 'Malaysia'),
(132, 'MV', 'Maldives'),
(133, 'ML', 'Mali'),
(134, 'MT', 'Malta'),
(135, 'MH', 'Marshall Islands'),
(136, 'MQ', 'Martinique'),
(137, 'MR', 'Mauritania'),
(138, 'MU', 'Mauritius'),
(139, 'TY', 'Mayotte'),
(140, 'MX', 'Mexico'),
(141, 'FM', 'Micronesia, Federated States of'),
(142, 'MD', 'Moldova, Republic of'),
(143, 'MC', 'Monaco'),
(144, 'MN', 'Mongolia'),
(145, 'MS', 'Montserrat'),
(146, 'MA', 'Morocco'),
(147, 'MZ', 'Mozambique'),
(148, 'MM', 'Myanmar'),
(149, 'NA', 'Namibia'),
(150, 'NR', 'Nauru'),
(151, 'NP', 'Nepal'),
(152, 'NL', 'Netherlands'),
(153, 'AN', 'Netherlands Antilles'),
(154, 'NC', 'New Caledonia'),
(155, 'NZ', 'New Zealand'),
(156, 'NI', 'Nicaragua'),
(157, 'NE', 'Niger'),
(158, 'NG', 'Nigeria'),
(159, 'NU', 'Niue'),
(160, 'NF', 'Norfork Island'),
(161, 'MP', 'Northern Mariana Islands'),
(162, 'NO', 'Norway'),
(163, 'OM', 'Oman'),
(164, 'PK', 'Pakistan'),
(165, 'PW', 'Palau'),
(166, 'PA', 'Panama'),
(167, 'PG', 'Papua New Guinea'),
(168, 'PY', 'Paraguay'),
(169, 'PE', 'Peru'),
(170, 'PH', 'Philippines'),
(171, 'PN', 'Pitcairn'),
(172, 'PL', 'Poland'),
(173, 'PT', 'Portugal'),
(174, 'PR', 'Puerto Rico'),
(175, 'QA', 'Qatar'),
(176, 'SS', 'Republic of South Sudan'),
(177, 'RE', 'Reunion'),
(178, 'RO', 'Romania'),
(179, 'RU', 'Russian Federation'),
(180, 'RW', 'Rwanda'),
(181, 'KN', 'Saint Kitts and Nevis'),
(182, 'LC', 'Saint Lucia'),
(183, 'VC', 'Saint Vincent and the Grenadines'),
(184, 'WS', 'Samoa'),
(185, 'SM', 'San Marino'),
(186, 'ST', 'Sao Tome and Principe'),
(187, 'SA', 'Saudi Arabia'),
(188, 'SN', 'Senegal'),
(189, 'RS', 'Serbia'),
(190, 'SC', 'Seychelles'),
(191, 'SL', 'Sierra Leone'),
(192, 'SG', 'Singapore'),
(193, 'SK', 'Slovakia'),
(194, 'SI', 'Slovenia'),
(195, 'SB', 'Solomon Islands'),
(196, 'SO', 'Somalia'),
(197, 'ZA', 'South Africa'),
(198, 'GS', 'South Georgia South Sandwich Islands'),
(199, 'ES', 'Spain'),
(200, 'LK', 'Sri Lanka'),
(201, 'SH', 'St. Helena'),
(202, 'PM', 'St. Pierre and Miquelon'),
(203, 'SD', 'Sudan'),
(204, 'SR', 'Suriname'),
(205, 'SJ', 'Svalbarn and Jan Mayen Islands'),
(206, 'SZ', 'Swaziland'),
(207, 'SE', 'Sweden'),
(208, 'CH', 'Switzerland'),
(209, 'SY', 'Syrian Arab Republic'),
(210, 'TW', 'Taiwan'),
(211, 'TJ', 'Tajikistan'),
(212, 'TZ', 'Tanzania, United Republic of'),
(213, 'TH', 'Thailand'),
(214, 'TG', 'Togo'),
(215, 'TK', 'Tokelau'),
(216, 'TO', 'Tonga'),
(217, 'TT', 'Trinidad and Tobago'),
(218, 'TN', 'Tunisia'),
(219, 'TR', 'Turkey'),
(220, 'TM', 'Turkmenistan'),
(221, 'TC', 'Turks and Caicos Islands'),
(222, 'TV', 'Tuvalu'),
(223, 'UG', 'Uganda'),
(224, 'UA', 'Ukraine'),
(225, 'AE', 'United Arab Emirates'),
(226, 'GB', 'United Kingdom'),
(227, 'UM', 'United States minor outlying islands'),
(228, 'UY', 'Uruguay'),
(229, 'UZ', 'Uzbekistan'),
(230, 'VU', 'Vanuatu'),
(231, 'VA', 'Vatican City State'),
(232, 'VE', 'Venezuela'),
(233, 'VN', 'Vietnam'),
(234, 'VG', 'Virgin Islands (British)'),
(235, 'VI', 'Virgin Islands (U.S.)'),
(236, 'WF', 'Wallis and Futuna Islands'),
(237, 'EH', 'Western Sahara'),
(238, 'YE', 'Yemen'),
(239, 'YU', 'Yugoslavia'),
(240, 'ZR', 'Zaire'),
(241, 'ZM', 'Zambia'),
(242, 'ZW', 'Zimbabwe');

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `eventlist`
--

CREATE TABLE `eventlist` (
  `id` int UNSIGNED NOT NULL,
  `participants` int NOT NULL,
  `eventname` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `day` int NOT NULL,
  `month` int NOT NULL,
  `year` int NOT NULL,
  `description` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `price` int NOT NULL,
  `picturepath` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `failed_jobs`
--

CREATE TABLE `failed_jobs` (
  `id` bigint UNSIGNED NOT NULL,
  `uuid` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `connection` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `queue` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `payload` longtext COLLATE utf8mb4_unicode_ci NOT NULL,
  `exception` longtext COLLATE utf8mb4_unicode_ci NOT NULL,
  `failed_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `fingerprints`
--

CREATE TABLE `fingerprints` (
  `id` bigint UNSIGNED NOT NULL,
  `fingerprint` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `user_id` int DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `freegames`
--

CREATE TABLE `freegames` (
  `id` int UNSIGNED NOT NULL,
  `gamename` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `picturepath` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `company` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `platform` int NOT NULL,
  `expiredate` date NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `linkpath` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `gamegenre_id` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `friends`
--

CREATE TABLE `friends` (
  `id` bigint UNSIGNED NOT NULL,
  `user_id` bigint NOT NULL,
  `friend_id` bigint NOT NULL,
  `accepted` tinyint NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `friends`
--

INSERT INTO `friends` (`id`, `user_id`, `friend_id`, `accepted`, `created_at`, `updated_at`, `deleted_at`) VALUES
(17, 2, 1, 0, '2023-06-21 20:19:03', '2023-06-21 20:19:03', NULL),
(18, 1, 2, 0, '2023-06-21 20:19:03', '2023-06-21 20:19:03', NULL),
(19, 2, 1, 0, '2023-06-21 20:20:01', '2023-06-21 20:20:01', NULL),
(20, 1, 2, 0, '2023-06-21 20:20:01', '2023-06-21 20:20:01', NULL),
(21, 2, 1, 0, '2023-06-21 20:20:03', '2023-06-21 20:20:03', NULL),
(22, 1, 2, 0, '2023-06-21 20:20:03', '2023-06-21 20:20:03', NULL),
(23, 2, 1, 0, '2023-06-21 20:27:00', '2023-06-21 20:27:00', NULL),
(24, 1, 2, 0, '2023-06-21 20:27:00', '2023-06-21 20:27:00', NULL),
(25, 2, 1, 0, '2023-06-21 20:27:02', '2023-06-21 20:27:02', NULL),
(26, 1, 2, 0, '2023-06-21 20:27:02', '2023-06-21 20:27:02', NULL),
(27, 2, 1, 0, '2023-06-21 20:27:02', '2023-06-21 20:27:02', NULL),
(28, 1, 2, 0, '2023-06-21 20:27:02', '2023-06-21 20:27:02', NULL),
(29, 2, 1, 0, '2023-06-21 20:27:03', '2023-06-21 20:27:03', NULL),
(30, 1, 2, 0, '2023-06-21 20:27:03', '2023-06-21 20:27:03', NULL),
(31, 2, 1, 0, '2023-06-21 20:27:05', '2023-06-21 20:27:05', NULL),
(32, 1, 2, 0, '2023-06-21 20:27:05', '2023-06-21 20:27:05', NULL),
(33, 2, 1, 0, '2023-06-21 20:27:06', '2023-06-21 20:27:06', NULL),
(34, 1, 2, 0, '2023-06-21 20:27:06', '2023-06-21 20:27:06', NULL),
(35, 2, 1, 0, '2023-06-21 20:27:07', '2023-06-21 20:27:07', NULL),
(36, 1, 2, 0, '2023-06-21 20:27:07', '2023-06-21 20:27:07', NULL),
(37, 2, 1, 0, '2023-06-21 20:27:16', '2023-06-21 20:27:16', NULL),
(38, 1, 2, 0, '2023-06-21 20:27:16', '2023-06-21 20:27:16', NULL),
(39, 2, 1, 0, '2023-06-21 20:27:20', '2023-06-21 20:27:20', NULL),
(40, 1, 2, 0, '2023-06-21 20:27:20', '2023-06-21 20:27:20', NULL),
(41, 2, 1, 0, '2023-06-21 20:27:21', '2023-06-21 20:27:21', NULL),
(55, 14, 1, 0, '2023-09-05 18:44:31', '2023-09-05 18:44:31', NULL),
(56, 1, 14, 0, '2023-09-05 18:44:31', '2023-09-05 18:44:31', NULL),
(57, 14, 2, 0, '2023-09-05 18:44:34', '2023-09-05 18:44:34', NULL),
(58, 2, 14, 0, '2023-09-05 18:44:34', '2023-09-05 18:44:34', NULL),
(59, 14, 3, 0, '2023-09-05 18:44:36', '2023-09-05 18:44:36', NULL),
(60, 3, 14, 0, '2023-09-05 18:44:36', '2023-09-05 18:44:36', NULL),
(61, 1, 2, 0, '2023-09-30 20:58:55', '2023-09-30 20:58:55', NULL),
(62, 2, 1, 0, '2023-09-30 20:58:55', '2023-09-30 20:58:55', NULL);

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `friend_requests`
--

CREATE TABLE `friend_requests` (
  `id` bigint UNSIGNED NOT NULL,
  `sender_id` bigint UNSIGNED NOT NULL,
  `receiver_id` bigint UNSIGNED NOT NULL,
  `opened` tinyint(1) NOT NULL DEFAULT '0',
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `messagetxt` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `friend_requests`
--

INSERT INTO `friend_requests` (`id`, `sender_id`, `receiver_id`, `opened`, `created_at`, `updated_at`, `messagetxt`) VALUES
(22, 14, 1, 0, '2023-09-05 18:44:31', '2023-09-05 18:44:31', 'Click the link to accept the FriendRequest from Username<br><a href=\"http://www.gamingfriends.gg/friends/accept/14\"style=\"color:green;\">Accept Friend</a> or <a href=\"http://www.gamingfriends.gg/friends/reject/14\" style=\"color:red;\">Deny the request</a>'),
(23, 14, 2, 0, '2023-09-05 18:44:34', '2023-09-05 18:44:34', 'Click the link to accept the FriendRequest from Username<br><a href=\"http://www.gamingfriends.gg/friends/accept/14\"style=\"color:green;\">Accept Friend</a> or <a href=\"http://www.gamingfriends.gg/friends/reject/14\" style=\"color:red;\">Deny the request</a>'),
(24, 14, 3, 0, '2023-09-05 18:44:36', '2023-09-05 18:44:36', 'Click the link to accept the FriendRequest from Username<br><a href=\"http://www.gamingfriends.gg/friends/accept/14\"style=\"color:green;\">Accept Friend</a> or <a href=\"http://www.gamingfriends.gg/friends/reject/14\" style=\"color:red;\">Deny the request</a>'),
(25, 1, 2, 0, '2023-09-30 20:58:55', '2023-09-30 20:58:55', 'Click the link to accept the FriendRequest from anderstaurus<br><a href=\"http://gamingfriends.gg/friends/accept/1\"style=\"color:green;\">Accept Friend</a> or <a href=\"http://gamingfriends.gg/friends/reject/1\" style=\"color:red;\">Deny the request</a>');

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `gamegenre`
--

CREATE TABLE `gamegenre` (
  `id` bigint UNSIGNED NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `genrename` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `gamemode`
--

CREATE TABLE `gamemode` (
  `id` bigint UNSIGNED NOT NULL,
  `gamemode` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `gameprofiles`
--

CREATE TABLE `gameprofiles` (
  `id` int UNSIGNED NOT NULL,
  `user_id` int DEFAULT NULL,
  `playstation` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `nintendoswitch` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `xbox` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `steam` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `origin` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `battlenet` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `epicgames` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `riot` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `battlenetserver` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `riotserver_id` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `gameprofiles`
--

INSERT INTO `gameprofiles` (`id`, `user_id`, `playstation`, `nintendoswitch`, `xbox`, `steam`, `origin`, `battlenet`, `epicgames`, `riot`, `created_at`, `updated_at`, `deleted_at`, `battlenetserver`, `riotserver_id`) VALUES
(1, 1, NULL, NULL, 'min xbox bruger', NULL, NULL, NULL, NULL, NULL, '2023-04-10 00:54:38', '2023-06-23 11:52:10', NULL, NULL, NULL),
(2, 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2023-04-10 16:15:16', '2023-04-10 16:17:41', NULL, NULL, NULL),
(3, 4, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2023-05-25 21:09:22', '2023-05-25 21:09:22', NULL, 'US', 'BR1'),
(4, 5, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2023-06-18 00:34:28', '2023-06-18 00:34:28', NULL, 'US', 'BR1'),
(5, 11, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2023-08-24 15:40:00', '2023-08-24 15:40:00', NULL, 'US', 'BR1'),
(6, 12, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2023-09-02 13:06:41', '2023-09-02 13:06:41', NULL, 'US', 'BR1'),
(7, 13, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2023-09-02 15:06:06', '2023-09-02 15:06:06', NULL, 'US', 'BR1'),
(8, 3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2023-09-02 21:15:20', '2023-09-02 21:15:20', NULL, 'US', 'BR1'),
(9, 14, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2023-09-05 18:44:13', '2023-09-05 18:44:13', NULL, 'US', 'BR1');

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `games`
--

CREATE TABLE `games` (
  `id` int UNSIGNED NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `platform` int NOT NULL DEFAULT '0',
  `picture` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `max_players` int DEFAULT NULL,
  `gamecover` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `games`
--

INSERT INTO `games` (`id`, `name`, `platform`, `picture`, `max_players`, `gamecover`, `deleted_at`, `created_at`, `updated_at`) VALUES
(1, 'League of Legends', 0, 'LOL', NULL, NULL, NULL, NULL, NULL),
(2, 'CSGO', 0, 'CSGO', NULL, NULL, NULL, NULL, NULL),
(3, 'Fortnite', 0, 'FORTNITE', NULL, NULL, NULL, NULL, NULL),
(4, 'Rocket League', 0, 'ROCKETLEAGUE', NULL, NULL, NULL, NULL, NULL),
(5, 'Starcraft II', 0, 'SC2', NULL, NULL, NULL, NULL, NULL),
(6, 'Valorant', 0, 'VALORANT', NULL, NULL, NULL, NULL, NULL),
(7, 'Apex Legends', 0, 'APEX', NULL, NULL, NULL, NULL, NULL),
(8, 'Call of Duty: Warzone', 0, 'WARZONE', NULL, NULL, NULL, NULL, NULL),
(9, 'Minecraft', 0, 'MINECRAFT', NULL, NULL, NULL, NULL, NULL),
(10, 'GTA 5', 0, 'GTA5', NULL, NULL, NULL, NULL, NULL),
(11, 'PUBG', 0, 'PUBG', NULL, NULL, NULL, NULL, NULL),
(12, 'Dota 2', 0, 'DOTA2', NULL, NULL, NULL, NULL, NULL),
(13, 'Overwatch', 0, 'OVERWATCH', NULL, NULL, NULL, NULL, NULL),
(14, 'ARK: Survival Evolved', 0, 'ARK', NULL, NULL, NULL, NULL, NULL),
(15, 'Rust', 0, 'RUST', NULL, NULL, NULL, NULL, NULL),
(16, 'Rainbow Six: Siege', 0, 'RAINBOWSIXSIEGE', NULL, NULL, NULL, NULL, NULL),
(17, 'Hearthstone', 0, 'HEARTHSTONE', NULL, NULL, NULL, NULL, NULL),
(18, 'World of Warcraft', 0, 'WOW', NULL, NULL, NULL, NULL, NULL),
(19, 'Among Us', 0, 'AMONGUS', NULL, NULL, NULL, NULL, NULL),
(20, 'Roblox', 0, 'ROBLOX', NULL, NULL, NULL, NULL, NULL),
(21, 'Destiny 2', 0, 'DESTINY2', NULL, NULL, NULL, NULL, NULL),
(22, 'Elden Ring', 0, 'ELDENRING', NULL, NULL, NULL, NULL, NULL),
(23, 'Teamfight Tactics (TFT)', 0, 'TFT', NULL, NULL, NULL, NULL, NULL);

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `game_ranks`
--

CREATE TABLE `game_ranks` (
  `id` int UNSIGNED NOT NULL,
  `game_id` int NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `weight` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `game_ranks`
--

INSERT INTO `game_ranks` (`id`, `game_id`, `name`, `weight`) VALUES
(1, 1, 'iron', NULL),
(2, 1, 'bronze', NULL),
(3, 1, 'silver', NULL),
(4, 1, 'gold', NULL),
(5, 1, 'platinium', NULL),
(6, 1, 'diamond', NULL),
(7, 1, 'master', NULL),
(8, 1, 'grandmaster', NULL),
(9, 1, 'Mixed Ranks', NULL),
(10, 2, 'silver', NULL),
(11, 2, 'gold Nova', NULL),
(12, 2, 'Master Guardian', NULL),
(13, 2, 'Legendary Eagle', NULL),
(14, 2, 'Supreme Master', NULL),
(15, 2, 'Global Elite', NULL),
(16, 2, 'Mixed Ranks', NULL),
(17, 3, 'open', NULL),
(18, 3, 'contender', NULL),
(19, 3, 'champion', NULL),
(20, 4, 'unranked', NULL),
(21, 4, 'bronze', NULL),
(22, 4, 'silver', NULL),
(23, 4, 'golden', NULL),
(24, 4, 'platinum', NULL),
(25, 4, 'diamond1', NULL),
(26, 4, 'champion', NULL),
(27, 4, 'grand champion', NULL),
(28, 5, 'unranked', NULL),
(29, 5, 'bronze', NULL),
(30, 5, 'silver', NULL),
(31, 5, 'gold', NULL),
(32, 5, 'platinum', NULL),
(33, 5, 'diamond', NULL),
(34, 5, 'master', NULL),
(35, 5, 'grandmaster', NULL),
(36, 6, 'iron', NULL),
(37, 6, 'silver', NULL),
(38, 6, 'gold', NULL),
(39, 6, 'platinum', NULL),
(40, 6, 'diamond', NULL),
(41, 6, 'ascendant', NULL),
(42, 6, 'immortal', NULL),
(43, 6, 'radiant', NULL),
(44, 7, 'bronze', NULL),
(45, 7, 'silver', NULL),
(46, 7, 'gold', NULL),
(47, 7, 'platinum', NULL),
(48, 7, 'diamond', NULL),
(49, 7, 'master', NULL),
(50, 7, 'Apex Predator', NULL),
(51, 13, 'unranked', NULL),
(52, 13, 'bronze', NULL),
(53, 13, 'silver', NULL),
(54, 13, 'gold', NULL),
(55, 13, 'platinum', NULL),
(56, 13, 'diamond', NULL),
(57, 13, 'master', NULL),
(58, 13, 'grandmaster', NULL),
(59, 13, 'top 500', NULL);

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `gear`
--

CREATE TABLE `gear` (
  `id` bigint UNSIGNED NOT NULL,
  `user_id` int NOT NULL,
  `webcam` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `mousepad` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `mouse` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `microphone` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `keyboard` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `headset` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `monitor` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `case` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `harddrive` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `motherboard` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `memory` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `cpu` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `cpucooler` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `gpu` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `psu` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `laptop` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `coolingfans` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `misc` text COLLATE utf8mb4_unicode_ci
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `guides`
--

CREATE TABLE `guides` (
  `id` bigint UNSIGNED NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `difficulty` int NOT NULL,
  `headline` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `appetizer` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `platformid` int NOT NULL,
  `gameid` int NOT NULL,
  `archieved` int DEFAULT NULL,
  `content` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `picturepath` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `youtubelink` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `vimeolink` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `videopath` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `author` int DEFAULT NULL,
  `onair` int DEFAULT '0',
  `upvotes` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `guide_comment`
--

CREATE TABLE `guide_comment` (
  `id` bigint UNSIGNED NOT NULL,
  `user_id` int NOT NULL,
  `guide_id` int NOT NULL,
  `message` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `helpdesk`
--

CREATE TABLE `helpdesk` (
  `id` bigint UNSIGNED NOT NULL,
  `user_id` int NOT NULL,
  `category_id` int NOT NULL,
  `topic` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `issue` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `solved` tinyint NOT NULL,
  `operator` int NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `language`
--

CREATE TABLE `language` (
  `id` int UNSIGNED NOT NULL,
  `language` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `code` varchar(5) COLLATE utf8mb4_unicode_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `language`
--

INSERT INTO `language` (`id`, `language`, `code`) VALUES
(1, 'English', 'en'),
(2, 'Afar', 'aa'),
(3, 'Abkhazian', 'ab'),
(4, 'Afrikaans', 'af'),
(5, 'Amharic', 'am'),
(6, 'Arabic', 'ar'),
(7, 'Assamese', 'as'),
(8, 'Aymara', 'ay'),
(9, 'Azerbaijani', 'az'),
(10, 'Bashkir', 'ba'),
(11, 'Belarusian', 'be'),
(12, 'Bulgarian', 'bg'),
(13, 'Bihari', 'bh'),
(14, 'Bislama', 'bi'),
(15, 'Bengali/Bangla', 'bn'),
(16, 'Tibetan', 'bo'),
(17, 'Breton', 'br'),
(18, 'Catalan', 'ca'),
(19, 'Corsican', 'co'),
(20, 'Czech', 'cs'),
(21, 'Welsh', 'cy'),
(22, 'Danish', 'da'),
(23, 'German', 'de'),
(24, 'Bhutani', 'dz'),
(25, 'Greek', 'el'),
(26, 'Esperanto', 'eo'),
(27, 'Spanish', 'es'),
(28, 'Estonian', 'et'),
(29, 'Basque', 'eu'),
(30, 'Persian', 'fa'),
(31, 'Finnish', 'fi'),
(32, 'Fiji', 'fj'),
(33, 'Faeroese', 'fo'),
(34, 'French', 'fr'),
(35, 'Frisian', 'fy'),
(36, 'Irish', 'ga'),
(37, 'Scots/Gaelic', 'gd'),
(38, 'Galician', 'gl'),
(39, 'Guarani', 'gn'),
(40, 'Gujarati', 'gu'),
(41, 'Hausa', 'ha'),
(42, 'Hindi', 'hi'),
(43, 'Croatian', 'hr'),
(44, 'Hungarian', 'hu'),
(45, 'Armenian', 'hy'),
(46, 'Interlingua', 'ia'),
(47, 'Interlingue', 'ie'),
(48, 'Inupiak', 'ik'),
(49, 'Indonesian', 'in'),
(50, 'Icelandic', 'is'),
(51, 'Italian', 'it'),
(52, 'Hebrew', 'iw'),
(53, 'Japanese', 'ja'),
(54, 'Yiddish', 'ji'),
(55, 'Javanese', 'jw'),
(56, 'Georgian', 'ka'),
(57, 'Kazakh', 'kk'),
(58, 'Greenlandic', 'kl'),
(59, 'Cambodian', 'km'),
(60, 'Kannada', 'kn'),
(61, 'Korean', 'ko'),
(62, 'Kashmiri', 'ks'),
(63, 'Kurdish', 'ku'),
(64, 'Kirghiz', 'ky'),
(65, 'Latin', 'la'),
(66, 'Lingala', 'ln'),
(67, 'Laothian', 'lo'),
(68, 'Lithuanian', 'lt'),
(69, 'Latvian/Lettish', 'lv'),
(70, 'Malagasy', 'mg'),
(71, 'Maori', 'mi'),
(72, 'Macedonian', 'mk'),
(73, 'Malayalam', 'ml'),
(74, 'Mongolian', 'mn'),
(75, 'Moldavian', 'mo'),
(76, 'Marathi', 'mr'),
(77, 'Malay', 'ms'),
(78, 'Maltese', 'mt'),
(79, 'Burmese', 'my'),
(80, 'Nauru', 'na'),
(81, 'Nepali', 'ne'),
(82, 'Dutch', 'nl'),
(83, 'Norwegian', 'no'),
(84, 'Occitan', 'oc'),
(85, '(Afan)/Oromoor/Oriya', 'om'),
(86, 'Punjabi', 'pa'),
(87, 'Polish', 'pl'),
(88, 'Pashto/Pushto', 'ps'),
(89, 'Portuguese', 'pt'),
(90, 'Quechua', 'qu'),
(91, 'Rhaeto-Romance', 'rm'),
(92, 'Kirundi', 'rn'),
(93, 'Romanian', 'ro'),
(94, 'Russian', 'ru'),
(95, 'Kinyarwanda', 'rw'),
(96, 'Sanskrit', 'sa'),
(97, 'Sindhi', 'sd'),
(98, 'Sangro', 'sg'),
(99, 'Serbo-Croatian', 'sh'),
(100, 'Singhalese', 'si'),
(101, 'Slovak', 'sk'),
(102, 'Slovenian', 'sl'),
(103, 'Samoan', 'sm'),
(104, 'Shona', 'sn'),
(105, 'Somali', 'so'),
(106, 'Albanian', 'sq'),
(107, 'Serbian', 'sr'),
(108, 'Siswati', 'ss'),
(109, 'Sesotho', 'st'),
(110, 'Sundanese', 'su'),
(111, 'Swedish', 'sv'),
(112, 'Swahili', 'sw'),
(113, 'Tamil', 'ta'),
(114, 'Telugu', 'te'),
(115, 'Tajik', 'tg'),
(116, 'Thai', 'th'),
(117, 'Tigrinya', 'ti'),
(118, 'Turkmen', 'tk'),
(119, 'Tagalog', 'tl'),
(120, 'Setswana', 'tn'),
(121, 'Tonga', 'to'),
(122, 'Turkish', 'tr'),
(123, 'Tsonga', 'ts'),
(124, 'Tatar', 'tt'),
(125, 'Twi', 'tw'),
(126, 'Ukrainian', 'uk'),
(127, 'Urdu', 'ur'),
(128, 'Uzbek', 'uz'),
(129, 'Vietnamese', 'vi'),
(130, 'Volapuk', 'vo'),
(131, 'Wolof', 'wo'),
(132, 'Xhosa', 'xh'),
(133, 'Yoruba', 'yo'),
(134, 'Chinese', 'zh'),
(135, 'Zulu', 'zu');

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `leaguescore`
--

CREATE TABLE `leaguescore` (
  `id` bigint UNSIGNED NOT NULL,
  `result` int DEFAULT NULL,
  `league_id` int DEFAULT NULL,
  `teamleader_id` int NOT NULL,
  `team1` int DEFAULT NULL,
  `team2` int DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `leaguestats`
--

CREATE TABLE `leaguestats` (
  `id` bigint UNSIGNED NOT NULL,
  `game_id` int DEFAULT NULL,
  `season` int DEFAULT NULL,
  `team_id` int DEFAULT NULL,
  `teamleader_id` int DEFAULT NULL,
  `playedmatches` int DEFAULT NULL,
  `wins` int DEFAULT NULL,
  `losses` int DEFAULT NULL,
  `draw` int DEFAULT NULL,
  `division` int DEFAULT NULL,
  `position` int DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `lobbymember`
--

CREATE TABLE `lobbymember` (
  `id` int UNSIGNED NOT NULL,
  `game_id` int DEFAULT NULL,
  `lobby_id` int NOT NULL,
  `user_id` int NOT NULL,
  `accepted` tinyint NOT NULL DEFAULT '0',
  `deleted_at` timestamp NULL DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `lobbymember`
--

INSERT INTO `lobbymember` (`id`, `game_id`, `lobby_id`, `user_id`, `accepted`, `deleted_at`, `created_at`, `updated_at`) VALUES
(1, NULL, 1, 1, 1, NULL, '2023-07-07 15:04:16', '2023-07-07 15:04:16'),
(2, NULL, 2, 1, 1, NULL, '2023-07-07 15:04:27', '2023-07-07 15:04:27');

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `lobbywall`
--

CREATE TABLE `lobbywall` (
  `id` bigint UNSIGNED NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `lobby_id` int NOT NULL,
  `user_id` int NOT NULL,
  `message` text COLLATE utf8mb4_unicode_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `loldata`
--

CREATE TABLE `loldata` (
  `id` bigint UNSIGNED NOT NULL,
  `user_id` int NOT NULL,
  `summoner_name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `summoner_id` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `summoner_level` int NOT NULL,
  `queue_type` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `wins` int NOT NULL,
  `losses` int NOT NULL,
  `tier` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `rank` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `leaguepoints` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `lolranknames`
--

CREATE TABLE `lolranknames` (
  `id` int UNSIGNED NOT NULL,
  `rankname` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ranknumber` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `lookingfor`
--

CREATE TABLE `lookingfor` (
  `id` int UNSIGNED NOT NULL,
  `lookingfor` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `lookingfor`
--

INSERT INTO `lookingfor` (`id`, `lookingfor`, `created_at`, `updated_at`) VALUES
(1, 'Friend', NULL, NULL),
(2, 'Teammate', NULL, NULL),
(3, 'Team', NULL, NULL),
(4, 'Organization', NULL, NULL);

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `matchmod`
--

CREATE TABLE `matchmod` (
  `id` bigint UNSIGNED NOT NULL,
  `match_id` bigint UNSIGNED NOT NULL,
  `team_1` bigint UNSIGNED NOT NULL,
  `team_2` bigint UNSIGNED NOT NULL,
  `result` int DEFAULT NULL,
  `approved` int DEFAULT NULL,
  `tournamentmoderator_id` int DEFAULT NULL,
  `added_by` bigint UNSIGNED DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `match_entries`
--

CREATE TABLE `match_entries` (
  `id` bigint UNSIGNED NOT NULL,
  `tournament_id` bigint UNSIGNED NOT NULL,
  `team1_id` int UNSIGNED NOT NULL,
  `team2_id` int UNSIGNED NOT NULL,
  `winner_id` int UNSIGNED DEFAULT NULL,
  `round` int NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `match_results`
--

CREATE TABLE `match_results` (
  `id` bigint UNSIGNED NOT NULL,
  `tournament_id` bigint UNSIGNED NOT NULL,
  `match_id` bigint UNSIGNED NOT NULL,
  `submitted_by` int UNSIGNED NOT NULL,
  `winner_id` int DEFAULT NULL,
  `loser_id` bigint UNSIGNED NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `message`
--

CREATE TABLE `message` (
  `id` bigint UNSIGNED NOT NULL,
  `user_id` bigint UNSIGNED NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `conversation_id` bigint UNSIGNED DEFAULT NULL,
  `messagetxt` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `opened` int NOT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `topic` text COLLATE utf8mb4_unicode_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `migrations`
--

CREATE TABLE `migrations` (
  `id` int UNSIGNED NOT NULL,
  `migration` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `batch` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `migrations`
--

INSERT INTO `migrations` (`id`, `migration`, `batch`) VALUES
(1, '2014_10_12_000000_create_users_table', 1),
(2, '2014_10_12_100000_create_password_resets_table', 1),
(3, '2019_08_19_000000_create_failed_jobs_table', 1),
(4, '2019_10_09_145901_add_info_users', 1),
(5, '2019_10_13_141025_create_news_table', 1),
(6, '2019_10_21_182146_guides', 1),
(7, '2019_10_28_184653_teampage', 1),
(8, '2019_11_01_223602_profile', 1),
(9, '2019_11_02_151322_newscomment', 1),
(10, '2019_11_05_235800_social', 1),
(11, '2019_11_07_162755_message', 1),
(12, '2019_11_18_130510_wall', 1),
(13, '2019_11_20_132731_create_game_ranks_table', 1),
(14, '2019_11_20_140157_create_training_schedule_types_table', 1),
(15, '2019_11_20_184819_create_teams_table', 1),
(16, '2019_11_20_184842_create_team_members_table', 1),
(17, '2019_11_20_184859_create_games_table', 1),
(18, '2019_11_20_184928_create_team_roles_table', 1),
(19, '2019_11_20_190610_create_ambitions_table', 1),
(20, '2019_12_06_232759_riotapi', 1),
(21, '2019_12_07_015031_xboxlive', 1),
(22, '2019_12_09_183440_nintendoswitch', 1),
(23, '2019_12_09_223019_steam', 1),
(24, '2020_01_31_162401_create_friends_table', 1),
(25, '2020_01_31_173438_add_topic_to_message_table', 1),
(26, '2020_02_01_111234_create_team_wall_table', 1),
(27, '2020_02_13_130908_helpdesk', 1),
(28, '2020_02_19_143246_gear', 1),
(29, '2020_02_25_112213_guidecomment', 1),
(30, '2020_02_25_212521_add_ref_to_users_table', 1),
(31, '2020_02_26_092003_add_roles_to_users_table', 1),
(32, '2020_02_28_174622_loldata', 1),
(33, '2020_02_28_180309_create_loldata_table', 1),
(34, '2020_02_28_185030_create_api_user_data_table', 1),
(35, '2020_10_11_122107_create_social_logins_table', 1),
(36, '2020_12_16_144408_free_games', 1),
(37, '2021_07_16_210900_country', 1),
(38, '2021_07_17_185902_language', 1),
(39, '2021_07_17_213128_lookingfor', 1),
(40, '2021_10_16_165643_gameprofiles', 1),
(41, '2021_10_16_212758_extra_teampage', 1),
(42, '2022_01_12_175015_teamlobby', 1),
(43, '2022_01_15_170910_lobbywall', 1),
(44, '2022_01_16_110534_platform', 1),
(45, '2022_02_07_064454_add_softdelete_to_profile', 1),
(46, '2022_02_12_101337_add_softdelete_to_social', 1),
(47, '2022_02_12_203808_userlog', 1),
(48, '2022_02_28_210908_add_softdelete_andlinkpath_to_fregames', 1),
(49, '2022_03_10_111243_addgenretofreegames', 1),
(50, '2022_03_13_101439_gamegenre', 1),
(51, '2022_03_18_121044_adddescriptiontoteamlobby', 1),
(52, '2022_03_24_120514_addpointstoguide', 1),
(53, '2022_04_01_220532_comment', 1),
(54, '2022_04_06_105917_page', 1),
(55, '2022_04_17_125836_lobbymember', 1),
(56, '2022_04_23_081455_add_likes_to_comment', 1),
(57, '2022_05_06_114226_add_servers_to_gameprofiles', 1),
(58, '2022_05_07_233431_riotserverlist', 1),
(59, '2022_05_08_201657_battlenetserver', 1),
(60, '2022_05_13_152502_eventlist', 1),
(61, '2022_05_13_180453_riotlolrankdata', 1),
(62, '2022_05_18_141114_add_gamecovers', 1),
(63, '2022_05_20_211812_tournamentwall', 1),
(64, '2022_06_05_163230_add_extradate_games', 1),
(65, '2022_06_08_152006_apikeys', 1),
(66, '2022_07_01_183558_add_riotloldataid', 1),
(67, '2022_07_06_094743_add_gameidtoteamlobby', 1),
(68, '2022_07_07_112106_add_rolestosummonerlist', 1),
(69, '2022_07_08_125247_lolranknames', 1),
(70, '2022_07_08_170342_addusernamerequest', 1),
(71, '2022_07_09_114419_add_requestprofileimage', 1),
(72, '2022_07_11_184158_post', 1),
(73, '2022_07_19_182457_add_parentid', 1),
(74, '2022_07_30_202145_article', 1),
(75, '2022_08_06_162427_agelimit', 1),
(76, '2022_08_08_191810_addscrimoption', 1),
(77, '2022_08_12_102637_leaguestats', 1),
(78, '2022_08_22_010810_addlanguageandgameid', 1),
(79, '2022_08_25_173326_mygames', 1),
(80, '2022_08_26_154249_leaguescore', 1),
(81, '2022_09_06_175240_organization', 1),
(82, '2022_09_08_180902_community', 1),
(83, '2022_09_09_114515_addmisctogear', 1),
(84, '2022_09_12_191701_addeddiscordservertoorganization', 1),
(85, '2022_09_12_194654_iambraindeadandforgettimeandsoftdelete', 1),
(86, '2022_09_13_092105_gamemode', 1),
(87, '2022_09_15_154912_top10games', 1),
(88, '2022_09_19_105857_approvedlobby', 1),
(89, '2022_09_24_122024_addedcoactoteam', 1),
(90, '2022_09_25_181033_usernotification', 1),
(91, '2022_09_28_105441_visible', 1),
(92, '2023_03_12_210125_add_is_banned_to_users_table', 1),
(93, '2023_03_21_011532_create_chats_table', 1),
(94, '2023_03_21_233049_add_action_to_userlog_table', 1),
(95, '2023_04_06_070310_create_tournaments_table', 1),
(96, '2023_04_06_070325_create_tournament_team_table', 1),
(97, '2023_04_06_070338_create_matches_table', 2),
(98, '2023_04_06_070354_create_match_results_table', 2),
(99, '2023_04_06_070413_create_tournament_moderators_table', 2),
(100, '2023_04_08_150605_add_game_id_to_tournaments_table', 3),
(101, '2023_04_08_222332_create_team_tournament_table', 4),
(102, '2023_04_08_233159_create_tournament_team_table', 5),
(103, '2023_04_08_233627_createtournamentteamconnector', 6),
(104, '2023_04_16_232354_rename_matches_table', 7),
(105, '2023_04_18_104748_create_matchmod_table', 8),
(106, '2023_04_18_105357_create_match_mods_table', 9),
(107, '2023_04_19_102659_add_winner_id_to_match_results_table', 9),
(108, '2023_04_20_234104_add_added_by_column_to_matchmod_table', 10),
(109, '2023_04_21_021745_add_loser_id_to_match_results_table', 11),
(110, '2023_04_21_152639_drop_score_column_from_match_results_table', 12),
(111, '2023_04_21_170804_create_scoreboards_table', 13),
(112, '2023_04_22_190321_add_tournament_id_to_scoreboards_table', 14),
(113, '2023_05_08_164547_add_tournament_id_to_match_results_table', 15),
(114, '2023_05_10_121011_add_tournament_leader_and_softdelete_to_tournaments_table', 16),
(115, '2023_05_10_124121_create_changelogs_table', 17),
(116, '2023_05_17_004608_add_conversation_id_to_message_table', 18),
(117, '2023_05_17_005304_create_conversations_table', 19),
(118, '2023_05_18_201420_create_conversation_participants_table', 20),
(119, '2023_05_18_210327_add_owner_id_to_conversations', 21),
(120, '2023_05_25_110859_add_user_id_to_messages_table', 22),
(121, '2023_05_25_131021_remove_sender_receiver_from_message', 23),
(122, '2023_06_17_091715_create_likes_table', 24),
(123, '2023_06_17_122054_create_likes_table', 25),
(124, '2023_06_19_095232_create_friend_requests_table', 26),
(125, '2023_06_19_095720_add_messagetxt_to_friend_requests_table', 27),
(126, '2023_06_27_121133_create_system_requirements_table', 28),
(127, '2023_07_01_030826_create_user_xp_table', 29),
(128, '2023_07_17_201032_create_fingerprints_table', 30),
(129, '0000_00_00_000000_create_websockets_statistics_entries_table', 31),
(130, '0000_00_00_000000_rename_statistics_counters', 31),
(131, '2023_12_27_082023_create_quotes_table', 31);

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `mygames`
--

CREATE TABLE `mygames` (
  `id` bigint UNSIGNED NOT NULL,
  `user_id` int DEFAULT NULL,
  `platform` int DEFAULT NULL,
  `game_id` int DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `news`
--

CREATE TABLE `news` (
  `id` bigint UNSIGNED NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `headline` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `archieved` int DEFAULT NULL,
  `content` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `picturepath` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `youtubelink` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `mediascript` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `user_id` int DEFAULT NULL,
  `category` int NOT NULL,
  `appetizer` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `onair` int NOT NULL DEFAULT '0',
  `language_id` int DEFAULT NULL,
  `game_id` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `news`
--

INSERT INTO `news` (`id`, `created_at`, `updated_at`, `headline`, `archieved`, `content`, `picturepath`, `youtubelink`, `mediascript`, `user_id`, `category`, `appetizer`, `onair`, `language_id`, `game_id`) VALUES
(1, '2023-06-16 14:01:22', '2023-06-16 14:01:22', 'System Update', NULL, 'We have now opened for the guide section', NULL, NULL, NULL, 1, 3, 'We have now opened for the guide section\nwe have been looking forward to this feature for a while and now its finally here', 0, 1, 1);

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `news_comment`
--

CREATE TABLE `news_comment` (
  `id` bigint UNSIGNED NOT NULL,
  `user_id` int NOT NULL,
  `news_id` int NOT NULL,
  `message` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `nintendoswitch`
--

CREATE TABLE `nintendoswitch` (
  `id` bigint UNSIGNED NOT NULL,
  `user_id` int NOT NULL,
  `nintendoswitchid` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `favoritegame` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `organization`
--

CREATE TABLE `organization` (
  `id` bigint UNSIGNED NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `leader_id` int NOT NULL,
  `leaderphone` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `streetname` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `postalcode` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `cityname` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `country_id` int DEFAULT NULL,
  `website` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `cashier_id` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `logo` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `googlemaps` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `emailadress` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `about` text COLLATE utf8mb4_unicode_ci,
  `membershippayment` int DEFAULT NULL,
  `membershipperiod` int DEFAULT NULL,
  `suspended` int DEFAULT NULL,
  `discordserver` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `discordleader` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `page`
--

CREATE TABLE `page` (
  `id` bigint UNSIGNED NOT NULL,
  `pagename` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `onair` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `password_resets`
--

CREATE TABLE `password_resets` (
  `email` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `token` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `platform`
--

CREATE TABLE `platform` (
  `id` bigint UNSIGNED NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `platformname` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `platform`
--

INSERT INTO `platform` (`id`, `created_at`, `updated_at`, `platformname`) VALUES
(1, NULL, NULL, 'Pc'),
(2, NULL, NULL, 'Xbox'),
(3, NULL, NULL, 'Nintendo Switch'),
(4, NULL, NULL, 'Playstation'),
(5, NULL, NULL, 'Origin'),
(6, NULL, NULL, 'Battlenet');

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `post`
--

CREATE TABLE `post` (
  `id` int UNSIGNED NOT NULL,
  `user_id` int NOT NULL,
  `page_id` int DEFAULT NULL,
  `item_id` int DEFAULT NULL,
  `message` text COLLATE utf8mb4_unicode_ci,
  `picturepath` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `like` int DEFAULT NULL,
  `status` int DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `post`
--

INSERT INTO `post` (`id`, `user_id`, `page_id`, `item_id`, `message`, `picturepath`, `like`, `status`, `deleted_at`, `created_at`, `updated_at`) VALUES
(1, 1, 3, 1, 'test', NULL, NULL, NULL, NULL, '2023-06-17 14:43:42', '2023-06-17 14:43:42'),
(2, 1, 3, 2, 'test', NULL, NULL, NULL, NULL, '2023-06-17 23:24:53', '2023-06-17 23:24:53'),
(3, 1, 3, 1, 'post 2', NULL, NULL, NULL, NULL, '2023-06-17 23:32:25', '2023-06-17 23:32:25'),
(4, 1, 3, 1, 'test', 'images/post/20230622164647.jpg', NULL, NULL, NULL, '2023-06-22 16:46:47', '2023-06-22 16:46:47'),
(5, 1, 3, 1, 'test', NULL, NULL, NULL, NULL, '2023-11-12 12:49:37', '2023-11-12 12:49:37');

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `profile`
--

CREATE TABLE `profile` (
  `id` bigint UNSIGNED NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `user_id` bigint UNSIGNED NOT NULL,
  `picturepath` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `selectedtheme` int DEFAULT NULL,
  `description` text COLLATE utf8mb4_unicode_ci,
  `url` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `gender` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `birthday` date DEFAULT NULL,
  `ambitionlevel` int DEFAULT NULL,
  `favoritegame` int DEFAULT NULL,
  `country` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `primarylanguage` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `secondarylanguage` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `lookingfor` int DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `riotloldataid` int DEFAULT NULL,
  `requestprofileimagechange` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `quotes`
--

CREATE TABLE `quotes` (
  `id` bigint UNSIGNED NOT NULL,
  `text` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `author` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `quotes`
--

INSERT INTO `quotes` (`id`, `text`, `author`, `created_at`, `updated_at`) VALUES
(1, 'You\'re headed in the right direction when you realize the customer viewpoint is more important than the company viewpoint. It\'s more productive to learn from your customers instead of about them.', 'John Romero', NULL, NULL),
(2, 'Focus is a matter of deciding what things you are not gonna do', 'John Carmack', NULL, NULL),
(3, 'Focused hard work is the real key to success', 'John Carmack', NULL, NULL);

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `riotapi`
--

CREATE TABLE `riotapi` (
  `id` int UNSIGNED NOT NULL,
  `profileIconId` int DEFAULT NULL,
  `revisionDate` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `puuid` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `summoner_name` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `enc_summoner_id` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `server_id` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `summonerLevel` int DEFAULT NULL,
  `user_id` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `riotlolrankdata`
--

CREATE TABLE `riotlolrankdata` (
  `id` int UNSIGNED NOT NULL,
  `user_id` int DEFAULT NULL,
  `summonername` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `server` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `tier` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `rank` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `wins` int DEFAULT NULL,
  `losses` int DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `primarylane` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `secondarylane` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `riotserverlist`
--

CREATE TABLE `riotserverlist` (
  `id` int UNSIGNED NOT NULL,
  `server_alias` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `server_name` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `scoreboards`
--

CREATE TABLE `scoreboards` (
  `id` bigint UNSIGNED NOT NULL,
  `team_id` int NOT NULL,
  `position` int NOT NULL DEFAULT '0',
  `wins` int NOT NULL DEFAULT '0',
  `losses` int NOT NULL DEFAULT '0',
  `draws` int NOT NULL DEFAULT '0',
  `games_played` int NOT NULL DEFAULT '0',
  `points` int NOT NULL DEFAULT '0',
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `tournament_id` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `scoreboards`
--

INSERT INTO `scoreboards` (`id`, `team_id`, `position`, `wins`, `losses`, `draws`, `games_played`, `points`, `created_at`, `updated_at`, `tournament_id`) VALUES
(14, 41, 0, 1, 0, 0, 1, 3, '2023-05-13 12:16:13', '2023-06-23 11:44:14', 5),
(15, 42, 0, 0, 2, 0, 2, 0, '2023-05-13 12:16:13', '2023-06-23 11:44:14', 5),
(16, 43, 0, 1, 0, 0, 1, 3, '2023-05-13 12:16:13', '2023-06-23 11:44:14', 5);

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `social`
--

CREATE TABLE `social` (
  `id` bigint UNSIGNED NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `user_id` bigint UNSIGNED NOT NULL,
  `discord` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `twitch` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `youtube` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `facebook` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `twitter` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `instagram` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `social_logins`
--

CREATE TABLE `social_logins` (
  `user_id` int NOT NULL,
  `provider_user_id` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `provider` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `steam`
--

CREATE TABLE `steam` (
  `id` bigint UNSIGNED NOT NULL,
  `user_id` int NOT NULL,
  `Steamid` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `favoritegame` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `system_requirements`
--

CREATE TABLE `system_requirements` (
  `id` bigint UNSIGNED NOT NULL,
  `games_id` int NOT NULL,
  `requirement_category` int NOT NULL,
  `cpu` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `gpu` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `ram` int NOT NULL,
  `os` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `storage` int NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `system_requirements`
--

INSERT INTO `system_requirements` (`id`, `games_id`, `requirement_category`, `cpu`, `gpu`, `ram`, `os`, `storage`, `created_at`, `updated_at`) VALUES
(1, 1, 1, 'Intel Core i3 530\nAMD A6 3650', 'AMD Radeon HD 6570\nNvidia GeForce 9600 GT', 2, 'Windows 10 64-bit', 16, NULL, NULL),
(2, 1, 2, 'Intel Core i5 3300\nAMD Ryzen 3 1200', 'AMD Radeon HD 6950\nNvidia GeForce 560', 4, 'Windows 10 64-bit', 16, NULL, NULL);

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `teamlobby`
--

CREATE TABLE `teamlobby` (
  `id` bigint UNSIGNED NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `leader_id` int NOT NULL,
  `platform` int DEFAULT NULL,
  `gamename` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `seats` int DEFAULT NULL,
  `lobbycode` int DEFAULT NULL,
  `privatelobby` tinyint(1) DEFAULT NULL,
  `headset` int DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `description` text COLLATE utf8mb4_unicode_ci,
  `game_id` int DEFAULT NULL,
  `officiallobby` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `teamlobby`
--

INSERT INTO `teamlobby` (`id`, `name`, `leader_id`, `platform`, `gamename`, `seats`, `lobbycode`, `privatelobby`, `headset`, `created_at`, `updated_at`, `deleted_at`, `description`, `game_id`, `officiallobby`) VALUES
(1, 'Anders lol lobby', 1, 1, NULL, 2, NULL, NULL, 1, '2023-07-07 15:04:16', '2023-07-07 15:04:16', NULL, NULL, 1, NULL),
(2, 'Anders lol lobby', 1, 1, NULL, 2, NULL, NULL, 1, '2023-07-07 15:04:27', '2023-07-07 15:04:27', NULL, NULL, 1, NULL);

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `teampage`
--

CREATE TABLE `teampage` (
  `id` bigint UNSIGNED NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `user_id` int NOT NULL,
  `gameid` int NOT NULL,
  `slug` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `teamname` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `webadress` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `discord` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `about` text COLLATE utf8mb4_unicode_ci,
  `headset` int DEFAULT NULL,
  `weeklytraining` int DEFAULT NULL,
  `minimumage` int DEFAULT NULL,
  `activated` int NOT NULL DEFAULT '1',
  `minimumrank` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `teams`
--

CREATE TABLE `teams` (
  `id` int UNSIGNED NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `leader_id` int DEFAULT NULL,
  `description` text COLLATE utf8mb4_unicode_ci,
  `logo` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT 'assets/TeamAssets/teamicon.jpg',
  `game_id` int DEFAULT NULL,
  `ambitionlevel` int DEFAULT NULL,
  `website` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `discord` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `req_voice` tinyint NOT NULL DEFAULT '0',
  `minimum_age` int NOT NULL DEFAULT '0',
  `minimum_rank` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `training_schedule` int DEFAULT NULL,
  `language_id` int DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL,
  `platform` int DEFAULT NULL,
  `gamename` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `seats` int DEFAULT NULL,
  `lobbycode` int DEFAULT NULL,
  `privatelobby` tinyint(1) DEFAULT NULL,
  `slug` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `openforscrim` int DEFAULT NULL,
  `coach` int DEFAULT NULL,
  `standincoach` int DEFAULT NULL,
  `visible` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `team_members`
--

CREATE TABLE `team_members` (
  `id` int UNSIGNED NOT NULL,
  `game_id` int NOT NULL,
  `team_id` int NOT NULL,
  `user_id` int NOT NULL,
  `role_id` int DEFAULT NULL,
  `accepted` tinyint NOT NULL DEFAULT '0',
  `standin` tinyint NOT NULL DEFAULT '0',
  `deleted_at` timestamp NULL DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `team_roles`
--

CREATE TABLE `team_roles` (
  `id` int UNSIGNED NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `icon` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `game_id` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `team_roles`
--

INSERT INTO `team_roles` (`id`, `name`, `icon`, `game_id`) VALUES
(1, 'Top', 'assets/teams/lol/lol_top.png', 1),
(2, 'Mid', 'assets/teams/lol/lol_mid.png', 1),
(3, 'Jungle', 'assets/teams/lol/lol_jgl.png', 1),
(4, 'ADC', 'assets/teams/lol/lol_bot.png', 1),
(5, 'Support', 'assets/teams/lol/lol_sup.png', 1),
(6, 'Leader', 'assets/teams/default_role.png', 2),
(7, 'Sniper / AWP', 'assets/teams/default_role.png', 2),
(8, 'Rifler', 'assets/teams/default_role.png', 2),
(9, 'Support', 'assets/teams/default_role.png', 2),
(10, 'Fragger', 'assets/teams/default_role.png', 2),
(11, 'Lurker', 'assets/teams/default_role.png', 2),
(12, 'Defense', 'assets/teams/default_role.png', 13),
(13, 'Offense', 'assets/teams/default_role.png', 13),
(14, 'Support', 'assets/teams/default_role.png', 13),
(15, 'Tank', 'assets/teams/default_role.png', 13);

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `team_wall`
--

CREATE TABLE `team_wall` (
  `id` bigint UNSIGNED NOT NULL,
  `user_id` int NOT NULL,
  `team_id` int NOT NULL,
  `message` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `tournaments`
--

CREATE TABLE `tournaments` (
  `id` bigint UNSIGNED NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `game_id` int NOT NULL,
  `start_date` date NOT NULL,
  `end_date` date DEFAULT NULL,
  `status` int NOT NULL DEFAULT '0',
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `tournament_leader` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `tournaments`
--

INSERT INTO `tournaments` (`id`, `name`, `game_id`, `start_date`, `end_date`, `status`, `created_at`, `updated_at`, `tournament_leader`, `deleted_at`) VALUES
(5, 'LOLTurnering', 1, '2023-05-08', '2023-07-07', 0, '2023-05-07 00:17:45', '2023-05-07 00:17:45', '', NULL),
(6, 'Anders Odgaard', 1, '2023-04-09', '2023-06-09', 0, '2023-05-10 12:23:26', '2023-05-10 12:23:26', '1', NULL);

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `tournamentwall`
--

CREATE TABLE `tournamentwall` (
  `id` bigint UNSIGNED NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `tournament_id` int NOT NULL,
  `user_id` int NOT NULL,
  `message` text COLLATE utf8mb4_unicode_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `tournament_moderators`
--

CREATE TABLE `tournament_moderators` (
  `id` bigint UNSIGNED NOT NULL,
  `user_id` bigint UNSIGNED NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `tournament_team_connector`
--

CREATE TABLE `tournament_team_connector` (
  `id` bigint UNSIGNED NOT NULL,
  `team_id` int NOT NULL,
  `tournament_id` int NOT NULL,
  `status` int NOT NULL DEFAULT '1',
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `tournament_team_connector`
--

INSERT INTO `tournament_team_connector` (`id`, `team_id`, `tournament_id`, `status`, `created_at`, `updated_at`) VALUES
(6, 39, 5, 1, '2023-05-07 00:20:53', '2023-05-07 00:20:53'),
(8, 41, 5, 1, '2023-05-07 00:29:43', '2023-05-07 00:29:43'),
(9, 42, 5, 1, '2023-05-08 16:03:56', '2023-05-08 16:03:56'),
(10, 43, 5, 1, '2023-05-13 11:19:35', '2023-05-13 11:19:35');

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `training_schedule_types`
--

CREATE TABLE `training_schedule_types` (
  `id` int UNSIGNED NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `training_schedule_types`
--

INSERT INTO `training_schedule_types` (`id`, `name`) VALUES
(1, 'Daily'),
(2, 'Weekly'),
(3, 'Weekends only'),
(4, 'No training required');

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `userlog`
--

CREATE TABLE `userlog` (
  `id` bigint UNSIGNED NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `ipadress` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `email` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `user_agent` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `success` int NOT NULL,
  `action` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `userlog`
--

INSERT INTO `userlog` (`id`, `created_at`, `updated_at`, `ipadress`, `email`, `user_agent`, `success`, `action`) VALUES
(1, '2023-04-20 23:19:42', '2023-04-20 23:19:42', '192.168.1.1', 'isatanclawsi@gmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/112.0', 1, 'Logged in with success'),
(2, '2023-04-22 22:31:42', '2023-04-22 22:31:42', '80.209.75.103', 'isatanclawsi@gmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36', 1, 'Logged in with success'),
(3, '2023-05-07 00:27:56', '2023-05-07 00:27:56', '192.168.1.1', 'isatanclawsi@gmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36', 1, 'Logged in with success'),
(4, '2023-05-10 12:21:47', '2023-05-10 12:21:47', '192.168.1.1', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/112.0', 1, 'Logged in with success'),
(5, '2023-05-13 11:17:23', '2023-05-13 11:17:23', '78.31.255.108', 'kasper.faerch@gmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36', 0, 'Tried to login'),
(6, '2023-05-13 11:17:27', '2023-05-13 11:17:27', '78.31.255.108', 'kasper.faerch@gmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36', 0, 'Tried to login'),
(7, '2023-05-13 11:18:29', '2023-05-13 11:18:29', '78.31.255.108', 'kasper.faerch@gmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36', 1, 'Logged in with success'),
(8, '2023-06-01 07:46:32', '2023-06-01 07:46:32', '78.156.127.196', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36', 1, 'Logged in with success'),
(9, '2023-06-02 07:38:36', '2023-06-02 07:38:36', '80.62.117.32', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/113.0', 1, 'Logged in with success'),
(10, '2023-06-02 07:39:05', '2023-06-02 07:39:05', '80.62.117.32', 'isatanclawsi@gmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/113.0', 1, 'Logged in with success'),
(11, '2023-06-02 07:44:17', '2023-06-02 07:44:17', '80.62.117.32', 'isatanclawsi@gmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/113.0', 1, 'Logged in with success'),
(12, '2023-06-02 07:47:43', '2023-06-02 07:47:43', '80.62.117.32', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/113.0', 1, 'Logged in with success'),
(13, '2023-06-05 07:08:56', '2023-06-05 07:08:56', '80.62.117.32', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Linux; Android 11; BE2013) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Mobile Safari/537.36 EdgA/113.0.1774.50', 1, 'Logged in with success'),
(14, '2023-06-19 10:59:52', '2023-06-19 10:59:52', '192.168.1.1', 'isatanclawsi@gmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36', 1, 'Logged in with success'),
(15, '2023-06-20 23:52:12', '2023-06-20 23:52:12', '192.168.1.1', 'anders@gamingfriends.gg', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/114.0', 1, 'Logged in with success'),
(16, '2023-07-02 20:26:49', '2023-07-02 20:26:49', '78.31.255.108', 'kasper.faerch@gmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36', 1, 'Logged in with success'),
(17, '2023-07-08 19:45:06', '2023-07-08 19:45:06', '192.168.1.1', 'isatanclawsi@gmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/115.0', 1, 'Logged in with success'),
(18, '2023-07-21 10:55:53', '2023-07-21 10:55:54', '192.168.1.1', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/115.0', 1, 'Logged in with success'),
(19, '2023-09-01 10:42:52', '2023-09-01 10:42:52', '192.168.1.1', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/117.0', 1, 'Logged in with success'),
(20, '2023-09-01 11:09:13', '2023-09-01 11:09:13', '192.168.1.1', 'kasper.faerch@gmail.com', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36', 1, 'Logged in with success'),
(21, '2023-09-02 12:34:49', '2023-09-02 12:34:49', '192.168.1.1', 'kasper.faerch@gmail.com', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36', 1, 'Logged in with success'),
(22, '2023-09-02 12:45:32', '2023-09-02 12:45:33', '192.168.1.1', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/117.0', 1, 'Logged in with success'),
(23, '2023-09-02 13:24:02', '2023-09-02 13:24:02', '192.168.1.1', 'isatanclawsi@gmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36', 1, 'Logged in with success'),
(24, '2023-09-04 18:47:56', '2023-09-04 18:47:56', '78.31.255.108', 'kasper.faerch@gmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36', 1, 'Logged in with success'),
(25, '2023-09-05 09:59:12', '2023-09-05 09:59:12', '78.156.113.152', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36', 1, 'Logged in with success'),
(26, '2023-09-06 10:19:00', '2023-09-06 10:19:00', '91.133.34.224', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/117.0', 1, 'Logged in with success'),
(27, '2023-09-06 10:28:12', '2023-09-06 10:28:12', '91.133.34.224', 'anders@gamingfriends.gg', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36 Edg/116.0.1938.69', 0, 'Tried to login'),
(28, '2023-09-06 10:28:29', '2023-09-06 10:28:29', '91.133.34.224', 'anders@gamingfriends.gg', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36 Edg/116.0.1938.69', 0, 'Tried to login'),
(29, '2023-09-06 10:28:48', '2023-09-06 10:28:48', '91.133.34.224', 'anders@gamingfriends.gg', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36 Edg/116.0.1938.69', 0, 'Tried to login'),
(30, '2023-09-06 10:29:28', '2023-09-06 10:29:28', '91.133.34.224', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36 Edg/116.0.1938.69', 1, 'Logged in with success'),
(31, '2023-09-09 16:57:04', '2023-09-09 16:57:04', '80.209.75.103', 'hrme2007@gmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/117.0', 0, 'Tried to login'),
(32, '2023-09-09 16:57:18', '2023-09-09 16:57:18', '80.209.75.103', 'hrme2007@gmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/117.0', 0, 'Tried to login'),
(33, '2023-09-09 16:58:15', '2023-09-09 16:58:15', '80.209.75.103', 'hrme2007@gmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/117.0', 1, 'Logged in with success'),
(34, '2023-09-30 20:58:39', '2023-09-30 20:58:39', '192.168.1.1', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/118.0', 1, 'Logged in with success'),
(35, '2023-10-17 14:09:34', '2023-10-17 14:09:34', '62.199.21.63', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/118.0', 1, 'Logged in with success'),
(36, '2023-11-08 20:43:06', '2023-11-08 20:43:06', '192.168.1.1', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/119.0', 1, 'Logged in with success'),
(37, '2023-11-12 12:49:10', '2023-11-12 12:49:10', '91.133.34.224', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/119.0', 1, 'Logged in with success'),
(38, '2023-11-16 10:27:34', '2023-11-16 10:27:34', '192.168.1.1', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/119.0', 1, 'Logged in with success'),
(39, '2023-11-18 19:03:42', '2023-11-18 19:03:42', '192.168.1.1', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/119.0', 1, 'Logged in with success'),
(40, '2023-11-26 12:43:12', '2023-11-26 12:43:12', '78.31.255.108', 'kasper.faerch@gmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36', 1, 'Logged in with success'),
(41, '2023-12-03 14:27:09', '2023-12-03 14:27:09', '192.168.1.1', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:120.0) Gecko/20100101 Firefox/120.0', 1, 'Logged in with success'),
(42, '2023-12-16 04:28:24', '2023-12-16 04:28:24', '192.168.1.1', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:120.0) Gecko/20100101 Firefox/120.0', 1, 'Logged in with success'),
(43, '2023-12-26 20:45:23', '2023-12-26 20:45:23', '192.168.1.1', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:121.0) Gecko/20100101 Firefox/121.0', 0, 'Tried to login'),
(44, '2023-12-29 12:54:19', '2023-12-29 12:54:19', '192.168.1.1', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:121.0) Gecko/20100101 Firefox/121.0', 0, 'Tried to login'),
(45, '2023-12-29 20:17:01', '2023-12-29 20:17:01', '192.168.1.1', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:121.0) Gecko/20100101 Firefox/121.0', 1, 'Logged in with success'),
(46, '2024-01-01 17:05:56', '2024-01-01 17:05:56', '192.168.1.1', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:121.0) Gecko/20100101 Firefox/121.0', 1, 'Logged in with success'),
(47, '2024-01-04 11:52:35', '2024-01-04 11:52:35', '192.168.1.1', 'anders_taurus@hotmail.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:121.0) Gecko/20100101 Firefox/121.0', 1, 'Logged in with success');

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `usernotification`
--

CREATE TABLE `usernotification` (
  `id` bigint UNSIGNED NOT NULL,
  `user_id` int DEFAULT NULL,
  `action` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `status` int DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `usernotification`
--

INSERT INTO `usernotification` (`id`, `user_id`, `action`, `status`, `created_at`, `updated_at`, `deleted_at`) VALUES
(1, 1, 'Team Created', NULL, '2023-05-03 21:55:09', '2023-05-03 21:55:09', NULL),
(2, 1, 'Team Created', NULL, '2023-05-03 22:04:46', '2023-05-03 22:04:46', NULL),
(3, 1, 'Team Created', NULL, '2023-05-07 00:21:41', '2023-05-07 00:21:41', NULL),
(4, 2, 'Team Created', NULL, '2023-05-07 00:29:23', '2023-05-07 00:29:23', NULL),
(5, 1, 'Team Created', NULL, '2023-05-08 16:00:42', '2023-05-08 16:00:42', NULL),
(6, 3, 'Team Created', NULL, '2023-05-13 11:19:08', '2023-05-13 11:19:08', NULL),
(7, 1, 'Lobby Created', NULL, '2023-07-07 15:04:16', '2023-07-07 15:04:16', NULL),
(8, 1, 'Lobby Created', NULL, '2023-07-07 15:04:27', '2023-07-07 15:04:27', NULL),
(9, 1, 'Team Created', NULL, '2023-11-12 12:50:12', '2023-11-12 12:50:12', NULL),
(10, 1, 'Team Created', NULL, '2023-11-16 10:02:36', '2023-11-16 10:02:36', NULL);

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `users`
--

CREATE TABLE `users` (
  `id` bigint UNSIGNED NOT NULL,
  `email` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `email_verified_at` timestamp NULL DEFAULT NULL,
  `password` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `remember_token` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `username` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `slug` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `active` int NOT NULL DEFAULT '0',
  `deleted_at` timestamp NULL DEFAULT NULL,
  `referrer_id` bigint UNSIGNED DEFAULT NULL,
  `invitehash` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_admin` int DEFAULT NULL,
  `is_moderator` int DEFAULT NULL,
  `is_contentgenerator` int DEFAULT NULL,
  `is_blocked` int DEFAULT NULL,
  `username_request` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_banned` tinyint(1) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `users`
--

INSERT INTO `users` (`id`, `email`, `email_verified_at`, `password`, `remember_token`, `created_at`, `updated_at`, `username`, `slug`, `active`, `deleted_at`, `referrer_id`, `invitehash`, `is_admin`, `is_moderator`, `is_contentgenerator`, `is_blocked`, `username_request`, `is_banned`) VALUES
(3, 'kasper.faerch@gmail.com', '2023-05-13 11:18:19', '$2y$10$UN8ZFyaMUTNYiS8s6xG6lurNrlQC1L7Nd164IdWN1z4PKloDePpea', NULL, '2023-05-13 11:18:10', '2023-05-13 11:18:19', 'neo', 'neo', 0, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, 0),
(11, 'silverofdragonttv@gmail.com', '2023-08-24 15:38:47', '$2y$10$g4uf9O4VXtTJ5gEn7rsDXOzQpJA8/Q/z9IX35ZnJ2Yk0LoEeR1Op6', NULL, '2023-08-24 15:38:14', '2023-08-24 15:38:47', 'silverofdragon', 'silverofdragon', 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
(12, 'minimoller81@gmail.com', '2023-09-02 13:05:53', '$2y$10$wl99Pc5IY1guimjq5IqdsOE.VG9.ATZ813vtWZr5CceYepG8J5fGm', NULL, '2023-09-02 13:05:39', '2023-09-02 13:05:53', 'Shadowcrushers', 'Shadowcrushers', 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
(14, 'hrme2007@gmail.com', '2023-09-05 18:40:59', '$2y$10$CTV9mkq4FrCv1L1./qlWS.QCYkp.qIhfirWI9T1ghLKIeUEdJE3zG', '5ltUJMnxC33ro9UHlM4iu5wvNpLY52UWZ5brYdWzz2e8VGBdV7xetGcUwvZk', '2023-09-05 18:40:25', '2023-09-09 16:58:10', 'Username', 'Username', 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
(25, 'anders@gamingfriends.gg', '2023-12-29 13:46:03', '$2y$10$Rkj6hoygMsfISkdtHR7zbO/zdWKg572Nzg4aSMjsCP8s7cKgzfEai', NULL, '2023-12-29 13:45:34', '2023-12-29 13:46:03', 'a', NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0),
(26, 'anders_taurus@hotmail.com', '2023-12-29 20:17:01', '$2y$10$uRV.jRl12cV/umfE1Qkw..kibVKk7qeBFqapZUVa7qPcZMy.dz6S2', NULL, '2023-12-29 20:16:01', '2023-12-29 20:17:01', 'b', NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `user_xp`
--

CREATE TABLE `user_xp` (
  `id` bigint UNSIGNED NOT NULL,
  `user_id` int NOT NULL,
  `action` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `xp` int NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Data dump for tabellen `user_xp`
--

INSERT INTO `user_xp` (`id`, `user_id`, `action`, `xp`, `created_at`, `updated_at`, `deleted_at`) VALUES
(9, 23, 'Email Verification', 100, '2023-12-29 12:55:44', '2023-12-29 12:55:44', NULL),
(10, 24, 'Email Verification', 100, '2023-12-29 13:05:06', '2023-12-29 13:05:06', NULL),
(11, 25, 'Email Verification', 100, '2023-12-29 13:46:03', '2023-12-29 13:46:03', NULL),
(12, 26, 'Email Verification', 100, '2023-12-29 20:17:01', '2023-12-29 20:17:01', NULL);

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `wall`
--

CREATE TABLE `wall` (
  `id` bigint UNSIGNED NOT NULL,
  `user_id` int NOT NULL,
  `profile_id` int DEFAULT NULL,
  `message` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `deleted_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `websockets_statistics_entries`
--

CREATE TABLE `websockets_statistics_entries` (
  `id` int UNSIGNED NOT NULL,
  `app_id` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `peak_connections_count` int NOT NULL,
  `websocket_messages_count` int NOT NULL,
  `api_messages_count` int NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `xboxlive`
--

CREATE TABLE `xboxlive` (
  `id` int UNSIGNED NOT NULL,
  `user_id` int NOT NULL,
  `xboxliveid` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `favoritegame` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Begrænsninger for dumpede tabeller
--

--
-- Indeks for tabel `agelimit`
--
ALTER TABLE `agelimit`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `ambitions`
--
ALTER TABLE `ambitions`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `apikeys`
--
ALTER TABLE `apikeys`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `api_user_data`
--
ALTER TABLE `api_user_data`
  ADD PRIMARY KEY (`id`),
  ADD KEY `api_user_data_user_id_index` (`user_id`);

--
-- Indeks for tabel `article`
--
ALTER TABLE `article`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `battlenetserverlist`
--
ALTER TABLE `battlenetserverlist`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `changelogs`
--
ALTER TABLE `changelogs`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `chats`
--
ALTER TABLE `chats`
  ADD PRIMARY KEY (`id`),
  ADD KEY `chats_sender_id_foreign` (`sender_id`),
  ADD KEY `chats_receiver_id_foreign` (`receiver_id`);

--
-- Indeks for tabel `comment`
--
ALTER TABLE `comment`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `community`
--
ALTER TABLE `community`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `conversations`
--
ALTER TABLE `conversations`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `conversation_participants`
--
ALTER TABLE `conversation_participants`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `country`
--
ALTER TABLE `country`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `eventlist`
--
ALTER TABLE `eventlist`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `failed_jobs`
--
ALTER TABLE `failed_jobs`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `failed_jobs_uuid_unique` (`uuid`);

--
-- Indeks for tabel `fingerprints`
--
ALTER TABLE `fingerprints`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `fingerprints_fingerprint_unique` (`fingerprint`);

--
-- Indeks for tabel `freegames`
--
ALTER TABLE `freegames`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `friends`
--
ALTER TABLE `friends`
  ADD PRIMARY KEY (`id`),
  ADD KEY `friends_user_id_index` (`user_id`),
  ADD KEY `friends_friend_id_index` (`friend_id`),
  ADD KEY `friends_accepted_index` (`accepted`);

--
-- Indeks for tabel `friend_requests`
--
ALTER TABLE `friend_requests`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `gamegenre`
--
ALTER TABLE `gamegenre`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `gamemode`
--
ALTER TABLE `gamemode`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `gameprofiles`
--
ALTER TABLE `gameprofiles`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `games`
--
ALTER TABLE `games`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `game_ranks`
--
ALTER TABLE `game_ranks`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `gear`
--
ALTER TABLE `gear`
  ADD PRIMARY KEY (`id`),
  ADD KEY `gear_user_id_index` (`user_id`);

--
-- Indeks for tabel `guides`
--
ALTER TABLE `guides`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `guide_comment`
--
ALTER TABLE `guide_comment`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `helpdesk`
--
ALTER TABLE `helpdesk`
  ADD PRIMARY KEY (`id`),
  ADD KEY `helpdesk_user_id_index` (`user_id`);

--
-- Indeks for tabel `language`
--
ALTER TABLE `language`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `leaguescore`
--
ALTER TABLE `leaguescore`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `leaguestats`
--
ALTER TABLE `leaguestats`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `lobbymember`
--
ALTER TABLE `lobbymember`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `lobbywall`
--
ALTER TABLE `lobbywall`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `loldata`
--
ALTER TABLE `loldata`
  ADD PRIMARY KEY (`id`),
  ADD KEY `loldata_user_id_index` (`user_id`),
  ADD KEY `loldata_summoner_name_index` (`summoner_name`),
  ADD KEY `loldata_summoner_id_index` (`summoner_id`),
  ADD KEY `loldata_queue_type_index` (`queue_type`);

--
-- Indeks for tabel `lolranknames`
--
ALTER TABLE `lolranknames`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `lookingfor`
--
ALTER TABLE `lookingfor`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `matchmod`
--
ALTER TABLE `matchmod`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `match_entries`
--
ALTER TABLE `match_entries`
  ADD PRIMARY KEY (`id`),
  ADD KEY `matches_tournament_id_foreign` (`tournament_id`),
  ADD KEY `matches_team1_id_foreign` (`team1_id`),
  ADD KEY `matches_team2_id_foreign` (`team2_id`),
  ADD KEY `matches_winner_id_foreign` (`winner_id`);

--
-- Indeks for tabel `match_results`
--
ALTER TABLE `match_results`
  ADD PRIMARY KEY (`id`),
  ADD KEY `match_results_match_id_foreign` (`match_id`),
  ADD KEY `match_results_submitted_by_foreign` (`submitted_by`);

--
-- Indeks for tabel `message`
--
ALTER TABLE `message`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `migrations`
--
ALTER TABLE `migrations`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `mygames`
--
ALTER TABLE `mygames`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `news`
--
ALTER TABLE `news`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `news_comment`
--
ALTER TABLE `news_comment`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `nintendoswitch`
--
ALTER TABLE `nintendoswitch`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `nintendoswitch_nintendoswitchid_unique` (`nintendoswitchid`);

--
-- Indeks for tabel `organization`
--
ALTER TABLE `organization`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `page`
--
ALTER TABLE `page`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `password_resets`
--
ALTER TABLE `password_resets`
  ADD KEY `password_resets_email_index` (`email`);

--
-- Indeks for tabel `platform`
--
ALTER TABLE `platform`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `post`
--
ALTER TABLE `post`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `profile`
--
ALTER TABLE `profile`
  ADD PRIMARY KEY (`id`),
  ADD KEY `profile_user_id_index` (`user_id`);

--
-- Indeks for tabel `quotes`
--
ALTER TABLE `quotes`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `riotapi`
--
ALTER TABLE `riotapi`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `riotlolrankdata`
--
ALTER TABLE `riotlolrankdata`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `riotserverlist`
--
ALTER TABLE `riotserverlist`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `scoreboards`
--
ALTER TABLE `scoreboards`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `social`
--
ALTER TABLE `social`
  ADD PRIMARY KEY (`id`),
  ADD KEY `social_user_id_index` (`user_id`);

--
-- Indeks for tabel `steam`
--
ALTER TABLE `steam`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `steam_steamid_unique` (`Steamid`);

--
-- Indeks for tabel `system_requirements`
--
ALTER TABLE `system_requirements`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `teamlobby`
--
ALTER TABLE `teamlobby`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `teampage`
--
ALTER TABLE `teampage`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `teams`
--
ALTER TABLE `teams`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `teams_name_unique` (`name`);

--
-- Indeks for tabel `team_members`
--
ALTER TABLE `team_members`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `team_roles`
--
ALTER TABLE `team_roles`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `team_wall`
--
ALTER TABLE `team_wall`
  ADD PRIMARY KEY (`id`),
  ADD KEY `team_wall_user_id_index` (`user_id`),
  ADD KEY `team_wall_team_id_index` (`team_id`);

--
-- Indeks for tabel `tournaments`
--
ALTER TABLE `tournaments`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `tournamentwall`
--
ALTER TABLE `tournamentwall`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `tournament_moderators`
--
ALTER TABLE `tournament_moderators`
  ADD PRIMARY KEY (`id`),
  ADD KEY `tournament_moderators_user_id_foreign` (`user_id`);

--
-- Indeks for tabel `tournament_team_connector`
--
ALTER TABLE `tournament_team_connector`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `training_schedule_types`
--
ALTER TABLE `training_schedule_types`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `userlog`
--
ALTER TABLE `userlog`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `usernotification`
--
ALTER TABLE `usernotification`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `users_email_unique` (`email`),
  ADD UNIQUE KEY `users_username_unique` (`username`),
  ADD KEY `users_referrer_id_foreign` (`referrer_id`);

--
-- Indeks for tabel `user_xp`
--
ALTER TABLE `user_xp`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `wall`
--
ALTER TABLE `wall`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `websockets_statistics_entries`
--
ALTER TABLE `websockets_statistics_entries`
  ADD PRIMARY KEY (`id`);

--
-- Indeks for tabel `xboxlive`
--
ALTER TABLE `xboxlive`
  ADD PRIMARY KEY (`id`);

--
-- Brug ikke AUTO_INCREMENT for slettede tabeller
--

--
-- Tilføj AUTO_INCREMENT i tabel `agelimit`
--
ALTER TABLE `agelimit`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- Tilføj AUTO_INCREMENT i tabel `ambitions`
--
ALTER TABLE `ambitions`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- Tilføj AUTO_INCREMENT i tabel `apikeys`
--
ALTER TABLE `apikeys`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `api_user_data`
--
ALTER TABLE `api_user_data`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `article`
--
ALTER TABLE `article`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `battlenetserverlist`
--
ALTER TABLE `battlenetserverlist`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `changelogs`
--
ALTER TABLE `changelogs`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=22;

--
-- Tilføj AUTO_INCREMENT i tabel `chats`
--
ALTER TABLE `chats`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `comment`
--
ALTER TABLE `comment`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- Tilføj AUTO_INCREMENT i tabel `community`
--
ALTER TABLE `community`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `conversations`
--
ALTER TABLE `conversations`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- Tilføj AUTO_INCREMENT i tabel `conversation_participants`
--
ALTER TABLE `conversation_participants`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- Tilføj AUTO_INCREMENT i tabel `country`
--
ALTER TABLE `country`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=243;

--
-- Tilføj AUTO_INCREMENT i tabel `eventlist`
--
ALTER TABLE `eventlist`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `failed_jobs`
--
ALTER TABLE `failed_jobs`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `fingerprints`
--
ALTER TABLE `fingerprints`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- Tilføj AUTO_INCREMENT i tabel `freegames`
--
ALTER TABLE `freegames`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `friends`
--
ALTER TABLE `friends`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=63;

--
-- Tilføj AUTO_INCREMENT i tabel `friend_requests`
--
ALTER TABLE `friend_requests`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- Tilføj AUTO_INCREMENT i tabel `gamegenre`
--
ALTER TABLE `gamegenre`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `gamemode`
--
ALTER TABLE `gamemode`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `gameprofiles`
--
ALTER TABLE `gameprofiles`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- Tilføj AUTO_INCREMENT i tabel `games`
--
ALTER TABLE `games`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=24;

--
-- Tilføj AUTO_INCREMENT i tabel `game_ranks`
--
ALTER TABLE `game_ranks`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=60;

--
-- Tilføj AUTO_INCREMENT i tabel `gear`
--
ALTER TABLE `gear`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- Tilføj AUTO_INCREMENT i tabel `guides`
--
ALTER TABLE `guides`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=29;

--
-- Tilføj AUTO_INCREMENT i tabel `guide_comment`
--
ALTER TABLE `guide_comment`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `helpdesk`
--
ALTER TABLE `helpdesk`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `language`
--
ALTER TABLE `language`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=136;

--
-- Tilføj AUTO_INCREMENT i tabel `leaguescore`
--
ALTER TABLE `leaguescore`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `leaguestats`
--
ALTER TABLE `leaguestats`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `lobbymember`
--
ALTER TABLE `lobbymember`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- Tilføj AUTO_INCREMENT i tabel `lobbywall`
--
ALTER TABLE `lobbywall`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `loldata`
--
ALTER TABLE `loldata`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `lolranknames`
--
ALTER TABLE `lolranknames`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `lookingfor`
--
ALTER TABLE `lookingfor`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- Tilføj AUTO_INCREMENT i tabel `matchmod`
--
ALTER TABLE `matchmod`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=54;

--
-- Tilføj AUTO_INCREMENT i tabel `match_entries`
--
ALTER TABLE `match_entries`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=20;

--
-- Tilføj AUTO_INCREMENT i tabel `match_results`
--
ALTER TABLE `match_results`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=46;

--
-- Tilføj AUTO_INCREMENT i tabel `message`
--
ALTER TABLE `message`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=233;

--
-- Tilføj AUTO_INCREMENT i tabel `migrations`
--
ALTER TABLE `migrations`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=132;

--
-- Tilføj AUTO_INCREMENT i tabel `mygames`
--
ALTER TABLE `mygames`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `news`
--
ALTER TABLE `news`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- Tilføj AUTO_INCREMENT i tabel `news_comment`
--
ALTER TABLE `news_comment`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `nintendoswitch`
--
ALTER TABLE `nintendoswitch`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `organization`
--
ALTER TABLE `organization`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `page`
--
ALTER TABLE `page`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `platform`
--
ALTER TABLE `platform`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- Tilføj AUTO_INCREMENT i tabel `post`
--
ALTER TABLE `post`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- Tilføj AUTO_INCREMENT i tabel `profile`
--
ALTER TABLE `profile`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=18;

--
-- Tilføj AUTO_INCREMENT i tabel `quotes`
--
ALTER TABLE `quotes`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- Tilføj AUTO_INCREMENT i tabel `riotapi`
--
ALTER TABLE `riotapi`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `riotlolrankdata`
--
ALTER TABLE `riotlolrankdata`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `riotserverlist`
--
ALTER TABLE `riotserverlist`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `scoreboards`
--
ALTER TABLE `scoreboards`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- Tilføj AUTO_INCREMENT i tabel `social`
--
ALTER TABLE `social`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- Tilføj AUTO_INCREMENT i tabel `steam`
--
ALTER TABLE `steam`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `system_requirements`
--
ALTER TABLE `system_requirements`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- Tilføj AUTO_INCREMENT i tabel `teamlobby`
--
ALTER TABLE `teamlobby`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- Tilføj AUTO_INCREMENT i tabel `teampage`
--
ALTER TABLE `teampage`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `teams`
--
ALTER TABLE `teams`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=46;

--
-- Tilføj AUTO_INCREMENT i tabel `team_members`
--
ALTER TABLE `team_members`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- Tilføj AUTO_INCREMENT i tabel `team_roles`
--
ALTER TABLE `team_roles`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- Tilføj AUTO_INCREMENT i tabel `team_wall`
--
ALTER TABLE `team_wall`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `tournaments`
--
ALTER TABLE `tournaments`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- Tilføj AUTO_INCREMENT i tabel `tournamentwall`
--
ALTER TABLE `tournamentwall`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `tournament_moderators`
--
ALTER TABLE `tournament_moderators`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `tournament_team_connector`
--
ALTER TABLE `tournament_team_connector`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- Tilføj AUTO_INCREMENT i tabel `training_schedule_types`
--
ALTER TABLE `training_schedule_types`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- Tilføj AUTO_INCREMENT i tabel `userlog`
--
ALTER TABLE `userlog`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=48;

--
-- Tilføj AUTO_INCREMENT i tabel `usernotification`
--
ALTER TABLE `usernotification`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- Tilføj AUTO_INCREMENT i tabel `users`
--
ALTER TABLE `users`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=27;

--
-- Tilføj AUTO_INCREMENT i tabel `user_xp`
--
ALTER TABLE `user_xp`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- Tilføj AUTO_INCREMENT i tabel `wall`
--
ALTER TABLE `wall`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `websockets_statistics_entries`
--
ALTER TABLE `websockets_statistics_entries`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Tilføj AUTO_INCREMENT i tabel `xboxlive`
--
ALTER TABLE `xboxlive`
  MODIFY `id` int UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Begrænsninger for dumpede tabeller
--

--
-- Begrænsninger for tabel `chats`
--
ALTER TABLE `chats`
  ADD CONSTRAINT `chats_receiver_id_foreign` FOREIGN KEY (`receiver_id`) REFERENCES `users` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `chats_sender_id_foreign` FOREIGN KEY (`sender_id`) REFERENCES `users` (`id`) ON DELETE CASCADE;

--
-- Begrænsninger for tabel `match_entries`
--
ALTER TABLE `match_entries`
  ADD CONSTRAINT `matches_team1_id_foreign` FOREIGN KEY (`team1_id`) REFERENCES `teams` (`id`),
  ADD CONSTRAINT `matches_team2_id_foreign` FOREIGN KEY (`team2_id`) REFERENCES `teams` (`id`),
  ADD CONSTRAINT `matches_tournament_id_foreign` FOREIGN KEY (`tournament_id`) REFERENCES `tournaments` (`id`),
  ADD CONSTRAINT `matches_winner_id_foreign` FOREIGN KEY (`winner_id`) REFERENCES `teams` (`id`);

--
-- Begrænsninger for tabel `match_results`
--
ALTER TABLE `match_results`
  ADD CONSTRAINT `match_results_match_id_foreign` FOREIGN KEY (`match_id`) REFERENCES `match_entries` (`id`),
  ADD CONSTRAINT `match_results_submitted_by_foreign` FOREIGN KEY (`submitted_by`) REFERENCES `teams` (`id`);

--
-- Begrænsninger for tabel `tournament_moderators`
--
ALTER TABLE `tournament_moderators`
  ADD CONSTRAINT `tournament_moderators_user_id_foreign` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`);

--
-- Begrænsninger for tabel `users`
--
ALTER TABLE `users`
  ADD CONSTRAINT `users_referrer_id_foreign` FOREIGN KEY (`referrer_id`) REFERENCES `users` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
