using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Information of action from agents
public class ReceiveInfo
{
    public string name;
    public int horizontal;
    public int vertical;
    public int fire;
}

public class DashboardManager : MonoBehaviour
{
    [SerializeField] private BattleTeam team;

    public List<BattleAgentController> agents;
    public List<GameObject> actionList;
    public List<Image> hpList;

    private List<ReceiveInfo> receiveInfoList = new List<ReceiveInfo>();

    private Color blueColor = new Color(0f, 0f, 255f);
    private Color redColor = new Color(255f, 0f, 0f);
    private Color emptyColor = new Color(255f, 255f, 255f);

    private void OnEnable()
    {
        BattleAgentController.SendInfo += ReceiveInfoFromAgents;
    }

    private void OnDisable()
    {
        BattleAgentController.SendInfo -= ReceiveInfoFromAgents;
    }

    private void Start()
    {
        for (int i = 0; i < agents.Count; i++) {
            ReceiveInfo rInfo = new ReceiveInfo
            {
                name = agents[i].gameObject.name,
                horizontal = 0,
                vertical = 0,
                fire = 0
            };

            receiveInfoList.Add(rInfo);
        }
    }

    private void Update()
    {
        for(int idx=0; idx<agents.Count; idx++) {
            if (!agents[idx].gameObject.activeSelf) {
                hpList[idx].GetComponent<Image>().fillAmount = 0.0f;
                actionList[idx].transform.GetChild(0).gameObject.GetComponent<Image>().color = emptyColor;
                actionList[idx].transform.GetChild(1).gameObject.GetComponent<Image>().color = emptyColor;
                actionList[idx].transform.GetChild(2).gameObject.GetComponent<Image>().color = emptyColor;
                actionList[idx].transform.GetChild(3).gameObject.GetComponent<Image>().color = emptyColor;
                actionList[idx].transform.GetChild(4).gameObject.GetComponent<Image>().color = emptyColor;
            }
            else {
                if (receiveInfoList[idx].horizontal > 0) {
                    actionList[idx].transform.GetChild(2).gameObject.GetComponent<Image>().color = emptyColor; // Left
                    if(team == BattleTeam.Blue)
                        actionList[idx].transform.GetChild(3).gameObject.GetComponent<Image>().color = blueColor;
                    else if(team == BattleTeam.Red)
                        actionList[idx].transform.GetChild(3).gameObject.GetComponent<Image>().color = redColor;

                }
                else if (receiveInfoList[idx].horizontal < 0) {
                    actionList[idx].transform.GetChild(3).gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f); // Right
                    if (team == BattleTeam.Blue)
                        actionList[idx].transform.GetChild(2).gameObject.GetComponent<Image>().color = blueColor;
                    else if (team == BattleTeam.Red)
                        actionList[idx].transform.GetChild(2).gameObject.GetComponent<Image>().color = redColor;
                }
                else {
                    actionList[idx].transform.GetChild(2).gameObject.GetComponent<Image>().color = emptyColor;
                    actionList[idx].transform.GetChild(3).gameObject.GetComponent<Image>().color = emptyColor;
                }

                if (receiveInfoList[idx].vertical > 0) {
                    actionList[idx].transform.GetChild(1).gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f); // Back
                    if (team == BattleTeam.Blue)
                        actionList[idx].transform.GetChild(0).gameObject.GetComponent<Image>().color = blueColor;
                    else if(team == BattleTeam.Red)
                        actionList[idx].transform.GetChild(0).gameObject.GetComponent<Image>().color = redColor;
                }
                else if (receiveInfoList[idx].vertical < 0) {
                    actionList[idx].transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f);// Front
                    if (team == BattleTeam.Blue)
                        actionList[idx].transform.GetChild(1).gameObject.GetComponent<Image>().color = blueColor; // Back
                    else if (team == BattleTeam.Red)
                        actionList[idx].transform.GetChild(1).gameObject.GetComponent<Image>().color = redColor; // Back
                }
                else {
                    actionList[idx].transform.GetChild(2).gameObject.GetComponent<Image>().color = emptyColor;
                    actionList[idx].transform.GetChild(3).gameObject.GetComponent<Image>().color = emptyColor;
                }

                if (receiveInfoList[idx].fire == 0) {
                    actionList[idx].transform.GetChild(4).gameObject.GetComponent<Image>().color = emptyColor; // Fire
                }
                else {
                    if (team == BattleTeam.Blue)
                        actionList[idx].transform.GetChild(4).gameObject.GetComponent<Image>().color = blueColor; // Fire
                    else if (team == BattleTeam.Red)
                        actionList[idx].transform.GetChild(4).gameObject.GetComponent<Image>().color = redColor; // Fire
                }
                
                hpList[idx].GetComponent<Image>().fillAmount = agents[idx].HP / 100.0f;
            }
        }
    }

    private void ReceiveInfoFromAgents(string name, int h, int v, int f)
    {
        for(int i=0; i<agents.Count; i++) {
            if (agents[i].gameObject.name == name) {
                ReceiveInfo rInfo = new ReceiveInfo
                {
                    name = name,
                    horizontal = h,
                    vertical = v,
                    fire = f
                };

                receiveInfoList[i] = rInfo;
            }
        }
    }
}
