using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class SubstrateIdToCubiquityColor
{
    [SerializeField]
    public static List<Color32> colorIds = new List<Color32>();
    /// <summary>
    /// maps substrate block id to Cubiquity Colors. 
    /// </summary>
    /// <param name="id"> </param>
    /// <returns></returns>
    public static Color32 SetColor(int id)
    {
        Dictionary<int, Color32> ColorType = new Dictionary<int, Color32>();
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
