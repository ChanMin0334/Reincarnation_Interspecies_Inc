using TMPro;
using UnityEngine;

public class CurrentGoodsUI : MonoBehaviour
{
    [Header("Gold")]
    [SerializeField] TextMeshProUGUI curGold;
    [Header("Diamond")]
    [SerializeField] TextMeshProUGUI curDiamond;
    [Header("SoulStone")]
    [SerializeField] TextMeshProUGUI curSoulStone;

    private void OnEnable()
    {
        if(User.Instance != null)
        {
            User.Instance.OnGoodsChanged += UpdateCurrentGoods;
            UpdateCurrentGoods();
        }
    }

    private void OnDisable()
    {
        if(User.Instance != null)
        {
            User.Instance.OnGoodsChanged -= UpdateCurrentGoods;
        }
    }

    private void UpdateCurrentGoods()
    {
        curGold.text = User.Instance.gold.ToString();
        curDiamond.text = User.Instance.diamond.ToString("N0");
        curSoulStone.text = User.Instance.soulStone.ToString();
    }
}
