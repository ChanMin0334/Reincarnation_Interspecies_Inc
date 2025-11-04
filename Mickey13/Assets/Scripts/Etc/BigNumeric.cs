using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Buffers;
using System.Text;
using System;

public class BigNumeric
{
    public BigInteger number { get; }
    private readonly Lazy<List<short>> _digits; // 처음 접근 시에만 계산되도록 Lazy 사용
    private static readonly ArrayPool<short> _pool = ArrayPool<short>.Shared; // ArrayPool 사용으로 메모리 할당 최소화
    private static StringBuilder _SBCache = new StringBuilder(10); // ToString용 StringBuilder 캐싱 적용으로 메모리 할당 최소화
    private static readonly ConditionalWeakTable<BigNumeric, object> _stringCache = new(); // ToString 결과 캐싱용
    private static readonly Lazy<WeakReference<string[]>> _unitCache = new(() => // 단위 문자열 캐싱용: WeakReference 사용으로 메모리 절약
    {
        var units = GenerateUnits();
        return new WeakReference<string[]>(units);
    });

    public BigNumeric(BigInteger num)
    {
        number = num;
        _digits = new Lazy<List<short>>(SeperateNumber);
    }

    public IReadOnlyList<short> SeperatedNumber => _digits.Value;

    private List<short> SeperateNumber()
    {
        if (number.IsZero) return new List<short> { 0 };

        int requiredLength = (int)Math.Ceiling(BigInteger.Log(number + 1, 1000));
        var rented = _pool.Rent(requiredLength);
        int count = 0;
        BigInteger temp = number;

        while (temp > 0)
        {
            rented[count++] = (short)(temp % 1000);
            temp /= 1000;
        }

        var result = new List<short>(count);
        for (int i = count - 1; i >= 0; i--)
        {
            result.Add(rented[i]);
        }
        _pool.Return(rented);
        return result;
    }

    public override string ToString()
    {
        if (_stringCache.TryGetValue(this, out var cached))
        {
            return (string)cached;
        }

        if (number.IsZero)
        {
            _stringCache.Add(this, "0");
            return "0";
        }

        var parts = SeperatedNumber;
        int count = parts.Count;
        int intPart = parts[0];
        int idx = count - 1;

        _SBCache.Clear();
        _SBCache.Append(parts[0]);

        if (count > 1)
        {
            int intDigits;
            if (intPart < 10) intDigits = 1;
            else if (intPart < 100) intDigits = 2;
            else intDigits = 3;

            int decimalPlaces = 4 - intDigits;

            if (decimalPlaces > 0)
            {
                var fracPart = parts[1];
                string fracString = fracPart.ToString("D3");

                _SBCache.Append('.');
                _SBCache.Append(fracString, 0, decimalPlaces);
            }
        }

        string unit = GetUnit(idx);
        _SBCache.Append(unit);

        string result = _SBCache.ToString();
        _stringCache.Add(this, result);
        return result;
    }

    private static string[] GetUnits()
    {
        if (_unitCache.Value.TryGetTarget(out var cache))
            return cache;
        var fresh = GenerateUnits();
        _unitCache.Value.SetTarget(fresh);
        return fresh;
    }

    private static string GetUnit(int index)
    {
        var units = GetUnits();
        return index < units.Length ? units[index] : $"[{index}]";
    }

    private static string[] GenerateUnits()
    {
        var list = new List<string> { "" };
        for (char a = 'a'; a <='z'; a++)
            list.Add(a.ToString());
        for (char a = 'a'; a <='z'; a++)
            for (char b = 'a'; b <='z'; b++)
                list.Add($"{a}{b}");

        return list.ToArray();
    }
    // ### 연산자 오버로딩
    #region operators
    // implicit overloading
    public static implicit operator BigNumeric(int num) => new BigNumeric(num);

    public static implicit operator BigNumeric(float num) => new BigNumeric((BigInteger)num);

    public static implicit operator BigInteger(BigNumeric num) => num.number;

    public static implicit operator BigNumeric(BigInteger num) => new BigNumeric(num);

