using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class SubstrateIdToCubiquityColor
{
    [SerializeField]
    public static List<Color32> colorIds = new List<Color32>();
    public static  Dictionary<int, Color32> ColorType = new Dictionary<int, Color32>();
    /// <summary>
    /// maps substrate block id to Cubiquity Colors. 
    /// </summary>
    /// <param name="id"> </param>
    /// <returns></returns>
    public static void SetColor()
    {        
        ColorType.Add(1, new Color32(123, 124, 121, 134));
        ColorType.Add(2, new Color32(49, 164, 42, 134));//
        ColorType.Add(3, new Color32(106, 76, 50, 109));
        ColorType.Add(4, new Color32(123, 124, 121, 134));
        ColorType.Add(5, new Color32(163, 139, 79, 109));
        ColorType.Add(6, new Color32(98, 141, 57, 134));//
        ColorType.Add(7, new Color32(50, 51, 48, 134));
        ColorType.Add(8, new Color32(54, 94, 253, 134));
        ColorType.Add(9, new Color32(54, 94, 253, 134));
        ColorType.Add(10, new Color32(253, 247, 115, 134));
        ColorType.Add(11, new Color32(253, 247, 115, 134));
        ColorType.Add(12, new Color32(214, 208, 161, 134));
        ColorType.Add(13, new Color32(124, 124, 120, 134));
        ColorType.Add(14, new Color32(111, 112, 111, 134));
        ColorType.Add(15, new Color32(111, 112, 111, 134));
        ColorType.Add(16, new Color32(116, 118, 116, 134));
        ColorType.Add(17, new Color32(158, 127, 74, 109));
        ColorType.Add(18, new Color32(65, 161, 17, 134));//
        ColorType.Add(19, new Color32(251, 208, 66, 134));
        ColorType.Add(20, new Color32(158, 159, 156, 134));
        ColorType.Add(21, new Color32(111, 112, 111, 134));
        ColorType.Add(22, new Color32(28, 72, 181, 134));
        ColorType.Add(23, new Color32(115, 114, 116, 114));
        ColorType.Add(24, new Color32(214, 208, 163, 134));
        ColorType.Add(25, new Color32(100, 59, 39, 109));
        ColorType.Add(26, new Color32(137, 20, 21, 109));
        ColorType.Add(27, new Color32(222, 84, 8, 134));//
        ColorType.Add(28, new Color32(118, 94, 57, 109));
        ColorType.Add(29, new Color32(121, 112, 70, 134));//
        ColorType.Add(30, new Color32(231, 236, 234, 134));
        ColorType.Add(31, new Color32(43, 119, 17, 134));//
        ColorType.Add(32, new Color32(143, 96, 39, 109));
        ColorType.Add(33, new Color32(118, 94, 75, 109));
        ColorType.Add(34, new Color32(166, 131, 82, 109));
        ColorType.Add(35, new Color32(172, 68, 180, 134));
        ColorType.Add(36, new Color32(154, 158, 156, 134));
        ColorType.Add(37, new Color32(245, 238, 56, 134));
        ColorType.Add(38, new Color32(202, 7, 14, 109));
        ColorType.Add(39, new Color32(133, 109, 83, 109));
        ColorType.Add(40, new Color32(202, 13, 15, 109));
        ColorType.Add(41, new Color32(250, 244, 85, 134));
        ColorType.Add(42, new Color32(145, 149, 147, 134));
        ColorType.Add(43, new Color32(96, 255, 248, 134));
        ColorType.Add(44, new Color32(96, 255, 248, 134));
        ColorType.Add(45, new Color32(149, 84, 65, 109));
        ColorType.Add(46, new Color32(197, 58, 19, 109));
        ColorType.Add(47, new Color32(171, 137, 84, 109));
        ColorType.Add(48, new Color32(79, 87, 54, 134));
        ColorType.Add(49, new Color32(11, 17, 22, 134));
        ColorType.Add(50, new Color32(150, 121, 70, 109));
        ColorType.Add(51, new Color32(217, 119, 25, 134));
        ColorType.Add(52, new Color32(22, 48, 60, 134));
        ColorType.Add(53, new Color32(163, 86, 0, 109));
        ColorType.Add(54, new Color32(128, 119, 45, 109));
        ColorType.Add(55, new Color32(246, 251, 246, 134));
        ColorType.Add(56, new Color32(78, 79, 77, 134));
        ColorType.Add(57, new Color32(142, 229, 224, 134));
        ColorType.Add(58, new Color32(177, 119, 78, 109));
        ColorType.Add(59, new Color32(0, 196, 19, 134));
        ColorType.Add(60, new Color32(100, 59, 39, 109));
        ColorType.Add(61, new Color32(87, 87, 84, 134));
        ColorType.Add(62, new Color32(87, 87, 87, 134));
        ColorType.Add(63, new Color32(158, 127, 74, 109));
        ColorType.Add(64, new Color32(141, 108, 53, 109));
        ColorType.Add(65, new Color32(141, 108, 60, 109));
        ColorType.Add(66, new Color32(141, 108, 53, 109));
        ColorType.Add(67, new Color32(78, 79, 77, 134));
        ColorType.Add(68, new Color32(177, 139, 87, 109));
        ColorType.Add(69, new Color32(103, 81, 46, 109));
        ColorType.Add(70, new Color32(123, 124, 121, 134));
        ColorType.Add(71, new Color32(174, 175, 173, 134));
        ColorType.Add(72, new Color32(158, 127, 74, 109));
        ColorType.Add(73, new Color32(111, 112, 111, 134));
        ColorType.Add(74, new Color32(111, 112, 111, 134));
        ColorType.Add(75, new Color32(157, 112, 70, 109));
        ColorType.Add(76, new Color32(249, 9, 17, 109));
        ColorType.Add(77, new Color32(123, 124, 121, 134));
        ColorType.Add(78, new Color32(246, 251, 246, 134));
        ColorType.Add(79, new Color32(66, 101, 157, 134));
        ColorType.Add(80, new Color32(246, 251, 248, 134));
        ColorType.Add(81, new Color32(43, 119, 23, 134));//
        ColorType.Add(82, new Color32(154, 161, 167, 134));
        ColorType.Add(83, new Color32(145, 181, 114, 134));
        ColorType.Add(84, new Color32(149, 84, 75, 109));
        ColorType.Add(85, new Color32(103, 81, 46, 109));
        ColorType.Add(86, new Color32(213, 134, 21, 134));
        ColorType.Add(87, new Color32(100, 59, 65, 109));
        ColorType.Add(88, new Color32(115, 87, 66, 109));
        ColorType.Add(89, new Color32(134, 104, 43, 134));
        ColorType.Add(90, new Color32(84, 5, 191, 134));
        ColorType.Add(91, new Color32(213, 134, 21, 134));
        ColorType.Add(92, new Color32(228, 231, 230, 134));
        ColorType.Add(93, new Color32(174, 175, 173, 134));
        ColorType.Add(94, new Color32(174, 170, 173, 134));
        ColorType.Add(95, new Color32(78, 218, 116, 134));
        ColorType.Add(96, new Color32(123, 90, 40, 109));
        ColorType.Add(97, new Color32(130, 132, 129, 134));
        ColorType.Add(98, new Color32(123, 124, 121, 134));
        ColorType.Add(99, new Color32(210, 177, 128, 134));
        ColorType.Add(100, new Color32(210, 177, 128, 134));
        ColorType.Add(101, new Color32(111, 112, 111, 134));
        ColorType.Add(102, new Color32(246, 251, 246, 134));
        ColorType.Add(103, new Color32(157, 153, 34, 134));
        ColorType.Add(104, new Color32(174, 175, 173, 134));
        ColorType.Add(105, new Color32(174, 175, 173, 134));
        ColorType.Add(106, new Color32(27, 76, 20, 134));//
        ColorType.Add(107, new Color32(118, 94, 57, 109));
        ColorType.Add(108, new Color32(115, 61, 49, 109));
        ColorType.Add(109, new Color32(131, 133, 130, 134));
        ColorType.Add(110, new Color32(116, 110, 112, 134));
        ColorType.Add(111, new Color32(27, 76, 20, 134));//
        ColorType.Add(112, new Color32(49, 22, 27, 109));
        ColorType.Add(113, new Color32(49, 22, 27, 109));
        ColorType.Add(114, new Color32(49, 22, 27, 109));
        ColorType.Add(115, new Color32(100, 28, 31, 109));
        ColorType.Add(116, new Color32(172, 50, 51, 109));
        ColorType.Add(117, new Color32(96, 97, 95, 134));//
        ColorType.Add(118, new Color32(63, 63, 61, 134));
        ColorType.Add(119, new Color32(145, 149, 147, 134));
        ColorType.Add(120, new Color32(72, 142, 139, 134));//
        ColorType.Add(121, new Color32(225, 224, 169, 134));
        ColorType.Add(122, new Color32(49, 22, 27, 109));
        ColorType.Add(123, new Color32(106, 50, 21, 109));
        ColorType.Add(124, new Color32(134, 104, 43, 134));
        ColorType.Add(125, new Color32(153, 121, 76, 109));
        ColorType.Add(126, new Color32(170, 135, 85, 109));
        ColorType.Add(127, new Color32(213, 119, 55, 134));
        ColorType.Add(128, new Color32(212, 206, 152, 134));
        ColorType.Add(129, new Color32(78, 79, 79, 134));
        ColorType.Add(130, new Color32(39, 49, 52, 134));
        ColorType.Add(131, new Color32(123, 124, 121, 134));
        ColorType.Add(132, new Color32(123, 124, 121, 134));
        ColorType.Add(133, new Color32(78, 218, 110, 134));
        ColorType.Add(134, new Color32(84, 63, 38, 109));
        ColorType.Add(135, new Color32(192, 175, 117, 134));
        ColorType.Add(136, new Color32(115, 87, 66, 109));
        ColorType.Add(137, new Color32(174, 160, 145, 109));
        ColorType.Add(138, new Color32(142, 229, 214, 134));
        ColorType.Add(139, new Color32(98, 100, 96, 134));
        ColorType.Add(140, new Color32(122, 67, 52, 109));
        ColorType.Add(141, new Color32(0, 196, 19, 134));
        ColorType.Add(142, new Color32(0, 196, 19, 134));
        ColorType.Add(143, new Color32(103, 81, 46, 109));
        ColorType.Add(144, new Color32(98, 77, 63, 109));
        ColorType.Add(145, new Color32(56, 56, 54, 134));
        ColorType.Add(146, new Color32(150, 98, 37, 109));
        ColorType.Add(147, new Color32(250, 244, 85, 134));
        ColorType.Add(148, new Color32(164, 175, 162, 134));
        ColorType.Add(149, new Color32(164, 161, 162, 134));
        ColorType.Add(150, new Color32(164, 165, 162, 134));
        ColorType.Add(339, new Color32(223, 223, 223, 109));
        ColorType.Add(340, new Color32(101, 75, 23, 109));
        ColorType.Add(341, new Color32(124, 194, 112, 109));
        ColorType.Add(342, new Color32(136, 103, 39, 109));
        ColorType.Add(343, new Color32(59, 59, 59, 109));
        ColorType.Add(344, new Color32(222, 205, 154, 109));
        ColorType.Add(345, new Color32(59, 59, 59, 109));
        ColorType.Add(346, new Color32(77, 57, 22, 109));
        ColorType.Add(347, new Color32(221, 221, 143, 109));
        ColorType.Add(348, new Color32(209, 209, 0, 109));
        ColorType.Add(349, new Color32(224, 105, 34, 109));
        ColorType.Add(350, new Color32(224, 113, 34, 109));
        ColorType.Add(351, new Color32(46, 123, 156, 109));
        ColorType.Add(352, new Color32(239, 240, 213, 109));
        ColorType.Add(353, new Color32(249, 249, 249, 109));
        ColorType.Add(354, new Color32(249, 249, 239, 109));
        ColorType.Add(355, new Color32(133, 14, 14, 109));
        ColorType.Add(356, new Color32(133, 24, 14, 109));
        ColorType.Add(357, new Color32(216, 130, 62, 109));
        ColorType.Add(358, new Color32(233, 233, 198, 109));
        ColorType.Add(359, new Color32(99, 66, 53, 109));
        ColorType.Add(360, new Color32(189, 52, 38, 109));
        ColorType.Add(361, new Color32(197, 208, 167, 109));
        ColorType.Add(362, new Color32(96, 84, 53, 109));
        ColorType.Add(363, new Color32(222, 112, 102, 109));
        ColorType.Add(364, new Color32(141, 86, 62, 109));
        ColorType.Add(365, new Color32(251, 215, 202, 109));
        ColorType.Add(366, new Color32(197, 118, 70, 109));
        ColorType.Add(367, new Color32(163, 64, 34, 109));
        ColorType.Add(368, new Color32(27, 121, 106, 109));
        ColorType.Add(369, new Color32(184, 146, 28, 109));
        ColorType.Add(370, new Color32(168, 189, 189, 109));
        ColorType.Add(371, new Color32(221, 221, 0, 109));
        ColorType.Add(372, new Color32(165, 37, 48, 109));
        ColorType.Add(373, new Color32(213, 92, 202, 109));
        ColorType.Add(151, new Color32(54, 44, 27, 109));
        ColorType.Add(152, new Color32(155, 26, 8, 109));
        ColorType.Add(153, new Color32(97, 46, 46, 109));
        ColorType.Add(154, new Color32(97, 97, 97, 109));
        ColorType.Add(155, new Color32(231, 227, 219, 109));
        ColorType.Add(156, new Color32(231, 227, 239, 109));
        ColorType.Add(157, new Color32(112, 64, 47, 109));
        ColorType.Add(158, new Color32(112, 112, 112, 109));
        ColorType.Add(159, new Color32(103, 106, 53, 109));
        ColorType.Add(160, new Color32(235, 235, 235, 109));
        ColorType.Add(162, new Color32(21, 16, 10, 109));
        ColorType.Add(163, new Color32(235, 235, 235, 109));
        ColorType.Add(164, new Color32(235, 235, 235, 109));
        ColorType.Add(170, new Color32(170, 141, 14, 109));
        ColorType.Add(171, new Color32(219, 129, 70, 109));
        ColorType.Add(172, new Color32(151, 94, 68, 109));
        ColorType.Add(173, new Color32(13, 13, 13, 109));
        ColorType.Add(174, new Color32(153, 181, 231, 109));
        ColorType.Add(175, new Color32(47, 72, 41, 109));
        ColorType.Add(256, new Color32(136, 103, 39, 109));
        ColorType.Add(257, new Color32(136, 103, 39, 109));
        ColorType.Add(258, new Color32(136, 103, 39, 109));
        ColorType.Add(259, new Color32(71, 71, 71, 109));
        ColorType.Add(260, new Color32(250, 27, 42, 109));
        ColorType.Add(261, new Color32(136, 103, 39, 109));
        ColorType.Add(262, new Color32(136, 103, 39, 109));
        ColorType.Add(263, new Color32(27, 27, 27, 109));
        ColorType.Add(264, new Color32(35, 31, 24, 109));
        ColorType.Add(265, new Color32(139, 246, 225, 109));
        ColorType.Add(266, new Color32(254, 254, 138, 109));
        ColorType.Add(267, new Color32(254, 254, 254, 109));
        ColorType.Add(268, new Color32(133, 101, 38, 109));
        ColorType.Add(269, new Color32(133, 101, 38, 109));
        ColorType.Add(270, new Color32(133, 101, 38, 109));
        ColorType.Add(271, new Color32(133, 101, 38, 109));
        ColorType.Add(272, new Color32(136, 136, 136, 109));
        ColorType.Add(273, new Color32(136, 136, 136, 109));
        ColorType.Add(274, new Color32(136, 136, 136, 109));
        ColorType.Add(275, new Color32(136, 136, 136, 109));
        ColorType.Add(276, new Color32(51, 234, 202, 109));
        ColorType.Add(277, new Color32(51, 234, 202, 109));
        ColorType.Add(278, new Color32(51, 234, 202, 109));
        ColorType.Add(279, new Color32(51, 234, 202, 109));
        ColorType.Add(280, new Color32(136, 136, 136, 109));
        ColorType.Add(281, new Color32(136, 136, 136, 109));
        ColorType.Add(282, new Color32(136, 136, 136, 109));
        ColorType.Add(283, new Color32(233, 237, 87, 109));
        ColorType.Add(284, new Color32(233, 237, 87, 109));
        ColorType.Add(285, new Color32(233, 237, 87, 109));
        ColorType.Add(286, new Color32(233, 237, 87, 109));
        ColorType.Add(287, new Color32(254, 254, 254, 109));
        ColorType.Add(288, new Color32(254, 254, 254, 109));
        ColorType.Add(289, new Color32(113, 113, 113, 109));
        ColorType.Add(290, new Color32(136, 103, 39, 109));
        ColorType.Add(298, new Color32(197, 92, 52, 109));
        ColorType.Add(299, new Color32(197, 92, 52, 109));
        ColorType.Add(300, new Color32(197, 92, 52, 109));
        ColorType.Add(301, new Color32(197, 92, 52, 109));
        ColorType.Add(302, new Color32(149, 149, 149, 109));
        ColorType.Add(303, new Color32(149, 149, 149, 109));
        ColorType.Add(304, new Color32(149, 149, 149, 109));
        ColorType.Add(305, new Color32(149, 149, 149, 109));
        ColorType.Add(306, new Color32(149, 149, 149, 109));
        ColorType.Add(307, new Color32(149, 149, 149, 109));
        ColorType.Add(308, new Color32(149, 149, 149, 109));
        ColorType.Add(309, new Color32(149, 149, 149, 109));
        ColorType.Add(310, new Color32(51, 234, 202, 109));
        ColorType.Add(311, new Color32(51, 234, 202, 109));
        ColorType.Add(312, new Color32(51, 234, 202, 109));
        ColorType.Add(313, new Color32(51, 234, 202, 109));
        ColorType.Add(314, new Color32(233, 237, 87, 109));
        ColorType.Add(315, new Color32(233, 237, 87, 109));
        ColorType.Add(316, new Color32(233, 237, 87, 109));
        ColorType.Add(317, new Color32(233, 237, 87, 109));
        ColorType.Add(318, new Color32(51, 51, 51, 109));
        ColorType.Add(319, new Color32(247, 117, 117, 109));
        ColorType.Add(320, new Color32(193, 175, 132, 109));
        ColorType.Add(321, new Color32(157, 149, 138, 109));
        ColorType.Add(322, new Color32(233, 237, 87, 109));
        ColorType.Add(323, new Color32(158, 131, 77, 109));
        ColorType.Add(324, new Color32(158, 131, 77, 109));
        ColorType.Add(325, new Color32(151, 151, 151, 109));
        ColorType.Add(326, new Color32(244, 250, 250, 109));
        ColorType.Add(327, new Color32(244, 250, 250, 109));
        ColorType.Add(328, new Color32(41, 41, 41, 109));
        ColorType.Add(329, new Color32(238, 125, 74, 109));
        ColorType.Add(330, new Color32(244, 250, 250, 109));
        ColorType.Add(331, new Color32(113, 0, 0, 109));
        ColorType.Add(332, new Color32(244, 250, 250, 109));
        ColorType.Add(333, new Color32(96, 71, 28, 109));
        ColorType.Add(334, new Color32(197, 92, 53, 109));
        ColorType.Add(335, new Color32(244, 250, 250, 109));
        ColorType.Add(336, new Color32(197, 92, 53, 109));
        ColorType.Add(337, new Color32(164, 168, 184, 109));
        ColorType.Add(338, new Color32(169, 218, 115, 109));
        ColorType.Add(374, new Color32(202, 202, 202, 109));
        ColorType.Add(375, new Color32(121, 13, 39, 109));
        ColorType.Add(376, new Color32(158, 73, 91, 109));
        ColorType.Add(377, new Color32(251, 160, 0, 109));
        ColorType.Add(378, new Color32(251, 160, 0, 109));
        ColorType.Add(379, new Color32(134, 134, 134, 109));
        ColorType.Add(380, new Color32(45, 45, 45, 109));
        ColorType.Add(381, new Color32(73, 118, 98, 109));
        ColorType.Add(382, new Color32(247, 146, 27, 109));
        ColorType.Add(383, new Color32(221, 139, 139, 109));
        ColorType.Add(391, new Color32(216, 121, 8, 109));
        ColorType.Add(392, new Color32(237, 192, 106, 109));
        ColorType.Add(393, new Color32(211, 135, 46, 109));
        ColorType.Add(394, new Color32(227, 191, 93, 109));
        ColorType.Add(395, new Color32(213, 213, 149, 109));
        ColorType.Add(396, new Color32(227, 191, 93, 109));
        ColorType.Add(397, new Color32(188, 141, 115, 109));
        ColorType.Add(398, new Color32(104, 78, 30, 109));
        ColorType.Add(399, new Color32(88, 82, 158, 109));
        ColorType.Add(400, new Color32(218, 119, 71, 109));
        ColorType.Add(401, new Color32(218, 119, 71, 109));
        ColorType.Add(402, new Color32(137, 137, 137, 109));
        ColorType.Add(403, new Color32(144, 71, 214, 109));
        ColorType.Add(404, new Color32(88, 1, 1, 109));
        ColorType.Add(405, new Color32(70, 38, 44, 109));
        ColorType.Add(406, new Color32(228, 219, 211, 109));
        ColorType.Add(407, new Color32(218, 68, 26, 109));
        ColorType.Add(408, new Color32(58, 58, 58, 109));
        ColorType.Add(417, new Color32(58, 58, 58, 109));
        ColorType.Add(418, new Color32(172, 122, 1, 109));
        ColorType.Add(419, new Color32(107, 128, 138, 109));
        ColorType.Add(420, new Color32(176, 147, 127, 109));
        ColorType.Add(421, new Color32(229, 198, 139, 109));
        ColorType.Add(422, new Color32(127, 84, 54, 109));
        ColorType.Add(2256, new Color32(64, 64, 64, 109));
        ColorType.Add(2257, new Color32(64, 64, 64, 109));
        ColorType.Add(2258, new Color32(64, 64, 64, 109));
        ColorType.Add(2259, new Color32(64, 64, 64, 109));
        ColorType.Add(2260, new Color32(64, 64, 64, 109));
        ColorType.Add(2261, new Color32(64, 64, 64, 109));
        ColorType.Add(2262, new Color32(64, 64, 64, 109));
        ColorType.Add(2263, new Color32(64, 64, 64, 109));
        ColorType.Add(2264, new Color32(64, 64, 64, 109));
        ColorType.Add(2265, new Color32(64, 64, 64, 109));
        ColorType.Add(2266, new Color32(64, 64, 64, 109));
        ColorType.Add(2267, new Color32(64, 64, 64, 109));
       
    }


     public static Color32 MapIdToColor(int id,int x, int y,int z)
        {
           
            if (ColorType.ContainsKey(id))
            {
                bool IsAvailable = colorIds.Contains(ColorType[id]);
                if (!IsAvailable)
                {                  
                    
                    colorIds.Add(ColorType[id]);
                    
                }
                return ColorType[id];
            }
            return new Color32(0, 0, 0, 0);
        }       
    }


