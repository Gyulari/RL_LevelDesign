using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class RewardGraphManager : MonoBehaviour
{
    // private enum GraphType { BlueGroupReward, BlueReward, RedGroupReward, RedReward };

    // [SerializeField] private GraphType graphType;

    public static bool sig_RewardRecord = false;
    public static bool sig_reachMax = false;
    public static float m_BlueGroupReward = 0.0f;
    public static float m_RedGroupReward = 0.0f;

    private int epiNum = 0;

    public GameObject blueDot;
    public GameObject redDot;
    public GameObject purpleDot;
    public Transform dotInitPosB;
    public Transform dotInitPosR;

    private void Update()
    {
        if (sig_RewardRecord) {
            epiNum++;

            if (sig_reachMax) {
                GameObject newPurpleDotToB = Instantiate(purpleDot, dotInitPosB);
                newPurpleDotToB.GetComponent<RectTransform>().anchoredPosition += new Vector2(1.6f * epiNum, 55f);

                GameObject newPurpleDotToR = Instantiate(purpleDot, dotInitPosR);
                newPurpleDotToR.GetComponent<RectTransform>().anchoredPosition += new Vector2(1.6f * epiNum, 55f);
            }
            else {
                GameObject newBlueDot = Instantiate(blueDot, dotInitPosB);
                newBlueDot.GetComponent<RectTransform>().anchoredPosition += new Vector2(1.6f * epiNum, 165f + (m_BlueGroupReward * 110f));

                GameObject newRedDot = Instantiate(redDot, dotInitPosR);
                newRedDot.GetComponent<RectTransform>().anchoredPosition += new Vector2(1.6f * epiNum, 165f + (m_RedGroupReward * 110f));
            }

            sig_reachMax = false;
            sig_RewardRecord = false;
            m_BlueGroupReward = 0;
            m_RedGroupReward = 0;
        }
    }
}
