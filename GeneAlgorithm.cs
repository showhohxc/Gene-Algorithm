using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ListSort : IComparer<ChromoSomeInfo>
{
    public int Compare(ChromoSomeInfo a, ChromoSomeInfo b)
    {
        return a.nValue.CompareTo(b.nValue);
    }
}

public class EnemyData
{
    public int nPower, nValue;

    public EnemyData(int nP, int nV)
    {
        this.nPower = nP;
        this.nValue = nV;
    }
}

public class ChromoSomeInfo
{
    public int nPower = 0;
    public int nValue = 0;
    public bool bMutat = false;
    public int[] Array_nCode;

    public void SetPowerVal(EnemyData[] Array_Enemy, bool bMutation = false)
    {
        this.nPower = 0;
        this.nValue = 0;
        this.bMutat = false;

        for (int i = 0; i < Array_nCode.Length; ++i)
        {
            if (Array_nCode[i] == 1)
            {
                this.nPower += Array_Enemy[i].nPower;
                this.nValue += Array_Enemy[i].nValue;
            }
        }

        if (bMutation)
        {
            this.nPower += 5;
            this.nPower += 5;
            this.bMutat = true;
        }
    }

    public ChromoSomeInfo DeepCopy()
    {
        ChromoSomeInfo info = new ChromoSomeInfo();
        info.Array_nCode = this.Array_nCode;
        info.nPower = this.nPower;
        info.nValue = this.nValue;

        return info;
    }
}


public class GeneAlgorithm 
{
    public float fSelect_Ratio = 0.5f;   //전체 집단에서 selection 할 비율
    public int nPopulation_Size = 12;    // 집단의 크기
    public ChromoSomeInfo[] chPopulation;
    public float fMutation_ratio = 0.45f;    // 돌연변이가 발생할 확률
    public int nMax_P = 10;              // 문제의 조건인 10Power을 의미
    public int t = 0;
    public bool bFlag = true;                
    public EnemyData[] Array_Enemys;

    public List<ChromoSomeInfo> list_chTemp = new List<ChromoSomeInfo>();    /
    public List<ChromoSomeInfo> list_chDeletion = new List<ChromoSomeInfo>();  
    public int nBest_unChange_Cnt = 0;          
    public ChromoSomeInfo chBest_chr;
    public int MAX_UNCHANGED = 30;

    public GeneAlgorithm()
    {
        SetMakeEnemy();
        SetMakeChromosomes();
    }

    void SetMakeEnemy()
    {
        Array_Enemys = new EnemyData[6];

        EnemyData EnemyData_1 = new EnemyData(2, 6);
        EnemyData EnemyData_2 = new EnemyData(4, 7);
        EnemyData EnemyData_3 = new EnemyData(5, 4);
        EnemyData EnemyData_4 = new EnemyData(3, 3);
        EnemyData EnemyData_5 = new EnemyData(2, 2);
        EnemyData EnemyData_6 = new EnemyData(1, 5);

        Array_Enemys[0] = EnemyData_1;
        Array_Enemys[1] = EnemyData_2;
        Array_Enemys[2] = EnemyData_3;
        Array_Enemys[3] = EnemyData_4;
        Array_Enemys[4] = EnemyData_5;
        Array_Enemys[5] = EnemyData_6;
    }

    void SetMakeChromosomes()
    {
        chPopulation = new ChromoSomeInfo[nPopulation_Size];
        int nCnt = 0;

        while(nCnt < nPopulation_Size)
        {
            ChromoSomeInfo ch = new ChromoSomeInfo();
            int[] Array_nCode = new int[Array_Enemys.Length];

            for(int i = 0; i < Array_Enemys.Length; ++i)
            {
                Thread.Sleep(10);
                System.Random Rand = new System.Random();
                double double_RandV = Rand.NextDouble();

                if(double_RandV > 0.5)  // 상위 0.5%
                {
                    Array_nCode[i] = 1;
                    ch.nPower += Array_Enemys[i].nPower;
                    ch.nValue += Array_Enemys[i].nValue;
                }
                else
                {
                    Array_nCode[i] = 0;
                }
            }

            if(ch.nPower <= nMax_P)
            {
                ch.Array_nCode = Array_nCode;
                chPopulation[nCnt++] = ch;
            }
        }

        for(int i = 0; i < nPopulation_Size; ++i)
        {
            Debug.Log(string.Format("Init_Chromosomes Power : {0} Value :{1}", chPopulation[i].nPower, chPopulation[i].nValue));
        }
    }

