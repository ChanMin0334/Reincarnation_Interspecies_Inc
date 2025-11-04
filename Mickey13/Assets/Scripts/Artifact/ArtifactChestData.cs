using System;
using System.Collections.Generic;

[Serializable]
public class ArtifactChestData
{
    public ArtifactDropTable dropTable;

    public ArtifactChestData(ArtifactDropTable table)
    {
        this.dropTable = table;
    }
}

[Serializable]
public class ChestQueueSaveData
{
    public int normalChestCount; // 일반 상자 개수
    public int specialChestCount; // 특별 상자 개수
    
    public ChestQueueSaveData()
    {
        normalChestCount = 0;
        specialChestCount = 0;
    }
}
