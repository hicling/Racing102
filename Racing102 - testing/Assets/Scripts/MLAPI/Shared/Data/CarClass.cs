using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racing102
{
    [CreateAssetMenu(menuName = "GameData/CarClass", order = 1)]
    public class CarClass : ScriptableObject
    {
        [Tooltip("which car this data represents")]
        public CarTypeEnum CarType;

        [Tooltip("Set to true if this represents an NPC, as opposed to a player.")]
        public bool IsNpc;

        [Tooltip("For players, this is the displayed \"class name\"")]
        public string DisplayedName;

        [Tooltip("For players, this is the class banner (when active)")]
        public Sprite CarBannerLit;

        [Tooltip("For players, this is the class banner (when inactive)")]
        public Sprite CarBannerUnlit;

    }
}

