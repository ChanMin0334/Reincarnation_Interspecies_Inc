using JetBrains.Annotations;
using System.Numerics;
using UnityEngine;

[System.Serializable]
public class BigNumericWrapper
{
    [SerializeField]
    private string serializedValue;

    private System.Numerics.BigInteger ValueCache;
    public BigNumeric value
    {
        get
        {
            if (string.IsNullOrEmpty(serializedValue) ||
                !System.Numerics.BigInteger.TryParse(serializedValue, out var parsed))
            {
                return BigInteger.Zero;
            }

            return parsed;
        }

        set => serializedValue = value.number.ToString();
    }
    public string ToString() => value.ToString();

    public BigNumericWrapper(string str)
    {
        serializedValue = str;
        //Debug.Log($"BigNumericWrapper 생성: {serializedValue}");
    }
    public BigNumericWrapper(double value)
    {
        serializedValue = value.ToString();
        //Debug.Log($"BigNumericWrapper 생성: {serializedValue}");
    }
    public BigNumericWrapper(BigNumeric num)
    {
        serializedValue = num.number.ToString();
        //Debug.Log($"BigNumericWrapper 생성: {serializedValue}");
    }

    public static implicit operator BigNumericWrapper(BigNumeric num) => new BigNumericWrapper(num.number.ToString());

    public static implicit operator BigNumericWrapper(double num) => new BigNumericWrapper(num.ToString());

    public static implicit operator BigNumericWrapper(int num) => new BigNumericWrapper(num.ToString());

    public static implicit operator BigNumericWrapper(float num) => new BigNumericWrapper(num.ToString());

    public static implicit operator BigNumeric(BigNumericWrapper wrapper) => wrapper.value;

    public static explicit operator float(BigNumericWrapper wrapper) => (float)wrapper.value;

    public static explicit operator int(BigNumericWrapper wrapper) => (int)wrapper.value;


    public BigNumericWrapper Clone() => new BigNumericWrapper(this.value);

    //추가
    /// <summary>
    /// BigNumberic끼리 퍼센테이지 계산용
    /// </summary>
    /// <returns></returns>
    public float ToFloat()
    {
        if (string.IsNullOrEmpty(serializedValue))
            return 0f;

        char lastChar = serializedValue[^1];
        double baseVal = 0;
        double scale = 1f;

        if(char.IsLetter(lastChar))
        {
            string numberPart = serializedValue[..^1];
            baseVal = double.TryParse(numberPart, out var d) ? d : 0;
            int index = lastChar - 'a';
            scale = Mathf.Pow(10, 15 + (index * 3));
        }
        else
        {
            baseVal = double.TryParse(serializedValue, out var d) ? d : 0;
        }

        return (float)(baseVal * scale);
    }
}
