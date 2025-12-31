using UnityEngine;
using UnityEngine.UI;

public class QuitTabUI : MonoBehaviour
{
    [SerializeField] private Button btnYes;
    [SerializeField] private Button btnNo;
    [SerializeField] private Button btnDoor;

    private int count = 0;

    private void Awake()
    {
        btnYes.onClick.AddListener(OnClickYes);
        btnNo.onClick.AddListener(OnClickNo);
        btnDoor.onClick.AddListener(OnClickDoor);

        count = 0;
    }

    private void OnClickYes()
    {
        GameQuit.Quit();
    }

    private void OnClickNo()
    {
        count += 100;
        ToastUtil.Success("그럼 우상단에 엑스 버튼 눌러죵~");
    }

    private void OnClickDoor()
    {
        if (count == 0)
        {
            ToastUtil.Success("이건 그냥 문이야~ 나가려면 버튼을 눌러야지~");
            count++;
        }
        else if (count == 1)
        {
            ToastUtil.Success("이건 그냥 문이라니까...");
            count++;
        }
        else if (count == 2)
        {
            ToastUtil.Success("말을 참~ 안듣네~");
            count++;
        }
        else if (count == 300)
        {
            ToastUtil.Success("잘 왔다. 취소 버튼을 세번 누르는 의식도 통과했나보군");
            count++;
        }
        else if (count == 301)
        {
            ToastUtil.Success("남자라면 자신의 프로그램에 야심을 숨겨야하는 법");
            count++;
        }
        else if (count == 302)
        {
            ToastUtil.Success("이것은 민만준이 만든 비밀의 방이다");
            count++;
        }
        else if (count == 303)
        {
            ToastUtil.Success("이것은 민만준이 만든 비밀의 방이다");
            count++;
        }
        else if (count == 304)
        {
            ToastUtil.Success("이방을 찾은 그대의 소원을 하나 들어주지");
            count++;
        }
        else if (count == 305)
        {
            ToastUtil.Success("010-4455-3517 여기로 전화를 건 후 소원을 말해라");
            count++;
        }
        else if (count == 30)
        {
            ToastUtil.Success("좋은 결과가 있을 것이다.");
            count++;
        }
        else
        {
            ToastUtil.Success("해머가 할 말이 있다던데...");
        }
    }
}
