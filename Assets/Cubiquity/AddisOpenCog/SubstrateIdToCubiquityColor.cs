using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace OCCubiquity
{
    /// <summary>
    /// this class uses to handle every x,y,z co-ordinates of a block and its color id.
    /// the id is handy for getting block color. 
    /// </summary>

    [Serializable]
    public class Point
    {
        public int x;
        public int y;
        public int z;
        public int id;
    }   

    public class SubstrateIdToCubiquityColor:CBScriptableObject
    {
        [SerializeField]
        private static CBScriptableObject cbObject;
        public static CBScriptableObject CBObject
        {
            get { return cbObject; }
        }
        /// <summary>
        /// maps substrate block id to Cubiquity Colors. 
        /// </summary>
        /// <param name="id"> </param>
        /// <returns></returns>
        public static void SetColor()
        {
            cbObject = ScriptableObject.CreateInstance<CBScriptableObject>();
            cbObject._cbColorType = new SortedDictionary<int, Color32>();
            cbObject._cbColors = new List<Color32>();
            cbObject._cbColorIds = new List<int>();
            cbObject._cb3DpointsPlusId = new List<Point>();
            cbObject._cbColorType.Add(1, new Color32(121, 124, 121, 134));
            cbObject._cbColorType.Add(2, new Color32(49, 164, 42, 134));
            cbObject._cbColorType.Add(3, new Color32(106, 76, 50, 109));
            cbObject._cbColorType.Add(4, new Color32(120, 124, 121, 134));
            cbObject._cbColorType.Add(5, new Color32(163, 139, 79, 109));
            cbObject._cbColorType.Add(6, new Color32(98, 141, 57, 134));
            cbObject._cbColorType.Add(7, new Color32(50, 51, 48, 134));
            cbObject._cbColorType.Add(8, new Color32(50, 94, 253, 134));
            cbObject._cbColorType.Add(9, new Color32(54, 94, 253, 134));
            cbObject._cbColorType.Add(10, new Color32(253, 247, 115, 134));
            cbObject._cbColorType.Add(11, new Color32(251, 247, 115, 134));
            cbObject._cbColorType.Add(12, new Color32(214, 208, 161, 134));
            cbObject._cbColorType.Add(13, new Color32(124, 124, 120, 134));
            cbObject._cbColorType.Add(14, new Color32(112, 112, 112, 134));
            cbObject._cbColorType.Add(15, new Color32(110, 112, 111, 134));
            cbObject._cbColorType.Add(16, new Color32(116, 118, 116, 134));
            cbObject._cbColorType.Add(17, new Color32(158, 127, 74, 109));
            cbObject._cbColorType.Add(18, new Color32(65, 161, 17, 134));//
            cbObject._cbColorType.Add(19, new Color32(251, 208, 66, 134));
            cbObject._cbColorType.Add(20, new Color32(158, 159, 156, 134));
            cbObject._cbColorType.Add(21, new Color32(110, 112, 117, 134));
            cbObject._cbColorType.Add(22, new Color32(28, 72, 181, 134));
            cbObject._cbColorType.Add(23, new Color32(115, 114, 116, 114));
            cbObject._cbColorType.Add(24, new Color32(213, 207, 163, 134));
            cbObject._cbColorType.Add(25, new Color32(100, 59, 39, 109));
            cbObject._cbColorType.Add(26, new Color32(137, 20, 21, 109));
            cbObject._cbColorType.Add(27, new Color32(222, 84, 8, 134));//
            cbObject._cbColorType.Add(28, new Color32(118, 94, 57, 109));
            cbObject._cbColorType.Add(29, new Color32(121, 112, 70, 134));//
            cbObject._cbColorType.Add(30, new Color32(231, 236, 234, 134));
            cbObject._cbColorType.Add(31, new Color32(43, 119, 17, 134));//
            cbObject._cbColorType.Add(32, new Color32(143, 96, 39, 109));
            cbObject._cbColorType.Add(33, new Color32(118, 94, 75, 109));
            cbObject._cbColorType.Add(34, new Color32(166, 131, 82, 109));
            cbObject._cbColorType.Add(35, new Color32(172, 68, 180, 134));
            cbObject._cbColorType.Add(36, new Color32(154, 158, 156, 134));
            cbObject._cbColorType.Add(37, new Color32(245, 238, 56, 134));
            cbObject._cbColorType.Add(38, new Color32(202, 7, 14, 109));
            cbObject._cbColorType.Add(39, new Color32(133, 109, 83, 109));
            cbObject._cbColorType.Add(40, new Color32(202, 13, 15, 109));
            cbObject._cbColorType.Add(41, new Color32(250, 244, 85, 134));
            cbObject._cbColorType.Add(42, new Color32(145, 149, 147, 134));
            cbObject._cbColorType.Add(43, new Color32(97, 255, 248, 134));
            cbObject._cbColorType.Add(44, new Color32(96, 255, 248, 134));
            cbObject._cbColorType.Add(45, new Color32(149, 84, 65, 109));
            cbObject._cbColorType.Add(46, new Color32(197, 58, 19, 109));
            cbObject._cbColorType.Add(47, new Color32(171, 137, 84, 109));
            cbObject._cbColorType.Add(48, new Color32(79, 87, 54, 134));
            cbObject._cbColorType.Add(49, new Color32(11, 17, 22, 134));
            cbObject._cbColorType.Add(50, new Color32(150, 121, 70, 109));
            cbObject._cbColorType.Add(51, new Color32(217, 119, 25, 134));
            cbObject._cbColorType.Add(52, new Color32(22, 48, 60, 134));
            cbObject._cbColorType.Add(53, new Color32(163, 86, 0, 109));
            cbObject._cbColorType.Add(54, new Color32(128, 119, 45, 109));
            cbObject._cbColorType.Add(55, new Color32(246, 251, 246, 134));
            cbObject._cbColorType.Add(56, new Color32(78, 79, 77, 134));
            cbObject._cbColorType.Add(57, new Color32(142, 229, 224, 134));
            cbObject._cbColorType.Add(58, new Color32(177, 119, 78, 109));
            cbObject._cbColorType.Add(59, new Color32(2, 196, 19, 134));
            //
            cbObject._cbColorType.Add(60, new Color32(101, 59, 39, 109));
            cbObject._cbColorType.Add(61, new Color32(87, 87, 84, 134));
            cbObject._cbColorType.Add(62, new Color32(87, 87, 87, 134));
            cbObject._cbColorType.Add(63, new Color32(157, 127, 74, 109));
            cbObject._cbColorType.Add(64, new Color32(141, 108, 53, 109));
            cbObject._cbColorType.Add(65, new Color32(141, 108, 60, 109));
            cbObject._cbColorType.Add(66, new Color32(141, 108, 53, 109));
            cbObject._cbColorType.Add(67, new Color32(79, 79, 77, 134));
            cbObject._cbColorType.Add(68, new Color32(177, 139, 87, 109));
            cbObject._cbColorType.Add(69, new Color32(103, 81, 46, 109));
            cbObject._cbColorType.Add(70, new Color32(121, 122, 123, 134));
            cbObject._cbColorType.Add(71, new Color32(174, 175, 173, 134));
            cbObject._cbColorType.Add(72, new Color32(158, 128, 74, 109));
            cbObject._cbColorType.Add(73, new Color32(110, 115, 113, 134));
            cbObject._cbColorType.Add(74, new Color32(110, 110, 109, 134));
            cbObject._cbColorType.Add(75, new Color32(157, 112, 70, 109));
            cbObject._cbColorType.Add(76, new Color32(249, 9, 17, 109));
            cbObject._cbColorType.Add(77, new Color32(119, 124, 121, 134));
            cbObject._cbColorType.Add(78, new Color32(245, 251, 246, 134));
            cbObject._cbColorType.Add(79, new Color32(66, 101, 157, 134));
            cbObject._cbColorType.Add(80, new Color32(246, 251, 248, 134));
            cbObject._cbColorType.Add(81, new Color32(43, 119, 23, 134));//
            cbObject._cbColorType.Add(82, new Color32(154, 161, 167, 134));
            cbObject._cbColorType.Add(83, new Color32(145, 181, 114, 134));
            cbObject._cbColorType.Add(84, new Color32(149, 84, 75, 109));
            cbObject._cbColorType.Add(85, new Color32(103, 81, 46, 109));
            cbObject._cbColorType.Add(86, new Color32(213, 134, 21, 134));
            cbObject._cbColorType.Add(87, new Color32(100, 59, 65, 109));
            cbObject._cbColorType.Add(88, new Color32(115, 87, 66, 109));
            cbObject._cbColorType.Add(89, new Color32(134, 104, 43, 134));
            cbObject._cbColorType.Add(90, new Color32(84, 5, 191, 134));
            cbObject._cbColorType.Add(91, new Color32(213, 134, 21, 134));
            cbObject._cbColorType.Add(92, new Color32(228, 231, 230, 134));
            cbObject._cbColorType.Add(93, new Color32(174, 175, 173, 134));
            cbObject._cbColorType.Add(94, new Color32(174, 170, 173, 134));
            cbObject._cbColorType.Add(95, new Color32(78, 218, 116, 134));
            cbObject._cbColorType.Add(96, new Color32(123, 90, 40, 109));
            cbObject._cbColorType.Add(97, new Color32(130, 132, 129, 134));
            cbObject._cbColorType.Add(98, new Color32(120, 123, 121, 134));
            cbObject._cbColorType.Add(99, new Color32(210, 177, 128, 134));
            cbObject._cbColorType.Add(100, new Color32(210, 177, 128, 134));
            cbObject._cbColorType.Add(101, new Color32(110, 112, 113, 134));
            cbObject._cbColorType.Add(102, new Color32(244, 251, 246, 134));
            cbObject._cbColorType.Add(103, new Color32(157, 153, 34, 134));
            cbObject._cbColorType.Add(104, new Color32(174, 175, 173, 134));
            cbObject._cbColorType.Add(105, new Color32(174, 175, 173, 134));
            cbObject._cbColorType.Add(106, new Color32(27, 76, 20, 134));//
            cbObject._cbColorType.Add(107, new Color32(117, 94, 57, 109));
            cbObject._cbColorType.Add(108, new Color32(115, 61, 49, 109));
            cbObject._cbColorType.Add(109, new Color32(131, 133, 130, 134));
            cbObject._cbColorType.Add(110, new Color32(116, 110, 112, 134));
            cbObject._cbColorType.Add(111, new Color32(27, 76, 20, 134));//
            cbObject._cbColorType.Add(112, new Color32(49, 22, 27, 109));
            cbObject._cbColorType.Add(113, new Color32(49, 22, 27, 109));
            cbObject._cbColorType.Add(114, new Color32(49, 22, 27, 109));
            cbObject._cbColorType.Add(115, new Color32(100, 28, 31, 109));
            cbObject._cbColorType.Add(116, new Color32(172, 50, 51, 109));
            cbObject._cbColorType.Add(117, new Color32(96, 97, 95, 134));//
            cbObject._cbColorType.Add(118, new Color32(63, 63, 61, 134));
            cbObject._cbColorType.Add(119, new Color32(146, 149, 147, 134));
            cbObject._cbColorType.Add(120, new Color32(72, 142, 139, 134));//
            cbObject._cbColorType.Add(121, new Color32(225, 224, 169, 134));
            cbObject._cbColorType.Add(122, new Color32(49, 22, 27, 109));
            cbObject._cbColorType.Add(123, new Color32(106, 50, 21, 109));
            cbObject._cbColorType.Add(124, new Color32(134, 104, 43, 134));
            cbObject._cbColorType.Add(125, new Color32(153, 121, 76, 109));
            cbObject._cbColorType.Add(126, new Color32(170, 135, 85, 109));
            cbObject._cbColorType.Add(127, new Color32(213, 119, 55, 134));
            cbObject._cbColorType.Add(128, new Color32(212, 206, 152, 134));
            cbObject._cbColorType.Add(129, new Color32(78, 79, 79, 134));
            cbObject._cbColorType.Add(130, new Color32(39, 49, 52, 134));
            cbObject._cbColorType.Add(131, new Color32(121, 121, 121, 134));
            cbObject._cbColorType.Add(132, new Color32(122, 122, 122, 134));
            cbObject._cbColorType.Add(133, new Color32(78, 218, 110, 134));
            cbObject._cbColorType.Add(134, new Color32(84, 63, 38, 109));
            cbObject._cbColorType.Add(135, new Color32(192, 175, 117, 134));
            cbObject._cbColorType.Add(136, new Color32(115, 87, 66, 109));
            cbObject._cbColorType.Add(137, new Color32(174, 160, 145, 109));
            cbObject._cbColorType.Add(138, new Color32(142, 229, 214, 134));
            cbObject._cbColorType.Add(139, new Color32(98, 100, 96, 134));
            cbObject._cbColorType.Add(140, new Color32(122, 67, 52, 109));
            cbObject._cbColorType.Add(141, new Color32(1, 196, 19, 134));
            cbObject._cbColorType.Add(142, new Color32(3, 196, 19, 134));
            cbObject._cbColorType.Add(143, new Color32(103, 81, 46, 109));
            cbObject._cbColorType.Add(144, new Color32(98, 77, 63, 109));
            cbObject._cbColorType.Add(145, new Color32(56, 56, 54, 134));
            cbObject._cbColorType.Add(146, new Color32(150, 98, 37, 109));
            cbObject._cbColorType.Add(147, new Color32(251, 244, 85, 134));
            cbObject._cbColorType.Add(148, new Color32(164, 175, 162, 134));
            cbObject._cbColorType.Add(149, new Color32(164, 161, 162, 134));
            cbObject._cbColorType.Add(150, new Color32(164, 165, 162, 134));
            cbObject._cbColorType.Add(339, new Color32(223, 223, 223, 109));
            cbObject._cbColorType.Add(340, new Color32(101, 75, 23, 109));
            cbObject._cbColorType.Add(341, new Color32(124, 194, 112, 109));
            cbObject._cbColorType.Add(342, new Color32(136, 103, 39, 109));
            cbObject._cbColorType.Add(343, new Color32(59, 59, 59, 109));
            cbObject._cbColorType.Add(344, new Color32(222, 205, 154, 109));
            cbObject._cbColorType.Add(345, new Color32(59, 59, 59, 109));
            cbObject._cbColorType.Add(346, new Color32(77, 57, 22, 109));
            cbObject._cbColorType.Add(347, new Color32(221, 221, 143, 109));
            cbObject._cbColorType.Add(348, new Color32(209, 209, 0, 109));
            cbObject._cbColorType.Add(349, new Color32(224, 105, 34, 109));
            cbObject._cbColorType.Add(350, new Color32(224, 113, 34, 109));
            cbObject._cbColorType.Add(351, new Color32(46, 123, 156, 109));
            cbObject._cbColorType.Add(352, new Color32(239, 240, 213, 109));
            cbObject._cbColorType.Add(353, new Color32(249, 249, 249, 109));
            cbObject._cbColorType.Add(354, new Color32(249, 249, 239, 109));
            cbObject._cbColorType.Add(355, new Color32(133, 14, 14, 109));
            cbObject._cbColorType.Add(356, new Color32(133, 24, 14, 109));
            cbObject._cbColorType.Add(357, new Color32(216, 130, 62, 109));
            cbObject._cbColorType.Add(358, new Color32(233, 233, 198, 109));
            cbObject._cbColorType.Add(359, new Color32(99, 66, 53, 109));
            cbObject._cbColorType.Add(360, new Color32(189, 52, 38, 109));
            cbObject._cbColorType.Add(361, new Color32(197, 208, 167, 109));
            cbObject._cbColorType.Add(362, new Color32(96, 84, 53, 109));
            cbObject._cbColorType.Add(363, new Color32(222, 112, 102, 109));
            cbObject._cbColorType.Add(364, new Color32(141, 86, 62, 109));
            cbObject._cbColorType.Add(365, new Color32(251, 215, 202, 109));
            cbObject._cbColorType.Add(366, new Color32(197, 118, 70, 109));
            cbObject._cbColorType.Add(367, new Color32(163, 64, 34, 109));
            cbObject._cbColorType.Add(368, new Color32(27, 121, 106, 109));
            cbObject._cbColorType.Add(369, new Color32(184, 146, 28, 109));
            cbObject._cbColorType.Add(370, new Color32(168, 189, 189, 109));
            cbObject._cbColorType.Add(371, new Color32(221, 221, 0, 109));
            cbObject._cbColorType.Add(372, new Color32(165, 37, 48, 109));
            cbObject._cbColorType.Add(373, new Color32(213, 92, 202, 109));
            cbObject._cbColorType.Add(151, new Color32(54, 44, 27, 109));
            cbObject._cbColorType.Add(152, new Color32(155, 26, 8, 109));
            cbObject._cbColorType.Add(153, new Color32(97, 46, 46, 109));
            cbObject._cbColorType.Add(154, new Color32(97, 97, 97, 109));
            cbObject._cbColorType.Add(155, new Color32(231, 227, 219, 109));
            cbObject._cbColorType.Add(156, new Color32(231, 227, 239, 109));
            cbObject._cbColorType.Add(157, new Color32(112, 64, 47, 109));
            cbObject._cbColorType.Add(158, new Color32(112, 112, 112, 109));
            cbObject._cbColorType.Add(159, new Color32(103, 106, 53, 109));
            cbObject._cbColorType.Add(160, new Color32(235, 235, 235, 109));
            cbObject._cbColorType.Add(162, new Color32(21, 16, 10, 109));
            cbObject._cbColorType.Add(163, new Color32(235, 235, 235, 109));
            cbObject._cbColorType.Add(164, new Color32(235, 235, 235, 109));
            cbObject._cbColorType.Add(170, new Color32(170, 141, 14, 109));
            cbObject._cbColorType.Add(171, new Color32(219, 129, 70, 109));
            cbObject._cbColorType.Add(172, new Color32(151, 94, 68, 109));
            cbObject._cbColorType.Add(173, new Color32(13, 13, 13, 109));
            cbObject._cbColorType.Add(174, new Color32(153, 181, 231, 109));
            cbObject._cbColorType.Add(175, new Color32(47, 72, 41, 109));
            cbObject._cbColorType.Add(256, new Color32(136, 103, 39, 109));
            cbObject._cbColorType.Add(257, new Color32(136, 103, 39, 109));
            cbObject._cbColorType.Add(258, new Color32(136, 103, 39, 109));
            cbObject._cbColorType.Add(259, new Color32(71, 71, 71, 109));
            cbObject._cbColorType.Add(260, new Color32(250, 27, 42, 109));
            cbObject._cbColorType.Add(261, new Color32(136, 103, 39, 109));
            cbObject._cbColorType.Add(262, new Color32(136, 103, 39, 109));
            cbObject._cbColorType.Add(263, new Color32(27, 27, 27, 109));
            cbObject._cbColorType.Add(264, new Color32(35, 31, 24, 109));
            cbObject._cbColorType.Add(265, new Color32(139, 246, 225, 109));
            cbObject._cbColorType.Add(266, new Color32(254, 254, 138, 109));
            cbObject._cbColorType.Add(267, new Color32(254, 254, 254, 109));
            cbObject._cbColorType.Add(268, new Color32(133, 101, 38, 109));
            cbObject._cbColorType.Add(269, new Color32(133, 101, 38, 109));
            cbObject._cbColorType.Add(270, new Color32(133, 101, 38, 109));
            cbObject._cbColorType.Add(271, new Color32(133, 101, 38, 109));
            cbObject._cbColorType.Add(272, new Color32(136, 136, 136, 109));
            cbObject._cbColorType.Add(273, new Color32(136, 136, 136, 109));
            cbObject._cbColorType.Add(274, new Color32(136, 136, 136, 109));
            cbObject._cbColorType.Add(275, new Color32(136, 136, 136, 109));
            cbObject._cbColorType.Add(276, new Color32(51, 234, 202, 109));
            cbObject._cbColorType.Add(277, new Color32(51, 234, 202, 109));
            cbObject._cbColorType.Add(278, new Color32(51, 234, 202, 109));
            cbObject._cbColorType.Add(279, new Color32(51, 234, 202, 109));
            cbObject._cbColorType.Add(280, new Color32(136, 136, 136, 109));
            cbObject._cbColorType.Add(281, new Color32(136, 136, 136, 109));
            cbObject._cbColorType.Add(282, new Color32(136, 136, 136, 109));
            cbObject._cbColorType.Add(283, new Color32(233, 237, 87, 109));
            cbObject._cbColorType.Add(284, new Color32(233, 237, 87, 109));
            cbObject._cbColorType.Add(285, new Color32(233, 237, 87, 109));
            cbObject._cbColorType.Add(286, new Color32(233, 237, 87, 109));
            cbObject._cbColorType.Add(287, new Color32(254, 254, 254, 109));
            cbObject._cbColorType.Add(288, new Color32(254, 254, 254, 109));
            cbObject._cbColorType.Add(289, new Color32(113, 113, 113, 109));
            cbObject._cbColorType.Add(290, new Color32(136, 103, 39, 109));
            cbObject._cbColorType.Add(298, new Color32(197, 92, 52, 109));
            cbObject._cbColorType.Add(299, new Color32(197, 92, 52, 109));
            cbObject._cbColorType.Add(300, new Color32(197, 92, 52, 109));
            cbObject._cbColorType.Add(301, new Color32(197, 92, 52, 109));
            cbObject._cbColorType.Add(302, new Color32(149, 149, 149, 109));
            cbObject._cbColorType.Add(303, new Color32(149, 149, 149, 109));
            cbObject._cbColorType.Add(304, new Color32(149, 149, 149, 109));
            cbObject._cbColorType.Add(305, new Color32(149, 149, 149, 109));
            cbObject._cbColorType.Add(306, new Color32(149, 149, 149, 109));
            cbObject._cbColorType.Add(307, new Color32(149, 149, 149, 109));
            cbObject._cbColorType.Add(308, new Color32(149, 149, 149, 109));
            cbObject._cbColorType.Add(309, new Color32(149, 149, 149, 109));
            cbObject._cbColorType.Add(310, new Color32(51, 234, 202, 109));
            cbObject._cbColorType.Add(311, new Color32(51, 234, 202, 109));
            cbObject._cbColorType.Add(312, new Color32(51, 234, 202, 109));
            cbObject._cbColorType.Add(313, new Color32(51, 234, 202, 109));
            cbObject._cbColorType.Add(314, new Color32(233, 237, 87, 109));
            cbObject._cbColorType.Add(315, new Color32(233, 237, 87, 109));
            cbObject._cbColorType.Add(316, new Color32(233, 237, 87, 109));
            cbObject._cbColorType.Add(317, new Color32(233, 237, 87, 109));
            cbObject._cbColorType.Add(318, new Color32(51, 51, 51, 109));
            cbObject._cbColorType.Add(319, new Color32(247, 117, 117, 109));
            cbObject._cbColorType.Add(320, new Color32(193, 175, 132, 109));
            cbObject._cbColorType.Add(321, new Color32(157, 149, 138, 109));
            cbObject._cbColorType.Add(322, new Color32(233, 237, 87, 109));
            cbObject._cbColorType.Add(323, new Color32(158, 131, 77, 109));
            cbObject._cbColorType.Add(324, new Color32(158, 131, 77, 109));
            cbObject._cbColorType.Add(325, new Color32(151, 151, 151, 109));
            cbObject._cbColorType.Add(326, new Color32(244, 250, 250, 109));
            cbObject._cbColorType.Add(327, new Color32(244, 250, 250, 109));
            cbObject._cbColorType.Add(328, new Color32(41, 41, 41, 109));
            cbObject._cbColorType.Add(329, new Color32(238, 125, 74, 109));
            cbObject._cbColorType.Add(330, new Color32(244, 250, 250, 109));
            cbObject._cbColorType.Add(331, new Color32(113, 0, 0, 109));
            cbObject._cbColorType.Add(332, new Color32(244, 250, 250, 109));
            cbObject._cbColorType.Add(333, new Color32(96, 71, 28, 109));
            cbObject._cbColorType.Add(334, new Color32(197, 92, 53, 109));
            cbObject._cbColorType.Add(335, new Color32(244, 250, 250, 109));
            cbObject._cbColorType.Add(336, new Color32(197, 92, 53, 109));
            cbObject._cbColorType.Add(337, new Color32(164, 168, 184, 109));
            cbObject._cbColorType.Add(338, new Color32(169, 218, 115, 109));
            cbObject._cbColorType.Add(374, new Color32(202, 202, 202, 109));
            cbObject._cbColorType.Add(375, new Color32(121, 13, 39, 109));
            cbObject._cbColorType.Add(376, new Color32(158, 73, 91, 109));
            cbObject._cbColorType.Add(377, new Color32(251, 160, 0, 109));
            cbObject._cbColorType.Add(378, new Color32(251, 160, 0, 109));
            cbObject._cbColorType.Add(379, new Color32(134, 134, 134, 109));
            cbObject._cbColorType.Add(380, new Color32(45, 45, 45, 109));
            cbObject._cbColorType.Add(381, new Color32(73, 118, 98, 109));
            cbObject._cbColorType.Add(382, new Color32(247, 146, 27, 109));
            cbObject._cbColorType.Add(383, new Color32(221, 139, 139, 109));
            cbObject._cbColorType.Add(391, new Color32(216, 121, 8, 109));
            cbObject._cbColorType.Add(392, new Color32(237, 192, 106, 109));
            cbObject._cbColorType.Add(393, new Color32(211, 135, 46, 109));
            cbObject._cbColorType.Add(394, new Color32(227, 191, 93, 109));
            cbObject._cbColorType.Add(395, new Color32(213, 213, 149, 109));
            cbObject._cbColorType.Add(396, new Color32(227, 191, 93, 109));
            cbObject._cbColorType.Add(397, new Color32(188, 141, 115, 109));
            cbObject._cbColorType.Add(398, new Color32(104, 78, 30, 109));
            cbObject._cbColorType.Add(399, new Color32(88, 82, 158, 109));
            cbObject._cbColorType.Add(400, new Color32(218, 119, 71, 109));
            cbObject._cbColorType.Add(401, new Color32(218, 119, 71, 109));
            cbObject._cbColorType.Add(402, new Color32(137, 137, 137, 109));
            cbObject._cbColorType.Add(403, new Color32(144, 71, 214, 109));
            cbObject._cbColorType.Add(404, new Color32(88, 1, 1, 109));
            cbObject._cbColorType.Add(405, new Color32(70, 38, 44, 109));
            cbObject._cbColorType.Add(406, new Color32(228, 219, 211, 109));
            cbObject._cbColorType.Add(407, new Color32(218, 68, 26, 109));
            cbObject._cbColorType.Add(408, new Color32(58, 58, 58, 109));
            cbObject._cbColorType.Add(417, new Color32(58, 58, 58, 109));
            cbObject._cbColorType.Add(418, new Color32(172, 122, 1, 109));
            cbObject._cbColorType.Add(419, new Color32(107, 128, 138, 109));
            cbObject._cbColorType.Add(420, new Color32(176, 147, 127, 109));
            cbObject._cbColorType.Add(421, new Color32(229, 198, 139, 109));
            cbObject._cbColorType.Add(422, new Color32(127, 84, 54, 109));
            cbObject._cbColorType.Add(2256, new Color32(64, 64, 64, 109));
            cbObject._cbColorType.Add(2257, new Color32(64, 62, 64, 109));
            cbObject._cbColorType.Add(2258, new Color32(64, 64, 62, 109));
            cbObject._cbColorType.Add(2259, new Color32(63, 64, 64, 109));
            cbObject._cbColorType.Add(2260, new Color32(63, 61, 61, 109));
            cbObject._cbColorType.Add(2261, new Color32(63, 61, 62, 109));
            cbObject._cbColorType.Add(2262, new Color32(64, 63, 64, 109));
            cbObject._cbColorType.Add(2263, new Color32(64, 62, 64, 109));
            cbObject._cbColorType.Add(2264, new Color32(64, 61, 64, 109));
            cbObject._cbColorType.Add(2265, new Color32(63, 63, 63, 109));
            cbObject._cbColorType.Add(2266, new Color32(64, 63, 63, 109));
            cbObject._cbColorType.Add(2267, new Color32(63, 64, 63, 109));


        }


        /// <summary>
        /// using block co-ordinates returns a given block color.
        /// </summary>
        /// <param name="id">unique id of a block</param>
        /// <param name="x">x coordinate of a block</param>
        /// <param name="y">y coordinate of a block</param>
        /// <param name="z">z coordinate of a block</param>
        /// <returns>a color at a time</returns>

        public Color32 MapIdToColor(int id, int x, int y, int z)
        {
            if (cbObject._cbColorType.ContainsKey(id))
            {
                Point p = new Point();
                p.x = x;
                p.y = y;
                p.z = z;
                p.id = id;
                cbObject._cb3DpointsPlusId.Add(p);
                bool IsAvailable = cbObject._cbColors.Contains(cbObject._cbColorType[id]);
                if (!IsAvailable)
                {
                    cbObject._cbColors.Add(cbObject._cbColorType[id]);
                    cbObject._cbColorIds.Add(id);
                }
                return cbObject._cbColorType[id];
            }
            return new Color32(0, 0, 0, 0);
        }
       
    }
}


