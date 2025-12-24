using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JYL
{
    public class TestBattleManager : MonoBehaviour
    {
        private List<CharController> characters;
        private List<TestEnemyController> enemies;
        [SerializeField] private TestGameManager gameManager;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            characters = gameManager.GetParty().ToList();
            foreach (var c in characters)
            {
                if(c.gameObject.activeSelf) c.Init(0,8f);
            }
            enemies = gameManager.GetEnemies().ToList();
            foreach (var c in enemies)
            {
                if (c.gameObject.activeSelf) c.Init();
            }
        }
        void Start()
        {
            
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.R))
                TestTimeRewind();
            foreach (var e in enemies)
            {
                if (!e.isDead) return;
            }

            StageClear();
        }

        public void TestTimeRewind()
        {
            foreach (var c in characters)
            {
                if(c.gameObject.activeSelf) 
                    c.RewindTime();
            }
        }

        private void StageClear()
        {
            // 클리어 UI 출력
            // 1. 보상 UI
            // 2. 재시작, 메인씬으로 나가기
        }

        private void RestartStage()
        {
            // 스테이지 재시작
            // 씬을 다시 불러오는 것으로 해결함. GameManager쪽으로 기능 이관
        }

        private void ExitStage()
        {
            // MainScene으로 씬 전환
        }
    }

}
