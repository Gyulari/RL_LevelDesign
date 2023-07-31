# RL_LevelDesign
Level design using Reinforcement Learning with Unity ML-Agents

<br/>

▷ ver.1.1.0 [2023-07-07]
- CollectObservation() 삭제
- Agents의 개별 reward 부여 코드 삭제, Group reward만 부여하도록 수정
- BattleStage를 파괴하고 재생성하여 scene을 초기화하는 방식 사용
- Group reward를 점 그래프로 시각화하는 코드 작성

<br/>

- Delete CollectObservation()
- Delete individual reward of agents, Modify to add group reward only
- Use a method of reset the scene by destroying and re-instantiate the BattleStage
- Add visualizing code for point graph of group reward

<br/>

▷ ver.1.1.1 [2023-07-18]
- BattleStage를 파괴하고 재생성하여 scene을 초기화하는 방식에서 Agent의 활성 상태 전환을 통해 scene을 초기화하는 방식으로 변경
  - Agent가 제거될 경우, Agent Object를 비활성 상태로 전환하는 방식
  - 모든 Agent들이 파괴되어 Agent Gorup 정보가 초기화되지 않도록 최후의 Agent는 비활성 상태로 전환하지 않음
- Agent의 action 및 HP 정보, 현재 episode의 steps 진행 현황을 확인할 수 있는 Dashboard UI 추가
- Episode 종료 원인과 종료 당시의 Agent Group 상태를 확인할 수 있는 Log UI 추가

<br/>

- Change from destroying and re-instantiating the BattleStage to reset the scene by switching the active state of the agent.
  - If the agent is killed, switch the active state of agent to inactive state.
  - To prevent Agent Group from being destroyed as all Agents are destroyed, last agent isn't switched to inactive state.
- Add Dashboard UI for checking actions and HP of each agents, and progress of steps of the current episode.
- Add Log UI for checking the reason for episode interruption and the status of Agent Group at interruption.

<br/>

▷ ver.1.1.2 [2023-07-31]
- Agent 제거 판정을 수신하는 OnDestroyed Event를 제거
- 다음 Episode로 넘어갈 때 제거된 Agent 수를 초기화하는 ResetKillCount가 누락된 오류를 수정
- 비활성 상태로 전환된 Agent의 HP가 초기화되지 않는 오류를 수정
- 다음 Episode로 전환됐을 때 사거리 밖의 Agent를 공격하는 오류를 수정
- 다음 Episode로 넘어갈 때 비활성 상태로 전환된 Agent를 Agent Group에 다시 Register하는 코드를 추가

<br/>

- Remove OnDestroyed Event that receives agents destruction decision.
- Debug an error omitting ResetKillCount function which reset the number of agents removed when switching to the next episode.
- Debug an error where the HP of an agents that has been switched to an inactive state is not initialized.
- Debug an error attacking agents outside the range when switched to the next episode.
- Add code to re-register an inactive agents to the AgentGroup when switching to the next episode.
