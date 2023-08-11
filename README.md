## Gene-Algorithm (유전 알고리즘)
# Unity Gene-Algorithm Test
### Review

- 평소 관심과 흥미가 일던, 유전자 알고리즘에 흥미를 느껴 연구해 보고 게임 시스템에 점목시킬수 있지 않을까? 하는 호기심이 생겨 만들어 보게 되었다.

- 유전 알고리즘을 학습 후, 각 세대마다 Player에게 대응하는 Enemys 최적의 조건을 맞추어보았다.

### Genetic Algorithm

- Player가 스테이지내에서 Enemys 사냥한다는 목적하에 Enemys 가 교배를 통한 진화와 돌연변이 발생들의 변수를 통해 Player를 압박하는 시스템을 구현해보았다.
- ChromoSomeInfo : 염색체 / nPopulation_Size : Enemy의 수 / fMutation_ratio : 돌연변이 발생 확률 / nMax_P : Enemy의 파워제한 / MAX_UNCHANGED : 세대

### Flow
- SetMakeEnemy
- SetMakeChromosomes
- chBest_chr
- ProcCrossOver
- ProcCombination
 - ProcChromosomeSwap
 - ProcCheckSum
- ProcWarningMutation