    public static explicit operator BigNumeric (decimal num) => new BigNumeric((BigInteger)num); 

    public static explicit operator float(BigNumeric num) => (float)num.number;


    public static BigNumeric operator +(BigNumeric a, BigNumeric b)
    {
        return new BigNumeric(a.number + b.number);
    }
    public static BigNumeric operator *(BigNumeric a, float m)
    {
        BigInteger Mult = (BigInteger)(m * 10000000000);
        BigNumeric result = ((a.number * Mult) / 10000000000);
        return result;

    }
    public static BigNumeric operator *(float m, BigNumeric a) => a * m;

    public static BigNumeric operator *(BigNumeric a, BigNumeric big) => new BigNumeric(a.number * big.number);

    public static BigNumeric operator -(BigNumeric a, BigNumeric b)
    {
        return new BigNumeric(a.number - b.number);
    }

    public static BigNumeric operator /(BigNumeric a, BigNumeric d) => new BigNumeric(a.number / d.number);

    #region Floats
    public static BigNumeric operator /(BigNumeric a, float d) => a * (1f / d);
    public static bool operator <(BigNumeric a, float b)
    {
        if (a.number == (BigInteger)b)
        {
            if (b < 0)
            {
                return false;
            }
            else return true;
        }
        else if (a.number < (BigInteger)b)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool operator >(BigNumeric a, float b) => !(a < b);
    public static bool operator <=(BigNumeric a, float b) => (a < b) || (a.number == (BigInteger)b);
    public static bool operator >=(BigNumeric a, float b) => !(a <= b);
    public static bool operator <(float a, BigNumeric b) => b > a;
    public static bool operator >(float a, BigNumeric b) => b < a;
    public static bool operator <=(float a, BigNumeric b) => b >= a;
    public static bool operator >=(float a, BigNumeric b) => b <= a;
    #endregion

    #region BigNumeric

    public static bool operator <(BigNumeric a, BigNumeric b)
    {
        if (a.number < b.number) return true;
        else return false;
    }
    public static bool operator >(BigNumeric a, BigNumeric b) => !(a < b);

    public static bool operator <=(BigNumeric a, BigNumeric b) => (a < b) || (a == b);
    public static bool operator >=(BigNumeric a, BigNumeric b) => !(a <= b);

    public static bool operator ==(BigNumeric a, BigNumeric b)
    {
        if (a.number == b.number) return true;
        else return false;
    }
    public static bool operator !=(BigNumeric a, BigNumeric b) => !(a == b);
    #endregion

    public float Ratio(BigNumeric b)
    {
        BigNumeric temp = this * 1000000;
        return (float)(temp / b) / 1000000;
    }

    public static BigNumeric Max(BigNumeric a, BigNumeric b)
    {
        if (a > b) return a;
        else return b;
    }
    public static BigNumeric Min(BigNumeric a, BigNumeric b)
    {
        if (a < b) return a;
        else return b;
    }
    public static BigNumeric Clamp(BigNumeric value, BigNumeric min, BigNumeric max)
    {
        if (value < min) return min;
        else if (value > max) return max;
        else return value;
    }
    
    public static BigNumeric Pow(BigNumeric a, long exp)
    {
        if (exp == 0) return 1;

        BigNumeric result = 1;
        BigNumeric b = a;
        long e = exp;
        while (e > 0)
        {
            if ((e % 2) == 1)
            {
                result *= b;
            }

            b *= b;
            e /= 2;
        }
        return result;
    }

    public static (BigNumeric Numerator, BigNumeric Denominator) ParseToFraction(float a)
    {
        int decimalPointIndex = a.ToString().IndexOf('.');
        if (decimalPointIndex == -1)
        {
            return (new BigNumeric((BigInteger)a), new BigNumeric(1));
        }

        int decimalPlaces = a.ToString().Length - decimalPointIndex - 1;

        BigNumeric denominator = BigNumeric.Pow(10, decimalPlaces);

        string strNumerator = a.ToString().Replace(".", "");
        BigNumeric numerator = new BigNumeric(BigInteger.Parse(strNumerator));

        return (numerator, denominator);
    }

    #endregion
}