using System;
using Unity.VisualScripting;

[Serializable]
public class ArtifactData : IInventoryData
{
    public string id;
    public int count;
    public bool isUsed; //1회성 트리거용 유물에서 사용할것
    public bool isApplyLimit; //여러개 보유가능한 유물이 한계치 까지 적용되었는지 체크하는 Bool

    public ArtifactSO Definition => DataManager.Instance.GetData<ArtifactSO>(id);
    public string ID => id;

    public string Description
    {
        get
        {
            return Definition.GetCalculatedValue(count).ToString();
        }
    }

    public ArtifactData(string id, int count = 1)
    {
        this.id = id;
        this.count = Math.Max(1, count);
        this.isUsed = false;
        this.isApplyLimit = false;
    }

    //public bool CanApplyMore()
    //{
    //    return count < Definition.MaxCount;
    //}
}
