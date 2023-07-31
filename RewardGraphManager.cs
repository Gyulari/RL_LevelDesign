using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class RewardGraphManager : MonoBehaviour
{
    [SerializeField] private float maxValue;
    [SerializeField] private int maxValueNum;

    public static bool sig_RewardRecord = false;
    public static bool sig_reachMax = false;
    public static float m_BlueGroupReward = 0.0f;
    public static float m_RedGroupReward = 0.0f;
    public static float m_DrawReward = 0.0f;

    private int epiNum = 0;

    public GameObject blueDot;
    public GameObject redDot;
    public GameObject purpleDot;
    public Transform dotInitPosB;
    public Transform dotInitPosR;

    private void Update()
    {
        /*
        if (sig_RewardRecord) {
            epiNum++;

            if (sig_reachMax) {
                GameObject newPurpleDotToB = Instantiate(purpleDot, dotInitPosB);
                newPurpleDotToB.GetComponent<RectTransform>().anchoredPosition
                    += new Vector2((470.0f / maxValueNum) * epiNum, 165.0f + 110.0f * (m_DrawReward + m_BlueGroupReward) / maxValue);

                GameObject newPurpleDotToR = Instantiate(purpleDot, dotInitPosR);
                newPurpleDotToR.GetComponent<RectTransform>().anchoredPosition
                    += new Vector2((470.0f / maxValueNum) * epiNum, 165.0f + 110.0f * (m_DrawReward + m_RedGroupReward) / maxValue);
            }
            else {
                GameObject newBlueDot = Instantiate(blueDot, dotInitPosB);
                newBlueDot.GetComponent<RectTransform>().anchoredPosition
                    += new Vector2((470.0f / maxValueNum) * epiNum, 165.0f + 110.0f * m_BlueGroupReward / maxValue);

                GameObject newRedDot = Instantiate(redDot, dotInitPosR);
                newRedDot.GetComponent<RectTransform>().anchoredPosition
                    += new Vector2((470.0f / maxValueNum) * epiNum, 165.0f + 110.0f * m_RedGroupReward / maxValue);
            }

            sig_reachMax = false;
            sig_RewardRecord = false;
            m_BlueGroupReward = 0;
            m_RedGroupReward = 0;
            m_DrawReward = 0;
        }
        */
    }
}
