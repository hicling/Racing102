using Racing102;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Racing102.Client
{
    /// <summary>
    /// Controls the "information box" on the character-select screen.
    /// </summary>
    /// <remarks>
    /// This box also includes the "READY" button. The Ready button's state (enabled/disabled) is controlled
    /// here, but note that the actual behavior (when clicked) is set in the editor: the button directly calls
    /// ClientCharSelectState.OnPlayerClickedReady().
    /// </remarks>
    public class UICarSelectClassInfoBox : MonoBehaviour
    {
        [SerializeField]
        private Text m_WelcomeBanner;
        [SerializeField]
        private Text m_ClassLabel;
        [SerializeField]
        private GameObject m_HideWhenNoClassSelected;
        [SerializeField]
        private Image m_ClassBanner;
        [SerializeField]
        private Button m_ReadyButton;
        [SerializeField]
        [Tooltip("Message shown in the char-select screen. {0} will be replaced with the player's seat number")]
        [Multiline]
        private string m_WelcomeMsg = "Welcome, P{0}!";


        private bool isLockedIn = false;

        public void OnSetPlayerNumber(int playerNumber)
        {
            m_WelcomeBanner.text = string.Format(m_WelcomeMsg, (playerNumber + 1));
        }

        public void ConfigureForNoSelection()
        {
            m_HideWhenNoClassSelected.SetActive(false);
            m_ReadyButton.interactable = false;
        }

        public void ConfigureForLockedIn()
        {
            m_ReadyButton.interactable = false;
            isLockedIn = true;
        }

        public void ConfigureForClass(CarTypeEnum characterType)
        {
            m_HideWhenNoClassSelected.SetActive(true);
            if (!isLockedIn)
            {
                m_ReadyButton.interactable = true;
            }

            CarClass carClass = GameDataSource.Instance.CharacterDataByType[characterType];
            m_ClassLabel.text = carClass.DisplayedName;
            m_ClassBanner.sprite = carClass.CarBannerLit;
        }
    }
}
