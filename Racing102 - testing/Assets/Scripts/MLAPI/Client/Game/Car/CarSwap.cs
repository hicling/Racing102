using UnityEngine.Assertions;
using UnityEngine;

namespace Racing102.Client
{
    /// <summary>
    /// Responsible for storing of all of the pieces of a character, and swapping the pieces out en masse when asked to.
    /// </summary>
    public class CarSwap : MonoBehaviour
    {
        [System.Serializable]
        public class CharacterModelSet
        {
            public GameObject body;

            public void SetFullActive(bool isActive)
            {
                body.SetActive(isActive);
            }
        }

        [SerializeField]
        private CharacterModelSet[] m_CharacterModels;


        /// <summary>
        /// Swap the visuals of the character to the index passed in. 
        /// </summary>
        /// <param name="modelIndex"></param>
        public void SwapToModel(int modelIndex)
        {
            Assert.IsTrue(modelIndex < m_CharacterModels.Length);

            for (int i = 0; i < m_CharacterModels.Length; i++)
            {
                m_CharacterModels[i].SetFullActive(i == modelIndex);
                
            }           
        }

        /// <summary>
        /// Used by special effects where the character should be invisible.
        /// </summary>
        public void SwapAllOff()
        {
            for (int i = 0; i < m_CharacterModels.Length; i++)
            {
                m_CharacterModels[i].SetFullActive(false);
            }
        }

#if UNITY_EDITOR

#endif
    }
}
