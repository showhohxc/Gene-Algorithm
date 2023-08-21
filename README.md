## Gene-Algorithm (유전 알고리즘)
# Unity Gene-Algorithm Test
### Review

- 평소 관심과 흥미가 일던, 유전자 알고리즘에 흥미를 느껴 연구해 보고 게임 시스템에 점목시킬수 있지 않을까? 하는 호기심이 생겨 만들어 보게 되었다.

- 유전 알고리즘을 학습 후, 각 세대마다 Player에게 대응하는 Enemys 최적의 조건을 맞추어보았다.

> Youtube

[![Video Label](http://img.youtube.com/vi/cHgll2BdbGk/0.jpg)](https://youtu.be/cHgll2BdbGk)

### Genetic Algorithm

- Player가 스테이지내에서 Enemys 사냥한다는 목적하에 Enemys 가 교배를 통한 진화와 돌연변이 발생들의 변수를 통해 Player를 압박하는 시스템을 구현해보았다.
- ChromoSomeInfo : 염색체 / nPopulation_Size : Enemy의 수 / fMutation_ratio : 돌연변이 발생 확률 / nMax_P : Enemy의 파워제한 / MAX_UNCHANGED : 세대

### Flow
- SetMakeEnemy : EnemyData Power / Value 값 정의
- SetMakeChromosomes : SetMakeEnemy 정의한 EnemyData 바탕으로 정의한 유전자(Enemy)와 염색체 구현
- chBest_chr : 구현한 Enemy들중 Best Enemy 선정
- ProcCrossOver : 어떤 조건으로 유전자끼리 교차할것인가
- ProcWarningMutation : 돌연변이 염색체를 가진 유전자를 어떤 방식으로 출현 시킬 것인가.

### SetMakeEnemy

- EnemyData를 가지고 있는 배열 생성

### SetMakeChromosomes

- EnemyData를 바탕으로 유전자와 염색체를 정의

- 너무 과한 Power를 밸런스 과잉을 불러올수 있으니 nMax_P 를 통하여 Power 제한

### chBest_chr(Variable)

- 생성된 유전자 중 가장 베스트 유전자 선정

### ProcCrossOver

- 유전자 교차 조건 형성
## ProcCombination

- 상위 nPopulation_Size * fSelect_Ratio 개의 유전자들을 서로 교차

## ProcChromosomeSwap

- 염색체 교환 그 과정에서 nMax_P 이상의 유전자가 발생시 바로 아웃 다음 유전자의 염색체를 교환
```
    void ProcChromosomeSwap(ChromoSomeInfo ch_1, ChromoSomeInfo ch_2, int nIndex)
    {
        int nTemp = ch_1.Array_nCode[nIndex];
        ch_1.Array_nCode[nIndex] = ch_2.Array_nCode[nIndex];
        ch_2.Array_nCode[nIndex] = nTemp;

        ch_1.SetPowerVal(Array_Enemys);
        ch_2.SetPowerVal(Array_Enemys);
    }
```

### ProcWarningMutation

- 돌연변이 염색체의 발생으로 Rare한 염색체를 가지고 있는 유전자(Enemy)를 생성
```
    void ProcWarningMutation()
    {
        System.Random rand = new System.Random();
        System.Random rand2 = new System.Random();

        int nMutation = (int)(rand.NextDouble() * nPopulation_Size);
        int nIdx = (int)(rand2.NextDouble() * Array_Enemys.Length);
        chPopulation[nMutation].Array_nCode[nIdx] = (chPopulation[nMutation].Array_nCode[nIdx] == 1) ? 0 : 1;
        chPopulation[nMutation].SetPowerVal(Array_Enemys, true);
    }
```

### Note / Result

- Player의 체력이 100기준 Enemy들의 각 세대마다 교차를 통한 진화를 통해 Player의 체력이 줄어드는게 점점 보이고있다.
- 치명적인게 있는데 Enemy들의 2~3 세대를 통해 바로 최적의 조건을 찾아내어 더 이상의 교차를 통한 진화가 의미없고 같은 Power만을 가지고 Player에게 데미지를 주고 있다.
- 이를 극복하고자, Mutant 확률을 높여 변수가 발생하게끔하여 해결했지만 돌연변이 의존도가 높다는 결론을 내었다.
- Mutant 의존도가 아닌 최적의 조건에 또 다른 예외처리를 하여 Mutant 의존도를 낮추는 방향으로 개선해보는 테스트를 진행해봐야겠다.
