using UnityEngine;

public static class StageProgressUtil
{
    public static bool CanEnter(int world, int stage)
    {
        SaveData data = SaveManager.Instance.CurrentData;

        // 이미 클리어한 스테이지는 언제든지 가능
        if (world < data.clearedWorld)
            return true;

        if (world == data.clearedWorld)
        {
            // 이미 깬 스테이지
            if (stage <= data.clearedStage)
                return true;

            // 바로 다음 스테이지
            if (stage == data.clearedStage + 1)
                return true;
        }

        // 다음 월드의 첫 스테이지
        if (world == data.clearedWorld + 1 && stage == 1)
        {
            // 이전 월드 마지막 스테이지 클리어 조건
            return data.clearedStage >= 5;
        }

        return false;
    }
}


public static class StageContext
{
    public static StageId SelectedStage;
}