    public void DoTest()
    {
        Array.Sort(chPopulation, (a, b) => (a.nValue > b.nValue) ? -1 : 1);

        if (chBest_chr == null)
        {
            chBest_chr = chPopulation[0];
        }
        else
        {
            if (chBest_chr.nValue < chPopulation[0].nValue)
            {
                chBest_chr = chPopulation[0].DeepCopy();
                nBest_unChange_Cnt++;
            }
            else
            {
                nBest_unChange_Cnt++;
            }
        }

        if (MAX_UNCHANGED < nBest_unChange_Cnt)
            bFlag = false;
    }

    public void DoSetSelect()
    {
        for(int i = (int)(nPopulation_Size * fSelect_Ratio); i < nPopulation_Size; ++i)
        {
            list_chDeletion.Add(chPopulation[i].DeepCopy());
            chPopulation[i] = null;
        }
    }

    public void DoSetAlter()
    {
        ProcCrossOver();
        System.Random rand = new System.Random();

        Thread.Sleep(10);
        double rand_v = rand.NextDouble();

        Debug.Log(string.Format("<color=red>돌연변이 확률! {0}% </color>", rand_v));

        if (rand_v < fMutation_ratio)
            ProcWarningMutation();
    }

    /// <summary>
    /// 유전자 교배
    /// </summary>
    void ProcCrossOver()
    {
        bool[] Array_bVisited = new bool[(int)(nPopulation_Size * fSelect_Ratio)];
        ProcCombination(chPopulation, Array_bVisited, 0, (int)(nPopulation_Size * fSelect_Ratio), 2);

        list_chTemp.Sort(new ListSort());
        list_chTemp.Reverse();

        int nIdx = 0;
        int d_nIdx = 0;

        for(int i = (int)(nPopulation_Size * fSelect_Ratio); i < nPopulation_Size; ++i)
        {
            if (nIdx < list_chTemp.Count)
                chPopulation[i] = list_chTemp[nIdx++];
            else
                chPopulation[i] = list_chDeletion[d_nIdx++];
        }

        list_chTemp.Clear();
    }

    void ProcCombination(ChromoSomeInfo[] Arr_Chromosome, bool[] Array_bVisited, int nStart, int n, int r)
    {
        if(r == 0)
        {
            ChromoSomeInfo[] Array_chSeleted = new ChromoSomeInfo[2];
            int nCnt = 0;

            for (int i = 0; i < n; ++i)
            {
                if (Array_bVisited[i])
                    Array_chSeleted[nCnt++] = Arr_Chromosome[i].DeepCopy();
            }

            // 앞에서부터 2개 CrossOver
            for (int i = 0; i < Array_Enemys.Length; ++i)
            {
                ProcChromosomeSwap(Array_chSeleted[0], Array_chSeleted[1], i);

                if (ProcCheckSum(Array_chSeleted[0]) && ProcCheckSum(Array_chSeleted[1]))
                    nCnt--;
                else
                    ProcChromosomeSwap(Array_chSeleted[0], Array_chSeleted[1], i);

                if (nCnt == 0) break;
            }

            if(nCnt <= 1)
            {
                // 하나이상 교환 되었을시
                list_chTemp.Add(Array_chSeleted[0]);
                list_chTemp.Add(Array_chSeleted[1]);
            }

            return;
        }

        for(int i = nStart; i < n; ++i)
        {
            // nStart 0 부터 시작
            // 0부터 6까지 모든 Index 순서대로 비교 후 교배
            Array_bVisited[i] = true;
            ProcCombination(Arr_Chromosome, Array_bVisited, i + 1, n, r - 1);

            Array_bVisited[i] = false;
        }
    }

    void ProcWarningMutation()
    {
        System.Random rand = new System.Random();
        System.Random rand2 = new System.Random();

        int nMutation = (int)(rand.NextDouble() * nPopulation_Size);
        int nIdx = (int)(rand2.NextDouble() * Array_Enemys.Length);
        chPopulation[nMutation].Array_nCode[nIdx] = (chPopulation[nMutation].Array_nCode[nIdx] == 1) ? 0 : 1;
        chPopulation[nMutation].SetPowerVal(Array_Enemys, true);
        Debug.Log(string.Format("<color=red>돌연변이 발생! Power {0} Value {1}</color>", chPopulation[nMutation].nPower, chPopulation[nMutation].nValue));
    }

    void ProcChromosomeSwap(ChromoSomeInfo ch_1, ChromoSomeInfo ch_2, int nIndex)
    {
        int nTemp = ch_1.Array_nCode[nIndex];
        ch_1.Array_nCode[nIndex] = ch_2.Array_nCode[nIndex];
        ch_2.Array_nCode[nIndex] = nTemp;

        ch_1.SetPowerVal(Array_Enemys);
        ch_2.SetPowerVal(Array_Enemys);
    }

    bool ProcCheckSum(ChromoSomeInfo chInfo)
    {
        if (nMax_P < chInfo.nPower)
            return false;

        return true;
    }
}
