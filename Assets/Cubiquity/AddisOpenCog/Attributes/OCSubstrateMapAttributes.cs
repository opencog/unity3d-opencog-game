
using UnityEngine;
using System.Collections;
using System;
namespace OCCubiquity
{
    [AttributeUsage(AttributeTargets.Field)]
    public class OCSubstrateMapAttributes : PropertyAttribute
    {
        private string tooltip;
        public string Tooltip
        {
            get { return tooltip; }
            set { tooltip = value; }
        }

        public OCSubstrateMapAttributes(string tooltip)
        {
            this.tooltip = tooltip;

        }
    }
}
