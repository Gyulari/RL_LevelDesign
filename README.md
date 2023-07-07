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
